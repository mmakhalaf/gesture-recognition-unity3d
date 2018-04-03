using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("General/OnScreenLog")]
public class OnScreenLog : MonoBehaviour
{
	public Color messageColor = Color.black;
	public Color errorColor = Color.red;
	public Color warningColor = Color.yellow;
	
	public int maxLogEnteries = 100;
	
	public bool displayStackTrace = true;
	
	public Rect logViewBoundsViewportCoordinates = new Rect(0,0,0.3f,1.0f);
	
	List<LogEntry> logArray;
	
	Vector2 scrollP;
	Rect logViewRect;
	Rect logViewSize;
	Rect logLineSize;
	
	void Start()
	{
		logArray = new List<LogEntry>();
		scrollP = new Vector2(0,0);
		logViewRect = new Rect(logViewBoundsViewportCoordinates.x*Screen.width, logViewBoundsViewportCoordinates.y*Screen.height, logViewBoundsViewportCoordinates.width*Screen.width, logViewBoundsViewportCoordinates.height*Screen.height);
		logLineSize = new Rect(0,0, logViewRect.width, 0.05f*Screen.height);
		logViewSize = new Rect(0, 0, logViewRect.width, maxLogEnteries * 0.1f*Screen.height);
		
		Application.RegisterLogCallback(SaveLog);
	}
	
	void OnGUI()
	{
		scrollP = GUI.BeginScrollView(logViewRect, scrollP, logViewSize);
		
//		string logMessage = "";
		Rect tmpLogLine = logLineSize;
		for(int i = logArray.Count-1; i >= 0; i--)
		{
			Color c = GUI.color;
			switch(logArray[i].logType)
			{
			case LogType.Assert:
			case LogType.Error:
			case LogType.Exception:
			{
				GUI.color = errorColor;
				break;
			}
			case LogType.Warning:
			{
				GUI.color = warningColor;
				break;
			}
			default:
			{
				GUI.color = messageColor;
				break;
			}
			}
			
			// assuming 60 characters per line
			int nlines = ((int)((float)logArray[i].message.Length / 60.0f)) + 1;
			string logMessage = logArray[i].message + "\n";
			if (displayStackTrace && logArray[i].stackTrace != "")
			{
				nlines += ((int)((float)logArray[i].stackTrace.Length / 60.0f)) + 1;
				logMessage += "----\n";
				logMessage += logArray[i].stackTrace;
				logMessage += "----\n";
			}
			tmpLogLine.height = 0.05f*Screen.width*(float)nlines;
			GUI.Label(tmpLogLine, logMessage);
			tmpLogLine.y += tmpLogLine.height;
			
			GUI.color = c;
		}
//		GUI.Label(logViewSize, logMessage);
		
		GUI.EndScrollView();
	}
	
	void SaveLog(string logString, string stackTrace, LogType type)
	{
//		logArray.Add((type == LogType.Error || type == LogType.Exception ? "ERR/CEPTION: " : (type == LogType.Warning ? "WARNING: " : "LOG: ")) + logString);
		logArray.Add(new LogEntry(logString, stackTrace, type));
		
		if (logArray.Count > maxLogEnteries)
		{
			logArray.RemoveAt(0);
		}
	}
}

public class LogEntry
{
	public readonly string message;
	public readonly string stackTrace;
	public readonly LogType logType;
	
	public LogEntry(string msg, string stackTrace, LogType type)
	{
		message = msg;
		this.stackTrace = stackTrace;
		logType = type;
	}
}