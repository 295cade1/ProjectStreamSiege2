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
		float dragAmount = 0.5f * AIRDENSITY * velocity.Length() * velocity.Length() * drag_coefficient;
		Vector3 dragDirection = (-this.GlobalTransform.Basis.X.Normalized() * velocity.Normalized().Dot(this.GlobalTransform.Basis.X.Normalized()) * x_area) +
									(-this.GlobalTransform.Basis.Y.Normalized() * velocity.Normalized().Dot(this.GlobalTransform.Basis.Y.Normalized()) * y_area) +
									(-this.GlobalTransform.Basis.Z.Normalized() * velocity.Normalized().Dot(this.GlobalTransform.Basis.Z.Normalized()) * z_area);
		Vector3 dragImpulse = dragDirection * dragAmount;
	
		//GD.Print("Dragging" + "DragImpulse" + dragImpulse + "DragAmount" + dragAmount + "dragArea" + dragArea);
		
		return dragImpulse + base.getSurfaceForce(velocity);
	}
}
