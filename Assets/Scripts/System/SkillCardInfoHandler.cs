using System;
using DG.Tweening;
using MessageSystem;
using TMPro;
using UnityEngine;

public class SkillCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
	private IStat stat;
	private SkillCardSpec spec;
	[SerializeField] private TextMeshPro name;
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
		
		name.text = spec.name;
		desc.text = "Some Description...";
		cost.text = $"{stat.GetValueByValueType(ValueType.Cost)}";
		TextureRenderer.material.SetTexture("_BaseMap", spec.cardResource.texture);
	}

	//todo: callback or notice?
	private void Update()
	{
	}
}