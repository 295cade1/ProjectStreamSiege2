using Godot;
using System;

interface PlaneEffector {
   void applyPlaneEffectorForce(PhysicsDirectBodyState3D state);
}