using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {
    [Header("Game State Management")]
    public int currentLevel = 1;       
    public int totalOfficers = 5;      
    public int availableOfficers = 5;  
    public int currentPoints = 0;      
    public int pointThreshold = 100;   
    public int playerLives = 3;        // Number of attempts player has

    [Header("Level Progression")]
    public int crimesToSolve = 5;      // Crimes needed to advance to next level
    public float totalGameTime = 120f; // Total time to solve all required crimes
    private int crimesSolved = 0;      // Track number of successfully solved crimes

    [Header("UI References")]
    public TMP_Text officerCountText;      
    public TMP_Text pointsText;            
    public TMP_Text levelText;             
    public TMP_Text gameOverText;          
    public TMP_Text livesText;             // UI text to display remaining lives

    [Header("Game Configuration")]
    public float crimeGenerationInterval = 30f;  
    public float maxGameTime = 300f;             

    [Header("Difficulty Scaling")]
    public float difficultyMultiplier = 1f;      

    private float currentGameTime = 0f;
    private List<CrimeReport> activeCrimes = new List<CrimeReport>();

    // Singleton instance to ensure only one GameManager exists
    public static GameManager Instance { get; private set; }

    void Awake() {
        // Implement singleton pattern
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        // Reset game to initial state
        InitializeGame();
    }

    void Update() {
        // Track game progression and time
        UpdateGameProgression();
    }

    void InitializeGame() {
        // Reset all game variables to starting values
        currentLevel = 1;
        totalOfficers = 5;
        availableOfficers = 5;
        currentPoints = 0;
        playerLives = 3;
        crimesSolved = 0;
        currentGameTime = 0f;
        pointThreshold = 100;
        
        UpdateUI();
    }

    void UpdateGameProgression() {
        // Increment game time
        currentGameTime += Time.deltaTime;

        // End game if time limit is reached
        if (currentGameTime >= totalGameTime) {
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
            SolveCrime(crime, assignedOfficers);
            
            // Update UI
            UpdateUI();
            return true;
        }
        
        // Not enough officers
        return false;
    }

    /* Resolve crime based on assigned officers
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
        CheckGameProgression();
    }*/


    // Determine crime solving success based on assigned officers
    public void SolveCrime(CrimeReport crime, int assignedOfficers) {
        // Calculate efficiency of crime resolution
        float efficiencyMultiplier = CalculateSolveEfficiency(crime, assignedOfficers);
        // Wait for crime resolution time, modified by efficiency
       // yield return new WaitForSeconds(crime.timeToSolve / efficiencyMultiplier);
        if (efficiencyMultiplier >= 0.5f) {
            // Successfully solve crime
            int pointsEarned = Mathf.RoundToInt(crime.pointValue * efficiencyMultiplier);
            currentPoints += pointsEarned;
            crimesSolved++;
        } else {
            // Failed to solve crime, lose a life
            playerLives--;
        }

        // Check game progression status
        CheckGameProgression();
        UpdateUI();
    }

    // Calculate crime solving efficiency based on officer assignment
    float CalculateSolveEfficiency(CrimeReport crime, int assignedOfficers) {
        // Determine efficiency by comparing assigned vs required officers
        float requiredRatio = (float)assignedOfficers / crime.officersRequired;
        
        // Efficiency tiers based on officer coverage
        if (requiredRatio >= 1f) return 1f;       // Fully staffed, maximum efficiency
        if (requiredRatio >= 0.75f) return 0.75f; // Mostly staffed, good efficiency
        if (requiredRatio >= 0.5f) return 0.5f;   // Minimum staffing, partial success
        return 0f;  // Not enough officers, crime not solved
    }

    void CheckGameProgression() {
        // Advance to next level if required crimes are solved
        if (crimesSolved >= crimesToSolve) {
            ProgressToNextLevel();
        }

        // Check for game over conditions
        if (playerLives <= 0 || currentGameTime >= totalGameTime) {
            EndGame(false);
        }
    }

    void ProgressToNextLevel() {
        // Increment level
        currentLevel++;
        
        // Increase game difficulty for level 2
        if (currentLevel == 2) {
            totalOfficers += 3;        // Add more available officers
            availableOfficers = totalOfficers;
            pointThreshold += 100;     // Increase point requirement
            crimesToSolve += 2;        // Require more crime solves
            totalGameTime += 60f;      // Extend time limit
        }
        
        // Reset progression tracking
        crimesSolved = 0;
        currentGameTime = 0f;
    }

    void EndGame(bool playerWon) {
        // Stop game time
        Time.timeScale = 0f;
        
        // Display appropriate game over message
        gameOverText.text = playerWon 
            ? "Congratulations! You Won!" 
            : "Game Over - Time Ran Out or Ran Out of Lives";
        
        gameOverText.gameObject.SetActive(true);
    }

    void UpdateUI() {
        // Update all UI text elements with current game state
        if (officerCountText != null)
            officerCountText.text = $"Available Officers: {availableOfficers}";
        
        if (pointsText != null)
            pointsText.text = $"Points: {currentPoints}";
        
        if (levelText != null)
            levelText.text = $"Level: {currentLevel}";
        
        if (livesText != null)
            livesText.text = $"Lives: {playerLives}";
    }
}
