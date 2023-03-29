using Godot;
using System;

public partial class ControlSurface : DragSurface
{
	[Export]
	public float controlCoefficient = 2.0f;
	private float controlAmount = 0.0f;
	[Export]
	private float maxControl = 45.0f;

	[Export]
	public string positiveAction = "";
	[Export]
	public string negativeAction = "";

	[Export]
	public float smoothing = 0.9f;

	[Export]
	public Curve controlCurve;

	public override Vector3 getSurfaceForce(Vector3 velocity){
		float angleOfAttack = orthographicProjection(this.GlobalTransform.Basis.X, velocity.Normalized()).SignedAngleTo(orthographicProjection(this.GlobalTransform.Basis.X, -this.GlobalTransform.Basis.Z.Normalized()), this.GlobalTransform.Basis.X.Normalized()) / (2 * Mathf.Pi) * 360;
		controlAmount = Mathf.Clamp(calculateActivation() * (1 - smoothing) + controlAmount * smoothing,-1.0f,1.0f);
		float controlAngle = controlAmount * maxControl;
		float totalAngle = ((angleOfAttack + controlAngle) * (2 * Mathf.Pi) / 360);
		float controlForce = Mathf.Sin(totalAngle) * 0.5f * AIRDENSITY * velocity.Length() * velocity.Length() * y_area * drag_coefficient;
		Vector3 control = this.GlobalTransform.Basis.Y.Normalized() * controlForce;

		//GD.Print(this.Name + "Controlling: " + controlForce + "Angles: (AOA, control, Total)" + angleOfAttack + " " + controlAngle + " " + totalAngle + "ControlActivation" + controlAmount);
		
		return control + base.getSurfaceForce(velocity);
	}

	private Vector3 orthographicProjection(Vector3 planeNormal, Vector3 point){
		return point - point.Project(planeNormal);
	}
	public float calculateActivation(){
		return (Input.GetActionStrength(positiveAction) - Input.GetActionStrength(negativeAction));
	}


}
