using Godot;
using System;

public partial class Thruster : Node3D, PlaneEffector
{
	public float thrusterForce = 120101.0f * 30f;

	[Export]
	public string positiveAction = "";
	public void applyPlaneEffectorForce(PhysicsDirectBodyState3D state){
		Vector3 force = -this.GlobalTransform.Basis.Z * thrusterForce * calculateActivation();
		//GD.Print("Thrusting" + force);
		state.ApplyImpulse(
			force * state.Step,
			this.GlobalPosition - state.Transform.Origin);
	}

	public float calculateActivation(){
		return Input.GetActionStrength(positiveAction);
	}
	
}