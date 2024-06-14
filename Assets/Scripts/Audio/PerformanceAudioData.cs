using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class PerformanceAudioData : MonoBehaviour
    {
        [SerializeField] private List<ActAudioSo> actAudioData;
    }
}