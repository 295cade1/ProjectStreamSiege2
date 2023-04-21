using Godot;
using System;

public partial class Thruster : Node3D, PlaneEffector, ControlSubscriber
{
	[Export]
	public float thrusterForce = 120101.0f;
	private float thrustAmount = 0;

	Controller controller;

	public (Vector3, Vector3) applyPlaneEffectorForce(PhysicsDirectBodyState3D state){
		Vector3 force = -this.GlobalTransform.Basis.Z * thrusterForce * controller.getControlValue(Controller.ControlType.Power);
		//GD.Print("Thrusting" + force);
		return (force * state.Step, this.GlobalPosition - state.Transform.Origin);
	}

	public void setController(Controller controller) {
		this.controller = controller;
	}
}