using UnityEngine;
using UnityEngine.Serialization;


public enum UseType
{
	Targeting,
	Global,
}

[CreateAssetMenu]
public class SkillCardSpec : ScriptableObject, ICardSpec
{
	public string nameKey;
	public string descKey;
	public SkillCardStatSpec statSpec;
	public Sprite cardResource;
	[FormerlySerializedAs("actionData")] public SkillCardActionSpec actionSpec;
	public UseType cardUseType;
	
	
	public Sprite CardResource => cardResource;
	public string NameKey => nameKey;
	public string DescKey => descKey;
}