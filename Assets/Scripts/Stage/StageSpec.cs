
using UnityEngine;

public abstract class StageSpec : ScriptableObject
{
	public string StageName => name.EndsWith("Spec") ? name.Substring(0, name.Length - 4) : name;
	[SerializeField] protected MapData mapData;
	public MapData MapData => mapData;
	[SerializeField] protected float camSize;
	public float CamSize => camSize;

	public abstract IStage InstantiateStage();
}