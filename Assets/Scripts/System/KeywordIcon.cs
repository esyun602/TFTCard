using TMPro;
using UnityEngine;

public class KeywordIcon : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    private int value;

    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            text.text = value.ToString();
            OnValueChange();
        }
    }

    protected virtual void OnValueChange()
    {
        
    }

    public int Importance { get; set; }
}