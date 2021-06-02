// DriftGauge
public struct DriftGauge
{
	public float gauge;

	public bool progressOn;

	public float progressTime;

	public float progress;

	public float lastProgress;

	public float savedProgress;

	public void Initialize()
	{
		gauge = 0f;
		progressOn = false;
		progress = 0f;
		lastProgress = 0f;
		savedProgress = 0f;
	}
}
