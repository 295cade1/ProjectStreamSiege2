using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerController : Controller 
{
	float pitch, yaw, roll, power;


	[Export(PropertyHint.Range, "0,1,0.01")]
	public float pitch_smoothing = 0.9f;
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float yaw_smoothing = 0.9f;
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float roll_smoothing = 0.9f;
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float power_smoothing = 0.9f;

	private bool joystick = false;

	protected override void updateValues(double delta) {
		delta = delta * 60;
		if (Input.GetConnectedJoypads().Count > 0) {
			pitch = Input.GetAxis("PitchDown", "PitchUp");
			roll = Input.GetAxis("RollRight", "RollLeft");
		} else {
			pitch = smooth(Input.GetAxis("PitchDown", "PitchUp"), pitch, pitch_smoothing * (float)delta);
			roll = smooth(Input.GetAxis("RollRight", "RollLeft"), roll, roll_smoothing * (float)delta);
		}
		yaw = smooth(Input.GetAxis("YawRight", "YawLeft"), yaw, yaw_smoothing * (float)delta);
		power = smooth(Input.GetActionStrength("Thrust"), power, power_smoothing * (float)delta);

	}

	private float smooth(float currentValue, float newValue, float smoothing) {
		return Mathf.Clamp(newValue * (1 - smoothing) + currentValue * smoothing,-1.0f,1.0f);
	}

	public override float getControlValue(Controller.ControlType type) {
		switch (type) {
			case Controller.ControlType.Pitch:
				return pitch;
			case Controller.ControlType.Roll:
				return roll;
			case Controller.ControlType.Yaw:
				return yaw;
			case Controller.ControlType.Power:
				return power;
		}
		return 0;
	}
}

