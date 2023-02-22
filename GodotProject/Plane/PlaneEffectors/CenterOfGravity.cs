using Godot;
using System;

public partial class CenterOfGravity : Node3D, PlaneEffector
{
	public float thrusterForce = 8028.58f * 1000.0f;

	public void applyPlaneEffectorForce(PhysicsDirectBodyState3D state){
		Vector3 force = Vector3.Down * 9.8f * 18143.72f;
		//GD.Print("Falling" + force);

		state.ApplyImpulse(
			force * state.Step,
			this.GlobalPosition - state.Transform.Origin);
	}
}