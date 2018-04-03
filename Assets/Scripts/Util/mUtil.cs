using System;
using System.Collections;
using System.Collections.Generic;

//using UnityEngine;

public class mUtil
{
	/**
	 * Find last element with minimum index in arr
	 */
	public static int[] min(float[] arr)
	{
		// find minimum value in the array
		float m = arr[0];
		int idx = 0;
		for(int i = 1; i < arr.Length; i++)
		{
			if (arr[i] < m)
			{
				idx = i;
				m = arr[i];
			}
		}
		
		// store all indices with minimum value
		List<int> idxarr = new List<int>();
		for(int i = 0; i < arr.Length; i++)
		{
			if (arr[i] == m)
			{
				idxarr.Add(i);
			}
		}
		
		return idxarr.ToArray();
	}
	
	public static float[][] normalize(float[][] mat)
	{
		float sum = 0;
		foreach(float[] farr in mat)
		{
			foreach(float f in farr)
			{
				sum += f;
			}
		}

		for(int x = 0; x < mat[0].Length; x++)
		{
			for(int y = 0; y < mat.Length; y++)
			{
				mat[y][x] /= sum;
			}
		}
		
		return mat;
	}
	
	public static float[][] normalize(float[][] mat, float nmin, float nmax)
	{
		float[][] nmat = new float[mat.Length][];
		
		float min = float.PositiveInfinity, max = 0;
		
		for(int y = 0; y < mat.Length; y++)
		{
			nmat[y] = new float[mat[y].Length];
			for(int x = 0; x < mat[0].Length; x++)
			{
				float v = mat[y][x];
				if (v > max)
				{
					max = v;
				}
				else if (v < min)
				{
					min = v;
				}
			}
		}
		
		float orange = max-min;
		float nrange = nmax-nmin;
		for(int x = 0; x < mat[0].Length; x++)
		{
			for(int y = 0; y < mat.Length; y++)
			{
				nmat[y][x] = ((mat[y][x]-min) * (nrange / orange)) + nmin;
			}
		}
		return nmat;
	}
	
	public static float[] subvector(float[] vec, int sidx, int eidx)
	{
		sidx = Math.Max(sidx,0);
		eidx = Math.Min(eidx,vec.Length-1);
		if (eidx <= sidx)	return new float[] {};
		
		float[] sub = new float[eidx-sidx];
		for(int i = sidx, idx = 0; i < eidx; i++, idx++)
		{
			sub[idx] = vec[i];
		}
		
//		mDebug.printVector(sub);
		
		return sub;
	}
}
