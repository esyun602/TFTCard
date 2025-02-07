#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ResourceUtils
{
	public static string GetResourcesPath(Object asset)
	{
#if UNITY_EDITOR
		if (asset == null) return null;

		string fullPath = AssetDatabase.GetAssetPath(asset);
		if (string.IsNullOrEmpty(fullPath))
		{
			return null;
		}

		const string resourcesPrefix = "Assets/Resources/";
		if (!fullPath.StartsWith(resourcesPrefix))
		{
			Debug.LogError($"Wrong Parameter To GetResourcesPath: {asset}, {fullPath}");
			return null;
		}

		int startIndex = resourcesPrefix.Length;

		string relativePath = fullPath.Split(".")[0].Substring(startIndex);
		return relativePath;
#else
    // 런타임 빌드에서는 AssetDatabase를 쓸 수 없음
    Debug.LogError("Use GetResourcesPath In Runtime Build Is ProHibited!!");
    return null;
#endif
	}
}