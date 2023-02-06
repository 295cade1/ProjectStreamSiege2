using Godot;
using System;
using System.Collections.Generic;

public partial class WormRiverGen : TerrainGenerator
{
	Image heightmap;
	List<Worm> worms;

	Vector3 lastPosition;

	[Export]
	int baseWormSize = 400;

	[Export]
	private Curve WORMDIGCURVE;
	
	public override void _Ready() {
		lastPosition = new Vector3(0,0,0);
		initWorms();
		initHeightmap();
	}

	private void initWorms() {
		Random rand = new Random();
		worms = new List<Worm>();
		worms.Add(new Worm(baseWormSize, ref rand));
	}

	private void initHeightmap() {
		heightmap = Image.Create(getWidth(), getHeight(), false, Image.Format.Rgb8);
		heightmap.Fill(new Color(0,0,0));
	}

	public override Image getHeightmapImage(Vector3 position) {
		updateWorms(getVelocity(position));
		updateHeightmap(getVelocity(position));
		return heightmap;
	}

	private Vector3 getVelocity(Vector3 position) {
		return position - lastPosition;
	}

	private void updateWorms(Vector3 position) {
		foreach (Worm worm in worms) {
			worm.update(position);
		}
	}

	private void updateHeightmap(Vector3 velocity) {
		heightmap.BlitRect(heightmap, new Rect2i(new Vector2i(0,0), getWidth(), getHeight()), new Vector2i((int)velocity.x, (int)velocity.z));
		for (int x = 0; x < getWidth(); x++) {
				float height = 1;
				foreach (Worm worm in worms) {
					GD.Print(Mathf.Clamp(worm.getDistance(x) / worm.getSize(), 0, 1));
					height -= 1 - WORMDIGCURVE.Sample(Mathf.Clamp(worm.getDistance(x) / worm.getSize(), 0, 1));
				}
				height = Mathf.Clamp(height, 0, 1);
				heightmap.SetPixel(x, 0, new Color(height, height, height));
		}
	}
	
	private class Worm {
		[Export]
		public float MAXVELOCITYCHANGEPERTICK = 0.1f;
		[Export]
		private float MAXVELOCITY = 20;
		
		private float position = 100;
		private float velocity = 0;
		private float size;

		private Random rand;

		public Worm(int size, ref Random rand) {
			this.rand = rand;
			this.size = size;
		}

		public void update(Vector3 planeVelocity) {
			updateVelocity(planeVelocity);
			updatePosition(planeVelocity);
		}

		private void updateVelocity(Vector3 planeVelocity) {
			velocity += ((rand.Next() % (MAXVELOCITYCHANGEPERTICK * 2)) - MAXVELOCITYCHANGEPERTICK) * planeVelocity.z;
			velocity %= MAXVELOCITY;
		}

		private void updatePosition(Vector3 planeVelocity) {
			position += velocity * planeVelocity.z;
		}
		public float getDistance(float position) {
			return Mathf.Abs(this.position - position);
		}
		public float getSize() {
			return size;
		}
	}
}