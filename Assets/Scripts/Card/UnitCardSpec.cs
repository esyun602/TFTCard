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
	public string NameKey => nameKey;
	public string DescKey => descKey;
}