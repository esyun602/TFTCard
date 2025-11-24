using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryBookPage : MonoBehaviour
{
	[SerializeField] private Image image;
	[SerializeField] private TextMeshProUGUI text;

	private void OnEnable()
	{
		SfxManager.Instance.Play2D("Page 05");
		var color = text.color;
		color.a = 0;
		text.color = color;
		image.color = new Color(1, 1, 1, 0);

		image.DOKill();
		text.DOKill();

		image.DOFade(1, 1);
		text.DOFade(1, 1);
	}

	private void OnDisable()
	{
		image.DOKill();
		text.DOKill();
	}
}