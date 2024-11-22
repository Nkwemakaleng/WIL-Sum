using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class NPCController : MonoBehaviour {
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float arrivalDistance = 0.1f;

    [Header("Components")]
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private NPCSoundManager soundManager;

    [Header("Movement Points")]
    public Transform spawnPoint;
    public Transform deliveryPoint;
    public Transform playerDeskPoint;

    [Header("Animation Parameters")]
    public string walkParameterName = "IsWalking";
    public string idleParameterName = "IsIdle";

    private bool isMoving = false;
    private Vector3 currentDestination;
    
    // Event for reaching desk
    public event System.Action OnReachedDesk;
    
    // Track if NPC has reached exit
    private bool hasReachedExit = false;

    void Awake() {
        // Get required components
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        soundManager = GetComponent<NPCSoundManager>();

        // Ensure components are set up
        if (navMeshAgent == null) {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        // Configure NavMesh Agent
        ConfigureNavMeshAgent();
    }

    void ConfigureNavMeshAgent() {
        navMeshAgent.speed = walkSpeed;
        navMeshAgent.stoppingDistance = arrivalDistance;
    }

    // Initialize NPC movement to player's desk
    public void MoveToPlayerDesk() {
        MoveToTarget(playerDeskPoint.position);
    }

    // Generic movement method
    void MoveToTarget(Vector3 destination) {
        if (navMeshAgent == null) return;

        currentDestination = destination;
        navMeshAgent.SetDestination(destination);
        
        // Start walking animation and sounds
        StartWalking();
    }

    void Update() {
        // Update walking state
        UpdateMovementState();
    }

    void UpdateMovementState() {
        // Check if NPC has reached destination
        if (navMeshAgent != null && navMeshAgent.remainingDistance <= arrivalDistance) {
            StopWalking();
        }
    }

    void StartWalking() {
        isMoving = true;
        
        // Update animator
        if (animator != null) {
            animator.SetBool(walkParameterName, true);
            animator.SetBool(idleParameterName, false);
        }

        // Play walking sounds
        if (soundManager != null) {
            soundManager.PlayFootstepSound();
        }
    }

    void StopWalking() {
        isMoving = false;
        
        // Update animator
        if (animator != null) {
            animator.SetBool(walkParameterName, false);
            animator.SetBool(idleParameterName, true);
        }

        // Optional: Trigger arrived at destination event
        OnReachedDestination();
    }

    void OnReachedDestination() {
        // Potential interaction or event trigger
        if (currentDestination == playerDeskPoint.position) {
            // Notify spawner that NPC has reached desk
            OnReachedDesk?.Invoke();
            ExecuteCrimeReportHandover();
        }
        else if (currentDestination == deliveryPoint.position) {
            hasReachedExit = true;
        }
    }

    // Method to initiate exit movement
    public void MoveToExit() {
        hasReachedExit = false;
        MoveToTarget(deliveryPoint.position);
    }

    // Method to check if NPC has reached exit
    public bool HasReachedExit() {
        return hasReachedExit;
    }

    /* Method to handle report acceptance
    public void OnReportAccepted() {
        if (animator != null) {
            animator.SetTrigger("ReportAccepted");
        }
    }*/

    void ExecuteCrimeReportHandover() {
        // Play delivery animation
        if (animator != null) {
            animator.SetTrigger("DeliverReport");
        }

        // Play delivery sound
        if (soundManager != null) {
            soundManager.PlayReportDeliverySound();
        }
    }

    /* Method to be called when player accepts/rejects report
    public void OnReportInteraction(bool accepted) {
        if (accepted) {
            // Play accepted animation/sound
            animator?.SetTrigger("ReportAccepted");
            soundManager?.PlayReportAcceptedSound();
        }
        else {
            // Play rejected animation/sound
            animator?.SetTrigger("ReportRejected");
            soundManager?.PlayReportRejectedSound();
        }

        // Move to delivery point
        MoveToTarget(deliveryPoint.position);
    }*/
}