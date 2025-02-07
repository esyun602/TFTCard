using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 타일 에디터에서 타일을 Serialize 후 저장 및 불러오기 위한 중간 정보
/// </summary>
[Serializable]
public class SerializedTileInfo
{
	public string prefabPath;
	public string name;

	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;

	public SerializedTileInfo(Transform transform, string prefabPath)
	{
		this.prefabPath = prefabPath;
		name = transform.name;
		position = transform.localPosition;
		rotation = transform.localRotation;
		scale = transform.localScale;
	}
	
	public GameObject DeSerialize()
	{
		var prefab = Resources.Load(prefabPath);
		GameObject ret = (GameObject)GameObject.Instantiate(prefab);
		ret.name = name;
		ret.transform.position = position;
		ret.transform.rotation = rotation;
		ret.transform.localScale = scale;

		return ret;
	}
	
#if UNITY_EDITOR
	public GameObject DeSerializeForMapEditor()
	{
		var prefab = Resources.Load(prefabPath);
		var ret = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
		ret.name = name;
		ret.transform.position = position;
		ret.transform.rotation = rotation;
		ret.transform.localScale = scale;

		return ret;
	}
#endif
}