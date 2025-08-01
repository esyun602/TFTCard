using System;
using System.Text;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SynergyLabel : MonoBehaviour
{
	private SynergySpec targetCategorySpec;

	private int synergyCount;

	public int SynergyCount
	{
		get => synergyCount;
		set
		{
			synergyCount = value;
			synergyCountInfo.text = GetSynergyCountString();
			synergyCountText.text = $"{synergyCount}";
		}
	}

	[SerializeField] private TextMeshProUGUI synergyName;
	[SerializeField] private TextMeshProUGUI synergyCountInfo;
	[SerializeField] private TextMeshProUGUI synergyCountText;

	public void Initialize(SynergyCategory targetSynergyCategory)
	{
		var spec = GameDataSystem.Instance.GetGameData<SynergyData>().GetSynergySpec(targetSynergyCategory);
		targetCategorySpec = spec;
		synergyName.text = spec.synergyName;
	}

	private string GetSynergyCountString()
	{
#if UNITY_EDITOR
		if (synergyCount <= 0)
		{
			throw new Exception();
		}
#endif

		var strBuilder = new StringBuilder();
		strBuilder.Append("<color=\"grey\">");
		bool found = false;
		
		for (var i = 0; i < targetCategorySpec.synergyCountList.Count; i++)
		{
			if (i != 0)
			{
				strBuilder.Append($"/ ");
			}
			
			if (found || synergyCount < targetCategorySpec.synergyCountList[i])
			{
				strBuilder.Append($"{targetCategorySpec.synergyCountList[i]} ");
			}
			else if (targetCategorySpec.synergyCountList[i] <= synergyCount &&
			         (i + 1 >= targetCategorySpec.synergyCountList.Count || targetCategorySpec.synergyCountList[i + 1] > synergyCount))
			{
				found = true;
				strBuilder.Append($"<color=\"white\">{targetCategorySpec.synergyCountList[i]}</color> ");
				
			}
		}

		strBuilder.Append("</color>");

		return strBuilder.ToString();
	}
}