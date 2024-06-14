using System;
using TMPro;
using UnityEngine;

public class StagePosition : MonoBehaviour
{
    public static event Action<StagePosition> OnStagePositionClicked;
    public static event Action<StagePosition> OnStagePositionChanged;
    
    [SerializeField] private Transform viewTarget;
    

    [Header("Stage Parameters")]
    public int stagePositionNumber;
    
    [Header("Musician")]
    public Musician musicianOccupied = null;

    [SerializeField] private SpriteRenderer musicianSprite;
    
    public TextMeshPro musicianSelection;
    
    [Header("Instrument")]
    public Instrument instrumentOccupied = null;
    public TextMeshPro instrumentSelection;

    public void OnInteract()
    {
        if (Chapter.Instance && Chapter.Instance.IsInCurrentStage(Chapter.ChapterStage.StageSelection))
        {
            OnStagePositionClicked?.Invoke(this);
        }
    }

    public void MusicianSelectionChanged(Musician selection)
    {
        musicianOccupied = selection;

        if (musicianOccupied)
        {
            musicianSelection.text = musicianOccupied.GetName();
            musicianSprite.sprite = selection.GetSprite();
        }
        else
        {
            musicianSelection.text = "";
            musicianSprite.sprite = null;
        }
        
        OnStagePositionChanged?.Invoke(this);
    }
    
    public void InstrumentSelectionChanged(Instrument selection)
    {
        instrumentOccupied = selection;
        
        instrumentSelection.text = instrumentOccupied ? instrumentOccupied.GetName() : "";
        OnStagePositionChanged?.Invoke(this);
    }

    public bool IsMusicianOccupied()
    {
        return musicianOccupied is not null;
    }
    
    public bool IsInstrumentOccupied()
    {
        return instrumentOccupied is not null;
    }

    public InstrumentProficiency GetMusicianProficiency()
    {
        return musicianOccupied.GetInstrumentProficiency(instrumentOccupied);
    }
    
    public int GetMusicianProficiencyRaw()
    {
        return (int)musicianOccupied.GetInstrumentProficiency(instrumentOccupied);
    }
    
    public Transform GetViewTarget()
    {
        return viewTarget; 
    }
}
