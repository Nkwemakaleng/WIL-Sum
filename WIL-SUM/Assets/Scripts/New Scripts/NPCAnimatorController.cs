using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimatorController : MonoBehaviour {
    private Animator animator;

    // Animation state constants
    private const string WALK_PARAM = "IsWalking";
    private const string DELIVER_PARAM = "DeliverReport";
    private const string IDLE_PARAM = "IsIdle";

    void Start() {
        animator = GetComponent<Animator>();
    }

    // Walking animation
    public void StartWalking() {
        animator.SetBool(WALK_PARAM, true);
        animator.SetBool(IDLE_PARAM, false);
    }

    // Stop walking
    public void StopWalking() {
        animator.SetBool(WALK_PARAM, false);
        animator.SetBool(IDLE_PARAM, true);
    }

    // Trigger report delivery animation
    public void DeliverReport() {
        animator.SetTrigger(DELIVER_PARAM);
    }

    // Reaction to successful report assignment
    public void OnReportAssigned() {
        animator.SetTrigger("ReportAssigned");
    }

    // Reaction to report rejection
    public void OnReportRejected() {
        animator.SetTrigger("ReportRejected");
    }
}