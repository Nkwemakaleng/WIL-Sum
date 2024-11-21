using System.Collections.Generic;
using UnityEngine;

public class CrimeManager : MonoBehaviour
{
    public enum CrimeSeverity { Low, Medium, High }
    public enum CrimeType { Robbery, GrandTheft, ShopLifting }

    public struct Crime
    {
        public int id;
        public CrimeType crimeType;
        public CrimeSeverity severity;
        public int officersRequired;
        public int officerLeeway;
        public float resolutionTime; // Time in seconds
        public bool isResolved;
        public int crimeLocationX;
        public int crimeLocationY;
        public string streetAddress;
        public int streetNumber;
        public string crimeDesc1;
        public string crimeDesc2;
        public string crimeDesc3;
        public int scoreMultiplier;
    }

    private int nextCrimeID = 1; // Unique identifier for crimes
    public Dictionary<int, Crime> activeCrimes = new Dictionary<int, Crime>();
    public int score;

    void Start()
    {
        InvokeRepeating(nameof(GenerateCrime), 5f, 10f);
    }

    void GenerateCrime()
    {
        CrimeSeverity severity = (CrimeSeverity)Random.Range(0, 3);

        // Determine requirements based on severity
        int officersRequired;
        float resolutionTime;
        int officerLeeway;

        switch (severity)
        {
            case CrimeSeverity.Low:
                officersRequired = Random.Range(1, 2);
                resolutionTime = Random.Range(5f, 15f);
                officerLeeway = 1; // Allow minor overcommitment
                break;

            case CrimeSeverity.Medium:
                officersRequired = Random.Range(3, 5);
                resolutionTime = Random.Range(20f, 40f);
                officerLeeway = 2; // Moderate flexibility
                break;

            case CrimeSeverity.High:
                officersRequired = Random.Range(5, 8);
                resolutionTime = Random.Range(50f, 80f);
                officerLeeway = 3; // Higher flexibility for resource management
                break;

            default:
                officersRequired = 1;
                resolutionTime = 10f;
                officerLeeway = 0;
                break;
        }

        Crime newCrime = new Crime
        {
            id = nextCrimeID,
            crimeType = (CrimeType)Random.Range(0, 3),
            severity = severity,
            officersRequired = officersRequired,
            officerLeeway = officerLeeway,
            resolutionTime = resolutionTime,
            isResolved = false,
            crimeLocationX = Random.Range(0, 100),
            crimeLocationY = Random.Range(0, 100),
            streetAddress = "Main Street",
            streetNumber = Random.Range(1, 100),
            crimeDesc1 = "A suspicious individual was seen.",
            crimeDesc2 = "A vehicle was stolen.",
            crimeDesc3 = "Reported by a civilian nearby."
        };

        activeCrimes.Add(nextCrimeID, newCrime);
        Debug.Log($"New {severity} crime reported with ID {nextCrimeID}: {newCrime.crimeType} at {newCrime.streetAddress}.");
        nextCrimeID++;
    }

    public void ResolveCrime(int crimeID, int officersAssigned)
    {
        if (activeCrimes.TryGetValue(crimeID, out Crime crime))
        {
            if (officersAssigned >= crime.officersRequired)
            {
                crime.isResolved = true;
                score += 10; // Reward points for solving the crime
                Debug.Log($"Crime ID {crimeID} resolved!");
            }
            else
            {
                score -= 5; // Penalty for failure
                Debug.Log($"Crime ID {crimeID} could not be resolved. Not enough officers!");
            }

            // Update the dictionary entry
            activeCrimes[crimeID] = crime;
        }
        else
        {
            Debug.Log($"Crime ID {crimeID} not found.");
        }
    }

    public void RemoveResolvedCrimes()
    {
        List<int> crimesToRemove = new List<int>();

        foreach (var crime in activeCrimes)
        {
            if (crime.Value.isResolved)
            {
                crimesToRemove.Add(crime.Key);
            }
        }

        foreach (int crimeID in crimesToRemove)
        {
            activeCrimes.Remove(crimeID);
            Debug.Log($"Crime ID {crimeID} removed from active list.");
        }
    }
}



