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
	public float smoothing = 0.9f;

	[Export]
	public Curve controlCurve;

	[Export]
	public float ANGLEOFATTACKMAX = 30.0f;

	[Export]
	public Curve liftCurve;
	public override Vector3 getSurfaceForce(Vector3 velocity){
		float angleOfAttack = velocity.Normalized().AngleTo(-this.GlobalTransform.Basis.Z) * 180 / Mathf.Pi;
		controlAmount = Mathf.Clamp(calculateActivation() * (1 - smoothing) + controlAmount * smoothing,-1.0f,1.0f);
		float controlForce = controlAmount * 0.5f * velocity.LengthSquared() * AIRDENSITY * y_area * controlCoefficient * liftCurve.Sample(Mathf.Clamp(angleOfAttack/ANGLEOFATTACKMAX, 0.0f, 1.0f));

		Vector3 control = this.GlobalTransform.Basis.Y * controlForce;

		//GD.Print("Controlling: " + control + "ControlActivation" + controlAmount);
		
		return control + base.getSurfaceForce(velocity);
	}
	public float calculateActivation(){
		return (Input.GetActionStrength(positiveAction) - Input.GetActionStrength(negativeAction));
	}


}