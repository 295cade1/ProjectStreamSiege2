using Godot;
using System;

public partial class DragSurface : Surface
{  
   [Export]
   public float x_area = 0.5f;
   [Export]
   public float y_area = 100f;
   [Export]
   public float z_area = 0.5f;
   [Export]
   public float drag_coefficient = 0.5f;
   protected static float AIRDENSITY = 1.204f; // Density of air
   public override Vector3 getSurfaceForce(Vector3 velocity){

      //To calculate the drag force of an object moving through a fluid use the formula: Fd = 1/2 * ρ * u² * A * Cd 
      //Where ρ is the liquid density, u is the relative velocity, A is the reference area and Cd is the drag coefficient.
      float dragArea = Mathf.Abs(velocity.Normalized().Dot(this.GlobalTransform.basis.x)) * x_area +
                       Mathf.Abs(velocity.Normalized().Dot(this.GlobalTransform.basis.y)) * y_area +
                       Mathf.Abs(velocity.Normalized().Dot(this.GlobalTransform.basis.z)) * z_area;
      float dragAmount = 0.5f * AIRDENSITY * velocity.LengthSquared() * dragArea * drag_coefficient;
      Vector3 dragImpulse = dragAmount * -velocity.Normalized();

      //GD.Print("Dragging" + "DragImpulse" + dragImpulse + "DragAmount" + dragAmount + "dragArea" + dragArea);
      
      return dragImpulse + base.getSurfaceForce(velocity);
   }
}