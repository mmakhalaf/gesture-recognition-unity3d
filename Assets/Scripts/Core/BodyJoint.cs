using UnityEngine;
using System.Collections;

public class BodyJoint : MonoBehaviour
{
	public enum JointType
	{
		SHOULDER_RIGHT = 0,
		SHOULDER_LEFT = 1,
		SHOULDER_CENTRE = 2,
		SPINE = 3,
		HIP_RIGHT = 4,
		HIP_LEFT = 5,
		HIP_CENTRE = 6,
		ELBOW_RIGHT = 7,
		ELBOW_LEFT = 8,
		WRIST_RIGHT = 9,
		WRIST_LEFT = 10,
		HAND_RIGHT = 11,
		HAND_LEFT = 12,
		KNEE_RIGHT = 13,
		KNEE_LEFT = 14,
		ANKLE_RIGHT = 15,
		ANKLE_LEFT = 16,
		FOOT_RIGHT = 17,
		FOOT_LEFT = 18,
		HEAD = 19
	}
	
	public JointType jointType;
	public mConstraint jointConstraint;
	
	void Awake()
	{
		jointConstraint.initialize();
	}
	
	public void UpdatePosition(Vector3 pos)
	{
		transform.localPosition = pos;
	}
	
	public void UpdateLocalRotation(Quaternion localRot)
	{
		transform.localRotation = jointConstraint.clamp(localRot);
	}
}

[System.Serializable]
public class mConstraint
{
	public Vector3 minRotation = new Vector3(-180,-180,-180), maxRotation = new Vector3(180,180,180);
	
	Quaternion[] minQuat, maxQuat;
	
	public void initialize()
	{
		minQuat = new Quaternion[3];
		maxQuat = new Quaternion[3];
		for(int i = 0; i < 3; i++)
		{
			Vector3 axis = Vector3.zero;
			axis[i] = 1;
			
			minQuat[i] = Quaternion.AngleAxis(minRotation[i], axis);
			maxQuat[i] = Quaternion.AngleAxis(maxRotation[i], axis);
		}
	}
	
	public Quaternion clamp(Quaternion rot)
	{
		Quaternion[] axesrot = new Quaternion[3];
		
		for(int i = 0; i < 3; i++)
		{
			float range = maxRotation[i] - minRotation[i];
			Quaternion minQ = minQuat[i];
			Quaternion maxQ = maxQuat[i];
			
			Vector3 axis = Vector3.zero;
			axis[i] = 1;
			Quaternion axisRot = Quaternion.AngleAxis(rot.eulerAngles[i], axis);
			
			float dMin = Quaternion.Angle(minQ, axisRot);
			float dMax = Quaternion.Angle(maxQ, axisRot);
			
			if (dMin < range && dMax < range)
			{
				axesrot[i] = axisRot;
			}
			else
			{
				axesrot[i] = Quaternion.identity;
			}
		}
		return axesrot[0]*axesrot[1]*axesrot[2];
	}
}