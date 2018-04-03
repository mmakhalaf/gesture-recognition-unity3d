using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class DTWTest : MonoBehaviour
{
	public GUITexture costMatrixTexture;
	
	void Start()
	{
		Random.seed = System.DateTime.Now.Millisecond;
		Pose[] vec1 = new Pose[] {
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(),
			Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random(), Pose.random()
		};
		Pose[] svec = new Pose[] {
			vec1[4], vec1[6], vec1[8], vec1[10],
			vec1[12], vec1[13], vec1[14], vec1[15],
			vec1[16], vec1[18], vec1[20], vec1[22],
			vec1[24], vec1[27], vec1[30], vec1[31],
			vec1[32], vec1[33], vec1[34], vec1[35],
			vec1[37], vec1[38], vec1[40], vec1[42]
		};
//		Float[] vec1 = new Float[] {
//			new Float(0), new Float(0.75f), new Float(1.5f), new Float(0.75f), new Float(0), new Float(-0.75f), new Float(-1.5f), new Float(-0.75f), new Float(0)
//		};
//		Float[] svec = new Float[] {
//			new Float(0), new Float(-0.5f), new Float(-1), new Float(-1.5f)
//		};
//		
//		float[] vec1 = new float[] { 0, 0.75f, 1.5f, 0.75f, 0, -0.75f, -1.5f, -0.75f, 0};
//		float[] svec = new float[] { 0, -0.5f, -1, -1.5f};
		
		int start = System.DateTime.Now.Millisecond;
		DTW dtw = new DTW(new mIndex[] {new mIndex(1,1), new mIndex(1,2), new mIndex(2,1)});
		List<mIndex> wres = dtw.warpSubsequenceDTW(vec1, svec);
		Debug.Log("-- Time " + (System.DateTime.Now.Millisecond-start));
		
		mDebug.printMatrix(dtw.CostMatrix);
		mDebug.printMatrix(dtw.AccumCostMatrix);
		foreach(mIndex step in wres)
		{
			print(step + " " + dtw.AccumCostMatrix[step.y][step.x]);
		}
		
//		Texture2D tmpT = mDebug.drawMatrix(dtw.CostMatrix);
		Texture2D tmpT = mDebug.drawMatrix(dtw.AccumCostMatrix);
		tmpT = mDebug.drawPoints(tmpT, wres.ToArray(), Color.red);
		
		costMatrixTexture.texture = tmpT;
		costMatrixTexture.transform.localScale = new Vector3(0.3f, 0.3f*((float)svec.Length/(float)vec1.Length), 1);
		costMatrixTexture.transform.position = new Vector3(0.5f*costMatrixTexture.transform.localScale.x, costMatrixTexture.transform.localScale.y, 0);
	}
}

public class Float : mDistanceMeasurable
{
	public readonly float num;
	
	public Float(float n)
	{
		num = n;
	}
	
	public float getDistance(mDistanceMeasurable d)
	{
		Float oth = (Float)d;
		if (oth == null)
		{
			return -1;
		}
		
		return Mathf.Abs(oth.num-num);
	}
}