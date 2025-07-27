using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentChapter;
    public SerializableDictionary<string, bool> IsLockedDoors = new SerializableDictionary<string, bool>();
    public Vector3 PlayerPosition;

    //the values in the constructor are default
    //when no data to load
    public GameData()
    {
        this.currentChapter = 0;
    }
}
