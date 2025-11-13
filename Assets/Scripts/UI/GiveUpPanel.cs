public class GiveUpPanel : UIInstance
{
    public override UIType UIType => UIType.Popup;
    protected override void Init(object param)
    {
    }

    public void OnGiveUp()
    {
        Game.Instance.ResetProgressInfo();
        Hide();
    }
    
    public void OnCancel()
    {
        Game.Instance.UIManager.RemoveUI(Id);
    }
}