// CollisionState
public struct CollisionState
{
	public bool kartCollide;

	public float kartCollideVel;

	public bool kartCollideDominant;

	public bool shock;

	public float shockVel;

	public bool hop;

	public float unmovingTime;

	public void Initialize()
	{
		kartCollide = false;
		kartCollideVel = 0f;
		kartCollideDominant = false;
		shock = false;
		hop = false;
		unmovingTime = 0f;
	}
}
