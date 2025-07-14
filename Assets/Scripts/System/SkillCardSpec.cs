using UnityEngine;


public enum UseType
{
	Targeting,
	Global,
}

[CreateAssetMenu]
public class SkillCardSpec : ScriptableObject, ICardSpec
{
	public SkillCardStatSpec statSpec;
	public Sprite cardResource;
	public SkillCardActionData actionData;
	public UseType cardUseType;
	
	
	public Sprite CardResource => cardResource;
	public string Name => name;
	public string Desc => "Some Desc..";
}