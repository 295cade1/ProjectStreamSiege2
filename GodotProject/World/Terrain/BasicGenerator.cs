using Godot;
using System;

public partial class BasicGenerator : TerrainGenerator
{
   float velocity = 0;
   RandomNumberGenerator rand;
	public BasicGenerator(int index) : base(index){
      rand = new RandomNumberGenerator();
   }
	public override Vector3 getNextPoint(Vector3 position){
      position = position +  new Vector3(velocity, 0, -500/10);
      velocity = velocity * 0.9f + rand.RandfRange(-20,20);
      return position;
   }
}