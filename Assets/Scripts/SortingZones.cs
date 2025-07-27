using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingZones : ZoneEffector
{
    [SerializeField] private Animator animator;
    [SerializeField] private float timeForSorting;
    [SerializeField] private AudioClip alarmSound;
    [SerializeField] private AudioClip alarmMusic;
    private float timeToCheckPlayer;
    private bool isPlayerInside;
    private bool isSorting = false;
    private AudioClip defaultSfx;
    private AudioClip defaultMusic;


    void Update()
    {
        if (!isSorting)
            return;
        CheckSortTimer();
    }

    private void CheckSortTimer()
    {
        if (Time.time >= timeToCheckPlayer)
        {
            CheckIfPlayerInZone();
            StopSorting();
        }

    }

    private void CheckIfPlayerInZone()
    {
        foreach (Zone zone in zones)
        {
            if (zone.IsPlayerInside)
            {
                isPlayerInside = true;
                break;
            }
            else
            {
                isPlayerInside = false;
            }
        }

        if (!isPlayerInside)
        {
            player.Kill();
        }
    }
    public void StartSorting()
    {
        timeToCheckPlayer = Time.time + timeForSorting;
        animator.SetBool("Alarm", true);
        isSorting = true;
        defaultSfx = AudioHandler.Instance.sfxClip;
        defaultMusic = AudioHandler.Instance.musicClip;
        AudioHandler.Instance.SetSfx(alarmSound);
        AudioHandler.Instance.SetMusic(alarmMusic);
    }
    public void StopSorting()
    {
        isSorting = false;
        animator.SetBool("Alarm", false);
        AudioHandler.Instance.SetSfx(defaultSfx);
        AudioHandler.Instance.SetMusic(defaultMusic);
    }

}
