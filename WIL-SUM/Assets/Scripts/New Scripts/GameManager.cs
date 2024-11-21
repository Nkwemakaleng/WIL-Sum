using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



// Manages the overall game state and progression
public class GameManager : MonoBehaviour {
    [Header("Game State Management")]
    public int currentLevel = 1;       // Current game level
    public int totalOfficers = 5;      // Total officers available
    public int availableOfficers = 5;  // Currently available officers
    public int currentPoints = 0;      // Current points accumulated
    public int pointThreshold = 100;   // Points needed to progress to next level

    [Header("UI References")]
    public TMP_Text officerCountText;      // UI text showing available officers
    public TMP_Text pointsText;            // UI text showing current points
    public TMP_Text levelText;             // UI text showing current level
    public TMP_Text gameOverText;          // UI text for game over conditions

    [Header("Game Configuration")]
    public float crimeGenerationInterval = 30f;  // Time between crime report generations
    public float maxGameTime = 300f;             // Maximum game time in seconds

    [Header("Difficulty Scaling")]
    public float difficultyMultiplier = 1f;      // Increases game difficulty over time

    // Internal tracking for game progression
    private float currentGameTime = 0f;
    private List<CrimeReport> activeCrimes = new List<CrimeReport>();

    // Singleton pattern to ensure only one GameManager exists
    public static GameManager Instance { get; private set; }

    void Awake() {
        // Singleton setup
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        // Initialize the game state
        InitializeGame();
    }

    void Update() {
        // Track game time and update difficulty
        UpdateGameProgression();
    }

    // Initialize game settings and starting conditions
    void InitializeGame() {
        // Reset all game variables
        currentLevel = 1;
        totalOfficers = 5;
        availableOfficers = 5;
        currentPoints = 0;
        pointThreshold = 100;
        currentGameTime = 0f;

        // Update UI to reflect initial state
        UpdateUI();
    }

    // Manage game progression and time-based difficulty
    void UpdateGameProgression() {
        // Track elapsed game time
        currentGameTime += Time.deltaTime;

        // Increase difficulty over time
        difficultyMultiplier = 1f + (currentGameTime / maxGameTime);

        // Check for game over conditions
        if (currentGameTime >= maxGameTime) {
            EndGame(false);
        }
    }

    // Generate a random crime report
    public CrimeReport GenerateCrimeReport() {
        // Determine crime level based on current difficulty
        CrimeLevel crimeLevel = DetermineCrimeLevel();

        return new CrimeReport {
            level = crimeLevel,
            crimeDescription = GenerateCrimeDescription(crimeLevel),
            officersRequired = CalculateOfficersRequired(crimeLevel),
            timeToSolve = CalculateTimeToSolve(crimeLevel),
            pointValue = CalculatePointValue(crimeLevel)
        };
    }

    // Determine crime level based on current game difficulty
    CrimeLevel DetermineCrimeLevel() {
        // Use difficulty multiplier to increase chance of higher-level crimes
        float randomValue = Random.value * difficultyMultiplier;

        if (randomValue > 0.8f) return CrimeLevel.High;
        if (randomValue > 0.4f) return CrimeLevel.Moderate;
        return CrimeLevel.Low;
    }

    // Generate a descriptive crime name
    string GenerateCrimeDescription(CrimeLevel level) {
        string[] lowCrimes = {
            "Noise Complaint", 
            "Littering", 
            "Minor Disturbance"
        };
        string[] moderateCrimes = {
            "Theft", 
            "Vandalism", 
            "Trespassing"
        };
        string[] highCrimes = {
            "Armed Robbery", 
            "Hostage Situation", 
            "Major Assault"
        };

        switch (level) {
            case CrimeLevel.Low:
                return lowCrimes[Random.Range(0, lowCrimes.Length)];
            case CrimeLevel.Moderate:
                return moderateCrimes[Random.Range(0, moderateCrimes.Length)];
            case CrimeLevel.High:
                return highCrimes[Random.Range(0, highCrimes.Length)];
            default:
                return "Unknown Crime";
        }
    }

    // Calculate officers needed based on crime level
    int CalculateOfficersRequired(CrimeLevel level) {
        // Base officers required increases with crime level
        return (int)level * 2;
    }

    // Calculate time to solve based on crime level
    float CalculateTimeToSolve(CrimeLevel level) {
        // Higher level crimes take longer to resolve
        return 60f * (float)level * difficultyMultiplier;
    }

    // Calculate point value for solving a crime
    int CalculatePointValue(CrimeLevel level) {
        // Point value increases with crime level and difficulty
        return (int)(50 * (float)level * difficultyMultiplier);
    }

    // Attempt to assign officers to a crime
    public bool AssignOfficersToCrime(CrimeReport crime, int assignedOfficers) {
        // Check if enough officers are available
        if (availableOfficers >= assignedOfficers) {
            // Reduce available officers
            availableOfficers -= assignedOfficers;
            
            // Start crime resolution coroutine
            StartCoroutine(ResolveCrime(crime, assignedOfficers));
            
            // Update UI
            UpdateUI();
            return true;
        }
        
        // Not enough officers
        return false;
    }

    // Resolve crime based on assigned officers
    IEnumerator ResolveCrime(CrimeReport crime, int assignedOfficers) {
        // Calculate efficiency based on assigned vs required officers
        float efficiencyMultiplier = Mathf.Clamp(
            assignedOfficers / (float)crime.officersRequired, 
            0.5f, 
            2f
        );

        // Wait for crime resolution time, modified by efficiency
        yield return new WaitForSeconds(crime.timeToSolve / efficiencyMultiplier);
        
        // Calculate points earned
        int pointsEarned = Mathf.RoundToInt(crime.pointValue * efficiencyMultiplier);
        currentPoints += pointsEarned;
        
        // Return excess officers to pool
        availableOfficers += assignedOfficers;
        
        // Check for level progression
        CheckLevelProgression();
    }

    // Check if player has reached level progression threshold
    void CheckLevelProgression() {
        if (currentPoints >= pointThreshold) {
            ProgressToNextLevel();
        }
    }

    // Progress to next level, increasing difficulty
    void ProgressToNextLevel() {
        currentLevel++;
        
        // Increase point threshold
        pointThreshold += 50 * currentLevel;
        
        // Increase total and available officers
        totalOfficers += 2;
        availableOfficers = totalOfficers;
        
        // Reduce crime generation interval (make game more challenging)
        crimeGenerationInterval = Mathf.Max(15f, crimeGenerationInterval - 2f);
    }

    // End the game
    void EndGame(bool playerWon) {
        // Stop all game activities
        Time.timeScale = 0f;
        
        // Display game over text
        if (playerWon) {
            gameOverText.text = "Congratulations! You Won!";
        } else {
            gameOverText.text = "Game Over - Time Ran Out";
        }
        
        // Show game over UI
        gameOverText.gameObject.SetActive(true);
    }

    // Update all UI elements
    void UpdateUI() {
        // Null checks to prevent errors
        if (officerCountText != null)
            officerCountText.text = $"Available Officers: {availableOfficers}";
        
        if (pointsText != null)
            pointsText.text = $"Points: {currentPoints}/{pointThreshold}";
        
        if (levelText != null)
            levelText.text = $"Level: {currentLevel}";
    }
}