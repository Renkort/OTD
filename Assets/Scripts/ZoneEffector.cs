using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Akkerman.FPS;

namespace Akkerman.InteractionSystem
{
    
    public class ZoneEffector : MonoBehaviour
    {
        [SerializeField] protected Player player;
        [SerializeField] protected List<Zone> zones;

        public UnityEvent OnPlayerEnter, OnPlayerExit;

        void Awake()
        {
            // Initialize();
        }

        private void Initialize()
        {
            foreach (Zone zone in zones)
            {
                zone.OnPlayerEnter = OnPlayerEnter;
                zone.OnPlayerExit = OnPlayerExit;
            }
        }
    }
}
