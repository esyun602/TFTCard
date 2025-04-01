using System;
using TMPro;
using UnityEngine;

public class CardInfoHandler : MonoBehaviour
{
	private IStat stat;
	private CardSpec spec;
	[SerializeField] private TextMeshPro cost;
	[SerializeField] private TextMeshPro atk;
	[SerializeField] private TextMeshPro hp;
	[SerializeField] private TextMeshPro speed;
	[SerializeField] private TextMeshPro name;
	[SerializeField] private TextMeshPro desc;
	
	public void Initialize(CardSpec spec, IStat stat = null)
	{
		this.spec = spec;
		this.stat = stat ?? spec.statSpec;
	}
	
	//todo: callback or notice?

	private void Update()
	{
		cost.text = $"{stat.Cost}";
		atk.text = $"{stat.Attack}";
		hp.text = $"{stat.Hp}";
		speed.text = $"{stat.Speed}";
		
		//todo: fix
		name.text = spec.name;
		desc.text = "Some Description...";
	}
}