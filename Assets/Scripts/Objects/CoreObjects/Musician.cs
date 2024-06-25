using System.Collections.Generic;
using System.Linq;
using Animation;
using GameStructure;
using UnityEngine;

public enum InstrumentProficiency
{
    Poor = 0,
    Beginner = 1,
    Proficient = 3,
    Expert = 5,
}
public class Musician : StSObject
{
    [SerializeField] private int age;
    [SerializeField] private string gender;
    [SerializeField] private string nationality;
    [SerializeField] private string occupation;
    [SerializeField] private string bio;
    [SerializeField] private string funFact;
    [SerializeField] private InstrumentSocket instrumentSockets;
    [SerializeField] private string unequipAnimTriggerName = "unequip_all";
    
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
        if (expertInstruments.Any(item => item.GetName() == instrument.GetName())) 
        {
            return InstrumentProficiency.Expert;
        }

        if (proficientInstruments.Any(item => item.GetName() == instrument.GetName()))
        {
            return InstrumentProficiency.Proficient;
        }

        if (beginnerInstruments.Any(item => item.GetName() == instrument.GetName()))
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
    
    public string GetBio()
    {
        return bio;
    }
    
    public string GetFunFact()
    {
        return funFact;
    }

    public void EquipInstrument(Instrument instrumentData)
    {
        UnequipInstrument();
        instrumentSockets.HoldInstrument(instrumentData);
        _animator.ResetTrigger(unequipAnimTriggerName);
    }
    
    public void UnequipInstrument()
    {
        instrumentSockets.RemoveInstrumentFromSocket();
        TriggerAnimation(unequipAnimTriggerName);
    }

    public void TriggerAnimation(string animTriggerName)
    {
        _animator.SetTrigger(animTriggerName);
    }
    
    public void SetAnimationBool(string animBoolName, bool enable)
    {
        _animator.SetBool(animBoolName,enable);
    }
}
