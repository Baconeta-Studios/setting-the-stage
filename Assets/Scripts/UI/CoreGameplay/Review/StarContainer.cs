using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarContainer : MonoBehaviour
{
    [Header("Star Image")]
    public Image starImage;          // The main UI image displaying the stars
    public Sprite[] starSprites;     // 11 images: 0, 0.5, 1, ... 5 stars

    [Header("Animation Settings")]
    [Tooltip("Total animation duration for all stars to fill")]
    public float animationDuration = 1f;
    public AnimationCurve revealCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio")]
    public AudioSource audioSource;  // Audio source for star sounds
    public AudioClip stepSound;      // Sound to play for each star step

    private int targetIndex = 0;

    /// <summary>
    /// Reveal stars with optional animation
    /// starsEarned: 0 to 5, can include halves
    /// animate: true = animate, false = instant
    /// </summary>
    public void RevealStars(float starsEarned, bool animate = true)
    {
        StopAllCoroutines();

        starsEarned = Mathf.Clamp(starsEarned, 0f, 5f);
        targetIndex = Mathf.RoundToInt(starsEarned * 2f); // multiply by 2 for 0.5 steps
        targetIndex = Mathf.Clamp(targetIndex, 0, starSprites.Length - 1);

        if (animate)
        {
            StartCoroutine(AnimateStarsCrossfade());
        }
        else
        {
            starImage.sprite = starSprites[targetIndex];
            PlayStepAudio();
        }
    }

    private IEnumerator AnimateStarsCrossfade()
    {
        // Determine starting index based on current sprite
        int startIndex = 0;
        for (int i = 0; i < starSprites.Length; i++)
        {
            if (starImage.sprite == starSprites[i])
            {
                startIndex = i;
                break;
            }
        }

        int currentIndex = startIndex;

        while (currentIndex < targetIndex)
        {
            int nextIndex = currentIndex + 1;

            // Create overlay for crossfade
            GameObject overlayGO = new GameObject("StarOverlay");
            overlayGO.transform.SetParent(starImage.transform, false);
            Image overlayImage = overlayGO.AddComponent<Image>();
            overlayImage.sprite = starSprites[nextIndex];
            overlayImage.color = new Color(1f, 1f, 1f, 0f); // transparent at start

            // Match overlay RectTransform to parent
            RectTransform overlayRect = overlayGO.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayRect.localScale = Vector3.one;

            float elapsed = 0f;
            float stepDuration = animationDuration / Mathf.Max(targetIndex - startIndex, 1);

            while (elapsed < stepDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / stepDuration);
                overlayImage.color = new Color(1f, 1f, 1f, revealCurve.Evaluate(t));
                yield return null;
            }

            // Swap main sprite and destroy overlay
            starImage.sprite = starSprites[nextIndex];
            Destroy(overlayGO);
            PlayStepAudio();

            currentIndex = nextIndex;
        }

        // Ensure final sprite
        starImage.sprite = starSprites[targetIndex];
        PlayStepAudio();
    }

    private void PlayStepAudio()
    {
        if (audioSource != null && stepSound != null)
        {
            audioSource.PlayOneShot(stepSound);
        }
    }
}
