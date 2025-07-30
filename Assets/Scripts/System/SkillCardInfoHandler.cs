using System;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
	private IStat stat;
	private SkillCardSpec spec;
	[SerializeField] private TextMeshPro nameText;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private TextMeshPro cost;
	[SerializeField] private MeshRenderer TextureRenderer;
	
	public void Initialize(ICardSpec cardSpec, IStat stat = null)
	{
		if (cardSpec is not SkillCardSpec spec)
		{
			throw new ArgumentException();
		}
		this.spec = spec;
		this.stat = stat ?? spec.statSpec;
		
		nameText.text = spec.name;
		desc.text = "Some Description...";
		cost.text = $"{stat.GetValueByValueType(BattleValueType.Cost)}";
		TextureRenderer.material.SetTexture("_BaseMap", spec.cardResource.texture);
	}

	//todo: callback or notice?
	private void Update()
	{
	}
}