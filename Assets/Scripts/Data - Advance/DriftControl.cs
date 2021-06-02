// DriftControl
public struct DriftControl
{
	public bool slipMode;

	public float slipTime;

	public bool forceSlip;

	public bool trigger;

	public float triggerTime;

	public void Initialize()
	{
		slipMode = false;
		slipTime = 0f;
		forceSlip = false;
		trigger = false;
		triggerTime = 0f;
	}

	public override string ToString()
	{
		return slipMode + " " + slipTime + " " + forceSlip + " " + trigger + " " + triggerTime;
	}
}
