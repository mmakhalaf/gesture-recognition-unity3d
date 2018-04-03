using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mDebug
{
	public static string quatToString(Quaternion att)
	{
		return "(" + att.x + ", " + att.y + ", " + att.z + ", " + att.w + ")";
	}
	
	public static void printQuat(Quaternion att)
	{
		Debug.Log(quatToString(att));
	}
	
	public static void printVector(float[] vec)
	{
		string str = "";
		foreach(float v in vec)
		{
			str += v + "\t";
		}
		Debug.Log(str);
	}
	
	public static void printMatrix(float[][] mat)
	{
		string str = "";
		for(int y = 0; y < mat.Length; y++)
		{
			for(int x = 0; x < mat[y].Length; x++)
			{
				str += string.Format("{0:F3}", new object[] {mat[y][x]}) + "\t";
			}
			str += "\n";
		}
		Debug.Log(str);
	}
	
	public static Texture2D drawMatrix(float[][] vals)
	{
		float[][] nvals = mUtil.normalize(vals, 0, 1);
		Texture2D tex = new Texture2D(nvals[0].Length, nvals.Length, TextureFormat.RGBA32, false, false);
		for(int i = 0; i < nvals[0].Length; i++)
		{
			for(int j = 0; j < nvals.Length; j++)
			{
				tex.SetPixel(i,j,new Color32((byte)(nvals[j][i]*255.0f), (byte)(nvals[j][i]*255.0f), (byte)(nvals[j][i]*255.0f), 255));
			}
		}
		tex.Apply();
		return tex;
	}
	
	public static Texture2D drawPoints(Texture2D tex, mIndex[] points, Color c)
	{
		foreach(mIndex p in points)
		{
			tex.SetPixel(p.x, p.y, c);
		}
		tex.Apply();
		return tex;
	}
}
