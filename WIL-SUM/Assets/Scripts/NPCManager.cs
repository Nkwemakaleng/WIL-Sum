using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    
    public GameObject npcPrefab; // The NPC prefab to be instantiated
    public Transform[] entryPoints; // Points where NPCs will enter
    public Transform interactionPoint; // Point where NPCs will interact with the player
    public float spawnInterval = 5.0f; // Time interval between NPC spawns
    public float moveSpeed = 2.0f; // Speed at which NPCs move

    private List<GameObject> activeNPCs = new List<GameObject>();

    // Reference to the UI manager for displaying the report
    public ReportUIManager reportUIManager;

    void Start()
    {
        StartCoroutine(SpawnNPCs());
    }

    IEnumerator SpawnNPCs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Select a random entry point
            Transform entryPoint = entryPoints[Random.Range(0, entryPoints.Length)];
            GameObject newNPC = Instantiate(npcPrefab, entryPoint.position, Quaternion.identity);
            activeNPCs.Add(newNPC);

            // Start the NPC's movement to the interaction point
            StartCoroutine(MoveNPCToInteractionPoint(newNPC));
        }
    }

    IEnumerator MoveNPCToInteractionPoint(GameObject npc)
    {
        while (Vector3.Distance(npc.transform.position, interactionPoint.position) > 0.1f)
        {
            npc.transform.position = Vector3.MoveTowards(npc.transform.position, interactionPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // NPC has reached the interaction point
        HandleInteraction(npc);
    }

    void HandleInteraction(GameObject npc)
    {
        // Generate a random report
        Report report = GenerateRandomReport();

        // Display the report on the player's screen
        reportUIManager.DisplayReport(report);

        // Log the report details
        Debug.Log("NPC has given a report: " + report.description);

        // Optional: Trigger an animation or sound effect for handing over the report

        // Remove the NPC from the active list after the interaction
        StartCoroutine(RemoveNPCAfterDelay(npc, 2.0f));
    }

    Report GenerateRandomReport()
    {
        // Sample data for generating a report
        string[] crimeDescriptions = { "Burglary at local store", "Vandalism at the park", "Suspicious activity reported" };
        int neededCops = Random.Range(1, 6);
        int threatLevel = Random.Range(1, 4);

        return new Report
        {
            description = crimeDescriptions[Random.Range(0, crimeDescriptions.Length)],
            neededCops = neededCops,
            threatLevel = threatLevel
        };
    }

    IEnumerator RemoveNPCAfterDelay(GameObject npc, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (activeNPCs.Contains(npc))
        {
            activeNPCs.Remove(npc);
            Destroy(npc);
        }
    }
}

// Class to represent the report structure
[System.Serializable]
public class Report
{
    public string description;
    public int neededCops;
    public int threatLevel;
}
