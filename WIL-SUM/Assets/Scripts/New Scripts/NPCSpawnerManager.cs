using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCSpawnerManager : MonoBehaviour
{
    [Header("Spawn Settings")] public GameObject[] npcPrefabs; // Different NPC models to spawn
    public Transform npcSpawnPoint; // Location where NPCs initially appear
    public Transform npcDeliveryPoint; // Location where NPCs go after report

    [Header("UI References")] public GameObject officerAssignmentPanel; // Panel for officer selection
    public TMP_Text crimeDescriptionText; // Displays current crime details
    public TMP_Text crimeLevelText; // Shows crime severity
    public TMP_Text officersRequiredText; // Shows recommended officer count
    public TMP_Text timeRemainingText;

    [Header("Officer Selection")] public Slider officerAssignmentSlider; // UI slider to select officer count
    public TMP_Text selectedOfficersText; // Displays currently selected officers
    public Button assignButton; // Button to confirm officer assignment

    [Header("Game Manager Reference")] public GameManager gameManager; // Reference to main game manager

    private CrimeReport currentCrime; // Crime currently being processed
    private GameObject currentNPC; // Currently spawned NPC

    // Track whether NPC has reached desk
    private bool npcAtDesk = false;

    // Track whether current crime is being processed
    private bool crimeProcessing = false;

    // Track active crime resolution coroutine
    private Coroutine activeResolutionCoroutine;

    private float crimeTimeRemaining;
    private Coroutine timeoutCoroutine;

    void Start()
    {
        // Initialize UI and start first NPC spawn
        officerAssignmentPanel.SetActive(false);
        officerAssignmentSlider.onValueChanged.AddListener(UpdateSelectedOfficers);
        assignButton.onClick.AddListener(AssignOfficers);
        SpawnInitialNPC();
    }

    void SpawnInitialNPC()
    {
        // Generate and spawn initial crime report
        CrimeReport initialCrime = gameManager.GenerateCrimeReport();
        SpawnNPCWithCrimeReport(initialCrime);
    }

    void SpawnNPCWithCrimeReport(CrimeReport crime)
    {
        // Reset state flags
        npcAtDesk = false;
        crimeProcessing = false;

        // Remove existing NPC if present
        if (currentNPC != null)
        {
            Destroy(currentNPC);
        }

        // Spawn new NPC at spawn point
        currentNPC = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)],
            npcSpawnPoint.position,
            Quaternion.identity);

        currentCrime = crime;
        // Get NPC controller and initialize movement
        NPCController npcController = currentNPC.GetComponent<NPCController>();
        if (npcController != null)
        {
            // Subscribe to the OnReachedDesk event
            npcController.OnReachedDesk += HandleNPCReachedDesk;
            npcController.MoveToPlayerDesk();
        }

        //PrepareAssignmentUI(crime);
    }

    // New method to handle NPC reaching the desk
    private void HandleNPCReachedDesk()
    {
        npcAtDesk = true;
        PrepareAssignmentUI(currentCrime);
    }


    void PrepareAssignmentUI(CrimeReport crime)
    {

        // Only show UI if NPC has reached desk
        if (!npcAtDesk) return;

        // Configure slider based on available officers and crime requirements
        officerAssignmentSlider.minValue = 1;
        officerAssignmentSlider.maxValue = Mathf.Min(
            gameManager.availableOfficers,
            crime.officersRequired + 2 // Allow some flexibility in officer selection
        );
        officerAssignmentSlider.value = crime.officersRequired;

        // Update UI texts with crime details
        crimeDescriptionText.text = $"Crime: {crime.crimeDescription}";
        crimeLevelText.text = $"Severity: {crime.level}";
        officersRequiredText.text = $"Recommended Officers: {crime.officersRequired}";

        officerAssignmentPanel.SetActive(true);
        StartCrimeTimeout();

    }

    void UpdateSelectedOfficers(float value)
    {
        // Update UI to show currently selected officer count
        int selectedOfficers = Mathf.RoundToInt(value);
        selectedOfficersText.text = $"Selected Officers: {selectedOfficers}";
    }

    void AssignOfficers()
    {
        // Get number of officers selected
        if (!npcAtDesk || crimeProcessing) return;

        int selectedOfficers = Mathf.RoundToInt(officerAssignmentSlider.value);

        // Check if enough officers are available
        if (gameManager.availableOfficers >= selectedOfficers)
        {
            // Mark crime as processing
            crimeProcessing = true;

            if (timeoutCoroutine != null)
            {
                StopCoroutine(timeoutCoroutine);
            }

            // Reduce available officers
            gameManager.availableOfficers -= selectedOfficers;
            gameManager.UpdateUI(); // Update UI to show reduced officers

            // Hide assignment panel
            officerAssignmentPanel.SetActive(false);

            // Start crime resolution coroutine
            if (activeResolutionCoroutine != null)
            {
                StopCoroutine(activeResolutionCoroutine);
            }

            activeResolutionCoroutine = StartCoroutine(
                ResolveCrimeProcess(currentCrime, selectedOfficers)
            );

            // Get NPC controller and trigger departure
            NPCController npcController = currentNPC.GetComponent<NPCController>();
            if (npcController != null)
            {
                // npcController.OnReportAccepted();
                StartCoroutine(HandleNPCDeparture(npcController));
            }


            /* Start crime resolution process
            StartCoroutine(ResolveCrimeAfterDelay(currentCrime, selectedOfficers));

            // Hide assignment panel and animate NPC leaving
            officerAssignmentPanel.SetActive(false);
            AnimateNPCLeave();

            // Spawn next NPC
            Invoke(nameof(SpawnInitialNPC), 3f);*/
        }
        else
        {
            Debug.LogWarning("Not enough officers available!");
        }
    }

    // New coroutine to handle the crime resolution process
    IEnumerator ResolveCrimeProcess(CrimeReport crime, int officersAssigned)
    {
        // Calculate resolution time based on officers assigned vs required
        float efficiencyMultiplier = Mathf.Clamp(
            officersAssigned / (float)crime.officersRequired,
            0.5f,
            2f
        );

        float resolveTime = crime.timeToSolve / efficiencyMultiplier;

        Debug.Log($"Crime resolution started. Will take {resolveTime} seconds.");

        // Wait for the resolution time
        yield return new WaitForSeconds(resolveTime);

        // Solve the crime through game manager
        gameManager.SolveCrime(crime, officersAssigned);

        Debug.Log("Crime resolved!");

        // Reset processing flag
        crimeProcessing = false;
        activeResolutionCoroutine = null;
    }


    // Coroutine to resolve crime after a delay based on officer efficiency
    IEnumerator ResolveCrimeAfterDelay(CrimeReport crime, int officersAssigned)
    {
        // Calculate resolution time based on officers assigned
        float resolveTime = crime.timeToSolve / (officersAssigned / (float)crime.officersRequired);
        yield return new WaitForSeconds(resolveTime);

        // Attempt to solve crime through game manager
        gameManager.SolveCrime(crime, officersAssigned);
    }

    // Updated HandleNPCDeparture to consider crime resolution
    IEnumerator HandleNPCDeparture(NPCController npcController)
    {
        // Wait for departure animation
        yield return new WaitForSeconds(2f);

        // Trigger NPC to move to exit
        npcController.MoveToExit();
        officerAssignmentPanel.SetActive(false);

        // Wait for NPC to reach exit point
        yield return new WaitUntil(() => npcController.HasReachedExit());

        // Clean up current NPC
        if (currentNPC != null)
        {
            NPCController controller = currentNPC.GetComponent<NPCController>();
            if (controller != null)
            {
                controller.OnReachedDesk -= HandleNPCReachedDesk;
            }

            Destroy(currentNPC);
        }

        // Only spawn next NPC after current crime is processed
        //if (!crimeProcessing) {
        SpawnInitialNPC();
        //} else {
        // Wait for crime processing to complete before spawning next NPC
        //  StartCoroutine(WaitForCrimeResolution());
        //}/
    }

    // New coroutine to wait for crime resolution before spawning next NPC
    IEnumerator WaitForCrimeResolution()
    {
        yield return new WaitUntil(() => !crimeProcessing);
        SpawnInitialNPC();
    }

    // Clean up on destroy
    void OnDestroy()
    {

        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
        }

        if (activeResolutionCoroutine != null)
        {
            StopCoroutine(activeResolutionCoroutine);
        }

        if (currentNPC != null)
        {
            NPCController npcController = currentNPC.GetComponent<NPCController>();
            if (npcController != null)
            {
                npcController.OnReachedDesk -= HandleNPCReachedDesk;
            }
        }
    }


    void AnimateNPCLeave()
    {
        // Initiate NPC leaving animation if NPC exists
        if (currentNPC != null)
        {
            StartCoroutine(MoveNPCToDeliveryPoint());
        }
    }

    // Coroutine to smoothly move NPC to delivery point
    IEnumerator MoveNPCToDeliveryPoint()
    {
        float moveDuration = 1f;
        float elapsedTime = 0;

        Vector3 startPosition = currentNPC.transform.position;
        Vector3 endPosition = npcDeliveryPoint.position;

        // Interpolate NPC movement
        while (elapsedTime < moveDuration)
        {
            currentNPC.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy NPC after movement
        Destroy(currentNPC);
    }

    // New method to handle crime timeout
    void StartCrimeTimeout()
    {
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
        }

        timeoutCoroutine = StartCoroutine(CrimeTimeoutCoroutine());
    }

    IEnumerator CrimeTimeoutCoroutine()
    {
        crimeTimeRemaining = gameManager.crimeTimeoutDuration;

        while (crimeTimeRemaining > 0 && !crimeProcessing)
        {
            if (timeRemainingText != null)
            {
                timeRemainingText.text = $"Time Remaining: {Mathf.Ceil(crimeTimeRemaining)}s";
            }

            crimeTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        // If crime wasn't processed, it times out
        if (!crimeProcessing)
        {
            OnCrimeTimeout();
        }
    }

    void OnCrimeTimeout()
    {
        // Notify game manager of failed crime
        gameManager.OnCrimeFailed();

        // Hide UI
        officerAssignmentPanel.SetActive(false);

        // Make NPC leave
        if (currentNPC != null)
        {
            NPCController npcController = currentNPC.GetComponent<NPCController>();
            if (npcController != null)
            {
                //npcController.OnReportRejected();
                StartCoroutine(HandleNPCDeparture(npcController));
            }
        }


    }
}