using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
	[SerializeField, HideInInspector]
	private byte[] data;

	public void SaveMapData(byte[] data)
	{
		this.data = data;
	}

	public IMap InstantiateMap()
	{
		if (data == null || data.Length == 0)
		{
			Debug.LogError($"Wrong Map Data: {name}");
			return null;
		}
		string json = JsonCompressor.DecompressJson(data);
		var sMapData = JsonUtility.FromJson<SerializedMapInfo>(json);
		
		GameObject go = sMapData.DeSerialize();
		go.transform.position = Vector3.zero;
		if (Game.Instance.GetGameMode<StageGameMode>()?.GetCurrentStage().StageGameObject != null)
		{
			go.transform.SetParent(Game.Instance.GetGameMode<StageGameMode>().GetCurrentStage().StageGameObject.transform);
		}
		
		var mapInstance = new Map(go);
		return mapInstance;
	}
	
#if UNITY_EDITOR
	public GameObject InstantiateMapForMapEditor()
	{
		GameObject go;
		string json = "";
		if (data == null || data.Length == 0)
		{
			go = new GameObject(name);
			go.transform.position = Vector3.zero;
		}
		else
		{
			json = JsonCompressor.DecompressJson(data);
			var sMapData = JsonUtility.FromJson<SerializedMapInfo>(json);

			go = sMapData.DeSerializeForMapEditor();
		}
		
		go.transform.position = Vector3.zero;
		SetEditHelperComponentForMapEditor(go, json);
		
		return go;
	}

	private void SetEditHelperComponentForMapEditor(GameObject mapInstance, string json)
	{
		var helper = mapInstance.AddComponent<MapEditHelper>();
		helper.TargetMapData = this;
		helper.json = json;
		helper.bytes = data;
	}
#endif
}