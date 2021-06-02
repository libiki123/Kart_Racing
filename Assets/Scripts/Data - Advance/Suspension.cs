// Suspension
using UnityEngine;

public struct Suspension
{
	public Vector2[] wheelOff;

	public Vector3 contactN;

	public bool[] wheelContact;

	public Vector3[] wheelContactN;

	public float maxTravel;

	public float[] travel;

	public float[] deltaTravel;

	public void InitVector2(ref Vector2 v, float x, float y)
	{
		v.x = x;
		v.y = y;
	}

	public void Initialize()
	{
		wheelOff = new Vector2[4];
		for (int i = 0; i < 4; i++)
		{
			InitVector2(ref wheelOff[i], (i % 2 != 0) ? 1f : (-1f), (i / 2 != 0) ? (-1f) : 1f);
		}
		wheelContact = new bool[4];
		wheelContactN = new Vector3[4];
		maxTravel = 0.3f;
		travel = new float[4]
		{
			0.3f,
			0.3f,
			0.3f,
			0.3f
		};
		deltaTravel = new float[4];
	}
}
