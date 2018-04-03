using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
	Transform targetOwnerTransform;
	Transform targetTransform;
	
	// navigation
	public float zoomSpeed = 20;
	public float distance = 100;
	public float MAX_DISTANCE = 250;
	public float MIN_DISTANCE = 50;
	
	public float minY = 5;
	public float maxY = 80;
	
	public float xSpeed = 50;
	public float ySpeed = 25;
	
	float currentX = 0;
	float currentY = 0;
	
	//
	Vector2 lastPosition_rot = Vector2.zero;
	Vector2 lastPosition_zoom = Vector2.zero;
	
	// mouse
	bool zoomButtonDown = false;
	
	void Start()
	{
		targetOwnerTransform = Camera.main.transform;
		targetTransform = gameObject.transform;
		
		Vector3 angles = targetOwnerTransform.eulerAngles;
    	currentX = angles.y;
    	currentY = angles.x;
		
		targetOwnerTransform.position = targetOwnerTransform.rotation * new Vector3(0,0,-distance) + targetTransform.position;
	}
	
	void Update()
	{
		if (targetTransform	== null || targetOwnerTransform == null)	return;
		
//		if (App.isDisplayingMenu)	return;
		
		////////
		// touch 
		int rotT = 0, zoomT = 0;
		updateTouchIds(ref rotT, ref zoomT);
		if (rotT != -1)
		{
			processTouchInput(rotT, zoomT);
		}
		else
		{
			processMouseInput();
		}
	}
	
	void processTouchInput(int rotT, int zoomT)
	{
		if (zoomT != -1)
		{
			Vector2 currPos_zoom = Input.GetTouch(zoomT).position;
			Vector2 zoomDeltaPosition = currPos_zoom - lastPosition_zoom;
			lastPosition_zoom = currPos_zoom;
			
			float deltaD = zoomDeltaPosition.magnitude * Mathf.Sign(zoomDeltaPosition.y) * Time.deltaTime * zoomSpeed;
			
			float tmpD = distance + deltaD;
			
			if (!((tmpD > MAX_DISTANCE && Mathf.Sign(deltaD) > 0) ||
		    	(tmpD < MIN_DISTANCE && Mathf.Sign(deltaD) < 0)))
			{
				distance = tmpD;
			}
		}
		
		// delta current touch position and last
		Vector2 currPos_rot = Input.GetTouch(rotT).position;
		Vector2 touchDeltaPosition = currPos_rot - lastPosition_rot;
		lastPosition_rot = currPos_rot;
		
		currentX += touchDeltaPosition.x * xSpeed * 0.02f;
        currentY -= touchDeltaPosition.y * ySpeed * 0.02f;
		
 		currentY = ClampAngle(currentY, minY, maxY);
 		
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 position = rotation * new Vector3(0, 0, -distance) + targetTransform.position;
        
        targetOwnerTransform.position = position;
        targetOwnerTransform.rotation = rotation;
	}
	
	void processMouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			lastPosition_rot = Input.mousePosition;
		}
		
		if (Input.GetMouseButtonDown(1))
		{
			zoomButtonDown = true;
			lastPosition_zoom = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(1))
		{
			zoomButtonDown = false;
			lastPosition_rot = Input.mousePosition;
			lastPosition_zoom = Input.mousePosition;
		}
		
		if (Input.GetMouseButton(0))
    	{
			Vector2 currPos = Input.mousePosition;
			if (zoomButtonDown)
			{
				Vector2 zoomDeltaPosition = currPos - lastPosition_zoom;
				lastPosition_zoom = currPos;
				
				float deltaD = zoomDeltaPosition.magnitude * Mathf.Sign(zoomDeltaPosition.y) * Time.deltaTime * zoomSpeed * 5;
				
				float tmpD = distance + deltaD;
				
				if (!((tmpD > MAX_DISTANCE && Mathf.Sign(deltaD) > 0) ||
			    	(tmpD < MIN_DISTANCE && Mathf.Sign(deltaD) < 0)))
				{
					distance = tmpD;
				}
			}
			else
			{
				// delta current touch position and last
				Vector2 rotDeltaPosition = currPos - lastPosition_rot;
				lastPosition_rot = currPos;
	    		
				currentX += rotDeltaPosition.x * xSpeed * 0.02f;
				currentY -= rotDeltaPosition.y * ySpeed * 0.02f;
				
		 		currentY = ClampAngle(currentY, minY, maxY);
		 		
		        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
		        targetOwnerTransform.rotation = rotation;
			}
			
	        Vector3 position = targetOwnerTransform.rotation * new Vector3(0, 0, -distance) + targetTransform.position;
	        targetOwnerTransform.position = position;
    	}
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	public static float signedMagnitude(Vector2 v)
	{
		return Mathf.Sqrt(Mathf.Sign(v.x)*v.x*v.x + Mathf.Sign(v.y)*v.y*v.y);
	}
	
	// check whether touched
	// which fingers and return touch id
	void updateTouchIds(ref int touchIdrot, ref int touchIdzoom)
	{
		if (Input.touchCount > 0 && Input.touchCount < 3)
		{
			// one touch, just panning
			if (Input.touchCount == 1)
			{
				touchIdrot = 0;
				touchIdzoom = -1;
			}
			
			// 2 touches, one for panning and one for zooming
			// the left most touch pans and the right one zooms
			if (Input.touchCount == 2)
			{
				touchIdrot = 0;
				touchIdzoom = 1;
				
				if (Input.GetTouch(1).position.x < Input.GetTouch(0).position.x)
				{
					touchIdrot = 1;
					touchIdzoom = 0;
				}
			}
			
			// when touch begins, set last positon to current
			if (Input.GetTouch(touchIdrot).phase == TouchPhase.Began)
			{
				lastPosition_rot = Input.GetTouch(touchIdrot).position;
			}
			
			if (touchIdzoom != -1 && Input.GetTouch(touchIdzoom).phase == TouchPhase.Began)
			{
				lastPosition_zoom = Input.GetTouch(touchIdzoom).position;
			}
		}
		else
		{
			touchIdrot = -1;
			touchIdzoom = -1;
		}
	}
}