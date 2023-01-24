using Godot;
using System;

public partial class LiftSurface : DragSurface
{
   public float liftCoefficient = 0.01f;
   public new void applyPlaneEffectorForce(PhysicsDirectBodyState3D state){
      base.applyPlaneEffectorForce(state);
      state.ApplyImpulse(this.GlobalTransform.basis.y * //Direction of lift
      state.LinearVelocity.Normalized().Dot(-this.GlobalTransform.basis.z) *  // Change lift based on speed direction
      state.LinearVelocity.Length() * liftCoefficient // Amount of lift based on speed
      , this.Transform.origin); // Location to provide lift
   }
}