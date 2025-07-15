using System;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
	private IStat stat;
	private UnitCardSpec spec;
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
			case ValueType.Hp:
				OnBattleHpChange(m);
				break;
			
			case ValueType.TurnCount:
				OnBattleTurnCountChange(m);
				break;
			
			case ValueType.Shield:
				OnBattleShieldChange(m);
				break;
		}
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
		
		
		DOTween.Kill(shield);
		shield.text = $"{stat.GetValueByValueType(ValueType.Shield)}";
		shield.transform.localScale = Vector3.one * 2f;
		shield.transform.DOScale(Vector3.one,  0.5f);
	}

	private void OnBattleHpChange(BattleValueChangeNotice m)
	{
		DOTween.Kill(hp);
		hp.text = $"{stat.GetValueByValueType(ValueType.Hp)}";
		hp.transform.localScale = Vector3.one * 2f;
		hp.transform.DOScale(Vector3.one,  0.5f);
	}

	private void OnBattleTurnCountChange(BattleValueChangeNotice m)
	{
		DOTween.Kill(turnCount);
		turnCount.text = $"{stat.GetValueByValueType(ValueType.TurnCount)}";
		turnCount.transform.localScale = Vector3.one * 2f;
		turnCount.transform.DOScale(Vector3.one, 0.5f);
	}

	//todo: callback or notice?
	private void Update()
	{
	}
}