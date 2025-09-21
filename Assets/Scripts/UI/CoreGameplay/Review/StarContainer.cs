using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Audio; // assuming your sound system is in this namespace

public class StarContainer : MonoBehaviour
{
    [Header("Star Image")]
    public Image starImage;          // The main UI image displaying the stars
    public Sprite[] starSprites;     // 11 images: 0, 0.5, 1, ... 5 stars

    [Header("Animation Settings")]
    [Tooltip("Seconds per full star fill (used to scale animation duration)")]
    public float secondsPerStar = 0.7f;
    public AnimationCurve revealCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio Names")]
    public string star1Sound;
    public string star2Sound;
    public string star3Sound;
    public string star4Sound;
    public string star5Sound;

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
            StartCoroutine(AnimateStarsCrossfade(starsEarned));
        }
        else
        {
            starImage.sprite = starSprites[targetIndex];
        }
    }

    private IEnumerator AnimateStarsCrossfade(float starsEarned)
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

        // How many steps we need to animate (0.5 per step)
        int steps = Mathf.Max(targetIndex - startIndex, 1);

        // Duration is based on stars earned (full stars * secondsPerStar)
        float totalDuration = starsEarned * secondsPerStar;
        float stepDuration = totalDuration / steps;

        // 🎵 Start audio immediately
        PlayResultAudio(starsEarned);

        while (currentIndex < targetIndex)
        {
            int nextIndex = currentIndex + 1;

            // Create overlay for crossfade
            GameObject overlayGO = new GameObject("StarOverlay");
            overlayGO.transform.SetParent(starImage.transform, false);
            Image overlayImage = overlayGO.AddComponent<Image>();
            overlayImage.sprite = starSprites[nextIndex];
            overlayImage.color = new Color(1f, 1f, 1f, 0f);

            // Match overlay RectTransform to parent
            RectTransform overlayRect = overlayGO.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayRect.localScale = Vector3.one;

            float elapsed = 0f;
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

            currentIndex = nextIndex;
        }

        // Ensure final sprite
        starImage.sprite = starSprites[targetIndex];
    }

    private void PlayResultAudio(float starsEarned)
    {
        int rounded = Mathf.FloorToInt(starsEarned);

        string soundName = null;
        switch (rounded)
        {
            case 1: soundName = star1Sound; break;
            case 2: soundName = star2Sound; break;
            case 3: soundName = star3Sound; break;
            case 4: soundName = star4Sound; break;
            case 5: soundName = star5Sound; break;
        }

        if (!string.IsNullOrEmpty(soundName))
        {
            AudioWrapper.Instance.PlaySoundVoid(soundName);
        }
    }
}
