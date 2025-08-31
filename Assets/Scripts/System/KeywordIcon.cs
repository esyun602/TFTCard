using TMPro;
using UnityEngine;

public class KeywordIcon : MonoBehaviour
{
	[SerializeField] private TextMeshPro text;
	private int value;
	public void SetValue(int value)
	{
		this.value = value;
		text.text = value.ToString();
	}
	
	public int Importance { get; set; }
}