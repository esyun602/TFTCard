using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelGenState
{
    public EventPanelGenState(EventSpec spec, Action onRoutineEnd)
    {
        EventSpec = spec;
        OnRoutineEnd = onRoutineEnd;
    }

    public EventSpec EventSpec { get; }
    public Action OnRoutineEnd { get; }
}

public class EventPanel : UIInstance
{
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField]
    private Image mainImage;
    [SerializeField]
    private Transform buttonParent;
    [SerializeField]
    private TextMeshProUGUI descText;
    public override UIType UIType => UIType.SceneUI;
    private List<PooledUnityObject> buttonPoList;

    private Action endAction;
    
    protected override void Init(object param)
    {
        if (param is not EventPanelGenState state)
        {
            throw new ArgumentException();
        }

        buttonPoList = new();
        
        UnityObjectPool.GetOrCreateUIPool("EventButton").transform.SetParent(transform);
        endAction = state.OnRoutineEnd;
        
        ApplyEventSpec(state.EventSpec);
    }

    private void ApplyEventSpec(EventSpec spec)
    {
        foreach (var po in buttonPoList)
        {
            po.Dispose();
        }
        buttonPoList.Clear();
        
        categoryText.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.CategoryKey);
        mainImage.sprite = spec.TargetSprite;
        descText.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(spec.Desc);
        
        var size = descText.rectTransform.sizeDelta;
        size.y = descText.preferredHeight + 100;
        descText.rectTransform.sizeDelta = size;

        foreach (var info in spec.ContinueEventInfos)
        {
            var po = UnityObjectPool.GetOrCreateUIPool("EventButton").Instantiate(parent: buttonParent);
            buttonPoList.Add(po);
            var button = po.GetComponent<Button>();
            var eventButton = po.GetComponent<EventButton>();
            var nextEvent = GameDataSystem.Instance.GetGameData<EventData>().GetEvent(info.NextEvent);
            
            eventButton.SetText(info.Desc);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                ApplyEventSpec(nextEvent);
            });
        }

        foreach (var endInfo in spec.EndInfos)
        {
            var po = UnityObjectPool.GetOrCreateUIPool("EventButton").Instantiate(parent: buttonParent);
            buttonPoList.Add(po);
            var button = po.GetComponent<Button>();
            var eventButton = po.GetComponent<EventButton>();

            eventButton.SetText(endInfo.Desc);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { endAction?.Invoke(); });
        }
    }
}