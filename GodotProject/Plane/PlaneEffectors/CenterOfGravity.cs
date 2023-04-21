using Godot;
using System;

public partial class CenterOfGravity : Node3D, PlaneEffector
{
	public (Vector3, Vector3) applyPlaneEffectorForce(PhysicsDirectBodyState3D state){
		Vector3 force = Vector3.Down * 9.8f * (1 / state.InverseMass);
		//GD.Print("Falling" + force);

		return (force * state.Step, new Vector3(0,0,0));

	}

}