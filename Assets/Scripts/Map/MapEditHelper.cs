using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEditor;
using UnityEngine;

//todo: 파일 삭제되었을 때 디버그 폴더 clear  하는 기능 개발
public class MapEditHelper : MonoBehaviour
{
	public MapData TargetMapData;
	//디버그 용, 빌드 시 제거
	public string jsonPath;
	public string bytePath;
	//
	public string json;
	public byte[] bytes;
#if UNITY_EDITOR
	public void SerializeMapDataToJson()
	{
		var sMap = SerializeMap(transform);
		json = JsonUtility.ToJson(sMap, true);
		WriteDebugJsonFile();
	}
#endif
	private void WriteDebugJsonFile()
	{
		string directoryPath =
			Path.Combine(Application.dataPath, "Debug", ResourceUtils.GetResourcesPath(TargetMapData));
		if (string.IsNullOrEmpty(jsonPath))
		{
			jsonPath = Path.Combine(directoryPath,
				"mapInfo.json");
		}
		
		if (!string.IsNullOrEmpty(Path.GetDirectoryName(directoryPath)))
		{
			Directory.CreateDirectory(directoryPath);
		}

		File.WriteAllText(jsonPath, json);
	}
#if UNITY_EDITOR
	private SerializedMapInfo SerializeMap(Transform map)
	{
		var mapInfo = new SerializedMapInfo();
		mapInfo.name = map.name;
		for (int i = 0; i < map.childCount; i++)
		{
			var child = map.GetChild(i);
			var prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
			if (prefab)
			{
				var path = ResourceUtils.GetResourcesPath(
					PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject));
				if (!string.IsNullOrEmpty(path))
				{
					var tileInfo = new SerializedTileInfo(child, ResourceUtils.GetResourcesPath(PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject)));
					mapInfo.tileInfos.Add(tileInfo);
				}
			}
			else
			{
				var layerInfo = SerializeLayer(child);
				mapInfo.layer.Add(layerInfo);
			}
		}

		return mapInfo;
	}
	private SerializedLayerInfo SerializeLayer(Transform layer)
	{
		var layerInfo = new SerializedLayerInfo();
		layerInfo.name = layer.name;
		for (int i = 0; i < layer.childCount; i++)
		{
			var child = layer.GetChild(i);
			var prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
			if (prefab)
			{
				var tileInfo = new SerializedTileInfo(child,  ResourceUtils.GetResourcesPath(PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject)));
				layerInfo.tileInfos.Add(tileInfo);
			}
		}

		return layerInfo;
		
	}
#endif

	public void UpdateBytesFromJson()
	{
		bytes = JsonCompressor.CompressJson(json);
		WriteDebugByteFile();
	}

	private void WriteDebugByteFile()
	{
		var directoryPath =
			Path.Combine(Application.dataPath, "Debug", ResourceUtils.GetResourcesPath(TargetMapData));
		if (string.IsNullOrEmpty(bytePath))
		{
			bytePath = Path.Combine(directoryPath,
				"mapInfo.bytes");
		}
		
		if (!string.IsNullOrEmpty(Path.GetDirectoryName(directoryPath)))
		{
			Directory.CreateDirectory(directoryPath);
		}
		File.WriteAllBytes(bytePath, bytes);
	}

	//byte 기반, todo: directory not found 수정
	public void RestoreFromDebugData()
	{
		bytePath = Path.Combine(Application.dataPath, "Debug", ResourceUtils.GetResourcesPath(TargetMapData),
			"mapInfo.bytes");
		bytes = File.ReadAllBytes(bytePath);
		json = Encoding.UTF8.GetString(bytes);
	}

#if UNITY_EDITOR
	public void ReInstantiateMap()
	{
		TargetMapData.InstantiateMapForMapEditor();
		DestroyImmediate(gameObject);
	}
	public void SaveToAsset()
	{
		TargetMapData.SaveMapData(bytes);
		EditorUtility.SetDirty(TargetMapData);
	}
	public void SerializeMapData()
	{
		SerializeMapDataToJson();
		UpdateBytesFromJson();
	}
#endif
}

