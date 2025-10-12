using System;
using MessageSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 1. 생성/삭제
/// 2. 액션 보관
/// 3. 포지션 설정
/// 4. infohandler 설정
/// 5. 스탯 보관
/// 6. owner 정보 보관
/// </summary>
/// 
public class EnemySkillCardObject : MonoBehaviour
{
	public UnitSkillCard TargetCard { get; private set; }
	public UnitSkillCardBattleStat Stat { get; private set; }
	public bool IsDead { get; set; }
	
	public void Activate()
	{
		gameObject.SetActive(true);
		transform.forward = Camera.main.transform.forward;
		GetComponentInChildren<ICardInfoHandler>().Initialize(TargetCard, Stat, () => false);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
	
	public void Dispose()
	{
		GetComponentInChildren<ICardInfoHandler>().Dispose();
	}
	
    private const string enemyCardPrefabPath = "Card/EnemySkillCard";
	
    //todo: 풀링으로 수정
    public static EnemySkillCardObject Instantiate(UnitSkillCard targetSkillCard, UnitSkillCardBattleStat skillCardStat)
    {
        var cardObject = GameObject.Instantiate(Resources.Load(enemyCardPrefabPath)).AddComponent<EnemySkillCardObject>();
        cardObject.gameObject.SetActive(false);
        cardObject.TargetCard = targetSkillCard;
        cardObject.Stat = skillCardStat;
        cardObject.TargetCard.Action.SetCardBattleStat(skillCardStat);
        cardObject.IsDead = false;
        
        return cardObject;
    }
}