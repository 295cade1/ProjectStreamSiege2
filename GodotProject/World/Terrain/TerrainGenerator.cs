using Godot;
using System;

public abstract partial class TerrainGenerator
{
	protected int index;
	public TerrainGenerator(int index){
		this.index = index;
	}
	public int getIndex(){
		return index;
	}
	public void setIndex(int index){
		this.index = index;
	}
	public abstract Vector3 getNextPoint(Vector3 position);
}