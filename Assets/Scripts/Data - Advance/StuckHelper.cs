// StuckHelper
public struct StuckHelper
{
	public float wallStuckTime;

	public float gndStuckTime;

	public float obstStuckTime;

	public bool inStuck;

	public void Initialize()
	{
		wallStuckTime = 0f;
		obstStuckTime = 0f;
		inStuck = false;
	}
}
