using System;
using System.Text;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SynergyLabel : MonoBehaviour
{
	private SynergySpec targetSpec;
	public Image img;
	public Sprite offSprite;
	public Sprite onSprite;
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

	public void Initialize(Synergy targetSynergy)
	{
		var spec = GameDataSystem.Instance.GetGameData<SynergyData>().GetSynergySpec(targetSynergy);
		targetSpec = spec;
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
		strBuilder.Append("<color=#282117>");
		bool found = false;
		
		for (var i = 0; i < targetSpec.synergyCountList.Count; i++)
		{
			if (i != 0)
			{
				strBuilder.Append($"/ ");
			}
			
			if (found || synergyCount < targetSpec.synergyCountList[i])
			{
				strBuilder.Append($"{targetSpec.synergyCountList[i]} ");
			}
			else if (targetSpec.synergyCountList[i] <= synergyCount &&
			         (i + 1 >= targetSpec.synergyCountList.Count || targetSpec.synergyCountList[i + 1] > synergyCount))
			{
				found = true;
				strBuilder.Append($"<color=#2C241E>{targetSpec.synergyCountList[i]}</color> ");
				
			}
		}

		if (found)
		{
			img.sprite = onSprite;
		}
		else
		{
			img.sprite = offSprite;
		}

		strBuilder.Append("</color>");

		return strBuilder.ToString();
	}
}