using Godot;
using System;

public partial class ControlSurface : DragSurface
{
	public float controlCoefficient = 2.0f;
	private float controlAmount = 0.0f;

	[Export]
	public string positiveAction = "";
	[Export]
	public string negativeAction = "";

	[Export]
   public Curve controlCurve;

   	[Export]
   public Curve liftCurve;
	public override Vector3 applySurfaceForce(Vector3 velocity){
		float angleOfAttack = velocity.Normalized().AngleTo(-this.GlobalTransform.basis.z);
		float controlAmount = calculateActivation() * 0.5f * velocity.LengthSquared() * AIRDENSITY * y_area * controlCoefficient * liftCurve.Sample(angleOfAttack/20.0f);

      	Vector3 control = this.GlobalTransform.basis.y * controlAmount;

		GD.Print("Controlling: " + control + "ControlActivation" + calculateActivation());
		
		return control + base.applySurfaceForce(velocity);
	}
	public float calculateActivation(){
		return (Input.GetActionStrength(positiveAction) - Input.GetActionStrength(negativeAction));
	}


}