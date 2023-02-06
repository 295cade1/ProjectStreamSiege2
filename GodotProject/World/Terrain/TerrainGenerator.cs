using Godot;
using System;

public abstract partial class TerrainGenerator : Node
{
	[Export]
	private int width = 512;
	[Export]
	private int height = 512;

	public abstract Image getHeightmapImage(Vector3 position);

	public int getHeight() {
		return height;
	}
	public int getWidth() {
		return width;
	}
}