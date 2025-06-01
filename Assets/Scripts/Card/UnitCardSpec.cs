using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class UnitCardSpec : ScriptableObject, ICardSpec
{
	public UnitCardStatSpec statSpec;
	public Sprite cardResource;
	public CardActionData actionData;

	public Sprite CardResource => cardResource;
}