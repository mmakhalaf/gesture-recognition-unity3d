using System.Threading;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public delegate void RecognitionDoneDelegate(string actionId, ActionInstance newinst);

public class ActionRecognizer : MonoBehaviour
{
	public GUITexture costMatrixTexture;
	
	private static ActionRecognizer _actionRecognizer;
	public static ActionRecognizer instance
	{
		get
		{
			if (_actionRecognizer == null)
			{
				_actionRecognizer = (ActionRecognizer)FindObjectOfType(typeof(ActionRecognizer));
			}
			return _actionRecognizer;
		}
	}
	
	public RecognitionDoneDelegate onRecognitionFinished = null;
	
	public mIndex[] stepConstraints = new mIndex[] {
		new mIndex(1,1), new mIndex(0,1), new mIndex(1,0)
	};
	
	bool _isProcessing = false;
	public bool isProcessing
	{
		get { return _isProcessing; }
	}
	
	List<Thread> _recognitionThreads;
	List<RecognitionThread> _recognitionWorkers;
	
	void Start()
	{
		_recognitionThreads = new List<Thread>();
		_recognitionWorkers = new List<RecognitionThread>();
	}
	
	void OnApplicationQuit()
	{
		_isProcessing = false;
		StopAllCoroutines();
		foreach(Thread t in _recognitionThreads)
		{
			t.Abort();
		}
		_recognitionThreads.Clear();
		_recognitionWorkers.Clear();
		print("Quit");
	}
	
	IEnumerator CheckThreadsCoroutine()
	{
		while(_isProcessing)
		{
			yield return new WaitForSeconds(1.0f);
			
			int idx = 0;
			foreach(RecognitionThread t in _recognitionWorkers)
			{
				if (t.isDone)
				{
					idx++;
				}
			}
			
			// if all threads are done
			if (idx == _recognitionWorkers.Count)
			{
				print("-- all done");
				
				float mincost = float.PositiveInfinity;
				ActionData mindata = null;
				RecognitionThread minwork = null;
				foreach(RecognitionThread t in _recognitionWorkers)
				{
					float cost = t.dtw.getWarpPathCost();
					if (cost < mincost)
					{
						mincost = cost;
						mindata = t._subseq;
						minwork = t;
					}
				}
			
				Texture2D tmpT = mDebug.drawMatrix(minwork.dtw.CostMatrix);
				tmpT = mDebug.drawPoints(tmpT, minwork.warppath.ToArray(), Color.red);
//				tmpT = mDebug.drawPoints(tmpT, new mIndex[] {minwork.warppath[0]}, Color.red);
//				tmpT = mDebug.drawPoints(tmpT, new mIndex[] {minwork.warppath[minwork.warppath.Count-1]}, Color.red);
				
				costMatrixTexture.texture = tmpT;
				costMatrixTexture.transform.localScale = new Vector3(0.2f, 0.2f*((float)minwork.dtw.AccumCostMatrix.Length/(float)minwork.dtw.AccumCostMatrix[0].Length), 1);
				costMatrixTexture.transform.position = new Vector3(0.5f*costMatrixTexture.transform.localScale.x, costMatrixTexture.transform.localScale.y, 0);
				
//				mDebug.printMatrix(minwork.dtw.CostMatrix);
//				mDebug.printMatrix(minwork.dtw.AccumCostMatrix);
				
				OnRecognitionDone(minwork._subseq, minwork._seq, minwork.dtw);
				
				foreach(Thread t in _recognitionThreads)
				{
					t.Abort();
				}
				_recognitionThreads.Clear();
				_recognitionWorkers.Clear();
				_isProcessing = false;
			}
		}
	}
	
	public bool recognizeSequence(ActionInstance inst)
	{
		if (_isProcessing)	return false;
		
		StopCoroutine("CheckThreadsCoroutine");
		
		_isProcessing = true;
		List<ActionData> actions = ActionDatabase.instance.actionsData;
		foreach(ActionData d in actions)
		{
			RecognitionThread w = new RecognitionThread(inst, d, stepConstraints);
			Thread t = new Thread(new ThreadStart(w.process));
			_recognitionThreads.Add(t);
			_recognitionWorkers.Add(w);
			t.Start();
		}
		
		StartCoroutine(CheckThreadsCoroutine());
		
		return true;
	}
	
	void OnRecognitionDone(ActionData matchedinst, ActionInstance insttomatch, DTW dtw)
	{
		ActionInstance warpedinst = new ActionInstance();
		foreach(mIndex idx in dtw.WarpPath)
		{
			warpedinst.poses.Add(insttomatch.poses[idx.x]);
		}
		
		print("-- cost " + matchedinst.actionId + " " + dtw.getWarpPathCost());
		
		onRecognitionFinished(matchedinst.actionId, warpedinst);
	}
}

class RecognitionThread
{
	public readonly ActionInstance _seq;
	public readonly ActionData _subseq;
	
	public readonly DTW dtw;
	public List<mIndex> warppath;
	
	bool _isDone;
	public bool isDone
	{
		get { return _isDone; }
	}
	
	public RecognitionThread(ActionInstance seq, ActionData subseq, mIndex[] stepConstraints)
	{
		_seq = seq;
		_subseq = subseq;
	 	dtw = new DTW(stepConstraints);
		
		_isDone = false;
	}
	
	public void process()
	{
		ActionInstance inst = _subseq.actionInstances[0];
		
		warppath = dtw.warpSubsequenceDTW(_seq.poses.ToArray(), inst.poses.ToArray());
		
		Debug.Log("worker thread done " + _subseq.actionId + ", cost " + dtw.getWarpPathCost());
		
		_isDone = true;
	}
}