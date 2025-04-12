using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private string majorFileName = "majorPointLevel1.game";
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        // Load the normal save file (not the major checkpoint) at the start of the game
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // Find all MonoBehaviour objects that implement IDataPersistence
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>(); // Use OfType to filter only IDataPersistence types

        return dataPersistenceObjects.ToList(); // Convert to a list
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load the normal save file
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data found. Initializing new game.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
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

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    // -------------------------------
    // Special Checkpoint Handling
    // -------------------------------

    public void SaveMajorCheckpoint()
    {
        // Save major checkpoint data
        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            dataPersistenceObjects = FindAllDataPersistenceObjects();
        }

        FileDataHandler majorDataHandler = new FileDataHandler(Application.persistentDataPath, majorFileName, useEncryption);

        GameData checkpointData = new GameData();

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref checkpointData);
        }

        majorDataHandler.Save(checkpointData);

        Debug.Log("Major checkpoint saved!");
    }

    public void LoadMajorCheckpoint()
    {
        // Load major checkpoint data only when explicitly requested
        FileDataHandler majorDataHandler = new FileDataHandler(Application.persistentDataPath, majorFileName, useEncryption);

        GameData checkpointData = majorDataHandler.Load();

        if (checkpointData == null)
        {
            Debug.LogWarning("No major checkpoint save found!");
            return;
        }

        // Refresh list of save/load objects for checkpoint loading
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        // Set the checkpoint data as the current gameData
        this.gameData = checkpointData;

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Major checkpoint loaded!");
    }
}
