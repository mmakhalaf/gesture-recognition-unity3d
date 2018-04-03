using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using System.Xml;
using System.Xml.Serialization;

public class ActionInstance
{
	[XmlArray("PosesArray")]
	[XmlArrayItem("Pose")]
	public List<Pose> poses;
	
	public ActionInstance()
	{
		poses = new List<Pose>();
	}
	
	public ActionInstance(string filename)
	{
		poses = new List<Pose>();
		
		// read file
		using (StreamReader reader = new StreamReader(filename))
		{
			// for each 20 lines create pose
			string line;
			Pose p = new Pose();
			int nlines = 0;
			int nposes = 0;
			Vector3 pos;
			while ((line = reader.ReadLine()) != null)
			{
				string[] vals = line.Split(' ');
				pos = new Vector3(float.Parse(vals[0]), float.Parse(vals[1]), float.Parse(vals[2]));
				p.joint_locations.Add(pos);
				p.data_confidence.Add(float.Parse(vals[3]));
				
				nlines++;
				if (nlines % 20 == 0)
				{
					if (p != null)
					{
						poses.Add(p);
					}
					p = new Pose();
					nposes++;
				}
			}
				
			Debug.Log("N poses: " + nposes);
		}
		
//		for(int i = 1; i < poses.Count-1; i++)
//		{
//			Pose p = poses[i];
//			for(int c = 0; c < p.data_confidence.Count; c++)
//			{
//				float cd = p.data_confidence[c];
//				p.joint_locations[c] = (Vector3.Slerp(poses[i-1].joint_locations[c], poses[i+1].joint_locations[c], 0.5f));// + cd*p.joint_locations[c];
//			}
//		}
		
		foreach(Pose p in poses)
		{
			p.convertLocationToJointRotation();
		}
	}
}