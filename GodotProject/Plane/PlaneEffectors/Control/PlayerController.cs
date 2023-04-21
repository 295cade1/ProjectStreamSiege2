using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerController : Controller 
{
	float pitch = 0.0f, yaw = 0.0f, roll = 0.0f, power = 0.0f;


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
		GD.Print("Pitch: " + pitch + " Yaw: " + yaw + " Roll: " + roll + " Thrust: " + power);
		pitch = smooth(pitch, Input.GetAxis("PitchDown", "PitchUp" ), 1 - (pitch_smoothing * (float)delta) * 50f);
		roll  = smooth(roll,  Input.GetAxis("RollRight", "RollLeft"), 1 - (roll_smoothing  * (float)delta) * 50f);
		yaw   = smooth(yaw,   Input.GetAxis("YawRight" , "YawLeft" ), 1 - (yaw_smoothing   * (float)delta) * 50f);
		if (Input.GetAxis("DownThrust", "Thrust") != 0){
			power = Mathf.Lerp(power, (Input.GetAxis("DownThrust"  ,"Thrust") + 1) / 2.0f, power_smoothing * (float)delta);
		}
		

	}

	private float smooth(float currentValue, float newValue, float smoothing) {
		float val = Mathf.Clamp(newValue * (1 - smoothing) + currentValue * smoothing,-1.0f,1.0f);
		if (Mathf.Abs(val) < 0.01f) {
			return 0;
		}else{
			return val;
		}
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

