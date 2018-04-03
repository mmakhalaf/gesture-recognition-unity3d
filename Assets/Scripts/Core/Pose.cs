using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.Xml.Serialization;

public class Pose : mDistanceMeasurable
{
	// confidance 
	public List<float> data_confidence;
	
	[XmlArray("JointLocationsArray")]
	[XmlArrayItem("JointLocation")]
	public List<Vector3> joint_locations;
	
	[XmlArray("JointRelativeOrientationsArray")]
	[XmlArrayItem("JointRelativeOrientation")]
	public Quaternion[] joint_rotations;
	
	
	public Pose()
	{
		joint_locations = new List<Vector3>();
		data_confidence = new List<float>();
		joint_rotations = new Quaternion[20];
	}
	
	// convert joint data from x,y,z to joint orientations
	public void convertLocationToJointRotation()
	{
		Vector3 tForward, tUp, tRight;
		Quaternion tAtt, mainAtt = Quaternion.identity, shoulderAtt = Quaternion.identity;
		
		// main body, identity
		setJointRot(BodyJoint.JointType.HIP_CENTRE, Quaternion.identity);
		
		// spine
//		tUp = getJointLoc(BodyJoint.JointType.SHOULDER_CENTRE)-getJointLoc(BodyJoint.JointType.SPINE);
//		float ang = Vector3.Angle(tUp.normalized, Vector3.up);
//		tAtt = Quaternion.AngleAxis(ang, Vector3.right);
//		setJointRot(BodyJoint.JointType.SPINE, tAtt);
//		mainAtt = tAtt;
//		shoulderAtt = mainAtt;
		
		// center shoulder
		tRight = Quaternion.AngleAxis(180, Vector3.up) * (getJointLoc(BodyJoint.JointType.SHOULDER_LEFT) - getJointLoc(BodyJoint.JointType.SHOULDER_RIGHT));
		tUp = Vector3.up;
		tForward = Vector3.Cross(tUp, tRight);
		tAtt = Quaternion.LookRotation(tForward.normalized, tUp.normalized);
		setJointRot(BodyJoint.JointType.SHOULDER_CENTRE, tAtt);
		mainAtt = tAtt;
		shoulderAtt = mainAtt;
		
		//// left shoulder
		tForward = Quaternion.AngleAxis(180, Vector3.up) * (getJointLoc(BodyJoint.JointType.ELBOW_LEFT) - getJointLoc(BodyJoint.JointType.SHOULDER_LEFT));
		tRight = (getJointLoc(BodyJoint.JointType.SHOULDER_CENTRE) - getJointLoc(BodyJoint.JointType.SHOULDER_LEFT)) + tForward;
		tUp = Vector3.Cross(tRight.normalized, tForward.normalized);
		tAtt = Quaternion.LookRotation(tForward.normalized, tUp.normalized) * Quaternion.AngleAxis(90, Vector3.up);
		setJointRot(BodyJoint.JointType.SHOULDER_LEFT, Quaternion.Inverse(mainAtt)*tAtt);
		shoulderAtt = tAtt;
		
		// left elbow
		shoulderAtt = _convertLocToRot(BodyJoint.JointType.ELBOW_LEFT, BodyJoint.JointType.WRIST_LEFT, shoulderAtt, 1);
		
		// left wrist
		shoulderAtt = _convertLocToRot(BodyJoint.JointType.WRIST_LEFT, BodyJoint.JointType.HAND_LEFT, shoulderAtt, 1);
		
		//// right shoulder
		tForward = Quaternion.AngleAxis(180, Vector3.up) * (getJointLoc(BodyJoint.JointType.ELBOW_RIGHT) - getJointLoc(BodyJoint.JointType.SHOULDER_RIGHT));
		tRight = (getJointLoc(BodyJoint.JointType.SHOULDER_CENTRE) - getJointLoc(BodyJoint.JointType.SHOULDER_RIGHT)) + tForward;
		tUp = Vector3.Cross(tRight.normalized, tForward.normalized);
		tAtt = Quaternion.LookRotation(tForward.normalized, tUp.normalized) * Quaternion.AngleAxis(-90, Vector3.up);
		setJointRot(BodyJoint.JointType.SHOULDER_RIGHT, Quaternion.Inverse(mainAtt)*tAtt);
		shoulderAtt = tAtt;
		
		// right elbow
		shoulderAtt = _convertLocToRot(BodyJoint.JointType.ELBOW_RIGHT, BodyJoint.JointType.WRIST_RIGHT, shoulderAtt, -1);
		
		// right wrist
		_convertLocToRot(BodyJoint.JointType.WRIST_RIGHT, BodyJoint.JointType.HAND_RIGHT, shoulderAtt, -1);
	}
	
	/*
	 * converts spatial information to joint relative rotations
	 * -1 for right, 1 for left
	 */
	Quaternion _convertLocToRot(BodyJoint.JointType jType, BodyJoint.JointType jChild, Quaternion parentAtt, float dir)
	{
		Vector3 tForward = Quaternion.AngleAxis(180, Vector3.up) * (getJointLoc(jChild) - getJointLoc(jType));
		Vector3 tRight = -Vector3.right;
		Vector3 tUp = Vector3.Cross(tRight.normalized, tForward.normalized);
		Quaternion tAtt = Quaternion.LookRotation(tForward.normalized, tUp.normalized) * Quaternion.AngleAxis(dir*90, Vector3.up);
		setJointRot(jType, Quaternion.Inverse(parentAtt) * tAtt);
		return tAtt;
	}
	
	public Quaternion setJointRot(BodyJoint.JointType type, Quaternion rot)
	{
		joint_rotations[(int)type] = rot;
		return rot;
	}
	
	public Quaternion getJointRot(BodyJoint.JointType type)
	{
		return (Quaternion)joint_rotations[(int)type];
	}
	
	public Vector3 getJointLoc(BodyJoint.JointType type)
	{
		return joint_locations[(int)type];
	}
	
	public override string ToString()
	{
		string str = "";
		foreach(Quaternion q in joint_rotations)
		{
			str += mDebug.quatToString(q);
		}
		return str;
	}
	
	// get distance between 2 poses
	// 0 - 1 with 0 identical, 1 is not the same
	public float getDistance(mDistanceMeasurable p)
	{
		Pose otherp = (Pose)p;
		if (otherp == null)
		{
			return -1;
		}
		
		float dist = 0;
		for(int i = 0; i < joint_rotations.Length; i++)
		{
//			dist += Mathf.Abs(Quaternion.Angle(joint_rotations[i], otherp.joint_rotations[i]))*Mathf.Deg2Rad;
			dist += Vector3.Distance(joint_locations[i], otherp.joint_locations[i]);
		}
		return (dist/(float)joint_rotations.Length);
	}
	
	// interpolate 2 poses, from this pose to othp
	public Pose lerp(Pose othp, float t)
	{
		Pose p = new Pose();
		for(int i = 0; i < joint_rotations.Length; i++)
		{
			p.joint_rotations[i] = Quaternion.Lerp(joint_rotations[i], othp.joint_rotations[i], t);
		}
		return p;
	}
	
	public static Pose random()
	{
		Pose p = new Pose();
		for(int i = 0; i < 20; i++)
		{
			p.joint_rotations[i] = Random.rotation;
		}
		return p;
	}
}