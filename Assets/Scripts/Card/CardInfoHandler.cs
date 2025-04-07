using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CardInfoHandler : MonoBehaviour
{
	private IStat stat;
	private CardSpec spec;
	[SerializeField] private TextMeshPro cost;
	[SerializeField] private TextMeshPro atk;
	[SerializeField] private TextMeshPro hp;
	[SerializeField] private TextMeshPro turnCount;
	[SerializeField] private TextMeshPro name;
	[SerializeField] private TextMeshPro desc;
	[SerializeField] private MeshRenderer TextureRenderer;
	
	public void Initialize(CardSpec spec, IStat stat = null)
	{
		this.spec = spec;
		this.stat = stat ?? spec.statSpec;
		
		name.text = spec.name;
		desc.text = "Some Description...";
		TextureRenderer.material.SetTexture("_MainTex", spec.cardResource.texture);
	}
	
	//todo: callback or notice?

	private void Update()
	{
		cost.text = $"{stat.Cost}";
		atk.text = $"{stat.Attack}";
		hp.text = $"{stat.Hp}";
		turnCount.text = $"{stat.TurnCount}";
	}
}