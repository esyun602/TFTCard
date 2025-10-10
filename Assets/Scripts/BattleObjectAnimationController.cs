using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MessageSystem;
using TMPro;
using Unity.VisualScripting;
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

        seq.AppendInterval(0);
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
            lastCallback += () => text.color = text.color.WithAlpha(1f);
        }

        seq.AppendInterval(0.5f);
        seq.AppendCallback(new TweenCallback(lastCallback));

        seq.Play();
    }
    
    public void RunHitAction()
    {
        onAnimation++;
        var movSeq = DOTween.Sequence();
        movSeq.Append(owner.FrameTransform
            .DOLocalMove((owner.ObjectType == ObjectType.Ally ? -1f : 1f) * 1f * owner.Transform.right,
                0.15f).SetEase(Ease.InQuart));
        movSeq.Append(owner.FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

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
            Quaternion.AngleAxis((owner.ObjectType == ObjectType.Ally ? 1 : -1) * 20f, Vector3.forward).eulerAngles,
            0.15f).SetEase(Ease.InQuart));

        rotSeq.Append(owner.FrameTransform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));

        var movSeq = DOTween.Sequence();
        movSeq.Append(owner.FrameTransform
            .DOLocalMove((owner.ObjectType == ObjectType.Ally ? 1f : -1f) * 3f * -owner.Transform.right,
                0.15f).SetEase(Ease.InQuart));
        movSeq.Append(owner.FrameTransform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuart));
        movSeq.AppendCallback(() => 
            onAnimation--);

        movSeq.Play();
        rotSeq.Play();
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