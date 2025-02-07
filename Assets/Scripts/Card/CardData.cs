using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
	public CardStatData statData;
	public Texture2D cardResource;
	public Texture2D creatureResource;
	public CardActionData actionData;
}