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
			synergyCountText.text = Constant.GetFullSynergyCount($"{synergyCount}", tier);
			synergyCountInfo.text = GetSynergyCountString();
		}
	}

	[SerializeField] private TextMeshProUGUI synergyCountText;
	[SerializeField] private TextMeshProUGUI synergyName;
	[SerializeField] private TextMeshProUGUI synergyCountInfo;
	[SerializeField] private Image frame;
	[SerializeField] private SynergyDescUI descPanel;
	private SynergyTier tier;
	
	public void Initialize(SynergyCategory targetSynergyCategory, int count)
	{
		var spec = GameDataSystem.Instance.GetGameData<SynergyData>().GetSynergySpec(targetSynergyCategory);
		tier = spec.GetCurrentSynergyTier(count);

		targetCategorySpec = spec;
		synergyName.text = Constant.GetFullSynergyName(GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.SynergyNameKey), tier);
		icon.sprite = spec.TargetSprite;

		frame.sprite = spec.GetBattleTierResource(count);
		SynergyCount = count;
		
		descPanel.gameObject.SetActive(false);
		descPanel.Initialize(targetCategorySpec.SynergyCategory);
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
				strBuilder.Append($"{targetCategorySpec.SynergyCountList[i]} ");
				
			}
			else
			{
				strBuilder.Append($"{targetCategorySpec.SynergyCountList[i]} ");
			}
		}
		
		return Constant.GetFullSynergyTotalCount(strBuilder.ToString(), tier);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		descPanel.gameObject.SetActive(true);
		descPanel.transform.position = descPanel.transform.position.GetX0z(Constant.HighlightYPos);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		descPanel.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		descPanel.gameObject.SetActive(false);
	}
}