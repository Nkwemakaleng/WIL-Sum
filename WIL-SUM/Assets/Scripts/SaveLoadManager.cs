using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public int totalPoints;
    public int officerCount;
    public float currentDifficulty;
    public List<SerializableCrimeReport> activeReports;
    public List<SerializableOfficer> officers;
}

[System.Serializable]
public class SerializableCrimeReport
{
    public string id;
    public int crimeLevel;
    public float progressBar;
    public List<string> assignedOfficerIds;
}

[System.Serializable]
public class SerializableOfficer
{
    public string id;
    public bool isAvailable;
    public string currentReportId;
}

public class SaveLoadManager : MonoBehaviour
{
    private const string SAVE_KEY = "CrimeGameSave";

    public void SaveGame()
    {
        var saveData = new GameSaveData
        {
            totalPoints = CrimeManagementSystem.Instance.GetTotalPoints(),
            officerCount = CrimeManagementSystem.Instance.GetAvailableOfficers().Count,
            currentDifficulty = DifficultyManager.Instance.GetCurrentDifficulty(),
            activeReports = SerializeReports(),
            officers = SerializeOfficers()
        };

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public bool LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return false;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
        
        // Implement loading logic in respective managers
        DifficultyManager.Instance.SetDifficulty(saveData.currentDifficulty);
        CrimeManagementSystem.Instance.LoadGameState(saveData);
        
        return true;
    }

    private List<SerializableCrimeReport> SerializeReports()
    {
        return CrimeManagementSystem.Instance.GetActiveReports()
            .Select(r => new SerializableCrimeReport
            {
                id = r.id,
                crimeLevel = (int)r.crimeLevel,
                progressBar = r.progressBar,
                assignedOfficerIds = r.assignedOfficers.Select(o => o.id).ToList()
            }).ToList();
    }

    private List<SerializableOfficer> SerializeOfficers()
    {
        return CrimeManagementSystem.Instance.GetAvailableOfficers()
            .Select(o => new SerializableOfficer
            {
                id = o.id,
                isAvailable = o.isAvailable,
                currentReportId = o.currentAssignment?.id
            }).ToList();
    }
}
