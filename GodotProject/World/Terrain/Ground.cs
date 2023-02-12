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
	Vector3[] points;
	public override void _Ready()
	{
		distance = 0;
		planeRef = GetNode<Plane>(plane);
		mat = (ShaderMaterial)this.Mesh.SurfaceGetMaterial(0);
		gen = getTerrainGenerator();
		initPoints();
		
	}

	private void initPoints(){
		points = new Vector3[250];
		for (int i = 0; i < points.Length; i++){
			points[i] = new Vector3(-10000, 1, -10000);
		}
		points[0] = new Vector3(0,1000,0);
		for(int i = 1; i < 250; i++){
			points[i] = new Vector3(0,300, (-150 * i) - 250);
		}
	}

	private TerrainGenerator getTerrainGenerator() {
		return GetChildren().OfType<TerrainGenerator>().First();
	}

	public override void _Process(double delta)
	{
		updatePosition(delta);
		updateGroundShader();
	}

	private void updatePosition(double delta){
		distance += -planeRef.getSpeed().z * (float)delta;
		for (int i = 0; i < points.Length; i++) {
			points[i] = points[i] + new Vector3(planeRef.getSpeed().x, 0, planeRef.getSpeed().z) * -(float)delta;
		}
	}

	private void updateGroundShader() {
		mat.SetShaderParameter("points", points);
	}
}
