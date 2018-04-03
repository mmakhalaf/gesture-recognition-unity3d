using UnityEngine;
using System.Collections;

public class VectorTest : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
	{
		Debug.DrawRay(Vector3.zero, Vector3.Cross(Vector3.right, Vector3.up), Color.red, 0, false);
	}
}
