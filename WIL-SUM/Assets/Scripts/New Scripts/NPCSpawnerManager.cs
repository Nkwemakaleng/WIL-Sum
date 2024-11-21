using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCSpawnerManager : MonoBehaviour {
    [Header("Spawn Settings")]
    public GameObject[] npcPrefabs;
    public Transform npcSpawnPoint;
    public Transform npcDeliveryPoint;

    [Header("UI References")]
    public GameObject officerAssignmentPanel;
    public TMP_Text crimeDescriptionText;
    public TMP_Text crimeLevelText;
    public TMP_Text officersRequiredText;

    [Header("Officer Selection")]
    public Slider officerAssignmentSlider;
    public TMP_Text selectedOfficersText;
    public Button assignButton;

    [Header("Game Manager Reference")]
    public GameManager gameManager;

    private CrimeReport currentCrime;
    private GameObject currentNPC;

    void Start() {
        // Ensure assignment panel starts hidden
        officerAssignmentPanel.SetActive(false);
        
        // Set up slider listener
        officerAssignmentSlider.onValueChanged.AddListener(UpdateSelectedOfficers);
        assignButton.onClick.AddListener(AssignOfficers);

        // Spawn initial NPC
        SpawnInitialNPC();
    }

    void SpawnInitialNPC() {
        // Create initial crime report
        CrimeReport initialCrime = GenerateInitialCrimeReport();
        SpawnNPCWithCrimeReport(initialCrime);
    }

    CrimeReport GenerateInitialCrimeReport() {
        return new CrimeReport {
            level = CrimeLevel.Low,
            crimeDescription = "Noise Complaint in Residential Area",
            officersRequired = 2,
            timeToSolve = 60f,
            pointValue = 25
        };
    }

    void SpawnNPCWithCrimeReport(CrimeReport crime) {
        // Destroy any existing NPC
        if (currentNPC != null) {
            Destroy(currentNPC);
        }

        // Spawn new NPC
        currentNPC = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)], 
            npcSpawnPoint.position, 
            Quaternion.identity);

        // Store current crime
        currentCrime = crime;

        // Prepare UI for crime report
        PrepareAssignmentUI(crime);
    }

    void PrepareAssignmentUI(CrimeReport crime) {
        // Configure slider based on available officers and crime requirements
        officerAssignmentSlider.minValue = 1;
        officerAssignmentSlider.maxValue = Mathf.Min(
            gameManager.availableOfficers, 
            crime.officersRequired + 2  // Allow some flexibility
        );
        officerAssignmentSlider.value = crime.officersRequired;

        // Update UI texts
        crimeDescriptionText.text = $"Crime: {crime.crimeDescription}";
        crimeLevelText.text = $"Severity: {crime.level}";
        officersRequiredText.text = $"Recommended Officers: {crime.officersRequired}";

        // Show assignment panel
        officerAssignmentPanel.SetActive(true);
    }

    void UpdateSelectedOfficers(float value) {
        // Round slider value to integer
        int selectedOfficers = Mathf.RoundToInt(value);
        selectedOfficersText.text = $"Selected Officers: {selectedOfficers}";
    }

    void AssignOfficers() {
        int selectedOfficers = Mathf.RoundToInt(officerAssignmentSlider.value);

        // Attempt to assign officers through game manager
        if (gameManager.AssignOfficersToCrime(currentCrime, selectedOfficers)) {
            // Hide assignment panel
            officerAssignmentPanel.SetActive(false);

            // Animate NPC leaving
            AnimateNPCLeave();

            // Optionally spawn next NPC after a delay
            Invoke(nameof(SpawnInitialNPC), 3f);
        }
        else {
            // Show error - not enough officers
            Debug.LogWarning("Not enough officers available!");
        }
    }

    void AnimateNPCLeave() {
        if (currentNPC != null) {
            // Simple movement to delivery point
            StartCoroutine(MoveNPCToDeliveryPoint());
        }
    }

    IEnumerator MoveNPCToDeliveryPoint() {
        float moveDuration = 1f;
        float elapsedTime = 0;

        Vector3 startPosition = currentNPC.transform.position;
        Vector3 endPosition = npcDeliveryPoint.position;

        while (elapsedTime < moveDuration) {
            currentNPC.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy NPC after movement
        Destroy(currentNPC);
    }
}
