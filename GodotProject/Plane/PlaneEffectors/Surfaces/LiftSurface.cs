using Godot;
using System;

public partial class LiftSurface : DragSurface
{
	[Export]
	public float liftCoefficient = 1.6f;

	[Export]
	public float ANGLEOFATTACKMAX = 30.0f;

	[Export]
	public float WINGAREA = 30.0f;

	[Export]
	public Curve liftCurve;
	public override Vector3 getSurfaceForce(Vector3 velocity){
		float angleOfAttack = velocity.Normalized().SignedAngleTo(-this.GlobalTransform.Basis.Z, this.GlobalTransform.Basis.X)  / (2 * Mathf.Pi) * 360;
		float liftAmount = 0.5f * velocity.Length() * velocity.Length() * AIRDENSITY * WINGAREA * Mathf.Clamp(liftCurve.Sample(Mathf.Clamp(angleOfAttack/ANGLEOFATTACKMAX, 0.0f, 1.0f)),0.0f,1.0f) * liftCoefficient;
		

		Vector3 lift = this.GlobalTransform.Basis.Y * liftAmount;

		//GD.Print("Lifting: " + lift);
		
		return lift + base.getSurfaceForce(velocity);
	}
}