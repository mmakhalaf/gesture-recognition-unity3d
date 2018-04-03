using UnityEngine;
using System.Collections;

public class AgentController : MonoBehaviour
{
	public float framesPerSecond = 20;
	
	public BodyJoint[] joints = null;
	
	public bool xyzData = false;
	
	public bool playOnce = true;
	
	int currpose = 0;
	bool isMoving = false;
	
	void Start()
	{
		joints = new BodyJoint[20];
		BodyJoint[] jcomps = gameObject.GetComponentsInChildren<BodyJoint>();
		foreach(BodyJoint bj in jcomps)
		{
			joints[(int)bj.jointType] = bj;
		}
	}
	
	public void StartAction(ActionInstance data)
	{
		StartCoroutine(StartActionCoroutine(data));
	}
	
	public void StopAction()
	{
		StartCoroutine(StopActionCoroutine());
	}
	
	IEnumerator StopActionCoroutine()
	{
		isMoving = false;
		StopCoroutine("PerformCoroutine");
		
		yield return new WaitForSeconds(0.05f);
		
		currpose = 0;
	}
	
	IEnumerator StartActionCoroutine(ActionInstance data)
	{
		if (isMoving)
		{
			yield return StartCoroutine(StopActionCoroutine());
		}
		
		isMoving = true;
		StartCoroutine(PerformCoroutine(data));
	}
	
	IEnumerator PerformCoroutine(ActionInstance data)
	{
		while(isMoving)
		{
			yield return new WaitForSeconds(1.0f/framesPerSecond);
			perform(data);
			
//			print(currpose);
			
			currpose++;
			
			if (currpose >= data.poses.Count)
			{
				currpose = 0;
				if (playOnce)
				{
					break;
				}
			}
		}
	}
	
	// perform next pose
	//  update joint rotation or position based on which type of skeleton is used based on user input
	void perform(ActionInstance data)
	{
		Pose p = data.poses[currpose];
		
		for(int i = 0; i < p.joint_locations.Count; i++)
		{
			if (joints[i] != null)
			{
				if (xyzData)
				{
					joints[i].UpdatePosition((Vector3)p.joint_locations[i]);
				}
				else 
				{
					joints[i].UpdateLocalRotation(p.getJointRot(joints[i].jointType));
				}
			}
		}
	}
}
