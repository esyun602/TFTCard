using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class UnitCardSpec : ScriptableObject, ICardSpec
{
	public UnitCardStatSpec statSpec;
	public Sprite cardResource;
	public UnitCardActionData actionData;

	public Sprite CardResource => cardResource;
	public string Name => name;
	public string Desc => "Some Desc..";
}