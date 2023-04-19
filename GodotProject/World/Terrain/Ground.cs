using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

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

	static float renderDist = 4096;

	[Export]
	int MAXPOINTS = 256;

	[Export]
	int HASHSIZE = 32;

	float vertDiff = 8192/128;

	float LOD_range = 8192/128;


	public override void _Ready()
	{
		position = new Vector3(0,0,0);
		pointsPosition = new Vector3(0,0,0);
		planeRef = GetNode<Plane>(plane);
		mat = (ShaderMaterial)this.GetSurfaceOverrideMaterial(0);
		//initMesh();
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

	private void initMesh() {
		int meshSize = Mathf.CeilToInt(renderDist) * 2;
		
		// Initialize the ArrayMesh.
		var arrMesh = new ArrayMesh();
		var arrays = new Godot.Collections.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = makePlane();

		// Create the Mesh.
		arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		this.Mesh = arrMesh;
	}

	private Vector3[] makePlane(){
		List<Vector3> verts = new List<Vector3>();
		float size = vertDiff;
		float LOD = 1;
		makeSquare(ref verts, 0, 0, 0, size);
		for (float i = vertDiff; i < renderDist; i += size * LOD) {
			if (i > Mathf.Pow(LOD_range, LOD)) {
				LOD += 1;
				size *= 2;
			}
			makePerimiter(ref verts, i, -LOD * 10, i, size * LOD);
		}
		
		GD.Print(verts.Count);
		return verts.ToArray();
	}

	private void makePerimiter(ref List<Vector3> verts, float x, float y, float z, float size) {
		Vector3 SW = new Vector3(-x, y, -z);
		Vector3 SE = new Vector3( x, y, -z);
		Vector3 NW = new Vector3(-x, y,  z);
		Vector3 NE = new Vector3( x, y,  z);
		makeStrip(ref verts, SW, SE, size);
		makeStrip(ref verts, NW, NE, size);
		makeStrip(ref verts, SW, NW, size);
		makeStrip(ref verts, SE, NE, size);

	}

	private void makeStrip(ref List<Vector3> verts, Vector3 start, Vector3 end, float size) {
		Vector3 move = (end - start).Normalized() * size;
		Vector3 point;
		for (point = start; end.DistanceSquaredTo(point) >= size * size; point += move) {
			makeSquare(ref verts, point.X, point.Y, point.Z, size);
		}
		makeSquare(ref verts, point.X, point.Y, point.Z, size);
	}

	private void makeSquare(ref List<Vector3> verts, float x, float y, float z, float size) {
		float halfSize = size / 2.0f;
		Vector3 SW = new Vector3(x - halfSize, y, z - halfSize);
		Vector3 SE = new Vector3(x + halfSize, y, z - halfSize);
		Vector3 NW = new Vector3(x - halfSize, y, z + halfSize);
		Vector3 NE = new Vector3(x + halfSize, y, z + halfSize);
		verts.Add(SE);
		verts.Add(NE);
		verts.Add(NW);

		verts.Add(SE);
		verts.Add(NW);
		verts.Add(SW);
	}

	public Vector3 addNoise(Vector3 point) {
		return point + new Vector3(noiseHash(point.X), noiseHash(point.Y), noiseHash(point.Z));
	}

	private float noiseHash(float val) {
		return fmod(((val * 2531) / 1289), 1.0f);
	}

	public override void _Process(double delta)
	{
		updatePoints();
		updatePosition(delta);
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
			return i + (Mathf.Floor(Mathf.Abs(i) / m)) * m;
		}else{
			return i - (Mathf.Floor(Mathf.Abs(i) / m)) * m;
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
