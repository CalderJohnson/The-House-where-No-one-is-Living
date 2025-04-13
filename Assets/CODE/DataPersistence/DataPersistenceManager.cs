using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private const string MajorFileName = "majorPointLevel1.game";

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple DataPersistenceManager instances detected!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>().ToList();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No data found. Initializing new game.");
            NewGame();
        }

        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("No game data found. Creating new game data before saving.");
            NewGame();
        }

        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

  
    // Special Checkpoint Saving
    public void SaveMajorCheckpoint()
    {
        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            dataPersistenceObjects = FindAllDataPersistenceObjects();
        }

        var majorDataHandler = new FileDataHandler(Application.persistentDataPath, MajorFileName, useEncryption);
        var checkpointData = new GameData();

        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref checkpointData);
        }

        majorDataHandler.Save(checkpointData);

        Debug.Log("Major checkpoint saved!");
    }

    public void LoadMajorCheckpoint()
    {
        var majorDataHandler = new FileDataHandler(Application.persistentDataPath, MajorFileName, useEncryption);
        var checkpointData = majorDataHandler.Load();

        if (checkpointData == null)
        {
            Debug.LogWarning("No major checkpoint data found.");
            return;
        }

        gameData = checkpointData;

        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Major checkpoint loaded!");
    }
}
