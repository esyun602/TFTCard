using System;
using DG.Tweening;
using MessageSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class BagUnitCard : BagUICard
{
    public override ICard TargetCard => TargetUnitCard;
    public UnitCard TargetUnitCard { get; private set; }

    protected override void InitializeInfo()
    {
        infoHandler.Initialize(TargetUnitCard, TargetUnitCard.Stat);
    }

    protected override void OnLeftClick()
    {
        if (stateMachine.CurrentState is BagUICardNormalState { IsHovered: true })
        {
            NoticeSystem.Instance.PublishSync(new BagUICardSelectedNotice(this));
            ChangeState(new BagUnitCardSelectedState(this));
        }
    }

    protected override void OnRightClick()
    {
        if (stateMachine.CurrentState is BagUICardNormalState { IsHovered: true }
            && cardPosInfo.Tile != null)
        {
            NoticeSystem.Instance.PublishSync(new BagUICardUnPlaceNotice(this, cardPosInfo.Tile));
        }
    }

    public void Initialize(UnitCard targetCard, BagUITile tile)
    {
        this.cardPosInfo = new BagCardPosInfo(tile.GetPosition(), tile);
        this.TargetUnitCard = targetCard;

        InitializeRoutine();
    }

    public void Initialize(UnitCard targetCard, Vector3 targetPos)
    {
        this.cardPosInfo = new BagCardPosInfo(targetPos, null);
        this.TargetUnitCard = targetCard;

        InitializeRoutine();
    }

    private class BagUnitCardSelectedState : IState, IUpdatable
    {
        private BagUnitCard owner;
        private Vector3 targetPos;
        private Quaternion targetRotation;
        private const float followSpeed = 8000f;
        private AnimationCurve followAnimationCurve;
        private float timePassed = 0f;

        private BagUITile currentTile;
        private bool canPlace => currentTile != null;

        public BagUnitCardSelectedState(BagUnitCard owner)
        {
            this.owner = owner;
        }

        public void Enter(IState prevState)
        {
            //todo: 액션 분리
            InputManager.Instance.InputActions.Player.UseHandCard.Enable();
            InputManager.Instance.InputActions.Player.CancelHandCard.Enable();
            InputManager.Instance.InputActions.Player.UseHandCard.performed += OnTryPlaceCard;
            InputManager.Instance.InputActions.Player.CancelHandCard.performed += OnCancelSelectCard;
            //

            followAnimationCurve = GameDataSystem.Instance.GetGameData<Constant>().CardFollowingSpeedCurve;

            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 0f;
            targetPos = mouseScreenPos;
            owner.transform.position = targetPos;

            BlockInput = InputBlockFlag.All;
        }

        private void OnTryPlaceCard(InputAction.CallbackContext obj)
        {
            if (canPlace)
            {
                if (owner.cardPosInfo.Tile != null)
                {
                    NoticeSystem.Instance.PublishSync(new BagUICardPlaceNotice(owner, currentTile));
                }

                NoticeSystem.Instance.PublishSync(new BagUICardPlaceNotice(owner, currentTile));
                owner.ChangeState(new BagUICardNormalState(owner));
            }
        }

        public void Exit(IState nextState)
        {
            owner.transform.rotation = quaternion.identity;

            InputManager.Instance.InputActions.Player.UseHandCard.Disable();
            InputManager.Instance.InputActions.Player.CancelHandCard.Disable();
            InputManager.Instance.InputActions.Player.UseHandCard.performed -= OnTryPlaceCard;
            InputManager.Instance.InputActions.Player.CancelHandCard.performed -= OnCancelSelectCard;

            BlockInput = InputBlockFlag.None;
        }

        private void OnCancelSelectCard(InputAction.CallbackContext obj)
        {
            NoticeSystem.Instance.PublishSync(new BagUICardSelectCancelNotice(owner));
            owner.ChangeState(new BagUICardNormalState(owner));
        }

        public void UpdateFrame(float dt)
        {
            //todo: optimize and fix - new input mouse pos not working
            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 0f;

            currentTile = PlayerBagPanel.Instance.CurrentHoverBagUITile;

            targetPos = canPlace ? currentTile.GetPosition() : mouseScreenPos;

            if (Vector3.Distance(targetPos, owner.transform.position) < 0.01f)
            {
                timePassed = 0f;
            }

            timePassed += dt;

            var realSpeed = followAnimationCurve.Evaluate(timePassed) * followSpeed;
            var totalTime = Vector3.Distance(targetPos, owner.transform.position) / realSpeed;
            owner.transform.position = Vector3.Lerp(owner.transform.position, targetPos, dt / totalTime);
            owner.transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(
                                                    Vector3.Distance(targetPos, owner.transform.position) * 50f *
                                                    (targetPos.x > owner.transform.position.x ? -1f : 1f), -45f, 45f),
                                                Vector3.Cross(Camera.main.transform.forward,
                                                    (targetPos - owner.transform.position).normalized)) *
                                            Camera.main.transform.localRotation;
        }
    }
}