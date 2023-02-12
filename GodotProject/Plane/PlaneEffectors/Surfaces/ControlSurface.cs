using Godot;
using System;

public partial class ControlSurface : DragSurface
{
	[Export]
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
	public override Vector3 getSurfaceForce(Vector3 velocity){
		float angleOfAttack = velocity.Normalized().AngleTo(-this.GlobalTransform.basis.z) * 180 / Mathf.Pi;
		controlAmount = Mathf.Clamp(calculateActivation() * 0.1f + controlAmount * 0.90f,-1,1);
		float controlForce = controlAmount * 0.5f * velocity.LengthSquared() * AIRDENSITY * y_area * controlCoefficient * liftCurve.Sample(Mathf.Clamp(angleOfAttack/30.0f, 0.0f, 1.0f));

		Vector3 control = this.GlobalTransform.basis.y * controlForce;

		//GD.Print("Controlling: " + control + "ControlActivation" + calculateActivation());
		
		return control + base.getSurfaceForce(velocity);
	}
	public float calculateActivation(){
		return (Input.GetActionStrength(positiveAction) - Input.GetActionStrength(negativeAction));
	}


}