using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using System.Linq;

/// <summary>
/// Manages all UI elements and their updates in the game
/// Handles displaying reports, officers, and game statistics
/// </summary>
public class GameUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform reportsContainer;      // Container for crime report UI elements
    [SerializeField] private Transform officersContainer;     // Container for officer UI elements
    [SerializeField] private GameObject reportPrefab;        // Prefab for crime report UI
    [SerializeField] private GameObject officerPrefab;       // Prefab for officer UI
    [SerializeField] private TextMeshProUGUI pointsText;     // Text display for points
    [SerializeField] private TextMeshProUGUI difficultyText; // Text display for current difficulty

    // Dictionaries to track UI elements
    private Dictionary<string, GameObject> activeReportUI = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> officerUI = new Dictionary<string, GameObject>();

    /// <summary>
    /// Initializes UI and subscribes to game events
    /// </summary>
    private void Start()
    {
        // Subscribe to all relevant events
        CrimeManagementSystem.Instance.OnNewReportGenerated += CreateReportUI;
        CrimeManagementSystem.Instance.OnReportResolved += RemoveReportUI;
        CrimeManagementSystem.Instance.OnOfficerAssigned += UpdateOfficerUI;
        CrimeManagementSystem.Instance.OnPrecinctUpgraded += UpdateOfficerCount;

        InitializeOfficerUI();
        UpdatePoints();
    }

    /// <summary>
    /// Creates UI element for a new crime report
    /// </summary>
    private void CreateReportUI(CrimeReport report)
    {
        GameObject reportUI = Instantiate(reportPrefab, reportsContainer);
        reportUI.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"Level {(int)report.crimeLevel} Crime\nOfficers: {report.officersRequired}";
        
        // Set up and start updating progress bar
        Image progressBar = reportUI.GetComponentInChildren<Image>();
        StartCoroutine(UpdateProgressBar(progressBar, report));

        activeReportUI.Add(report.id, reportUI);
    }

    /// <summary>
    /// Continuously updates the progress bar for an active crime report
    /// </summary>
    private System.Collections.IEnumerator UpdateProgressBar(Image progressBar, CrimeReport report)
    {
        while (!report.isResolved)
        {
            progressBar.fillAmount = report.progressBar;
            yield return null;
        }
    }

    /// <summary>
    /// Removes UI element when a crime report is resolved
    /// </summary>
    private void RemoveReportUI(CrimeReport report)
    {
        if (activeReportUI.TryGetValue(report.id, out GameObject reportUI))
        {
            Destroy(reportUI);
            activeReportUI.Remove(report.id);
            UpdatePoints();
        }
    }
    private void InitializeOfficerUI()
    {
        foreach (var officer in CrimeManagementSystem.Instance.GetAvailableOfficers())
        {
            CreateOfficerUI(officer);
        }
    }

    private void CreateOfficerUI(Officer officer)
    {
        GameObject officerUI = Instantiate(officerPrefab, officersContainer);
        this.officerUI.Add(officer.id, officerUI);
        UpdateSingleOfficerUI(officer, officerUI);
    }

    private void UpdateOfficerUI(Officer officer, CrimeReport report)
    {
        if (officerUI.TryGetValue(officer.id, out GameObject ui))
        {
            UpdateSingleOfficerUI(officer, ui);
        }
    }

    private void UpdateSingleOfficerUI(Officer officer, GameObject ui)
    {
        var statusText = ui.GetComponentInChildren<TextMeshProUGUI>();
        statusText.text = officer.isAvailable ? "Available" : "Assigned";
        ui.GetComponent<Image>().color = officer.isAvailable ? Color.green : Color.red;
    }

    private void UpdateOfficerCount(int totalOfficers)
    {
        foreach (var officer in CrimeManagementSystem.Instance.GetAvailableOfficers())
        {
            if (!officerUI.ContainsKey(officer.id))
            {
                CreateOfficerUI(officer);
            }
        }
    }

    /// <summary>
    /// Updates points display with current total
    /// </summary>
    private void UpdatePoints()
    {
        pointsText.text = $"Points: {CrimeManagementSystem.Instance.GetTotalPoints()}";
    }
}