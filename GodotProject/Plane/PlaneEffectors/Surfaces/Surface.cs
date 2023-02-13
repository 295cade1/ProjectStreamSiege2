using Godot;
using System;

public abstract partial class Surface : Node3D, PlaneEffector
{
	private Vector3 previousPosition;
	private MeshInstance3D debugLines;
	//Applies the calculated surface forces
	public void applyPlaneEffectorForce(PhysicsDirectBodyState3D state){
		if (previousPosition == new Vector3(0,0,0)) {updatePrevousPosition(state);}
		Vector3 addedVelocity = (getSurfaceForce(getVelocity(state))) * state.Step;
		Vector3 forcePosition = this.GlobalPosition - state.Transform.Origin;
		state.ApplyImpulse(addedVelocity, forcePosition);
		if (debugLines != null){
			debugLines.QueueFree();
		}
		
		debugLines = drawDebugForceLines(this.GlobalPosition, this.GlobalPosition + addedVelocity / 1000f);
		
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
		return this.GlobalPosition - state.Transform.Origin;
	}

	private MeshInstance3D drawDebugForceLines(Vector3 start, Vector3 end){
		MeshInstance3D meshInstance = new MeshInstance3D();
		ImmediateMesh immediateMesh = new ImmediateMesh();
		
		meshInstance.Mesh = immediateMesh;
		meshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

		immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		immediateMesh.SurfaceAddVertex(start);
		immediateMesh.SurfaceAddVertex(end);
		immediateMesh.SurfaceEnd();
		
		GetTree().Root.AddChild(meshInstance);
		
		return meshInstance;
	}
}