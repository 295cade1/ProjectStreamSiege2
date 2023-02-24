using Godot;
using System;
using System.Linq;

public partial class Ground : MeshInstance3D
{
	[Export]
	NodePath plane;
	Plane planeRef;

	Vector3 position;
	Vector3 pointsPosition;

	ShaderMaterial mat;
	TerrainGenerator[] generators;
	Vector3[] points;

	float renderDist = 4096;

	[Export]
	int MAXPOINTS = 256;

	[Export]
	int HASHSIZE = 32;

	int vertDiff = (int)8192/128;


	public override void _Ready()
	{
		position = new Vector3(0,0,0);
		pointsPosition = new Vector3(0,0,0);
		planeRef = GetNode<Plane>(plane);
		mat = (ShaderMaterial)this.GetSurfaceOverrideMaterial(0);
		initPoints();
		initGenerators();
	}

	private void initPoints(){
		points = new Vector3[MAXPOINTS];
		for (int i = 0; i < points.Length; i++){
			points[i] = new Vector3(-10000, 1, -10000);
		}
	}

	private void initGenerators() {
		generators = new TerrainGenerator[1];
		generators[0] = new BasicGenerator(0);
		points[0] = new Vector3(0,500,0);
	}

	public override void _Process(double delta)
	{
		updatePosition(delta);
		updatePoints();
		updateGroundShader();
	}

	private void updatePoints(){
		for (int i = 0; i < generators.Length; i++){
			TerrainGenerator gen = generators[i];
			while(getSquaredDistToPoint(gen.getIndex()) < renderDist) {
				Vector3 nextPoint = gen.getNextPoint(points[gen.getIndex()]);
				gen.setIndex(incIndex(gen.getIndex()));
				points[gen.getIndex()] = nextPoint;
				//GD.Print(points[gen.getIndex()]);
			}
		}
	}

	private int incIndex(int index){
		index = (index + 1) % MAXPOINTS;
		return index;
	}

	private float getSquaredDistToPoint(int pointIndex){
		Vector2 planePos = new Vector2(planeRef.GlobalTransform.Origin.X, planeRef.GlobalTransform.Origin.Z);
		Vector2 pointPos = new Vector2(points[pointIndex].X, points[pointIndex].Z);
		//GD.Print(planePos.DistanceTo(pointPos));
		return planePos.DistanceTo(pointPos);
	}

	private void updatePosition(double delta){
		position -= planeRef.getSpeed() * (float)delta;
		position = new Vector3(position.X, 0, position.Z);
		if(position.DistanceTo(pointsPosition) > vertDiff){
			Vector3 pointsOffset = (position - modVec(position, vertDiff)) - pointsPosition;
			pointsPosition += pointsOffset;
			for (int i = 0; i < points.Length; i++) {
				points[i] = points[i] + pointsOffset;
			}
		}
		this.Position = position - pointsPosition;
	}

	private Vector3 modVec(Vector3 vec, float m){
		return new Vector3(fmod(vec.X, m), fmod(vec.Y, m), fmod(vec.Z, m));
	}

	private float fmod(float i, float m){
		if(Mathf.Abs(i) < m){
			return i;
		}else if(i < 0){
			return fmod(i + m, m);
		}else{
			return fmod(i - m, m);
		}
	}

	private void updateGroundShader() {
		mat.SetShaderParameter("points", hashPoints());
		mat.SetShaderParameter("position", pointsPosition);
	}

	private Vector3[] hashPoints(){
		Vector3[] hashedPoints = new Vector3[HASHSIZE * HASHSIZE];
		foreach (Vector3 point in points){
			//Ensure the point is inside the actual rendered area
			if(point.X + renderDist < renderDist * 2 && point.X + renderDist > 0
			 && point.Z + renderDist < renderDist * 2 && point.Z + renderDist > 0){
				int hashDivide = (int)renderDist * 2 / HASHSIZE;
				int x, y;
				x = Mathf.FloorToInt((point.X + renderDist) / hashDivide);
				y = Mathf.FloorToInt((point.Z + renderDist) / hashDivide);
				//TODO REFACTOR
				int i;
				for(i = x + y * HASHSIZE; i < hashedPoints.Length && hashedPoints[i] != new Vector3(0,0,0); i++){}
				if(i < hashedPoints.Length){
					hashedPoints[i] = point;
				}
			}
		}
		return hashedPoints;
	}
}
