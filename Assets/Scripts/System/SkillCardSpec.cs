using UnityEngine;

[CreateAssetMenu]
public class SkillCardSpec : ScriptableObject, ICardSpec
{
	public SkillCardStatSpec statSpec;
	public Sprite cardResource;
	public SkillCardActionData actionData;
	public string cpp;

	public Sprite CardResource => cardResource;
	public string cardPrefabPath => cpp;
	public string Name => name;
	public string Desc => "Some Desc..";
}