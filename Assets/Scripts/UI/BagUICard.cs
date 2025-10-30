using System;
using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//unit and skill
public abstract class BagUICard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
	IMessageReceiver
{
	public abstract ICard TargetCard { get; }
	protected ICardInfoHandler infoHandler;
	protected BagCardPosInfo cardPosInfo;
	protected SimpleStateMachine stateMachine;

	public static InputBlockFlag BlockInput { get; set; }

	private void Awake()
	{
		infoHandler = GetComponentInChildren<ICardInfoHandler>();
	}

	protected void InitializeRoutine()
	{
		stateMachine = new SimpleStateMachine();
		stateMachine.ChangeState(new BagUICardNormalState(this));

		InitializeInfo();
	}

	protected void ChangeState(IState nextState)
	{
		stateMachine.ChangeState(nextState);
	}

	protected abstract void InitializeInfo();


	public void OnPointerClick(PointerEventData eventData)
	{
		if ((BlockInput & InputBlockFlag.Select) != InputBlockFlag.None) return;
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			OnLeftClick();
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			OnRightClick();
		}
	}

	protected virtual void OnLeftClick()
	{
		
	}

	protected virtual void OnRightClick()
	{
		
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((BlockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;
		OnPointerEnterImpl();
	}

	protected virtual void OnPointerEnterImpl()
	{
		if (stateMachine.CurrentState is BagUICardNormalState normalState)
		{
			normalState.SetHover();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if ((BlockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;
		OnPointerExitImpl();
	}

	protected virtual void OnPointerExitImpl()
	{
		if (stateMachine.CurrentState is BagUICardNormalState { IsHovered: true } normalState)
		{
			normalState.RemoveHover();
		}
	}

	public void CatchMessage(Message m)
	{
		if (m is BagCardPosUpdateNotice notice)
		{
			cardPosInfo = notice.BagCardPosInfo;
			NoticeSystem.Instance.SendSync(m, stateMachine);
		}
	}

	private void Update()
	{
		(stateMachine as IUpdatable)?.UpdateFrame(Time.deltaTime);
	}

	protected class BagUICardNormalState : IState, IUpdatable, IMessageReceiver
	{
		private BagUICard owner;

		//일단 시간으로
		private const float returnTime = 0.5f;
		private float timePassed = 0f;
		private Vector3 startPos;
		private AnimationCurve returnAnimationCurve;

		//todo:fix
		private bool isHovered;
		public bool IsHovered => isHovered;
		private float hoverTimePassed = 0f;
		private float hoverTime = 0.2f;
		private Vector3 hoverTarget;
		private Vector3 startScale;
		private Vector3 originalScale = Vector3.one;

		public BagUICardNormalState(BagUICard owner)
		{
			this.owner = owner;
			hoverTarget = originalScale;
		}

		public void SetHover()
		{
			owner.transform.parent.SetAsLastSibling();
			owner.transform.SetAsLastSibling();
			isHovered = true;
			hoverTarget = originalScale * 1.8f;
			RestartHover();
			Restart();
		}

		public void RemoveHover()
		{
			isHovered = false;
			hoverTarget = originalScale;
			RestartHover();
		}

		public void Enter(IState prevState)
		{
			Restart();
			RestartHover();
			//todo: 임시
			returnAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardReturnAnimationCurve;
		}

		public void Exit(IState nextState)
		{
			if (isHovered) RemoveHover();
		}

		public void UpdateFrame(float dt)
		{
			UpdateAlignment(dt);
			UpdateScale(dt);
		}

		private void UpdateAlignment(float dt)
		{
			timePassed += dt;
			var progress = returnAnimationCurve.Evaluate(timePassed / returnTime);
			if (owner.cardPosInfo.Tile == null)
			{
				owner.transform.localPosition =
					Vector3.Lerp(startPos, owner.cardPosInfo.TargetPos, progress);
			}
			else
			{
				//일단 타일의 경우 global position 사용
				//todo: 수정
				owner.transform.position = Vector3.Lerp(startPos, owner.cardPosInfo.TargetPos, progress);
			}
		}

		private void UpdateScale(float dt)
		{
			hoverTimePassed += dt;
			var progress = returnAnimationCurve.Evaluate(hoverTimePassed / hoverTime);
			owner.transform.localScale = Vector3.Lerp(startScale, hoverTarget, progress);
		}

		public void CatchMessage(Message m)
		{
			if (m is BagCardPosUpdateNotice)
			{
				Restart();
			}
		}

		private void Restart()
		{
			timePassed = 0f;
			if (owner.cardPosInfo.Tile == null)
			{
				startPos = owner.transform.localPosition;
			}
			else
			{
				//일단 타일의 경우 global position 사용
				//todo: 수정
				startPos = owner.transform.position;
			}
		}

		private void RestartHover()
		{
			hoverTimePassed = 0f;
			startScale = owner.transform.localScale;
		}
	}
}