using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using DialogueSystem.Cutscenes;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    private bool isPlaying = false;

    public bool IsPlaying => isPlaying;
    private CutsceneSegment[] cutsceneSegments;
    private DialogueCutsceneEvents[] allEvents;
    private DialogueCutsceneEvents currentEvents;


    private void AddCutsceneEvents()
    {
        if (currentEvents == null)
            return;
        this.cutsceneSegments = currentEvents.Events;
        if (cutsceneSegments.Length == 0)
            Debug.Log($"WARNIGN: Cutscene segments == 0");
        //this.cutsceneSegments = cutsceneEvents;
    }
    public void AddCutsceneEvents(CutsceneSegment[] cutsceneSegments)
    {
        if (cutsceneSegments.Length == 0)
            return;
        this.cutsceneSegments = cutsceneSegments;
    }
    public void AddAllCutsceneEvents(DialogueCutsceneEvents[] allEvents)
    {
        if (allEvents.Length == 0)
            return;
        this.allEvents = allEvents;
    }


    public void OnOpenSegment(int segmentIndex)
    {
        if (cutsceneSegments == null || cutsceneSegments.Length == 0)
            return;
        cutsceneSegments[segmentIndex].OnOpenSegment?.Invoke();
    }

    public void SetCurrentEvents(DialogueObject dialogueObject)
    {
        if (allEvents == null)
            return;
        for (int i = 0; i < allEvents.Length; i++)
            {
                if (allEvents[i].DialogueObject == dialogueObject)
                {
                    currentEvents = allEvents[i];
                    break;
                }
            }
        AddCutsceneEvents();
    }

    public void ActivateGO(GameObject go)
    {
        go.SetActive(true);
    }

    public void DiactivateGO(GameObject go)
    {
        go.SetActive(false);
    }

    public void StartCutscene()
    {
        StartCoroutine(StepThroughCutscene());
    }

    private IEnumerator StepThroughCutscene()
    {
        for (int i = 0; i < cutsceneSegments.Length; i++)
        {
            Debug.Log("DEBUG: CUTSCENE HANDLER");
            CutsceneSegment currentSegment = cutsceneSegments[i];
            cutsceneSegments[i].OnOpenSegment?.Invoke();
            yield return new WaitForSeconds(currentSegment.SegmentTime);
            cutsceneSegments[i].OnCloseSegment?.Invoke();
            yield return new WaitForSeconds(currentSegment.TimeBeforeNextSegment);
        }
    }
}
