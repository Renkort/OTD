using UnityEditor;
using UnityEngine;
using Akkerman.DialogueSystem.Cutscenes;

namespace Akkerman.CutsceneSystem
{
    [CustomEditor(typeof(DialogueCutsceneEvents))]
    public class DialogueCutsceneEventsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DialogueCutsceneEvents cutsceneEvents = (DialogueCutsceneEvents)target;

            if (GUILayout.Button("Refresh"))
            {
                cutsceneEvents.OnValidate();
            }
        }
    }
}
