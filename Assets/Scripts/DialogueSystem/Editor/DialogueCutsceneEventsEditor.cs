using UnityEditor;
using UnityEngine;
using DialogueSystem.Cutscenes;

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
