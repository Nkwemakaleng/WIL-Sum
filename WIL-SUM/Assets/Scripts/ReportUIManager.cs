using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportUIManager : MonoBehaviour
{
    public GameObject reportPanel; // Panel to display the report
    public TextMeshProUGUI descriptionText; // UI element to display the crime description
    public TextMeshProUGUI neededCopsText; // UI element to display the number of cops needed
    public TextMeshProUGUI threatLevelText; // UI element to display the threat level

    public void DisplayReport(Report report)
    {
        // Activate the panel and update the report details
        reportPanel.SetActive(true);
        descriptionText.text = "Crime: " + report.description;
        neededCopsText.text = "Needed Cops: " + report.neededCops;
        threatLevelText.text = "Threat Level: " + report.threatLevel;

        // Optionally, you can style the text based on threat level (e.g., color coding)
        Color threatColor = report.threatLevel switch
        {
            1 => Color.green,
            2 => Color.yellow,
            3 => Color.red,
            _ => Color.white,
        };
        threatLevelText.color = threatColor;
    }

    public void CloseReport()
    {
        // Hide the panel
        reportPanel.SetActive(false);
    }
}
