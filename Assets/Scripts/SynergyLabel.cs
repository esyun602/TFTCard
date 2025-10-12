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
	[SerializeField] private Image icon;
	private int synergyCount;

	public int SynergyCount
	{
		get => synergyCount;
		set
		{
			synergyCount = value;
			synergyCountInfo.text = GetSynergyCountString();
		}
	}

	[SerializeField] private TextMeshProUGUI synergyName;
	[SerializeField] private TextMeshProUGUI synergyCountInfo;

	public void Initialize(SynergyCategory targetSynergyCategory)
	{
		var spec = GameDataSystem.Instance.GetGameData<SynergyData>().GetSynergySpec(targetSynergyCategory);
		targetCategorySpec = spec;
		synergyName.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.SynergyNameKey);
		icon.sprite = spec.TargetSprite;
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
		strBuilder.Append("<color=#5E999E>");
		bool found = false;
		
		for (var i = 0; i < targetCategorySpec.SynergyCountList.Length; i++)
		{
			if (i != 0)
			{
				strBuilder.Append($"/ ");
			}
			
			if (found || synergyCount < targetCategorySpec.SynergyCountList[i])
			{
				strBuilder.Append($"{targetCategorySpec.SynergyCountList[i]} ");
			}
			else if (targetCategorySpec.SynergyCountList[i] <= synergyCount &&
			         (i + 1 >= targetCategorySpec.SynergyCountList.Length || targetCategorySpec.SynergyCountList[i + 1] > synergyCount))
			{
				found = true;
				strBuilder.Append($"<color=#335764>{targetCategorySpec.SynergyCountList[i]}</color> ");
				
			}
			else
			{
				strBuilder.Append($"<color=#335764>{targetCategorySpec.SynergyCountList[i]}</color> ");
			}
		}

		strBuilder.Append("</color>");

		return strBuilder.ToString();
	}
}