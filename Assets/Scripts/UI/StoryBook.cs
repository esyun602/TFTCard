using System;
using UnityEngine;
using UnityEngine.UI;

public class StoryBook : MonoBehaviour
{
	[SerializeField] private GameObject[] pages;
	[SerializeField] private Image[] dots;
	[SerializeField] private GameObject left;
	[SerializeField] private GameObject right;
	[SerializeField] private GameObject skip;
	[SerializeField] private GameObject end;
	private int currentIdx;
	private int MaxIdx => pages.Length - 1;
	
	private void Start()
	{
		currentIdx = 0;
		UpdatePage();
	}

	public void SetNextPage()
	{
		currentIdx++;
		UpdatePage();
	}

	public void SetPrevPage()
	{
		currentIdx--;
		UpdatePage();
	}

	private void UpdatePage()
	{
		for (var i = 0; i < pages.Length; i++)
		{
			pages[i].SetActive(i == currentIdx);
		}
		
		for (var i = 0; i < pages.Length; i++)
		{
			dots[i].color = i == currentIdx ? Color.black : Color.white;
		}
		
		if (currentIdx == MaxIdx)
		{
			skip.SetActive(false);
			end.SetActive(true);
			left.SetActive(true);
			right.SetActive(false);
		}
		else if (currentIdx == 0)
		{
			skip.SetActive(true);
			end.SetActive(false);
			left.SetActive(false);
			right.SetActive(true);
		}
		else
		{
			skip.SetActive(true);
			end.SetActive(false);
			left.SetActive(true);
			right.SetActive(true);
		}
		
	}
}