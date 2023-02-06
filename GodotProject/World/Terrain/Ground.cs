using Godot;
using System;
using System.Linq;

public partial class Ground : MeshInstance3D
{
	[Export]
	NodePath plane;
	Plane planeRef;

	float distance;

	ShaderMaterial mat;
	TerrainGenerator gen;
	ImageTexture heightmapTexture;
	public override void _Ready()
	{
		distance = 0;
		planeRef = GetNode<Plane>(plane);
		mat = (ShaderMaterial)this.Mesh.SurfaceGetMaterial(0);
		gen = getTerrainGenerator();
		heightmapTexture = ImageTexture.CreateFromImage(gen.getHeightmapImage(planeRef.GlobalTransform.origin + new Vector3(0, 0, distance)));
	}

	private TerrainGenerator getTerrainGenerator() {
		return GetChildren().OfType<TerrainGenerator>().First();
	}

	public override void _Process(double delta)
	{
		updatePosition(delta);
		updateHeightmapTexture();
		updateGroundShader();
	}

	private void updatePosition(double delta){
		distance += -planeRef.getSpeed() * (float)delta;
	}

	private void updateHeightmapTexture() {
		heightmapTexture.Update(gen.getHeightmapImage(planeRef.GlobalTransform.origin + new Vector3(0, 0, distance)));
	}

	private void updateGroundShader() {
		mat.SetShaderParameter("heightmap", heightmapTexture);
	}
}
