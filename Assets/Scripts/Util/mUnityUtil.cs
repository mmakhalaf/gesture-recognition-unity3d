using UnityEngine;
using System.Collections;

public class mUnityUtil
{
	public static float clampAngle(float angle)
	{
		return (angle >= 180 ? 360-angle : angle);
	}
	
	public static Vector3 clampAngle(Vector3 angle)
	{
		angle.x = clampAngle(angle.x);
		angle.y = clampAngle(angle.y);
		angle.z = clampAngle(angle.z);
		return angle;
	}
}
