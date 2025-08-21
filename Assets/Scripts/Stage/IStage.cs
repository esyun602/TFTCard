
using UnityEngine;

public enum StageType
{
	EventStage = 0,
	BattleStage = 1,
	BossStage = 2,
}

//배틀 관련된 것 제거
public interface IStage
{
	public StageType StageType { get; }
	public GameObject StageGameObject { get; }
	public void Load();
	public void Start();
	public void End();
	public void UnLoad();
}