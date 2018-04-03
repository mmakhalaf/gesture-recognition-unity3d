using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour
{
	Transform targetObject;
	Transform targeCameraObject;
	
	public float distance = 5;
	public float xSpeed = 20;
	public float yAngle = 10;
	
	float currentX = 0;
	
	void OnEnable()
	{
		targetObject = gameObject.transform;
		targeCameraObject = Camera.main.transform;
		
		Vector3 angles = targeCameraObject.eulerAngles;
    	currentX = angles.y;
	}
	
	void Update()
	{
		currentX += xSpeed*Time.deltaTime;
		
		Quaternion rot = Quaternion.Euler(new Vector3(yAngle, currentX, 0));
		Vector3 pos = rot * new Vector3(0,0,-distance) + targetObject.position;
		
		targeCameraObject.position = pos;
		targeCameraObject.rotation = rot;
	}
}