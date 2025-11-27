using System;
using System.Collections.Generic;
using System.Linq;
using Coroutine;
using DG.Tweening;
using MessageSystem;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class BattleObjectAnimationController : IMessageReceiver
{
    private int onAnimation;
    public bool OnAnimation => onAnimation > 0;
    private IBattleObject owner;
    private Material damageMaterialCache;
    private Material DamageMaterial => damageMaterialCache == null ? damageMaterialCache = owner.FrameTransform.Find("DamageFx").GetComponent<MeshRenderer>().material : damageMaterialCache;
    
    public BattleObjectAnimationController(IBattleObject owner)
    {
        this.owner = owner;
    }

    public void RunDieAction()
    {
        UnityObjectPool.GetOrCreatePool("Fx", "DieFx").Instantiate(owner.Position, quaternion.identity, followTarget: owner.Transform);
        
        int id = Shader.PropertyToID("_Lerp");
        var grayScaleMaterials = owner.FrameTransform.GetComponentsInChildren<MeshRenderer>().SelectMany(x => x.materials).Where(x => x.HasProperty(id));
        var tmpros = owner.FrameTransform.GetComponentsInChildren<TextMeshPro>();
        
        var seq = DOTween.Sequence();
        seq.AppendInterval(0);
        Action lastCallback = null;
        foreach (var material in grayScaleMaterials)
        {
            seq.Join(DOTween.To(
                () => material.GetFloat("_Lerp"),
                x => material.SetFloat("_Lerp", x),
                1f, 1f
            ));
            lastCallback += () => material.SetFloat("_Lerp", 0);
        }

        seq.Join(DOVirtual.DelayedCall(0.7f, () => SfxManager.Instance.PlayAt("death", owner.Position)));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => SfxManager.Instance.PlayAt("deathburn", owner.Position));
        foreach (var material in grayScaleMaterials)
        {
            seq.Join(DOTween.To(
                () => material.GetFloat("_BurnAmount"),
                x => material.SetFloat("_BurnAmount", x),
                1f, 1f
            ));
            lastCallback += () => material.SetFloat("_Lerp", 0);
        }

        foreach (var text in tmpros)
        {
            seq.Join(text.DOFade(0, 0.5f));
            var color = text.color;
            color.a = 1;
            lastCallback += () => text.color = color;
        }

        seq.AppendInterval(0.5f);
        seq.AppendCallback(new TweenCallback(lastCallback));

        seq.Play();
    }
    
    public void RunHitAction()
    {
        SfxManager.Instance.PlayAt("hit", owner.Position);
        onAnimation++;
        var movSeq = DOTween.Sequence();
        movSeq.Append(owner.FrameTransform
            .DOLocalMove((owner.ObjectType == ObjectType.Ally ? -1f : 1f) * 1f * owner.Transform.right,
                0.15f).SetEase(Ease.InQuart));
        movSeq.Append(owner.FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

        DamageMaterial.DOKill(true);
        DamageMaterial.DOFade(1, 0.15f).SetLoops(2, LoopType.Yoyo);
		
        movSeq.AppendCallback(() => 
            onAnimation--);
		
        movSeq.Play();
    }



    public void RunAttackMotion()
    {
        onAnimation++;
        var rotSeq = DOTween.Sequence();
        rotSeq.Append(owner.FrameTransform.DOLocalRotate(
            Quaternion.AngleAxis((owner.ObjectType == ObjectType.Ally ? -1 : 1) * 20f, Vector3.forward).eulerAngles,
            0.15f).SetEase(Ease.InQuart));

        rotSeq.Append(owner.FrameTransform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

        var movSeq = DOTween.Sequence();
        movSeq.Append(owner.FrameTransform
            .DOLocalMove((owner.ObjectType == ObjectType.Ally ? 1f : -1f) * 3f * owner.Transform.right,
                0.15f).SetEase(Ease.InQuart));
        movSeq.Append(owner.FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));
        movSeq.AppendCallback(() => 
            onAnimation--);

        movSeq.Play();
        rotSeq.Play();
    }
    
    public void RunReboundMotion()
    {
        onAnimation++;
        var rotSeq = DOTween.Sequence();
        rotSeq.Append(owner.FrameTransform.DOLocalRotate(
            Quaternion.AngleAxis((owner.ObjectType == ObjectType.Ally ? 1 : -1) * 10f, Vector3.forward).eulerAngles,
            0.15f).SetEase(Ease.InQuart));

        rotSeq.Append(owner.FrameTransform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

        var movSeq = DOTween.Sequence();
        movSeq.Append(owner.FrameTransform
            .DOLocalMove((owner.ObjectType == ObjectType.Ally ? 1f : -1f) * 1.5f * -owner.Transform.right,
                0.15f).SetEase(Ease.InQuart));
        movSeq.Append(owner.FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));
        movSeq.AppendCallback(() => 
            onAnimation--);

        movSeq.Play();
        rotSeq.Play();
    }
    
    //todo: 기본값 변경
    public void RunGaugeMotion(string key = "!!!!!")
    {
        var movSeq = DOTween.Sequence();
        movSeq.Append(owner.FrameTransform.DOPunchScale(Vector3.one * 0.2f, 0.6f, 0, 0));
        
        var textSeq = DOTween.Sequence();
        var text = UnityObjectPool.GetOrCreatePool("Fx", "TextFx")
            .Instantiate(owner.Position.GetX0z(5f) + Vector3.forward * 1f, parent: owner.Transform);
        var tmp = text.GetComponentInChildren<TextMeshPro>();
        tmp.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(key);
        textSeq.Append(tmp.DOFade(0f, 1.5f));
        textSeq.Join(text.transform.DOLocalMoveY(1.5f, 1f).SetEase(Ease.OutQuart));
        textSeq.AppendInterval(5f);
        textSeq.AppendCallback(() =>
        {
            tmp.alpha = 1f;
            text.Dispose();
        });
        
        movSeq.Play();
        textSeq.Play();
    }

	
    public void RunDodgeAction()
    {
        var movSeq = DOTween.Sequence();
        movSeq.Append(owner.FrameTransform
            .DOLocalMove((owner.ObjectType == ObjectType.Ally ? -1f : 1f) * 1f * owner.Transform.right,
                0.15f).SetEase(Ease.InQuart));
        movSeq.Append(owner.FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));
		
        movSeq.Play();
    }

    public void CatchMessage(Message m)
    {
        //todo: 분리
        if (m is DamageNotice)
        {
            RunHitAction();
        }
        else if (m is DamageDodgeNotice)
        {
            RunDodgeAction();
        }
    }
}