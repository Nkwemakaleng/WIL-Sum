using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimeManagementSystem : MonoBehaviour
{
    public static CrimeManagementSystem Instance { get; private set; }

    [SerializeField] private int startingOfficerCount = 3;
    [SerializeField] private float reportGenerationInterval = 15f;
    
    private List<CrimeReport> activeReports = new List<CrimeReport>();
    private List<Officer> officers = new List<Officer>();
    private float timer;
    private int totalPoints;
    
    // Events for UI and other systems to subscribe to
    public event Action<CrimeReport> OnNewReportGenerated;
    public event Action<Officer, CrimeReport> OnOfficerAssigned;
    public event Action<CrimeReport> OnReportResolved;
    public event Action<int> OnPrecinctUpgraded;

    // Getter methods for UI and other systems
    public List<CrimeReport> GetActiveReports() => activeReports;
    public List<Officer> GetAvailableOfficers() => officers.FindAll(o => o.isAvailable);
    public int GetTotalPoints() => totalPoints;

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

    private void Start()
    {
        InitializeOfficers();
    }

    private void Update()
    {
        // Generate new reports periodically
        timer += Time.deltaTime;
        if (timer >= reportGenerationInterval)
        {
            GenerateRandomReport();
            timer = 0;
        }

        // Update all active reports
        UpdateReports();
    }

    private void InitializeOfficers()
    {
        for (int i = 0; i < startingOfficerCount; i++)
        {
            officers.Add(new Officer());
        }
    }

    public void GenerateRandomReport()
    {
        // Generate a random crime level based on current game difficulty
        CrimeLevel level = (CrimeLevel)UnityEngine.Random.Range(1, 4);
        CrimeReport newReport = new CrimeReport(level);
        activeReports.Add(newReport);

        // Notify UI or other systems
        OnNewReportGenerated?.Invoke(newReport);
    }

    private void UpdateReports()
    {
        for (int i = activeReports.Count - 1; i >= 0; i--)
        {
            var report = activeReports[i];
            if (!report.isResolved && report.assignedOfficers.Count >= report.officersRequired)
            {
                // Progress the report
                report.progressBar += Time.deltaTime / report.timeToResolve;

                if (report.progressBar >= 1f)
                {
                    ResolveReport(report);
                }
            }
        }
    }

    public bool AssignOfficerToReport(string reportId)
    {
        var report = activeReports.Find(r => r.id == reportId);
        var availableOfficer = officers.Find(o => o.isAvailable);

        if (report != null && availableOfficer != null)
        {
            availableOfficer.isAvailable = false;
            availableOfficer.currentAssignment = report;
            report.assignedOfficers.Add(availableOfficer);
            
            // Notify UI or other systems
            OnOfficerAssigned?.Invoke(availableOfficer, report);
            return true;
        }
        return false;
    }

    private void ResolveReport(CrimeReport report)
    {
        report.isResolved = true;
        totalPoints += report.pointValue;

        // Free up assigned officers
        foreach (var officer in report.assignedOfficers)
        {
            officer.isAvailable = true;
            officer.currentAssignment = null;
        }

        activeReports.Remove(report);
        
        // Notify UI or other systems
        OnReportResolved?.Invoke(report);
    }

    public void UpgradePrecinctOfficers(int additionalOfficers)
    {
        for (int i = 0; i < additionalOfficers; i++)
        {
            officers.Add(new Officer());
        }
        
        // Notify UI or other systems
        OnPrecinctUpgraded?.Invoke(officers.Count);
    }

    
}
