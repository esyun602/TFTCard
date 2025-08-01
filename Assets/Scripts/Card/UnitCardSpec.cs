using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class UnitCardSpec : ScriptableObject, ICardSpec
{
	public string nameKey;
	public string descKey;
	public UnitCardStatSpec statSpec;
	public Sprite cardResource;
	public UnitCardActionData actionData;
	public SkillCardSpec targetSkillCardSpec;

	public Sprite CardResource => cardResource;
	public string Name => GameDataSystem.Instance.GetGameData<GameString>().GetString(nameKey);
	public string Desc => GameDataSystem.Instance.GetGameData<GameString>().GetString(descKey);
}