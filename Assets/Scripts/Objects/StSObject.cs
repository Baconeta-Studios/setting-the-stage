using System;
using UnityEngine;

namespace GameStructure
{
    /// <summary>
    /// The base class for all Setting the Stage interactable objects, such as musicians and instruments.
    /// </summary>
    public class StSObject : MonoBehaviour, IComparable<StSObject>, IEquatable<StSObject>
    {
        [SerializeField] private string Name;
        [SerializeField] private Animator _animator; 

        public int CompareTo(StSObject comparison)
        {
            return string.Compare(Name, comparison.GetName(), StringComparison.Ordinal);
        }

        public bool Equals(StSObject comparison)
        {
            if (!comparison)
            {
                return false;
            }
        
            return Name == comparison.GetName();
        }
    
        public string GetName()
        {
            return Name;
        }
    }
}
