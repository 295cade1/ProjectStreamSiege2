using Godot;
using System;

public abstract partial class Surface : Node3D, PlaneEffector
{
	private Vector3 previousPosition;
	//Applies the calculated surface forces
	public void applyPlaneEffectorForce(PhysicsDirectBodyState3D state, RigidBody3D obj){
		if (previousPosition == new Vector3(0,0,0)) {updatePrevousPosition(state);}

		state.ApplyForce(
			(getSurfaceForce(getVelocity(state))) * state.Step * 0.558f,
			this.GlobalPosition - state.Transform.origin);
		updatePrevousPosition(state);
	}

	//returns the force vector to be applied to this surface (to be overwritten)
	public virtual Vector3 getSurfaceForce(Vector3 velocity){ return new Vector3(0,0,0); }

	//Returns the velocity of this surface in global space
	protected Vector3 getVelocity(PhysicsDirectBodyState3D state){
		return (getGlobalOffset(state) - previousPosition) / state.Step + state.LinearVelocity;
	}
	//updates previousPosition with the current offset in global space
	private void updatePrevousPosition(PhysicsDirectBodyState3D state){
		previousPosition = getGlobalOffset(state);
	}
	//Returns the offset from the center of the state in global space
	private Vector3 getGlobalOffset(PhysicsDirectBodyState3D state){
		return this.GlobalPosition - state.Transform.origin;
	}
}