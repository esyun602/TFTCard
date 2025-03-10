using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class CardSpec : ScriptableObject
{
	public CardStatSpec statSpec;
	public Sprite cardResource;
	public Sprite creatureResource;
	public CardActionData actionData;
}