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
	private UnitCardSpec spec;
	private Dictionary<BattleValueType, TextMeshPro> valueMap;
	[SerializeField] private TextMeshPro cost;
	[SerializeField] private TextMeshPro atk;
	[SerializeField] private TextMeshPro hp;
	[SerializeField] private TextMeshPro turnCount;
	[SerializeField] private TextMeshPro nameText;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private MeshRenderer TextureRenderer;
	[SerializeField] private TextMeshPro shield;
	
	public void Initialize(ICardSpec cardSpec, IStat stat = null)
	{
		if (cardSpec is not UnitCardSpec spec)
		{
			throw new ArgumentException();
		}
		
		this.spec = spec;
		this.stat = stat ?? spec.statSpec;

		valueMap = new()
		{
			[BattleValueType.Attack] = atk,
			[BattleValueType.Cost] = cost,
			[BattleValueType.Hp] = hp,
			[BattleValueType.TurnCount] = turnCount,
			[BattleValueType.Shield] = shield,
		};
		
		nameText.text = spec.name;
		desc.text = "Some Description...";
		TextureRenderer.material.SetTexture("_BaseMap", spec.cardResource.texture);
		
		cost.text = $"{stat.GetValueByValueType(BattleValueType.Cost)}";
		atk.text = $"{stat.GetValueByValueType(BattleValueType.Attack)}";
		hp.text = $"{stat.GetValueByValueType(BattleValueType.Hp)}";
		turnCount.text = $"{stat.GetValueByValueType(BattleValueType.TurnCount)}";
		
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