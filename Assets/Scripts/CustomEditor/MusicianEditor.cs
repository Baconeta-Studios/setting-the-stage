using UnityEngine;

namespace CustomEditor
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(Musician))]
    public class MusicianEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Musician musician = (Musician)target;

            if (GUILayout.Button("Spawn Instrument (Editor)"))
            {
                musician.EquipEditorInstrument();
            }
        }
    }
#endif

}