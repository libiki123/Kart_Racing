// Control
public struct Control
{
	public float accel;

	public float brake;

	public bool accelBrakeSwap;

	public float steer;

	public bool wheelFlip;

	public bool wheelDevil;

	public float stayTime;

	public float steerAngle;

	public float oldSteerAngle;

	public float getRealAccel()
	{
		return (!accelBrakeSwap) ? accel : brake;
	}

	public float getRealBrake()
	{
		return (!accelBrakeSwap) ? brake : accel;
	}

	public float getRealSteer()
	{
		return ((!wheelFlip && !wheelDevil) ? 1f : (-1f)) * steer;
	}

	public void Initialize()
	{
		accel = 0f;
		brake = 0f;
		accelBrakeSwap = false;
		steer = 0f;
		wheelFlip = false;
		wheelDevil = false;
		stayTime = 0f;
		steerAngle = 0f;
		oldSteerAngle = 0f;
	}
}
