using DG.Tweening;
using MessageSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DraftUICard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private Image tint;
	public ICard TargetCard { get; protected set; }
	protected ICardInfoHandler infoHandler;
	private SimpleStateMachine stateMachine;

	public static InputBlockFlag BlockInput { get; set; }

	private void Awake()
	{
		infoHandler = GetComponentInChildren<ICardInfoHandler>();
	}
	
	public void Initialize(ICardSpec targetCard)
	{
		stateMachine = new SimpleStateMachine();
		ChangeState(new DraftUICardNormalState(this));
		
		OnInitialize(targetCard);
	}
	
	public abstract void OnInitialize(ICardSpec targetCard);
	
	private void ChangeState(IState nextState)
	{
		stateMachine.ChangeState(nextState);
	}
	
	public void OnPointerClick(PointerEventData eventData)
	{
		if ((BlockInput & InputBlockFlag.Select) != InputBlockFlag.None) return;
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			NoticeSystem.Instance.Publish(new DraftUICardSelectedNotice(this));
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((BlockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;

		if (stateMachine.CurrentState is DraftUICardNormalState normalState)
		{
			normalState.SetHover();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if ((BlockInput & InputBlockFlag.Hover) != InputBlockFlag.None) return;

		if (stateMachine.CurrentState is DraftUICardNormalState { IsHovered: true } normalState)
		{
			normalState.RemoveHover();
		}
	}


	private void Update()
	{
		(stateMachine as IUpdatable)?.UpdateFrame(Time.deltaTime);
	}

	private class DraftUICardNormalState : IState, IUpdatable
	{
		private DraftUICard owner;
		private AnimationCurve returnAnimationCurve;

		//todo:fix
		private bool isHovered;
		public bool IsHovered => isHovered;
		private float hoverTimePassed = 0f;
		private float hoverTime = 0.2f;
		private Vector3 hoverTarget;
		private Vector3 startScale;
		private Vector3 originalScale = Vector3.one;

		public DraftUICardNormalState(DraftUICard owner)
		{
			this.owner = owner;
			hoverTarget = originalScale;
		}

		public void SetHover()
		{
			isHovered = true;
			hoverTarget = originalScale * 1.3f;
			owner.tint.DOKill();
			owner.tint.DOFade(0, 0.2f);
			RestartHover();
		}

		public void RemoveHover()
		{
			isHovered = false;
			hoverTarget = originalScale;
			owner.tint.DOKill();
			owner.tint.DOFade(0.5f, 0.2f);
			RestartHover();
		}

		public void Enter(IState prevState)
		{
			owner.tint.color = new Color(owner.tint.color.r, owner.tint.color.g, owner.tint.color.b, 0.5f); 
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
			UpdateScale(dt);
		}

		private void UpdateScale(float dt)
		{
			hoverTimePassed += dt;
			var progress = returnAnimationCurve.Evaluate(hoverTimePassed / hoverTime);
			owner.transform.localScale = Vector3.Lerp(startScale, hoverTarget, progress);
		}

		private void RestartHover()
		{
			hoverTimePassed = 0f;
			startScale = owner.transform.localScale;
		}
	}
}