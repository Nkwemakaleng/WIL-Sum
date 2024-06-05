using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
  public Vector3 spawnerSize;

  public Color gizmosColor;

  public GameObject targetObject;

  public GameObject spawnTargetObject;
  
  public GameObject Object;

  public GameObject Object1;
  public GameObject Object2;

  public int objectCount;
    // Start is called before the first frame update
    void Start()
    {
        /*while (CheckSpawn(objectCount))
        {
            StartCoroutine(spawnObject());
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        while (CheckSpawn(objectCount))
        {
            StartCoroutine(spawnObject());
        }
    }

    public void addSpawnCount(int i)
    {
        objectCount = i;
    }

    bool CheckSpawn(int count)
    {
        if (count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator spawnObject()
    {
        Vector3 gizmosPos = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y,0);
        Vector3 SpawnPos = gizmosPos + new Vector3(Random.Range(-spawnerSize.x / 2, spawnerSize.x / 2),
            Random.Range(-spawnerSize.y / 2, spawnerSize.y / 2), 0);
        Instantiate(spawnTargetObject, SpawnPos, Quaternion.identity);
        objectCount -= 1;
        Debug.Log("Object Spawned");
        yield return new WaitForSeconds(1f);
    }

   public void OnDrawGizmos()
    {
        if (targetObject != null)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawSphere(targetObject.transform.position, spawnerSize.x/2);
        }
    }

   private void unitSelection(GameObject Object)
   {
       targetObject = Object;
   }
   
   
}
