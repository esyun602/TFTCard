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
	private Dictionary<ValueType, TextMeshPro> valueMap;
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
			[ValueType.Attack] = atk,
			[ValueType.Cost] = cost,
			[ValueType.Hp] = hp,
			[ValueType.TurnCount] = turnCount,
			[ValueType.Shield] = shield,
		};
		
		nameText.text = spec.name;
		desc.text = "Some Description...";
		TextureRenderer.material.SetTexture("_BaseMap", spec.cardResource.texture);
		
		cost.text = $"{stat.GetValueByValueType(ValueType.Cost)}";
		atk.text = $"{stat.GetValueByValueType(ValueType.Attack)}";
		hp.text = $"{stat.GetValueByValueType(ValueType.Hp)}";
		turnCount.text = $"{stat.GetValueByValueType(ValueType.TurnCount)}";
		
		NoticeSystem.Instance.Subscribe<BattleValueChangeNotice>(OnBattleValueChange);
	}

	private void OnBattleValueChange(BattleValueChangeNotice m)
	{
		if (m.Stat != stat) return;
		
		switch (m.Type)
		{
			case ValueType.Shield:
				OnBattleShieldChange(m);
				break;
			
			default:
				JustChangeValue(m);
				break;
		}
	}

	private void JustChangeValue(BattleValueChangeNotice m)
	{
		if (!valueMap.TryGetValue(m.Type, out var targetText)) return;
		
		DOTween.Kill(targetText);
		targetText.text = $"{stat.GetValueByValueType(m.Type)}";
		targetText.transform.localScale = Vector3.one * 2f;
		targetText.transform.DOScale(Vector3.one,  0.5f);
		
	}

	private void OnBattleShieldChange(BattleValueChangeNotice m)
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