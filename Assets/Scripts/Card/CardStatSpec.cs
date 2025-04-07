using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class CardStatSpec : ScriptableObject, IStat
{
	public int hp;
	public int attack;
	public int turnCount;
	public int cost;
	public Synergy[] synergy;

	#region IStat

	int IStat.MaxHp => hp;
	int IStat.Hp => hp;
	int IStat.MaxTurnCount => turnCount;
	public int TurnCount => turnCount;
	int IStat.Attack => attack;
	int IStat.Cost => cost;

	#endregion
}