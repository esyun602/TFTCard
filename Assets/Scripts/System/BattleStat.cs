
using System.Collections.Generic;
using UnityEngine;

public class BattleStat
{
	//todo: actiondata -> action으로 만드는거 전투 중 혹은 게임 중 파라메터가 바뀔 여지는 없나?
	//                    피격 범위를 어떻게 포현할 수 있을까
	//                    키워드를 어떻게 추가/제거할 것인가 -> IOption?,
	//                    키워드가 정확히 어떤 것을 바꾸는가? -> 반격 : 데미지 후 체이닝 등록, 연속공격 : 트리거 시 체이닝 등록, 눈, 암흑 - 그냥 공격 데미지에 추가로 바르는 것, 그랩: 체이닝,   
	//                    체인을 어떻게 구성할 것인가 -> ITurnObject에서 체이닝 관리
	//battlestat -> 
	private CardStat originStat;
	public int Attack { get; }
	private int hp;

	public int Hp
	{
		get => hp;
		set => hp = Mathf.Max(value, 0);
	}
	public bool IsDead => Hp == 0;
	public int MaxHp { get; }
	public float Speed { get; }
	private List<IOption> optionList;
	//field scope 기믹
	private List<IBuff> buffList;

	public BattleStat(CardStat cardStat)
	{
		originStat = cardStat;
		Attack = cardStat.Attack;
		MaxHp = Hp = cardStat.MaxHp;
		Speed = cardStat.Speed;
	}
}