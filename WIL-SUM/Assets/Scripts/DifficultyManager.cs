using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] private float baseCrimeInterval = 15f;
    [SerializeField] private float difficultyScalingFactor = 0.1f;
    [SerializeField] private int pointsPerDifficultyIncrease = 100;

    private float currentDifficulty = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateDifficulty(int totalPoints)
    {
        currentDifficulty = 1f + (totalPoints / pointsPerDifficultyIncrease) * difficultyScalingFactor;
        UpdateCrimeGeneration();
    }

    private void UpdateCrimeGeneration()
    {
        float newInterval = baseCrimeInterval / currentDifficulty;
        CrimeManagementSystem.Instance.SetReportInterval(newInterval);
    }

    public float GetCurrentDifficulty() => currentDifficulty;
    public void SetDifficulty(float difficulty) => currentDifficulty = difficulty;
}
