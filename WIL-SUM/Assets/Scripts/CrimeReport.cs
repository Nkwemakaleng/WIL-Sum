using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Enum to represent different crime severity levels
public enum CrimeLevel {
    Low = 1,
    Moderate = 2, 
    High = 3
}

// Represents an individual crime report
[System.Serializable]
public class CrimeReport {
    public CrimeLevel level;
    public string crimeDescription;
    public int officersRequired;
    public float timeToSolve;
    public int pointValue;
}
