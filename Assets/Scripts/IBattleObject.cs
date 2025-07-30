
using UnityEngine;

public interface IBattleObject
{
	public ObjectType ObjectType { get; }
	public Vector3 Position { get; }
	public Transform Transform { get; }
	public Transform FrameTransform { get; }
	public UnitCardBattleStat UnitCardBattleStat { get; }
	public void Damage(IBattleObject sender, int dmg);
}

public static class IBattleObjectExtensions
{
	//todo: 이후 기능이 많이 추가되면 데미지 관련 정책을 별도 interface로 개별적으로 분산시키는게 좋을듯
	public static int CalculateDamageFromStat(this IBattleObject bo, int dmg)
	{
		var catalyst = bo.UnitCardBattleStat.GetValueByValueType(BattleValueType.Catalyst);

		return dmg + catalyst;
	}
}