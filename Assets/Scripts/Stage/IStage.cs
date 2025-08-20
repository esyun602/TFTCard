
using UnityEngine;

//배틀 관련된 것 제거
public interface IStage
{
	public GameObject StageGameObject { get; }
	public IMap Map { get; }
	public void Load();
	public void Start();
	public void End();
	public void UnLoad();
}