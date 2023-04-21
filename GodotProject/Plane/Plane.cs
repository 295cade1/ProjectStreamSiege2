using Godot;
using System;
using System.Collections.Generic;

public partial class Plane : RigidBody3D
{
	private PlaneEffector[] planeEffectors;
	private Vector3 speed;
	private Vector3 accelleration;
	[Export]
	private bool isLocked = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		speed = new Vector3(0,0,0);
		planeEffectors = getAllEffectors();
		speed = LinearVelocity;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state) {
		if(isLocked) {state.LinearVelocity = new Vector3(speed.X * 0.999f ,state.LinearVelocity.Y ,speed.Z * 0.999f); }
		List<(Vector3, Vector3)> forces = new List<(Vector3, Vector3)>();
		foreach (PlaneEffector effector in planeEffectors) {
			forces.Add(effector.applyPlaneEffectorForce(state));
		}
		foreach ((Vector3 force, Vector3 position) in forces) {
			state.ApplyImpulse(force, position);
		}
		accelleration = state.LinearVelocity - speed;
		speed = state.LinearVelocity;
		GD.Print("AIRSPEED:" + state.LinearVelocity.Length() + "ALT: " + state.Transform.Origin.Y);
		if (isLocked) {state.LinearVelocity = new Vector3(0, state.LinearVelocity.Y, 0); }

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

	public Vector3 getSpeed() {
		return speed;
	}
	public Vector3 getAccelleration() {
		return accelleration;
	}
}
