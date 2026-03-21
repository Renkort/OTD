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
    [SerializeField] private EnemySpawner turretSpawner;
    private List<Enemy> turrets;
    private float timeToCheckPlayer;
    private bool isSorting = false;
    private AudioClip defaultSfx;
    private AudioClip defaultMusic;

    void Start()
    {
        turrets = turretSpawner.SpawnAll();
        SetActiveTurrets(false);
    }

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
            StopSorting();
        }

    }


    public void DeactivateTurrets()
    {
        SetActiveTurrets(false);
    }
    public void SetActiveTurrets(bool isActive)
    {
        foreach (Enemy turret in turrets)
        {
            turret.enabled = isActive;
        }
    }
    public void StartSorting()
    {
        if (isSorting) return;
        timeToCheckPlayer = Time.time + timeForSorting;
        animator.SetBool("Alarm", true);
        isSorting = true;
        player.OnPlayerDied += DeactivateTurrets;
        SetActiveTurrets(true);

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
