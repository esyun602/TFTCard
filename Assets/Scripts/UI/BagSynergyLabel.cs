using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
	public class BagSynergyLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private SynergySpec targetCategorySpec;
		[SerializeField] private Image icon;
		private int synergyCount;

		[SerializeField] private TextMeshProUGUI synergyName;
		[SerializeField] private Image synergyDescPanel;
		[SerializeField] private TextMeshProUGUI synergyDesc;
		[SerializeField] private TextMeshProUGUI countText;

		private int NextCount
		{
			get
			{
				if (count >= targetCategorySpec.SynergyCountList[^1])
				{
					return -1;
				}
				
				int left = 0;
				int right = targetCategorySpec.SynergyCountList.Length;
				while (left < right)
				{
					int mid = (left + right) / 2;
					if (targetCategorySpec.SynergyCountList[mid] <= count)
						left = mid + 1;
					else
						right = mid;
				}

				return targetCategorySpec.SynergyCountList[left];
			}
		}

		private int count;

		public void Initialize(SynergyCategory targetSynergyCategory, int count)
		{
			this.count = count;
			var spec = GameDataSystem.Instance.GetGameData<SynergyData>().GetSynergySpec(targetSynergyCategory);
			targetCategorySpec = spec;
			synergyName.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.SynergyNameKey);
			synergyDesc.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.CommonDescKey);
			var size = synergyDescPanel.rectTransform.sizeDelta;
			size.y = synergyDesc.preferredHeight + 100;
			synergyDescPanel.rectTransform.sizeDelta = size;
			icon.sprite = spec.TargetSprite;
			var next = NextCount;
			countText.text = next == -1 ? $"{count}" : $"{count}/{next}";
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			synergyDescPanel.gameObject.SetActive(true);
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
}