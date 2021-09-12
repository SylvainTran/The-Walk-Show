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
    public GameController GameController;
    public GameObject FXCanvas;
    public GameObject FXParticleSystem;
    public TMP_Text FXCanvasMessage;
    public ParticleSystem ps;
    public float notificationFXDuration = 5.0f;
    public GameObject subscriberRequestWindow;

    private void OnEnable()
    {
        Bot._OnMainActorIsDead += TriggerNotification;
        Viewer._OnNewSubscriberAction += TriggerSubscriberNotification;
        Viewer._OnNewDonationAction += TriggerDonationNotification;
        ChannelController._OnSubscriberRequestAction += TriggerSubscriberRequest;
        Subscriber._OnNewSubscriberRequestAction += TriggerSubscriberRequest;
    }

    private void OnDisable()
    {
        Bot._OnMainActorIsDead -= TriggerNotification;
        Viewer._OnNewSubscriberAction -= TriggerSubscriberNotification;
        Viewer._OnNewDonationAction -= TriggerDonationNotification;
        ChannelController._OnSubscriberRequestAction -= TriggerSubscriberRequest;
        Subscriber._OnNewSubscriberRequestAction -= TriggerSubscriberRequest;
    }

    private void Start()
    {
        ps = FXParticleSystem.GetComponent<ParticleSystem>();
    }
    public void TriggerSubscriberNotification(string subscriberName)
    {
        // Pop up notification button at the top
        TriggerTextAnimation($"{subscriberName} just subscribed to the channel! {GameController.encouragementDatabase.encouragements[UnityEngine.Random.Range(0, GameController.encouragementDatabase.encouragements.Length)]}");

        // Confetti animation pops up from FX Canvas bottom right
        if (ps != null || !ps.isPlaying || ps.isPaused || ps.isStopped)
        {
            ps.Play();
        }
        FXCanvas.GetComponentInChildren<TMP_Text>().SetText($"{subscriberName} started following you.");
        FXCanvas.SetActive(true);
        
        StartCoroutine(FadeOutCanvas(FXCanvas, notificationFXDuration));
    }

    public void TriggerDonationNotification(string donatorName, int donationAmount)
    {
        TriggerTextAnimation($"{donatorName} just donated to the cause. {GameController.encouragementDatabase.encouragements[UnityEngine.Random.Range(0, GameController.encouragementDatabase.encouragements.Length)]}");
        if (ps != null || !ps.isPlaying || ps.isPaused || ps.isStopped)
        {
            ps.Play();
        }
        FXCanvasMessage.SetText($"{donatorName} just donated ${donationAmount}.");
        FXCanvas.SetActive(true);
        StartCoroutine(FadeOutCanvas(FXCanvas, notificationFXDuration));
    }

    public void TriggerSubscriberRequest(string subscriberName, string message)
    {
        TriggerTextAnimation(1.0f, 5.0f, false, subscriberRequestWindow, $"{subscriberName} has a special message for you: {message}");
    }

    public IEnumerator FadeOutCanvas(GameObject canvas, float delay)
    {
        yield return new WaitForSeconds(delay);
        canvas.SetActive(false);
        
        if (ps != null)
        {
            if (ps.isPlaying)
            {
                ps.Stop();
            }
        }
    }

    public void TriggerNotification(GameObject go)
    {
        TriggerTextAnimation($"{go.gameObject.name}'s data was preservedr");
    }

    /// <summary>
    /// Default version overload
    /// </summary>
    /// <param name="message"></param>
    public void TriggerTextAnimation(string message)
    {
        // Re-enable and lerp the image component's alpha values from 0-1 (in), then 1-0 (out)
        ecnb.enabled = true;
        text.SetText(message);
        text.enabled = true;
        ecnb.canvasRenderer.SetAlpha(0f);
        text.canvasRenderer.SetAlpha(0f);
        ecnb.CrossFadeAlpha(1f, 6f, false);
        text.CrossFadeAlpha(1f, 6f, false);
        StartCoroutine(FadeOutImage(4.5f, 0f, 3f, 1.5f, ecnb, text));
    }

    /// <summary>
    /// The other version for modularity
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="duration"></param>
    /// <param name="ignoreTimeScale"></param>
    /// <param name="window"></param>
    /// <param name="message"></param>
    // Floating descending text animation
    public void TriggerTextAnimation(float alpha, float duration, bool ignoreTimeScale, GameObject window, string message)
    {
        // Re-enable and lerp the image component's alpha values from 0-1 (in), then 1-0 (out)
        Image image = window.GetComponent<Image>();
        TMP_Text text = window.GetComponentInChildren<TMP_Text>();

        image.enabled = true;
        text.SetText(message);
        text.enabled = true;

        image.canvasRenderer.SetAlpha(0f);
        text.canvasRenderer.SetAlpha(0f);
        image.CrossFadeAlpha(alpha, duration, false);
        text.CrossFadeAlpha(alpha, duration/3, false);
        StartCoroutine(FadeOutImage(15.0f, 0f, 0f, 0f, image, text));
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
}
