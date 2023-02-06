using Godot;
using System;

public partial class LiftSurface : DragSurface
{
	[Export]
	public float liftCoefficient = 1.6f;
	[Export]
	public Curve liftCurve;
	public override Vector3 getSurfaceForce(Vector3 velocity){
		float angleOfAttack = velocity.Normalized().AngleTo(-this.GlobalTransform.basis.z);
		float liftAmount = 0.5f * velocity.LengthSquared() * AIRDENSITY * y_area * liftCurve.Sample(angleOfAttack/20.0f) 
		* liftCoefficient;

		Vector3 lift = this.GlobalTransform.basis.y * liftAmount;

		//GD.Print("Lifting: " + lift);
		
		return lift + base.getSurfaceForce(velocity);
	}
}