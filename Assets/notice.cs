using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class notice : MonoBehaviour
{
	private void OnEnable()
	{
		var images = GetComponentsInChildren<Image>();

		var fadeSeq1 = DOTween.Sequence();
		fadeSeq1.Append(images[0].DOFade(0.5f, 1f));
		fadeSeq1.AppendInterval(0.5f);
		fadeSeq1.Append(images[0].DOFade(0f, 1f));
		
		var fadeSeq2 = DOTween.Sequence();
		fadeSeq2.Append(images[1].DOFade(1f, 1f));
		fadeSeq2.AppendInterval(0.5f);
		fadeSeq2.Append(images[1].DOFade(0f, 1f));
		fadeSeq2.AppendCallback(() => gameObject.SetActive(false));
		
		fadeSeq1.Play();
		fadeSeq2.Play();
	}
}
