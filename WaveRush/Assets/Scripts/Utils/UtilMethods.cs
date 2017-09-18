using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UtilMethods
{
	public static int RandSign()
	{
		return Random.value < 0.5 ? 1 : -1;
	}

	public static Vector2 RadianToVector2(float radian)
	{
		return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
	}

	public static Vector2 DegreeToVector2(float degree)
	{
		return RadianToVector2(degree * Mathf.Deg2Rad);
	}

	public static Vector2 RandomOffsetVector2(Vector3 vec, float offset)
	{
		return new Vector3 (Random.Range (-offset, offset),
			Random.Range (-offset, offset)) + vec;
	}
}

