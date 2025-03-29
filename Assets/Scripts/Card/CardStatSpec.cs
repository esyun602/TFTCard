using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class CardStatSpec : ScriptableObject
{
	public int hp;
	public int attack;
	public int speed;
	public int cost;
	public Synergy[] synergy;
}