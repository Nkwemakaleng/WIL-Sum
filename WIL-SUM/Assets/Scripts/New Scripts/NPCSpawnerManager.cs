using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCSpawnerManager : MonoBehaviour {
    [Header("Spawn Settings")]
    public GameObject[] npcPrefabs;        // Different NPC models to spawn
    public Transform npcSpawnPoint;        // Location where NPCs initially appear
    public Transform npcDeliveryPoint;     // Location where NPCs go after report

    [Header("UI References")]
    public GameObject officerAssignmentPanel;  // Panel for officer selection
    public TMP_Text crimeDescriptionText;      // Displays current crime details
    public TMP_Text crimeLevelText;            // Shows crime severity
    public TMP_Text officersRequiredText;      // Shows recommended officer count

    [Header("Officer Selection")]
    public Slider officerAssignmentSlider;  // UI slider to select officer count
    public TMP_Text selectedOfficersText;   // Displays currently selected officers
    public Button assignButton;             // Button to confirm officer assignment

    [Header("Game Manager Reference")]
    public GameManager gameManager;         // Reference to main game manager

    private CrimeReport currentCrime;       // Crime currently being processed
    private GameObject currentNPC;          // Currently spawned NPC

    void Start() {
        // Initialize UI and start first NPC spawn
        officerAssignmentPanel.SetActive(false);
        officerAssignmentSlider.onValueChanged.AddListener(UpdateSelectedOfficers);
        assignButton.onClick.AddListener(AssignOfficers);
        SpawnInitialNPC();
    }

    void SpawnInitialNPC() {
        // Generate and spawn initial crime report
        CrimeReport initialCrime = gameManager.GenerateCrimeReport();
        SpawnNPCWithCrimeReport(initialCrime);
    }

    void SpawnNPCWithCrimeReport(CrimeReport crime) {
        // Remove existing NPC if present
        if (currentNPC != null) {
            Destroy(currentNPC);
        }

        // Spawn new NPC at spawn point
        currentNPC = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)], 
            npcSpawnPoint.position, 
            Quaternion.identity);

        currentCrime = crime;
        PrepareAssignmentUI(crime);
    }

    void PrepareAssignmentUI(CrimeReport crime) {
        // Configure slider based on available officers and crime requirements
        officerAssignmentSlider.minValue = 1;
        officerAssignmentSlider.maxValue = Mathf.Min(
            gameManager.availableOfficers, 
            crime.officersRequired + 2  // Allow some flexibility in officer selection
        );
        officerAssignmentSlider.value = crime.officersRequired;

        // Update UI texts with crime details
        crimeDescriptionText.text = $"Crime: {crime.crimeDescription}";
        crimeLevelText.text = $"Severity: {crime.level}";
        officersRequiredText.text = $"Recommended Officers: {crime.officersRequired}";

        officerAssignmentPanel.SetActive(true);
    }

    void UpdateSelectedOfficers(float value) {
        // Update UI to show currently selected officer count
        int selectedOfficers = Mathf.RoundToInt(value);
        selectedOfficersText.text = $"Selected Officers: {selectedOfficers}";
    }

    void AssignOfficers() {
        // Get number of officers selected
        int selectedOfficers = Mathf.RoundToInt(officerAssignmentSlider.value);

        // Check if enough officers are available
        if (gameManager.availableOfficers >= selectedOfficers) {
            // Reduce available officers
            gameManager.availableOfficers -= selectedOfficers;
            
            // Start crime resolution process
            StartCoroutine(ResolveCrimeAfterDelay(currentCrime, selectedOfficers));
            
            // Hide assignment panel and animate NPC leaving
            officerAssignmentPanel.SetActive(false);
            AnimateNPCLeave();

            // Spawn next NPC
            Invoke(nameof(SpawnInitialNPC), 3f);
        }
        else {
            Debug.LogWarning("Not enough officers available!");
        }
    }

    // Coroutine to resolve crime after a delay based on officer efficiency
    IEnumerator ResolveCrimeAfterDelay(CrimeReport crime, int officersAssigned) {
        // Calculate resolution time based on officers assigned
        float resolveTime = crime.timeToSolve / (officersAssigned / (float)crime.officersRequired);
        yield return new WaitForSeconds(resolveTime);

        // Attempt to solve crime through game manager
        gameManager.SolveCrime(crime, officersAssigned);
    }

    void AnimateNPCLeave() {
        // Initiate NPC leaving animation if NPC exists
        if (currentNPC != null) {
            StartCoroutine(MoveNPCToDeliveryPoint());
        }
    }

    // Coroutine to smoothly move NPC to delivery point
    IEnumerator MoveNPCToDeliveryPoint() {
        float moveDuration = 1f;
        float elapsedTime = 0;

        Vector3 startPosition = currentNPC.transform.position;
        Vector3 endPosition = npcDeliveryPoint.position;

        // Interpolate NPC movement
        while (elapsedTime < moveDuration) {
            currentNPC.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy NPC after movement
        Destroy(currentNPC);
    }
}