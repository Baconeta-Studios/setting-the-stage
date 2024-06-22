using System.Collections.Generic;
using Animation;
using GameStructure;
using UnityEngine;

public enum InstrumentProficiency
{
    Expert = 3,
    Proficient = 2,
    Beginner = 1,
    Poor = 0,
}
public class Musician : StSObject
{
    [SerializeField] private int age;
    [SerializeField] private string gender;
    [SerializeField] private string nationality;
    [SerializeField] private string occupation;
    [SerializeField] private InstrumentSocket instrumentSockets;
    
    [Header("Instruments")]
    [Tooltip("Instruments the musician is best at.\nAny instruments not listed in any of these lists are considered poor.")] 
    [SerializeField] private List<Instrument> expertInstruments;
    
    [Tooltip("Instruments the musician is proficient in.\nAny instruments not listed in any of these lists are considered poor.")]
    [SerializeField] private List<Instrument> proficientInstruments;

    [Tooltip("Instruments the musician is a beginner in.\nAny instruments not listed in any of these lists are considered poor.")] 
    [SerializeField] private List<Instrument> beginnerInstruments;
    
    
    /// <summary>
    /// Get the proficiency with the given instrument
    /// </summary>
    /// <param name="instrument">The instrument to check against.</param>
    /// <returns> Returns Expert, Proficient, Beginner, or Poor based on the musicians proficiency with the given instrument. </returns>
    public InstrumentProficiency GetInstrumentProficiency(Instrument instrument)
    {
        if (expertInstruments.Contains(instrument))
        {
            return InstrumentProficiency.Expert;
        }

        if (proficientInstruments.Contains(instrument))
        {
            return InstrumentProficiency.Proficient;
        }

        if (beginnerInstruments.Contains(instrument))
        {
            return InstrumentProficiency.Beginner;
        }

        return InstrumentProficiency.Poor;
    }

    public int GetAge()
    {
        return age;
    }

    public string GetGender()
    {
        return gender;
    }

    public string GetNationality()
    {
        return nationality;
    }

    public string GetOccupation()
    {
        return occupation;
    }

    public void EquipInstrument(Instrument instrumentData)
    {
        instrumentSockets.HoldInstrument(instrumentData);
    }
}
