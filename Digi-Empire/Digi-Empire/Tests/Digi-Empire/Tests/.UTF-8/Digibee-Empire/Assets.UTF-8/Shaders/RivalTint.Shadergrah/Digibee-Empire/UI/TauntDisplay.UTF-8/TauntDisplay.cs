// TauntDisplay.cs
// React-style toast.
using UnityEngine;
using TMPro;

public class TauntDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] float fadeTime = 3f;

    public void Show(string text)
    {
        label.text = text;
        label.CrossFadeAlpha(1f, 0.2f, false);
        CancelInvoke();
        Invoke(nameof(FadeOut), fadeTime);
    }

    void FadeOut() => label.CrossFadeAlpha(0f, 0.5f, false);
}