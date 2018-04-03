using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.Xml.Serialization;

[XmlRoot("ActionData")]
public class ActionData
{
	[XmlElement("ActionID")]
	public string actionId;
	
	[XmlArray("ActionInstancesArray")]
	[XmlArrayItem("ActionInstance")]
	public List<ActionInstance> actionInstances;
	
	// NEEDED by the XML serializer/deserializer
	public ActionData()
	{
		actionInstances = new List<ActionInstance>();
	}
	
	public ActionData(string actionId)
	{
		this.actionId = actionId;
		actionInstances = new List<ActionInstance>();
	}
}
