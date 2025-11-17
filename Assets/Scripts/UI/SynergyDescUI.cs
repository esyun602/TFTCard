using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyDescUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI name;
	[SerializeField] private TextMeshProUGUI desc;
	[SerializeField] private TextMeshProUGUI member;
	[SerializeField] private Image bg;
	
	public void Initialize(SynergyCategory category)
	{
		var stringData = GameDataSystem.Instance.GetGameData<GameString>();
		var synergyData = GameDataSystem.Instance.GetGameData<SynergyData>();
		name.text = stringData.GetString(synergyData.GetSynergySpec(category).SynergyNameKey);
		desc.text = stringData.GetString(synergyData.GetSynergySpec(category).CommonDescKey);
		var strb = new StringBuilder();
		var nameList = GameDataSystem.Instance.GetGameData<CardData>().GetSynergyMembers(category).Select(x => x.NameKey);
		foreach (var nameKey in nameList)
		{
			strb.Append(stringData.GetString(nameKey));
			strb.Append(", ");
		}

		strb.Remove(strb.Length - 2, 2);

		member.text = strb.ToString();
		
		
		var size = bg.rectTransform.sizeDelta;
		size.y = desc.preferredHeight + 160;
		bg.rectTransform.sizeDelta = size;
	}
}