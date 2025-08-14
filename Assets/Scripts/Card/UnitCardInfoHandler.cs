using System;
using System.Collections.Generic;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
	private IStat stat;
	private ICard targetCard;
	private Dictionary<BattleValueType, TextMeshPro> valueMap;
	[SerializeField] private TextMeshPro atk;
	[SerializeField] private TextMeshPro hp;
	[SerializeField] private TextMeshPro turnCount;
	[SerializeField] private TextMeshPro nameText;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private MeshRenderer TextureRenderer;
	[SerializeField] private TextMeshPro shield;
	
	public void Initialize(ICard card, IStat stat)
	{
		if (card is not UnitCard)
		{
			throw new ArgumentException();
		}

		valueMap = new()
		{
			[BattleValueType.Attack] = atk,
			[BattleValueType.Hp] = hp,
			[BattleValueType.TurnCount] = turnCount,
			[BattleValueType.Shield] = shield,
		};

		this.stat = stat;

		targetCard = card;
		nameText.text = card.Name;
		desc.text = card.Desc;
		if (card.CardStaticSpec.CardResource != null)
		{
			TextureRenderer.material.SetTexture("_BaseMap", card.CardStaticSpec.CardResource.texture);
		}
		
		atk.text = $"{stat.GetValueByValueType(BattleValueType.Attack)}";
		hp.text = $"{stat.GetValueByValueType(BattleValueType.Hp)}";
		turnCount.text = $"{stat.GetValueByValueType(BattleValueType.TurnCount)}";
		
		//todo: important
		//todo: dispose
		NoticeSystem.Instance.Subscribe<UnitBattleValueChangeNotice>(OnBattleValueChange);
	}

	private void OnBattleValueChange(UnitBattleValueChangeNotice m)
	{
		if (m.Stat != stat) return;
		
		switch (m.Type)
		{
			case BattleValueType.Shield:
				OnBattleShieldChange(m);
				break;
			
			default:
				JustChangeValue(m);
				break;
		}
	}

	private void JustChangeValue(UnitBattleValueChangeNotice m)
	{
		if (!valueMap.TryGetValue(m.Type, out var targetText)) return;
		
		DOTween.Kill(targetText);
		targetText.text = $"{stat.GetValueByValueType(m.Type)}";
		targetText.transform.localScale = Vector3.one * 2f;
		targetText.transform.DOScale(Vector3.one,  0.5f);

		desc.text = targetCard.Desc;
	}

	private void OnBattleShieldChange(UnitBattleValueChangeNotice m)
	{
		if (m.ChangedValue == 0)
		{
			shield.gameObject.SetActive(false);
		}
		else if (m.ChangedValue > 0 && !shield.gameObject.activeSelf)
		{
			shield.gameObject.SetActive(true);	
		}

		JustChangeValue(m);
	}

	//todo: callback or notice?
	private void Update()
	{
	}
}