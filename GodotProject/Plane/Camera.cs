using Godot;
using System;

public partial class Camera : Camera3D
{
	[Export]
	Plane plane;

	[Export]
	Vector3 globalDirection = new Vector3(0,0,-1);

	[Export]
	float targetPriority = 0.25f;

	[Export]
	float cameraDistance = 20.0f;

	[Export]
	float maxCameraDistance = 30.0f;

	[Export]
	float cameraUpOffset = 5.0f;

	private Vector3 previousAccellerationOffset = new Vector3(0,0,0);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		TopLevel = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

		float planeSpeed = getPlaneSpeedAmount();
		this.Fov = Mathf.Lerp(75, 100, planeSpeed);

		Vector3 planeAccellOffset = getPlaneAccellerationOffset(delta);

		float distance = Mathf.Lerp(cameraDistance, maxCameraDistance, planeSpeed) + planeAccellOffset.LengthSquared();

		Vector3 camDir = getCameraTargetDirection();
		Vector3 camPos = getCameraPos(camDir, distance) + planeAccellOffset;
		this.LookAtFromPosition(camPos, camPos + camDir, Vector3.Up);


	}

	private float getPlaneSpeedAmount() {
		return Mathf.Clamp(plane.getSpeed().Length() / 700.0f, 0.0f, 1.0f);
	}

	private Vector3 getPlaneAccellerationOffset(double delta) {
		Vector3 retVal = previousAccellerationOffset.Lerp(plane.getAccelleration(), (float)delta);
		previousAccellerationOffset = retVal;
		return retVal;
	}

	private Vector3 getCameraPos(Vector3 targetDir, float distance) {
		Vector3 cameraOffset = -targetDir * cameraDistance;
		Vector3 planePos = plane.GlobalTransform.Origin;
		return planePos + cameraOffset + new Vector3(0, cameraUpOffset, 0);
	}

	private Vector3 getCameraTargetDirection() {
		Vector3 planeDir = -plane.GlobalTransform.Basis.Z;
		return planeDir.Lerp(globalDirection, targetPriority);
	}
}
