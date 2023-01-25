using Godot;
using System;
using System.Collections.Generic;

public partial class Plane : RigidBody3D
{
   PlaneEffector[] planeEffectors;
   float Z_speed = 0.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

      planeEffectors = getAllEffectors();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state) {
      //state.LinearVelocity += (new Vector3(0,0,Z_speed) * 0.997f);
      foreach (PlaneEffector effector in planeEffectors) {
         effector.applyPlaneEffectorForce(state, this);
      }
      Z_speed = state.LinearVelocity.z;
      GD.Print(state.LinearVelocity);
      //state.LinearVelocity = new Vector3(state.LinearVelocity.x, state.LinearVelocity.y, 0);

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
}
