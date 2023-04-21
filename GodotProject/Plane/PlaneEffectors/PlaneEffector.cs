using Godot;
using System;

interface PlaneEffector {
   (Vector3, Vector3) applyPlaneEffectorForce(PhysicsDirectBodyState3D state);
}