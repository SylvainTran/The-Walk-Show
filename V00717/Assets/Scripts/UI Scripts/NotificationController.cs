using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
    // The following are cached from inspector
    // Tool tip text to update
    public TMP_Text tooltipGO;
    public Image ecnb = null;
    public TMP_Text text = null;
    // The price tip
    public TMP_Text priceTip;

    private void OnEnable()
    {
        GameClockEvent._OnColonistIsDead += TriggerNotification;
    }

    private void OnDisable()
    {
        
    }

    public void TriggerNotification(GameClockEvent e, GameObject go)
    {
        TriggerTextAnimation($"{go.gameObject.name}'s death added to neural networks.");
    }

    // Floating descending text animation
    public void TriggerTextAnimation(string message)
    {
        // Re-enable and lerp the image component's alpha values from 0-1 (in), then 1-0 (out)
        ecnb.enabled = true;
        text.SetText(message);
        text.enabled = true;
        ecnb.canvasRenderer.SetAlpha(0f);
        text.canvasRenderer.SetAlpha(0f);
        ecnb.CrossFadeAlpha(1f, 3f, false);
        text.CrossFadeAlpha(1f, 3f, false);
        StartCoroutine(FadeOutImage(4.5f, 0f, 3f, 3f, ecnb, text));
    }

    // Fade out the image and its child text components after a certain delay
    private IEnumerator FadeOutImage(float delay, float target, float fadeIn, float fadeOut, Image image, TMP_Text text)
    {
        yield return new WaitForSeconds(delay);
        image.CrossFadeAlpha(target, fadeIn, false);
        text.CrossFadeAlpha(target, fadeOut, false);
        // Disable the image and text (separate behaviour from canvas renderer)
        StartCoroutine(DisableButtonImage(fadeOut, ecnb, text));
    }

    // Disable the image and its child text components
    private IEnumerator DisableButtonImage(float delay, Image image, TMP_Text text)
    {
        yield return new WaitForSeconds(delay);
        image.enabled = false;
        text.enabled = false;
    }

    // Update tool tip box content on pointer enter some UI elements
    public void UpdateToolTip(string text)
    {
        tooltipGO.SetText(text);
    }

    // Clear the tool tip canvas's TMP_Text component
    public void ClearToolTip()
    {
        tooltipGO.SetText("");
    }

    // Update tool tip box content on pointer enter some UI elements
    public void UpdatePriceTip(string text)
    {
        priceTip.SetText(text);
    }

    // Clear the tool tip canvas's TMP_Text component
    public void ClearPriceTip()
    {
        priceTip.SetText("");
    }
}
