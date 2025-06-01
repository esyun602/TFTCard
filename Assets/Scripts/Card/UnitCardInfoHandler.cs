using System;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitCardInfoHandler : MonoBehaviour
{
	private IStat stat;
	private UnitCardSpec spec;
	[SerializeField] private TextMeshPro cost;
	[SerializeField] private TextMeshPro atk;
	[SerializeField] private TextMeshPro hp;
	[SerializeField] private TextMeshPro turnCount;
	[SerializeField] private TextMeshPro name;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private MeshRenderer TextureRenderer;
	
	public void Initialize(UnitCardSpec spec, IStat stat = null)
	{
		this.spec = spec;
		this.stat = stat ?? spec.statSpec;
		
		name.text = spec.name;
		desc.text = "Some Description...";
		TextureRenderer.material.SetTexture("_BaseMap", spec.cardResource.texture);
		
		cost.text = $"{stat.GetValueByValueType(ValueType.Cost)}";
		atk.text = $"{stat.GetValueByValueType(ValueType.Attack)}";
		hp.text = $"{stat.GetValueByValueType(ValueType.Hp)}";
		turnCount.text = $"{stat.GetValueByValueType(ValueType.TurnCount)}";
		
		NoticeSystem.Instance.Subscribe<BattleHpChangeNotice>(OnBattleHpChange);
		NoticeSystem.Instance.Subscribe<BattleTurnCountChangedNotice>(OnBattleTurnCountChange);
	}

	private void OnBattleHpChange(BattleHpChangeNotice m)
	{
		if (m.Stat != stat) return;
		DOTween.Kill(hp);
		hp.text = $"{stat.GetValueByValueType(ValueType.Hp)}";
		hp.transform.localScale = Vector3.one * 2f;
		hp.transform.DOScale(Vector3.one,  0.5f);
	}

	private void OnBattleTurnCountChange(BattleTurnCountChangedNotice m)
	{
		if (m.Stat != stat) return;
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