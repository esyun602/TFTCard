
using UnityEngine;

public interface IStage
{
	public GameObject StageGameObject { get; }
	public IMap Map { get; }
	public void Load();
	public void Start();
	public void End();
	public void UnLoad();
}