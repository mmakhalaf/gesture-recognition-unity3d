using UnityEngine;
using System.Collections;
using System.IO;

/**
 * Responsible for receiving many instances of the action manually, aligning them, storing them
 */
public class MenuUI : MonoBehaviour
{
	public GUISkin guiSkin;
	
	string actionName = "wave1";
	string actionFileName;
	FileBrowser fileBrowser;
	
	bool playOnce = true;
	
	bool _displayMenu = false;
	string _message = "";
	string _actionsList = "";
	
	void Start()
	{
		actionFileName = Path.Combine(Application.dataPath.Replace('/',Path.DirectorySeparatorChar),Path.Combine(Path.Combine("Dataset","MSRAction3DSkeletonReal3D"), "a01_s01_e01_skeleton3D.txt"));
//		actionFileName = Application.dataPath+"/Dataset/MSRAction3DSkeletonReal3D/a01_s01_e01_skeleton3D.txt";
		
		ActionDatabase.readDatabase();
		_updateActionsList();
		
		ActionRecognizer.instance.onRecognitionFinished = new RecognitionDoneDelegate(OnRecognitionEnd);
	}
	
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.M))
		{
			setMenuVisible(!_displayMenu);
		}
		
		if (_displayMenu && Input.GetKeyUp(KeyCode.Escape))
		{
			setMenuVisible(!_displayMenu);
		}
	}
	
	void OnGUI()
	{
		GUI.skin = guiSkin;
		
		if (fileBrowser != null)
		{
			fileBrowser.OnGUI();
		}
		
		if (_displayMenu)
		{
			GUI.Box(new Rect(0.2f*Screen.width, 0.2f*Screen.height, 0.6f*Screen.width, 0.6f*Screen.height), "");
		
			GUILayout.BeginArea(new Rect(0.2f*Screen.width, 0.2f*Screen.height, 0.6f*Screen.width, 0.6f*Screen.height));
			
				GUILayout.BeginVertical();
					
					// action id
					GUILayout.BeginHorizontal();
						GUILayout.Label("Action Name");
						actionName = GUILayout.TextField(actionName, GUILayout.MinWidth(0.4f*Screen.width), GUILayout.MaxWidth(0.4f*Screen.width));
					GUILayout.EndHorizontal();
					
					// action instance
					GUILayout.BeginHorizontal();
						GUILayout.Label("Action File path");
						if (GUILayout.Button(actionFileName, GUILayout.MinWidth(0.4f*Screen.width), GUILayout.MaxWidth(0.4f*Screen.width)))
						{
							fileBrowser = new FileBrowser(
															new Rect(0.2f*Screen.width, 0.2f*Screen.height, 0.6f*Screen.width, 0.6f*Screen.height),
															"Action File",
															actionFileName,
															onActionChosen);
							fileBrowser.SelectionPattern = "*.txt";
							_displayMenu = false;
						}
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
						if (GUILayout.Button("Perform Action", GUILayout.MinWidth(0.4f*Screen.width), GUILayout.MaxWidth(0.4f*Screen.width)))
						{
							ActionInstance data = new ActionInstance(actionFileName);
							_playAction(data);
							setMenuVisible(false);
						}
						playOnce = GUILayout.Toggle(playOnce, "Once");
					GUILayout.EndHorizontal();
					
					if (GUILayout.Button("Add Action"))
					{
						// create action data if not exist in database
						// add new action instance to actiondata
						// save action data to database
						ActionData data = ActionDatabase.instance.getAction(actionName);
						if (data == null)
						{
							data = new ActionData(actionName);
						}
				
						ActionInstance actionInst = new ActionInstance(actionFileName);
						data.actionInstances.Add(actionInst);
						ActionDatabase.instance.addAction(data);
						
						setMenuVisible(false);
					}
			
					if (GUILayout.Button("Remove Action"))
					{
						ActionDatabase.instance.removeAction(actionName);		
						setMenuVisible(false);
					}
					
					if (GUILayout.Button("Recognize Action"))
					{
						// recognize a sequence
						//  if nothing exists or no matching subsequence, add as a new sequence
						//  otherwise, add new subsequence to data
						
						if (ActionRecognizer.instance.recognizeSequence(new ActionInstance(actionFileName)))
						{
							_message = "Performing Recognition...";
						}
						else
						{
							_message = "Recognition already in progress";
						}
						setMenuVisible(false);
					}
				
				GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		
		GUI.Box(new Rect(0,0,0.2f*Screen.width,0.2f*Screen.height), "");
		GUILayout.BeginArea(new Rect(0,0,0.2f*Screen.width,0.2f*Screen.height));
			GUILayout.Label(_actionsList+"\n--\n"+_message);
		GUILayout.EndArea();
	}
	
	void setMenuVisible(bool f)
	{
		_displayMenu = f;
		gameObject.GetComponent<CameraControls>().enabled = !f;
		_updateActionsList();
	}
	
	void OnRecognitionEnd(string actionId, ActionInstance newinst)
	{
		_message = "Recognition Done\n";
		_message += " - Class: " + actionId + "\n";
		_playAction(newinst);
	}
	
	void onActionChosen(string path)
	{
		fileBrowser = null;
		if (path != null)
		{
			actionFileName = path;
		}
		setMenuVisible(true);
	}
	
	void _playAction(ActionInstance data)
	{
		AgentController[] agents = (AgentController[])FindObjectsOfType(typeof(AgentController));
		foreach(AgentController agent in agents)
		{
			agent.playOnce = playOnce;
			agent.StartAction(data);
		}
	}
	
	void _updateActionsList()
	{
		_actionsList = "Actions:";
		foreach(ActionData d in ActionDatabase.instance.actionsData)
		{
			_actionsList += "\n - " + d.actionId;
		}
	}
}
