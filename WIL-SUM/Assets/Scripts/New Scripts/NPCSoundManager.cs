using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSoundManager : MonoBehaviour {
    [System.Serializable]
    public class SoundCategory {
        public string categoryName;
        public List<AudioClip> clips = new List<AudioClip>();
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private AudioSource interactionAudioSource;

    [Header("Sound Categories")]
    [SerializeField] private SoundCategory footstepSounds = new SoundCategory { categoryName = "Footsteps" };
    [SerializeField] private SoundCategory reportSounds = new SoundCategory { categoryName = "Report Interactions" };

    // Ensure audio sources are always available
    private void Awake() {
        // Auto-create audio sources if not assigned
        if (walkingAudioSource == null) {
            walkingAudioSource = gameObject.AddComponent<AudioSource>();
            walkingAudioSource.playOnAwake = false;
        }

        if (interactionAudioSource == null) {
            interactionAudioSource = gameObject.AddComponent<AudioSource>();
            interactionAudioSource.playOnAwake = false;
        }

        // Set default audio source parameters
        ConfigureAudioSources();
    }

    // Configure audio source settings
    private void ConfigureAudioSources() {
        walkingAudioSource.volume = 0.5f;
        walkingAudioSource.spatialBlend = 1f; // 3D sound
        
        interactionAudioSource.volume = 0.7f;
        interactionAudioSource.spatialBlend = 1f; // 3D sound
    }

    // Safely play a random clip from a category
    private void PlayRandomClip(AudioSource source, List<AudioClip> clipList) {
        if (clipList == null || clipList.Count == 0) return;

        AudioClip randomClip = clipList[Random.Range(0, clipList.Count)];
        source.clip = randomClip;
        source.Play();
    }

    // Play walking/footstep sound
    public void PlayFootstepSound() {
        PlayRandomClip(walkingAudioSource, footstepSounds.clips);
    }

    // Play report delivery sound
    public void PlayReportDeliverySound() {
        PlayRandomClip(interactionAudioSource, reportSounds.clips);
    }

    // Optional: Add methods for specific sound types
    public void PlayReportAcceptedSound() {
        // Could add a specific "accepted" sound category
    }

    public void PlayReportRejectedSound() {
        // Could add a specific "rejected" sound category
    }

    // Inspector helper to validate sound lists
    public void ValidateSoundLists() {
        if (footstepSounds.clips == null) 
            footstepSounds.clips = new List<AudioClip>();
        
        if (reportSounds.clips == null) 
            reportSounds.clips = new List<AudioClip>();
    }

    // Optional debug logging
    private void OnValidate() {
        ValidateSoundLists();
    }
}