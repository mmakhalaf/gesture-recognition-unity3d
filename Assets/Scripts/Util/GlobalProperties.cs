using UnityEngine;

using System.Collections;

using System.IO;

public class GlobalProperties
{
//	public static string datasetPath = "D:\\_work_repo_windows\\datasets\\_Actions\\MSRAction3DSkeletonReal3D\\";
	public static string datasetPath = Path.Combine(Application.dataPath, Path.Combine("Dataset","MSRAction3DSkeletonReal3D"));
}