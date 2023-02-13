using Godot;
using System;
using System.Linq;

public partial class Ground : MeshInstance3D
{
	[Export]
	NodePath plane;
	Plane planeRef;

	Vector3 position;

	ShaderMaterial mat;
	TerrainGenerator gen;
	Vector3[] points;
	public override void _Ready()
	{
		position = new Vector3(0,0,0);
		planeRef = GetNode<Plane>(plane);
		mat = (ShaderMaterial)this.GetSurfaceOverrideMaterial(0);
		//gen = getTerrainGenerator();
		initPoints();
		
	}

	private void initPoints(){
		points = new Vector3[250];
		for (int i = 0; i < points.Length; i++){
			points[i] = new Vector3(-10000, 1, -10000);
		}
		points[0] = new Vector3(0,3000,0);
		for(int i = 1; i < 250; i++){
			points[i] = new Vector3(Mathf.Sin(i / 20f) * 1000 ,600, (-100 * i) - 250);
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
		position -= planeRef.getSpeed() * (float)delta;
		for (int i = 0; i < points.Length; i++) {
			points[i] = points[i] + new Vector3(planeRef.getSpeed().X, 0, planeRef.getSpeed().Z) * -(float)delta;
		}
	}

	private void updateGroundShader() {
		mat.SetShaderParameter("points", points);
		mat.SetShaderParameter("position", position);
	}
}
