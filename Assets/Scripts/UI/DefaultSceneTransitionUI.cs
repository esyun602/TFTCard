using System;
using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.UI;

public class DefaultSceneTransitionUI : UIInstance
{
	public static DefaultSceneTransitionUI Instance { get; private set; }
	public override UIType UIType => UIType.DontDestroyUI;
	[SerializeField]
	private Image image;
	protected override void Init(object param)
	{
		if (Instance != null)
		{
			DestroyImmediate(this);
			return;
		}

		Instance = this;
	}

	public void Set()
	{
		transform.parent.SetAsLastSibling();
		image.gameObject.SetActive(true);
		var color = image.color;
		color.a = 0;
		image.color = color;
		image.DOKill();
		image.DOFade(1f, 0.3f);
	}

	
	
	public void Unset()
	{
		var seq = DOTween.Sequence();
		image.DOKill();
		seq.Append(image.DOFade(0f, 0.3f));
		seq.AppendCallback(() =>
		{
			image.gameObject.SetActive(false);
			NoticeSystem.Instance.Publish(new TransitionMotionDoneNotice());
		});
		seq.SetTarget(image);
		
		seq.Play();
	}
}