using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryBookPage : MonoBehaviour
{
	[SerializeField] private Image image;
	[SerializeField] private TextMeshProUGUI text;

	private bool initialized = false;
	
	private void OnEnable()
	{
		if (!initialized)
		{
			initialized = true;
			return;
		}
		SfxManager.Instance.Play2D("Page 05");
		var color = text.color;
		color.a = 0;
		text.color = color;

		image.DOKill();
		text.DOKill();

		text.DOFade(1, 1);
	}

	private void OnDisable()
	{
		image.DOKill();
		text.DOKill();
	}
}