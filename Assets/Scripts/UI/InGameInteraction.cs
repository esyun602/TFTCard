
using MessageSystem;
using TMPro;
using UnityEngine;

public class InGameInteraction : UIInstance
{
	[SerializeField] private TextMeshProUGUI goldText;
	public override UIType UIType => UIType.DontDestroyUI;
	public static InGameInteraction Instance { get; private set; }
	protected override void Init(object param)
	{
		if (Instance != null)
		{
			DestroyImmediate(this);
			return;
		}

		Instance = this;

		goldText.text = GetGoldText(Game.Instance.GetPlayer().CurrentPlayInfo.Gold);
		NoticeSystem.Instance.Subscribe<GoldUpdateNotice>(OnGoldUpdate);
	}

	protected override void OnRemove()
	{
		NoticeSystem.Instance.Unsubscribe<GoldUpdateNotice>(OnGoldUpdate);
	}

	private void OnGoldUpdate(GoldUpdateNotice m)
	{
		goldText.text = GetGoldText(m.Value);
	}

	private string GetGoldText(int val)
	{
		return  $"{val}G";
	}


	public void OpenBagPanel()
	{
		Game.Instance.UIManager.GenerateUI<PlayerBagPanel>();
	}
}