using Godot;
using System;

public abstract partial class Surface : Node3D, PlaneEffector
{
	private Vector3 previous_position;
	public void applyPlaneEffectorForce(PhysicsDirectBodyState3D state, RigidBody3D obj){
		if (previous_position == new Vector3(0,0,0)) {updatePrevousPosition();}

		state.ApplyImpulse(
			(applySurfaceForce(getVelocity(state))) * state.Step,
			this.GlobalPosition - obj.GlobalPosition);
		GD.Print("Offset: " + (this.GlobalPosition - obj.GlobalPosition));
		updatePrevousPosition();
	}

	public virtual Vector3 applySurfaceForce(Vector3 velocity){ return new Vector3(0,0,0); }

	protected Vector3 getVelocity(PhysicsDirectBodyState3D state){
		return (this.GlobalPosition - previous_position) / state.Step;
	}
	private void updatePrevousPosition(){
		previous_position = this.GlobalPosition;
	}
}