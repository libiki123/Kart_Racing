// External
using UnityEngine;

public struct External
{
	public bool slip;

	public float dragFactor;

	public float compensationDragFactor;

	public float wheelFactor;

	public Vector3 annexForce;

	public Vector3 force;

	public Vector3 torque;

	public float gravityFactor;

	public float speedLimit;

	public float upDownTime;

	public float upDownLastTime;

	public float upDownInterval;

	public Vector3 upDownForce;

	public uint upDownForceIndex;

	public Vector3 liftVel;

	public void Initialize()
	{
		slip = false;
		dragFactor = 1f;
		compensationDragFactor = 1f;
		wheelFactor = 1f;
		annexForce = Vector3.zero;
		force = Vector3.zero;
		torque = Vector3.zero;
		upDownTime = 0f;
		upDownLastTime = 0f;
		gravityFactor = 1f;
		speedLimit = 0f;
	}
}
