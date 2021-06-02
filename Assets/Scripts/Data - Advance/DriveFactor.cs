// DriveFactor
public struct DriveFactor
{
	public float speedLimit;

	public float frontGripFactor;

	public float rearGripFactor;

	public float driftSlipFactor;

	public float backFrontGripFactor;

	public float backRearGripFactor;

	public float backDriftSlipFactor;

	public float betaCut;

	public float onTriggerSteerFactor;

	public float onDriftSteerFactor;

	public float onRestTimeSteerFactor;

	public void Initialize()
	{
		frontGripFactor = 0f;
		rearGripFactor = 0f;
		driftSlipFactor = 1f;
		backFrontGripFactor = 0f;
		backRearGripFactor = 0f;
		backDriftSlipFactor = 1f;
		betaCut = 1f;
		onDriftSteerFactor = 1f;
		onRestTimeSteerFactor = 1f;
		onTriggerSteerFactor = 1f;
		speedLimit = 120f;
	}
}
