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

		[SerializeField] private SynergyDescUI descPanel;
		[SerializeField] private TextMeshProUGUI countText;
		[SerializeField] private Image frameImage;

		private RectTransform root;
		
		private int NextIndex
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

				return left;
			}
			
		}
		
		private int NextCount => NextIndex == -1 ? -1 : targetCategorySpec.SynergyCountList[NextIndex];

		private int count;

		public void Initialize(SynergyCategory targetSynergyCategory, int count, RectTransform root)
		{
			this.root = root;
			this.count = count;
			var spec = GameDataSystem.Instance.GetGameData<SynergyData>().GetSynergySpec(targetSynergyCategory);
			targetCategorySpec = spec;
			descPanel.Initialize(targetSynergyCategory);
			icon.sprite = spec.TargetSprite;
			var next = NextCount;
			countText.text = next == -1 ? $"{count}" : $"{count}/{next}";
			frameImage.sprite = spec.GetBagTierResource(count);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			descPanel.gameObject.SetActive(true);
			root.SetAsLastSibling();
			var size = root.sizeDelta;
			size.x = 600f;
			root.sizeDelta = size;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			var size = root.sizeDelta;
			size.x = 165f;
			root.sizeDelta = size;
			descPanel.gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			descPanel.gameObject.SetActive(false);
		}
	}
}