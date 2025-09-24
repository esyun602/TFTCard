using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleCardListPanelGenState
{
	public List<BattleCardObjectInHand> cardInfoList;
}

public class BattleCardListPanel : UIInstance
{
	public override UIType UIType => UIType.SceneUI;
	
	[SerializeField] private float bottomOffsetHeight;

	[SerializeField] private float originHeight;
	[SerializeField] private Vector3 leftTopOffset;
	[SerializeField] private float horizontalSpace;
	[SerializeField] private float verticalSpace;
	[SerializeField] private int cardCountPerRow;

	private List<PooledUnityObject> poList;
	[SerializeField] private RectTransform contentRect;
	private BattleCardListPanelGenState genState;
	
	private int CardRowCount => 
		genState.cardInfoList.Count == 0 
			? 0 
			: (genState.cardInfoList.Count - 1) / cardCountPerRow + 1;
	
	protected override void Init(object param)
	{
		if (param is not BattleCardListPanelGenState genState)
		{
			throw new ArgumentException();
		}

		poList = new();

		this.genState = genState;
		var allyUnitSkillPool = UnityObjectPool.GetOrCreateUIPool("UIAllySkillCard");
		var enemyUnitSkillPool = UnityObjectPool.GetOrCreateUIPool("UIEnemySkillCard");
		var tacticsPool = UnityObjectPool.GetOrCreateUIPool("UITacticsCard");
		allyUnitSkillPool.transform.SetParent(transform);
		tacticsPool.transform.SetParent(transform);
		enemyUnitSkillPool.transform.SetParent(transform);
		for (var i = 0; i < genState.cardInfoList.Count; i++)
		{
			var card = genState.cardInfoList[i];
			PooledUnityObject po = null;
			if (card.TargetCard is UnitSkillCard)
			{
				if (card.CardType == ObjectType.Enemy)
				{
					po = enemyUnitSkillPool.Instantiate(CalculatePosWithIdx(i), useLocalPos: true);
				}
				else
				{
					po = allyUnitSkillPool.Instantiate(CalculatePosWithIdx(i), useLocalPos: true);
				}
			}
			else if(card.TargetCard is TacticsCard)
			{
				po = tacticsPool.Instantiate(CalculatePosWithIdx(i), useLocalPos: true);
			}

			po.AddComponent<BattleUICard>().Initialize(card);
			
			poList.Add(po);
		}

		ExpandContentArea();
	}

	public void OnClose()
	{
		Game.Instance.UIManager.RemoveUI(Id);
	}
	
	protected override void OnRemove()
	{
		foreach (var po in poList)
		{
			po.Dispose();
		}

		poList = null;
	}

	private Vector3 CalculatePosWithIdx(int idx)
	{
		return leftTopOffset
		       + Vector3.right * ((idx % cardCountPerRow) * horizontalSpace)
		       + Vector3.down * ((idx / cardCountPerRow) * verticalSpace);
	}
	
	private void ExpandContentArea()
	{
		//todo: fix
		contentRect.offsetMin = new Vector2(0, Mathf.Min(0,
			-(CardRowCount) * verticalSpace + leftTopOffset.y + originHeight - bottomOffsetHeight));
	}
}