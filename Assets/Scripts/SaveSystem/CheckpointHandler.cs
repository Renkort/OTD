using System.Collections.Generic;
using UnityEngine;
using Akkerman.FPS;


namespace Akkerman.SaveSystem
{
    public class CheckpointHandler : MonoBehaviour
    {
        [SerializeField] private List<CheckpointActivator> acivators = new();

        public void SavePlayerData(Player player)
        {

        }
    }
}
