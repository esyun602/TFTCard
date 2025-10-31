using MessageSystem;
using UnityEngine;

public class DraftSelectPanel : MonoBehaviour
{
    private DraftUICard target;
    private Transform targetParent;
    [SerializeField] private Transform transparentPanel;
    [SerializeField] private Transform button;
    public void Activate(DraftUICard target)
    {
        transform.SetAsLastSibling();
        this.target = target;
        targetParent = target.transform.parent;
        target.transform.SetParent(transform, true);
        transparentPanel.SetAsLastSibling();
        button.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        target.transform.SetParent(targetParent, true);
        target.ResetState();
        target = null;
        gameObject.SetActive(false);
    }

    public void OnClickSelect()
    {
        NoticeSystem.Instance.Publish(new DraftUICardSelectedNotice(target));
        Deactivate();
    }
}