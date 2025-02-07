using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapEditHelper))]
public class MapEditHelperEditor : Editor
{
	public override void OnInspectorGUI()
	{  
		GUI.enabled = false;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetMapData"));
		GUI.enabled = true;
		if (GUILayout.Button("ApplyData"))
		{
			((MapEditHelper)target).SerializeMapData();
		}

		if (GUILayout.Button("Save To Asset"))
		{
			((MapEditHelper)target).SaveToAsset();
		}
		
		if (GUILayout.Button("Restore Data From Debug Bytes"))
		{
			((MapEditHelper)target).RestoreFromDebugData();
		}
		
		if (GUILayout.Button("ReInstantiate Object"))
		{
			((MapEditHelper)target).ReInstantiateMap();
		}
	}
}