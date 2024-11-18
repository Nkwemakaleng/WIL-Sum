using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject to define tutorial sequences
/// </summary>
[CreateAssetMenu(fileName = "TutorialSequence", menuName = "Tutorial/New Tutorial Sequence")]
public class TutorialSequence : ScriptableObject
{
    public TutorialStep[] steps;

    /// <summary>
    /// Example tutorial sequence setup
    /// </summary>
    public void CreateBasicTutorial()
    {
        steps = new TutorialStep[]
        {
            new TutorialStep
            {
                stepId = "welcome",
                title = "Welcome to Police Dispatch!",
                description = "Learn how to manage your police station and solve crimes.",
                triggerType = TutorialTriggerType.Immediate,
                requiresInteraction = false
            },
            new TutorialStep
            {
                stepId = "first_report",
                title = "Your First Crime Report",
                description = "Wait for a crime report to come in. Reports appear in the left panel.",
                triggerType = TutorialTriggerType.ReportGenerated,
                requiresInteraction = true,
                highlightObjectTags = new string[] { "ReportPanel" }
            },
            new TutorialStep
            {
                stepId = "assign_officer",
                title = "Assign an Officer",
                description = "Click on an available officer to assign them to the case.",
                triggerType = TutorialTriggerType.OfficerAssigned,
                requiresInteraction = true,
                highlightObjectTags = new string[] { "OfficerPanel" }
            },
            new TutorialStep
            {
                stepId = "complete_case",
                title = "Complete the Case",
                description = "Wait for your officer to resolve the crime.",
                triggerType = TutorialTriggerType.ReportResolved,
                requiresInteraction = true
            },
            new TutorialStep
            {
                stepId = "tutorial_complete",
                title = "Tutorial Complete!",
                description = "You're ready to protect and serve! Good luck!",
                triggerType = TutorialTriggerType.Immediate,
                requiresInteraction = false
            }
        };
    }
}
