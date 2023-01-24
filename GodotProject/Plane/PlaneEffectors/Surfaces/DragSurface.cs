using Godot;
using System;

public partial class DragSurface : Node3D, PlaneEffector
{
   public float x_drag = 0.1f;
   public float y_drag = 10.0f;
   public float z_drag = 0.05f;
   public void applyPlaneEffectorForce(PhysicsDirectBodyState3D state){
      state.ApplyImpulse(
         Mathf.Max(0.0f,state.LinearVelocity.Normalized().Dot(-this.GlobalTransform.basis.z)) * z_drag *  this.GlobalTransform.basis.z +
         Mathf.Max(0.0f,state.LinearVelocity.Normalized().Dot( this.GlobalTransform.basis.z)) * z_drag * -this.GlobalTransform.basis.z +
         Mathf.Max(0.0f,state.LinearVelocity.Normalized().Dot(-this.GlobalTransform.basis.x)) * x_drag *  this.GlobalTransform.basis.x +
         Mathf.Max(0.0f,state.LinearVelocity.Normalized().Dot( this.GlobalTransform.basis.x)) * x_drag * -this.GlobalTransform.basis.x +
         Mathf.Max(0.0f,state.LinearVelocity.Normalized().Dot(-this.GlobalTransform.basis.y)) * y_drag *  this.GlobalTransform.basis.y +
         Mathf.Max(0.0f,state.LinearVelocity.Normalized().Dot( this.GlobalTransform.basis.y)) * y_drag * -this.GlobalTransform.basis.y
      , this.Transform.origin);
   }
}