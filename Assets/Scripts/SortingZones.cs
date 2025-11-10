using System.Collections.Generic;
using UnityEngine;
using Akkerman.AI;
using Akkerman.Audio;
using Akkerman.InteractionSystem;

public class SortingZones : ZoneEffector
{
    [SerializeField] private Animator animator;
    [SerializeField] private float timeForSorting;
    [SerializeField] private AudioClip alarmSound;
    [SerializeField] private AudioClip alarmMusic;
    [SerializeField] private List<TurretAI> turrets;
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
            // player.Kill();
            player.OnPlayerDied += DeactivateTurrets;
            ActivateTurrets(true);
        }
    }

    public void DeactivateTurrets()
    {
        ActivateTurrets(false);
    }
    public void ActivateTurrets(bool isActive)
    {
        foreach (TurretAI turret in turrets)
        {
            turret.Activate(isActive);
        }
    }
    public void StartSorting()
    {
        if (isSorting) return;
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
