using UnityEngine;


namespace Akkerman.SaveSystem
{
    
    [System.Serializable]
    public class GameData
    {
        public int currentChapter;
        public string LastOpenedScene;
        public SerializableDictionary<string, bool> IsLockedDoors = new SerializableDictionary<string, bool>();
        // Dialogues( PersonID, DialogueID)
        public SerializableDictionary<string, string> Dialogues = new();

        // [PLAYER DATA]
        public Vector3 PlayerPosition;
        public Vector3 PlayerRotation;
        public Vector3 CameraRotation;
        public bool IsFlashlightActive;
        public bool CanMove;
        public bool CanLookAround;
        public bool UsePhysics;
        public bool IsDead;

        // [ UI ]
        public bool ShowDebugInfo;

        // [ DIALOGUES ]

        // [ ZONES ]
        public SerializableDictionary<string, bool> ActiveZones = new SerializableDictionary<string, bool>();

        //the values in the constructor are default
        //when no data to load
        public GameData()
        {
            this.currentChapter = 0;
            IsFlashlightActive = false;
            ShowDebugInfo = false;
            IsDead = false;
            CanMove = true;
            CanLookAround = true;
            UsePhysics = true;
            LastOpenedScene = "OTD_Intro";
        }
    }
}
