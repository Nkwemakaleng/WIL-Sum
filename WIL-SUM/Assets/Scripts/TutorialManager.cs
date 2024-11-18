using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

/// <summary>
/// Defines each tutorial step with its trigger conditions and content
/// </summary>
[System.Serializable]
public class TutorialStep
{
    public string stepId;                // Unique identifier for the step
    public string title;                 // Step title
    public string description;           // Detailed instructions
    public TutorialTriggerType triggerType;  // What triggers this step
    public string targetObjectTag;       // GameObject tag this step focuses on
    public bool requiresInteraction;     // Whether player needs to perform an action
    public bool completed;               // Whether step is finished
    public string[] highlightObjectTags; // Objects to highlight during this step
}

/// <summary>
/// Types of triggers that can start a tutorial step
/// </summary>
public enum TutorialTriggerType
{
    Immediate,           // Triggers as soon as previous step completes
    ReportGenerated,     // Triggers when a new crime report appears
    OfficerAssigned,     // Triggers when officer is assigned
    ReportResolved,      // Triggers when crime is solved
    PlayerProximity,     // Triggers when player is near tagged object
    TimeDelay           // Triggers after specified time
}

/// <summary>
/// Manages the tutorial flow and UI
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial Configuration")]
    [SerializeField] private TutorialStep[] tutorialSteps;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipTutorialButton;
    [SerializeField] private float highlightPulseSpeed = 1f;
    
    [Header("Tutorial UI Elements")]
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private Material highlightMaterial;

    private int currentStepIndex = -1;
    private Dictionary<string, GameObject> highlightObjects = new Dictionary<string, GameObject>();
    private bool tutorialActive = false;
    private const string TUTORIAL_COMPLETE_KEY = "TutorialCompleted";

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

    /// <summary>
    /// Initialize tutorial if not completed
    /// </summary>
    private void Start()
    {
        if (!PlayerPrefs.HasKey(TUTORIAL_COMPLETE_KEY))
        {
            StartTutorial();
        }

        // Subscribe to game events
        CrimeManagementSystem.Instance.OnNewReportGenerated += OnReportGenerated;
        CrimeManagementSystem.Instance.OnOfficerAssigned += OnOfficerAssigned;
        CrimeManagementSystem.Instance.OnReportResolved += OnReportResolved;

        nextButton.onClick.AddListener(AdvanceToNextStep);
        skipTutorialButton.onClick.AddListener(SkipTutorial);
    }

    /// <summary>
    /// Begins the tutorial sequence
    /// </summary>
    public void StartTutorial()
    {
        tutorialActive = true;
        currentStepIndex = -1;
        AdvanceToNextStep();
    }

    /// <summary>
    /// Advances to the next tutorial step
    /// </summary>
    public void AdvanceToNextStep()
    {
        // Clear previous step highlights
        ClearHighlights();

        currentStepIndex++;
        if (currentStepIndex >= tutorialSteps.Length)
        {
            CompleteTutorial();
            return;
        }

        // Set up new step
        TutorialStep currentStep = tutorialSteps[currentStepIndex];
        DisplayTutorialStep(currentStep);
        SetupStepTriggers(currentStep);
    }

    /// <summary>
    /// Displays the current tutorial step UI
    /// </summary>
    private void DisplayTutorialStep(TutorialStep step)
    {
        tutorialPanel.SetActive(true);
        titleText.text = step.title;
        descriptionText.text = step.description;
        
        // Create highlights for relevant objects
        foreach (string tag in step.highlightObjectTags)
        {
            CreateHighlight(tag);
        }

        // Show/hide next button based on interaction requirement
        nextButton.gameObject.SetActive(!step.requiresInteraction);
    }

    /// <summary>
    /// Creates highlight effect around specified game object
    /// </summary>
    private void CreateHighlight(string targetTag)
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);
        if (target != null)
        {
            GameObject highlight = Instantiate(highlightPrefab, target.transform.position, Quaternion.identity);
            highlight.transform.SetParent(target.transform);
            highlightObjects.Add(targetTag, highlight);

            // Start highlight animation
            StartCoroutine(PulseHighlight(highlight));
        }
    }

    /// <summary>
    /// Animates the highlight effect
    /// </summary>
    private System.Collections.IEnumerator PulseHighlight(GameObject highlight)
    {
        Material instanceMaterial = new Material(highlightMaterial);
        highlight.GetComponent<Renderer>().material = instanceMaterial;
        
        while (highlight != null)
        {
            float pulse = (Mathf.Sin(Time.time * highlightPulseSpeed) + 1) / 2;
            instanceMaterial.SetFloat("_EmissionIntensity", pulse);
            yield return null;
        }
    }

    /// <summary>
    /// Sets up appropriate triggers for the current step
    /// </summary>
    private void SetupStepTriggers(TutorialStep step)
    {
        switch (step.triggerType)
        {
            case TutorialTriggerType.Immediate:
                // No setup needed
                break;
            case TutorialTriggerType.TimeDelay:
                StartCoroutine(TimedTrigger());
                break;
            case TutorialTriggerType.PlayerProximity:
                StartCoroutine(CheckProximity(step.targetObjectTag));
                break;
        }
    }

    /// <summary>
    /// Checks for player proximity to trigger next step
    /// </summary>
    private System.Collections.IEnumerator CheckProximity(string targetTag)
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (target != null && player != null)
        {
            while (Vector3.Distance(player.transform.position, target.transform.position) > 2f)
            {
                yield return new WaitForSeconds(0.2f);
            }
            CompleteCurrentStep();
        }
    }

    /// <summary>
    /// Triggers step completion after time delay
    /// </summary>
    private System.Collections.IEnumerator TimedTrigger()
    {
        yield return new WaitForSeconds(3f);
        CompleteCurrentStep();
    }

    /// <summary>
    /// Event handlers for game events
    /// </summary>
    private void OnReportGenerated(CrimeReport report)
    {
        if (!tutorialActive) return;
        
        if (currentStepIndex >= 0 && 
            tutorialSteps[currentStepIndex].triggerType == TutorialTriggerType.ReportGenerated)
        {
            CompleteCurrentStep();
        }
    }

    private void OnOfficerAssigned(Officer officer, CrimeReport report)
    {
        if (!tutorialActive) return;
        
        if (currentStepIndex >= 0 && 
            tutorialSteps[currentStepIndex].triggerType == TutorialTriggerType.OfficerAssigned)
        {
            CompleteCurrentStep();
        }
    }

    private void OnReportResolved(CrimeReport report)
    {
        if (!tutorialActive) return;
        
        if (currentStepIndex >= 0 && 
            tutorialSteps[currentStepIndex].triggerType == TutorialTriggerType.ReportResolved)
        {
            CompleteCurrentStep();
        }
    }

    /// <summary>
    /// Completes the current tutorial step
    /// </summary>
    private void CompleteCurrentStep()
    {
        if (currentStepIndex >= 0 && currentStepIndex < tutorialSteps.Length)
        {
            tutorialSteps[currentStepIndex].completed = true;
            AdvanceToNextStep();
        }
    }

    /// <summary>
    /// Removes all highlight effects
    /// </summary>
    private void ClearHighlights()
    {
        foreach (var highlight in highlightObjects.Values)
        {
            Destroy(highlight);
        }
        highlightObjects.Clear();
    }

    /// <summary>
    /// Completes the tutorial
    /// </summary>
    private void CompleteTutorial()
    {
        tutorialActive = false;
        tutorialPanel.SetActive(false);
        ClearHighlights();
        PlayerPrefs.SetInt(TUTORIAL_COMPLETE_KEY, 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Skips the tutorial entirely
    /// </summary>
    public void SkipTutorial()
    {
        CompleteTutorial();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (CrimeManagementSystem.Instance != null)
        {
            CrimeManagementSystem.Instance.OnNewReportGenerated -= OnReportGenerated;
            CrimeManagementSystem.Instance.OnOfficerAssigned -= OnOfficerAssigned;
            CrimeManagementSystem.Instance.OnReportResolved -= OnReportResolved;
        }
    }
}
