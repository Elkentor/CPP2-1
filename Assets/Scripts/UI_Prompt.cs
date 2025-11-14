using UnityEngine;
using TMPro;

public class UI_Prompt : MonoBehaviour
{
    public static UI_Prompt Instance;
    public GameObject panel;             
    public TextMeshProUGUI text;         

    private void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void Show(string message)
    {
        if (text != null) text.text = message;
        if (panel != null) panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }
}