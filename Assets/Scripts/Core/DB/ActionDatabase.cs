using System.Xml;
using System.Xml.Serialization;

using System.Collections;
using System.Collections.Generic;

using System.IO;

using UnityEngine;

[XmlRoot("ActionDatabase")]
public class ActionDatabase
{
	private static ActionDatabase _actionDB = null;
	public static ActionDatabase instance
	{
		get
		{
			if (_actionDB == null)
			{
				_actionDB = new ActionDatabase();
			}
			return _actionDB;
		}
	}
	
	[XmlArray("ActionsArray")]
	[XmlArrayItem("ActionData")]
	public List<ActionData> actionsData;
	
	public ActionDatabase()
	{
		actionsData = new List<ActionData>();
	}
	
	public static void readDatabase()
	{
		XmlSerializer xmls = new XmlSerializer(typeof(ActionDatabase));
		try
		{
			using (FileStream fs = new FileStream(Path.Combine(Application.dataPath,"database.xml"), FileMode.Open))
			{
				_actionDB = (ActionDatabase)xmls.Deserialize(fs);
			}
		} catch(FileNotFoundException e) {
			Debug.Log("File Not found");
			Debug.LogWarning(e.Message);
		}
	}
	
	void writeDatabase()
	{
		XmlSerializer xmls = new XmlSerializer(typeof(ActionDatabase));
		using (FileStream fs = new FileStream(Path.Combine(Application.dataPath,"database.xml"), FileMode.Create))
		{
			xmls.Serialize(fs, this);
		}
	}
	
	public ActionData getAction(string actionid)
	{
		foreach(ActionData d in actionsData)
		{
			if (d.actionId == actionid)
			{
				return d;
			}
		}
		return null;
	}
	
	public void addAction(ActionData d)
	{
		// add,replace actiondata
		
		ActionData exD = null;
		foreach(ActionData dd in actionsData)
		{
			if (d.actionId == dd.actionId)
			{
				exD = dd;
			}
		}
		if (exD != null)
		{
			actionsData.Remove(exD);
		}
		actionsData.Add(d);
		writeDatabase();
	}
	
	public void removeAction(string actionId)
	{
		ActionData fd = null;
		foreach(ActionData d in actionsData)
		{
			if (d.actionId == actionId)
			{
				fd = d;
			}
		}
		if (fd != null)
		{
			actionsData.Remove(fd);
		}
		writeDatabase();
	}
}