using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ChapterCarouselOptions : MonoBehaviour
    {
        public List<GameObject> musicians = new List<GameObject>();
        public List<GameObject> instruments = new List<GameObject>();
        [SerializeField] public Transform StsObjectStash;
        [HideInInspector] public List<Musician> availableMusicians;
        [HideInInspector] public List<Instrument> availableInstruments;

        public void Awake()
        {
            musicians.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
            instruments.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
            availableMusicians = new List<Musician>();
            availableInstruments = new List<Instrument>();

            // Spawn all musicians into the stash.
            for (int i = 0; i < musicians.Count; i++)
            {
                GameObject musicianGameObject = Instantiate(musicians[i], StsObjectStash);

                musicianGameObject.SetActive(false);

                Musician musician = musicianGameObject.GetComponent<Musician>();
                if (!musician)
                {
                    StSDebug.LogError($"No Class<Musician> found on {musicianGameObject.name}");
                }
                else
                {
                    availableMusicians.Add(musician);
                }

                // Replace the prefab with the spawned object.
                musicians[i] = musicianGameObject;
            }

            // Spawn all Instruments into the stash.
            for (int i = 0; i < instruments.Count; i++)
            {
                GameObject instrumentGameObject = Instantiate(instruments[i], StsObjectStash);

                Instrument instrument = instrumentGameObject.GetComponent<Instrument>();
                if (!instrument)
                {
                    StSDebug.LogError($"No Class<Instrument> found on {instrumentGameObject.name}");
                }
                else
                {
                    availableInstruments.Add(instrument);
                }

                // Replace the prefab with the spawned object.
                instruments[i] = instrumentGameObject;
            }
        }
    }
}