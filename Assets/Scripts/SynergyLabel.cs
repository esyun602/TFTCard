using System;
using System.Text;
using MessageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SynergyLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
			synergyCountText.text = $"{synergyCount}";
			synergyCountInfo.text = GetSynergyCountString();
		}
	}

	[SerializeField] private TextMeshProUGUI synergyCountText;
	[SerializeField] private TextMeshProUGUI synergyName;
	[SerializeField] private TextMeshProUGUI synergyCountInfo;
	[SerializeField] private Image frame;
	[SerializeField] private Image synergyDescPanel;
	[SerializeField] private TextMeshProUGUI synergyDesc;
	
	public void Initialize(SynergyCategory targetSynergyCategory, int count)
	{
		var spec = GameDataSystem.Instance.GetGameData<SynergyData>().GetSynergySpec(targetSynergyCategory);
		targetCategorySpec = spec;
		synergyName.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.SynergyNameKey);
		icon.sprite = spec.TargetSprite;

		frame.sprite = spec.GetBattleTierResource(count);
		SynergyCount = count;
		
		synergyDesc.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.CommonDescKey);
		var size = synergyDescPanel.rectTransform.sizeDelta;
		size.y = synergyDesc.preferredHeight + 50;
		synergyDescPanel.rectTransform.sizeDelta = size;
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
		strBuilder.Append("<color=#c5ae80>");
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
				strBuilder.Append($"<color=#392d17>{targetCategorySpec.SynergyCountList[i]}</color> ");
				
			}
			else
			{
				strBuilder.Append($"<color=#392d17>{targetCategorySpec.SynergyCountList[i]}</color> ");
			}
		}

		strBuilder.Append("</color>");

		return strBuilder.ToString();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		synergyDescPanel.gameObject.SetActive(true);
		synergyDescPanel.transform.position = synergyDescPanel.transform.position.GetX0z(Constant.HighlightYPos);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		synergyDescPanel.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		synergyDescPanel.gameObject.SetActive(false);
	}
}