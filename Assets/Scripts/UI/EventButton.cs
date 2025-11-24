using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EventButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textUI;

    public void SetText(string text)
    {
        textUI.text = GameDataSystem.Instance.GetGameData<GameString>().GetString(text);
    }
}