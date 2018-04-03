using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DTW
{
	private float[][] _costMatrix, _accumCostMatrix;
	public float[][] CostMatrix
	{
		get { return _costMatrix; }
	}
	public float[][] AccumCostMatrix
	{
		get { return _accumCostMatrix; }
	}
	
	private mIndex[] _stepConstraints;
	private List<mIndex> _warpPath;
	public List<mIndex> WarpPath
	{
		get { return _warpPath; }
	}
	
	public DTW(mIndex[] stepConstraints)
	{
		_stepConstraints = stepConstraints;
	}
	
//	private void _init(float[] xsig, float[] ysig)
	private void _init(mDistanceMeasurable[] xsig, mDistanceMeasurable[] ysig)
	{
		// calculate cost matrix, calculate distance between each sample
		// initialize accumulated cost matrix
		_costMatrix = new float[ysig.Length][];
		_accumCostMatrix = new float[ysig.Length][];
		for(int y = 0; y < ysig.Length; y++)
		{
			_costMatrix[y] = new float[xsig.Length];
			_accumCostMatrix[y] = new float[xsig.Length];
			for(int x = 0; x < xsig.Length; x++)
			{
//				_costMatrix[y][x] = Math.Abs(xsig[x]-ysig[y]);
				_costMatrix[y][x] = ysig[y].getDistance(xsig[x]);
				_accumCostMatrix[y][x] = float.PositiveInfinity;
			}
		}
		_accumCostMatrix[0][0] = _costMatrix[0][0];
		
		// initialize optimal warp path
		if (_warpPath != null)
		{
			_warpPath.Clear();
		}
		_warpPath = new List<mIndex>();
	}
	
	/**
	 * Align p2 with a subsequence of p1
	 * Returns subsequence of p1 which is closest match
	 */
//	public List<mIndex> warpDTW(float[] p1, float[] p2)
	public List<mIndex> warpDTW(mDistanceMeasurable[] p1, mDistanceMeasurable[] p2)
	{
		_init(p1, p2);
		
		_createAccumCostMatrix(0,0);
		
		_findWarpPath();
		
		return _warpPath;
	}
	
	/**
	 * Finds subsequence of seq closest to subseq which minimizes cost finction
	 */
//	public List<mIndex> warpSubsequenceDTW(float[] seq, float[] subseq)
	public List<mIndex> warpSubsequenceDTW(mDistanceMeasurable[] seq, mDistanceMeasurable[] subseq)
	{
		_init(seq, subseq);
		
//		_costMatrix = new float[][] {
//			new float[] {1, 3, 5, 7, 9},
//			new float[] {3, 2, 5, 6, 7},
//			new float[] {2, 4, 3, 5, 7},
//			new float[] {3, 4, 5, 4, 8},
//			new float[] {2, 4, 6, 7, 5}
//		};
//		_accumCostMatrix = new float[][] {
//			new float[] {float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity},
//			new float[] {float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity},
//			new float[] {float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity},
//			new float[] {float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity},
//			new float[] {float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity}
//		};
		
		for(int x = 1; x < _costMatrix[0].Length; x++)
		{
			_accumCostMatrix[0][x] = _costMatrix[0][x];
		}
		
		for(int y = 1; y < _costMatrix.Length; y++)
		{
//			float sum = 0;
//			for(int x = 0; x < _costMatrix[y].Length; x++)
//			{
//				sum += _costMatrix[y][x];
//			}
//			_accumCostMatrix[y][0] = sum;
			_accumCostMatrix[y][0] = _accumCostMatrix[y-1][0] + _costMatrix[y][0];
		}
		
		_createAccumCostMatrix(1,1);
		
		_findSubsequenceWarpPath();
		
		return _warpPath;
	}
	
	private void _createAccumCostMatrix(int startx, int endx)
	{
		for(int x = 1; x < _accumCostMatrix[0].Length; x++)
		{
			for(int y = 1; y < _accumCostMatrix.Length; y++)
			{
				int minidx = -1;
				float mincost = float.PositiveInfinity;
				for(int i = 0; i < _stepConstraints.Length; i++)
				{
					int sx = x-_stepConstraints[i].x, sy = y-_stepConstraints[i].y;
					if (sx >= 0 && sy >= 0)
					{
						float cost = _accumCostMatrix[sy][sx];
						if (cost < mincost)
						{
							mincost = cost;
							minidx = i;
						}
						else if (cost == mincost)
						{
							// if equal, choose lexiographically smaller index
							if (_stepConstraints[i] < _stepConstraints[minidx])
							{
								mincost = cost;
								minidx = i;
							}
						}
					}
				}
				
				if (minidx != -1)
				{
					_accumCostMatrix[y][x] = mincost + _costMatrix[y][x];
				}
				else
				{
					Debug.Log("minidx -1 for " + x + "," + y);
				}
			}
		}
	}
	
	/**
	 * Iteratively look for the path which minimizes the cost of the following step
	 */
	private void _findWarpPath()
	{
		int xcurr = _costMatrix[0].Length-1, ycurr = _costMatrix.Length-1;
		
		while(xcurr > 0 && ycurr > 0)
		{
			_findNextStep(ref xcurr, ref ycurr);
		}
		_warpPath.Insert(0,mIndex.zero);
	}
	
	private void _findSubsequenceWarpPath()
	{
//		int xcurr = _accumCostMatrix[0].Length-1;
//		float mindelta = float.PositiveInfinity;
//		for(int x = 1; x < _accumCostMatrix[0].Length; x++)
//		{
//			float delta = Math.Abs(_accumCostMatrix[_accumCostMatrix.Length-1][x]-_accumCostMatrix[_accumCostMatrix.Length-1][x-1]);
//			if (delta < mindelta)
//			{
//				mindelta = delta;
//				xcurr = x;
//			}
//		}
		
		// find path which minimizes cost
		
		int[] xcurr_arr = mUtil.min(_accumCostMatrix[_accumCostMatrix.Length-1]);
		
		List<mIndex> minwarppath = new List<mIndex>();
		float mincost = float.PositiveInfinity;
		
		for(int i = 0; i < xcurr_arr.Length; i++)
		{
			_warpPath.Clear();
			int xcurr = xcurr_arr[i];
			int ycurr = _accumCostMatrix.Length-1;		
			while(ycurr > 0)
			{
				_findNextStep(ref xcurr, ref ycurr);
			}
			_warpPath.Insert(0,new mIndex(xcurr,0));
			
			float cost = getWarpPathCost();
			if (cost < mincost)
			{
				mincost = cost;
				minwarppath = _warpPath;
			}
			
			Debug.Log("Path " + i + " , Cost: " + cost + ", Start " + xcurr);
		}
		
		_warpPath = minwarppath;
	}
	
	/**
	 * Find next step which minimizes cost function
	 */
	private void _findNextStep(ref int sx, ref int sy)
	{
		_warpPath.Insert(0,new mIndex(sx,sy));
		
		int ex = 0, ey = 0;
		float minCost = float.PositiveInfinity;
		
		for(int i = 0; i < _stepConstraints.Length; i++)
		{
			int x = Math.Max(sx-_stepConstraints[i].x, 0), y = Math.Max(sy-_stepConstraints[i].y, 0);
			if (_accumCostMatrix[y][x] < minCost)
			{
				minCost = _accumCostMatrix[y][x];
				ex = x;
				ey = y;
			}
		}
		sx = ex;
		sy = ey;
	}
	
	public float getWarpPathCost()
	{
		// normalize cost based on subsequance length (not cost path length)
		mIndex lastIdx = _warpPath[_warpPath.Count-1];
		return _accumCostMatrix[lastIdx.y][lastIdx.x]/(float)_costMatrix.Length;;
	}
}

/*
 * Class which defines an index for a 2D array
 */
[System.Serializable]
public class mIndex
{
	public static mIndex zero = new mIndex(0,0);
	public static mIndex one = new mIndex(1,1);
	
	public int x, y;
	
	public mIndex(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public override string ToString()
	{
		return "( " + y + ", " + x + " )";
	}
	
	public static bool operator<(mIndex lhs, mIndex rhs)
	{
		if (lhs.x+lhs.y < rhs.x+rhs.y)
		{
			return true;
		}
		return false;
	}
	public static bool operator>(mIndex lhs, mIndex rhs)
	{
		return !(lhs<rhs);
	}
	public static bool operator==(mIndex lhs, mIndex rhs)
	{
		if (lhs.x == rhs.x && lhs.y == rhs.y)
		{
			return true;
		}
		return false;
	}
	public static bool operator!=(mIndex lhs, mIndex rhs)
	{
		return !(lhs==rhs);
	}
	
	public override bool Equals(object obj)
	{
		return ((mIndex)obj==this);
	}
}

public interface mDistanceMeasurable
{
	float getDistance(mDistanceMeasurable other);
}