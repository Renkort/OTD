using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Akkerman.SaveSystem
{
    
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName;
        [SerializeField] private bool useEncryption;

        [Header("Debugging")]
        [SerializeField] private bool initializeDataIfNull = false;

        public static DataPersistenceManager Instance { get; private set; }
        private GameData gameData;
        private List<IDataPersistance> dataPersistanceObjects;
        private FileDataHandler dataHandler;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.Log($"Found more than one DataPersistanceManager. Destroying newest one.");
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[DEBUG] OnSceneLoaded. Loading Game...");
            this.dataPersistanceObjects = FindAllDataPersistanceObjects();
            // LoadGame();
            SaveGame();
        }

        public void OnSceneUnloaded(Scene scene)
        {
            // Debug.Log($"[DEBUG] OnSceneUnloaded. Saving Game...");
            // SaveGame();
        }


        public void NewGame()
        {
            this.gameData = new GameData();
        }

        public void LoadGame()
        {
            this.gameData = dataHandler.Load();

            if (this.gameData == null && initializeDataIfNull)
            {
                NewGame();
            }

            //if no data can be loaded, don't continue 
            if (this.gameData == null)
            {
                Debug.Log($"No game data found. A New Game needs to be started before data can be loaded");
                return;
            }

            foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
            {
                dataPersistanceObj.LoadData(gameData);
            }
        }

        public void SaveGame()
        {
            if (this.gameData == null && initializeDataIfNull)
            {
                NewGame();
            }

            if (this.gameData == null)
            {
                Debug.LogWarning($"No data was found. A New Game needs to be started before data can be loaded");
                return;
            }

            foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
            {
                dataPersistanceObj.SaveData(ref gameData);
            }

            dataHandler.Save(gameData);
        }

        private void OnApplicationQuit()
        {
            //SaveGame();
        }

        private List<IDataPersistance> FindAllDataPersistanceObjects()
        {
            IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistance>();

            return new List<IDataPersistance>(dataPersistanceObjects);
        }


        public bool HasGameData()
        {
            return gameData != null;
        }
    }
}
