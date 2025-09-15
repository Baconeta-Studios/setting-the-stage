using System.Collections;
using Audio;
using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{
    public Image starImage; // Image component showing star
    public Sprite emptySprite; // Empty star
    public Sprite halfSprite; // Half-filled star
    public Sprite fullSprite; // Full star
    private string fillStarAudioName;

    private float currentFill = 0f; // 0 = empty, 0.5 = half, 1 = full

    public void ResetStar()
    {
        currentFill = 0f;
        starImage.sprite = emptySprite;
    }
    
    public void ShowStarInstant(float value)
    {
        currentFill = value;
        UpdateSprite(currentFill);
    }

    // Animate the star from 0 → target (0, 0.5, 1)
    public void ShowStarAnimated(float target, float duration, AnimationCurve curve)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateStar(target, duration, curve));
    }

    private IEnumerator AnimateStar(float target, float duration, AnimationCurve curve)
    {
        float start = currentFill;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            float eval = Mathf.Lerp(start, target, curve.Evaluate(t));
            UpdateSprite(eval);
            yield return null;
        }

        currentFill = target;
        UpdateSprite(currentFill);

        if (!string.IsNullOrEmpty(fillStarAudioName))
        {
            AudioWrapper.Instance.PlaySound(fillStarAudioName);
        }
    }

    private void UpdateSprite(float value)
    {
        if (value >= 1f)
            starImage.sprite = fullSprite;
        else if (value >= 0.5f)
            starImage.sprite = halfSprite;
        else
            starImage.sprite = emptySprite;
    }
}