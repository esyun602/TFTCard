using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSfxEmitter : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private string sfxName;
    public void OnPointerClick(PointerEventData eventData)
    {
        SfxManager.Instance.PlayAt(sfxName, Camera.main.ScreenToWorldPoint(eventData.position));
    }
}