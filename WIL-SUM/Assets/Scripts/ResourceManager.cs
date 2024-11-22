using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int totalOfficers;
    public int availableOfficers;

    public void AssignOfficers(int officersRequired)
    {
        if (availableOfficers >= officersRequired)
        {
            availableOfficers -= officersRequired;
            Debug.Log($"{officersRequired} officers assigned.");
        }
        else
        {
            Debug.Log("Not enough officers available!");
        }
    }

    public void ReleaseOfficers(int officersReleased)
    {
        availableOfficers += officersReleased;
        if (availableOfficers > totalOfficers)
            availableOfficers = totalOfficers;
        Debug.Log($"{officersReleased} officers released.");
    }
}

