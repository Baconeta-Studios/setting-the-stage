using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ArrowHint : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2f; // Speed of pulse
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    [Header("Colour Pulse")]
    [SerializeField] private bool pulseColour = false;
    [SerializeField] private Color colourA = Color.white; // Base colour
    [SerializeField] private Color colourB = Color.yellow; // Target pulse colour
    
    [Header("Scale Pulse")]
    [SerializeField] private bool pulseScale = false; 
    private Vector3 _scaleOriginal; // Base colour
    [SerializeField] private Vector3 scaleMax = Vector3.one; // Target local scale in all 3 spaces

    [Header("Sync Offset")]
    [SerializeField] private float phaseOffset = 0f; // Offset in seconds

    [Header("Tutorial Tracking")]
    [SerializeField] private string tutorialKey = "doneTutorial";

    private Image _arrowImage;
    private bool _pulsing = false;
    private float _timer = 0f;

    private void Awake()
    {
        _arrowImage = GetComponent<Image>();
        if (_arrowImage == null)
        {
            StSDebug.LogError($"ArrowHint cannot find an Image component on gameObject {name}");
        }
        
        _scaleOriginal =  _arrowImage.transform.localScale;
    }

    private void Start()
    {
        // Only run if tutorial hasn't been done yet
        if (!SaveSystem.Instance.IsThingDone(tutorialKey))
        {
            StartPulsing();
        }
        else
        {
            SetVisual(maxAlpha, colourA, _scaleOriginal);
        }
    }

    private void Update()
    {
        if (!_pulsing) return;

        // advance timer, include phase offset
        _timer += Time.deltaTime * pulseSpeed;
        var wave = Mathf.Sin(_timer + phaseOffset);

        // normalised 0–1
        var t = (wave + 1f) / 2f;

        // Alpha pulse
        var alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        
        // Scale pulse
        var scaleX = pulseScale ? Mathf.Lerp(_scaleOriginal.x, scaleMax.x, t) : _scaleOriginal.x;
        var scaleY = pulseScale ? Mathf.Lerp(_scaleOriginal.y, scaleMax.y, t) : _scaleOriginal.y;
        var scaleZ = pulseScale ? Mathf.Lerp(_scaleOriginal.z, scaleMax.z, t) : _scaleOriginal.z;
        var scale = new Vector3(scaleX, scaleY, scaleZ); 

        // Colour pulse (optional)
        Color col = pulseColour ? Color.Lerp(colourA, colourB, t) : colourA;

        SetVisual(alpha, col, scale);
    }

    private void StartPulsing()
    {
        _pulsing = true;
        _timer = 0f;
    }

    public void StopPulsing()
    {
        _pulsing = false;
        SetVisual(maxAlpha, colourA, _scaleOriginal);
        SaveSystem.Instance.MarkAsDone(tutorialKey);
    }

    private void SetVisual(float alpha, Color baseColour, Vector3 scale)
    {
        Color c = baseColour;
        c.a = alpha;
        _arrowImage.color = c;
        _arrowImage.transform.localScale = scale;
    }
}