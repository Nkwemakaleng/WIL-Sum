using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class UnitMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private LayerMask ground; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
             RaycastHit hit;
             Ray ray = cam.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
             {
                 agent.SetDestination(hit.point);
             }
                    
        }
    }
}
