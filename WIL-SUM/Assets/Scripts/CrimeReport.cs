using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Enum for crime severity levels
public enum CrimeLevel
{
    Low = 1,
    Moderate = 2,
    High = 3
}

// Class to represent a crime report
[System.Serializable]
public class CrimeReport
{
    public string id;
    public CrimeLevel crimeLevel;
    public float timeToResolve;
    public int officersRequired;
    public float progressBar;
    public int pointValue;
    public List<Officer> assignedOfficers;
    public bool isResolved;

    public CrimeReport(CrimeLevel level)
    {
        id = System.Guid.NewGuid().ToString();
        crimeLevel = level;
        assignedOfficers = new List<Officer>();
        isResolved = false;
        
        // Set requirements based on crime level
        switch (level)
        {
            case CrimeLevel.Low:
                timeToResolve = 30f;
                officersRequired = 1;
                pointValue = 10;
                break;
            case CrimeLevel.Moderate:
                timeToResolve = 60f;
                officersRequired = 2;
                pointValue = 25;
                break;
            case CrimeLevel.High:
                timeToResolve = 120f;
                officersRequired = 3;
                pointValue = 50;
                break;
        }
    }
}
