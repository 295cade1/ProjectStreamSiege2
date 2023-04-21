using Godot;
using System;

public partial class ControlSurface : DragSurface, ControlSubscriber
{
	[Export]
	public float controlCoefficient = 2.0f;
	[Export]
	private float maxControl = 45.0f;

	[Export(PropertyHint.Enum, "No:1,Yes:-1")]
	public int flip = 1;

	Controller controller;

	[Export]
	public Controller.ControlType inputType { get; set; }



	public override Vector3 getSurfaceForce(Vector3 velocity){
		float angleOfAttack = orthographicProjection(this.GlobalTransform.Basis.X, velocity.Normalized()).SignedAngleTo(orthographicProjection(this.GlobalTransform.Basis.X, -this.GlobalTransform.Basis.Z.Normalized()), this.GlobalTransform.Basis.X.Normalized()) / (2 * Mathf.Pi) * 360;
		

		float controlAngle = controller.getControlValue(inputType) * maxControl * flip;

		float totalAngle = ((angleOfAttack + controlAngle) * (2 * Mathf.Pi) / 360);
		float controlForce = Mathf.Sin(totalAngle) * 0.5f * AIRDENSITY * velocity.LengthSquared() * y_area * drag_coefficient;
		Vector3 control = this.GlobalTransform.Basis.Y.Normalized() * controlForce;

		//GD.Print(this.Name + "Controlling: " + controlForce + "Angles: (AOA, control, Total)" + angleOfAttack + " " + controlAngle + " " + totalAngle + "ControlActivation" + controller.getControlValue(inputType));
		
		return control + base.getSurfaceForce(velocity);
	}

	private Vector3 orthographicProjection(Vector3 planeNormal, Vector3 point){
		return point - point.Project(planeNormal);
	}
	//public float calculateActivation(){
	//	return (Input.GetActionStrength(positiveAction) - Input.GetActionStrength(negativeAction));
	//}
	public void setController(Controller controller) {
		this.controller = controller;
	}
}
