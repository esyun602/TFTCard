using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillCardInfoHandler : MonoBehaviour, ICardInfoHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private Image img;
	
    public void Initialize(ICard card, IStat stat)
    {
        if (card is not SkillCard)
        {
            throw new ArgumentException();
        }
		
        nameText.text = card.Name;
        desc.text = card.Desc;
        if (card.CardStaticSpec.CardResource != null)
        {
            img.sprite = card.CardStaticSpec.CardResource;
        }
    }
	
    //todo: callback or notice?
    private void Update()
    {
    }
}