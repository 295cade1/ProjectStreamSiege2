using Godot;
using System;

public partial class Thruster : Node3D, PlaneEffector
{
	public float thrusterForce = 130000.0f;

	[Export]
	public string positiveAction = "";
	public void applyPlaneEffectorForce(PhysicsDirectBodyState3D state, RigidBody3D obj){
		Vector3 force = -this.GlobalTransform.basis.z * thrusterForce * calculateActivation();
		GD.Print("Thrusting" + force);
		state.ApplyForce(
			force * state.Step * 558.0f,
			this.GlobalPosition - obj.GlobalPosition);
	}

	public float calculateActivation(){
		return Input.GetActionStrength(positiveAction);
	}

}