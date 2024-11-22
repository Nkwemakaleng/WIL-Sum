using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Officer
{
    public string id;
    public bool isAvailable;
    public CrimeReport currentAssignment;

    public Officer()
    {
        id = System.Guid.NewGuid().ToString();
        isAvailable = true;
        currentAssignment = null;
    }
}
