using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public string npcName;
    public string[] dialogueOptions;
    public CrimeLevel preferredCrimeLevel;
}

public class NPCReportSystem : MonoBehaviour
{
    [SerializeField] private NPCData[] npcs;
    [SerializeField] private float npcSpawnInterval = 20f;
    [SerializeField] private Transform npcSpawnPoint;
    [SerializeField] private GameObject npcPrefab;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= npcSpawnInterval)
        {
            SpawnRandomNPC();
            spawnTimer = 0f;
        }
    }

    private void SpawnRandomNPC()
    {
        NPCData npcData = npcs[UnityEngine.Random.Range(0, npcs.Length)];
        GameObject npcObject = Instantiate(npcPrefab, npcSpawnPoint.position, Quaternion.identity);
        NPCController controller = npcObject.GetComponent<NPCController>();
        controller.Initialize(npcData);
    }
}

public class NPCController : MonoBehaviour
{
    private NPCData data;
    private bool hasReported = false;

    public void Initialize(NPCData npcData)
    {
        data = npcData;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasReported && other.CompareTag("Player"))
        {
            PresentReport();
        }
    }

    private void PresentReport()
    {
        hasReported = true;
        string dialogue = data.dialogueOptions[UnityEngine.Random.Range(0, data.dialogueOptions.Length)];
    
        // Generate crime report through the management system
        CrimeManagementSystem.Instance.GenerateSpecificReport(
            System.Guid.NewGuid().ToString(),  // id
            data.preferredCrimeLevel,          // crimeLevel
            0f,                                // assignedOfficers
            false                              // isResolved
        );
    
        // Show dialogue UI (implement separately)
        ShowDialogue(dialogue);
    
        // Destroy NPC after delay
        StartCoroutine(DestroyAfterDelay());
    }

    private void ShowDialogue(string dialogue)
    {
        // Implement dialogue UI system here
        Debug.Log($"NPC {data.npcName}: {dialogue}");
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}