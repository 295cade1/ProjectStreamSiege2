using Godot;
using System;
using System.Collections.Generic;

public partial class Plane : RigidBody3D
{
	private PlaneEffector[] planeEffectors;
	private float zSpeed;
	[Export]
	private bool isLocked = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		zSpeed = 0;
		planeEffectors = getAllEffectors();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state) {
		if(isLocked) {state.LinearVelocity += new Vector3(0 ,0 ,zSpeed * 0.997f); }
		foreach (PlaneEffector effector in planeEffectors) {
			effector.applyPlaneEffectorForce(state, this);
		}
		zSpeed = state.LinearVelocity.z;
		if (isLocked) {state.LinearVelocity = new Vector3(state.LinearVelocity.x, state.LinearVelocity.y, 0); }

	}

	private PlaneEffector[] getAllEffectors() {
		List<PlaneEffector> effectors = new List<PlaneEffector>();
		foreach (Node child in getAllChildren(this)) {
			if (child is PlaneEffector) {
				effectors.Add(child as PlaneEffector);
			}
		}
		return effectors.ToArray();
	}

	private List<Node> getAllChildren(Node curNode) {
		List<Node> children = new List<Node>(curNode.GetChildren());
		List<Node> returnArray = new List<Node>();
		foreach (Node child in children) {
			returnArray.AddRange(getAllChildren(child));
		}
		returnArray.AddRange(children);
		return returnArray;
	}

	public float getSpeed() {
		return zSpeed;
	}
}
