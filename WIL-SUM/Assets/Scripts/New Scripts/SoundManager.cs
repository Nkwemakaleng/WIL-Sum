using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;



public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;
    
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    
    [Header("Sound Settings")]
    [SerializeField] private Sound[] sounds;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float ambientVolume = 1f;

    // Pool for sound effects
    private Dictionary<string, Queue<AudioSource>> soundPool;
    private int poolSize = 5;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSoundPool();
        SetupAudioSources();
    }

    private void InitializeSoundPool()
    {
        soundPool = new Dictionary<string, Queue<AudioSource>>();

        // Create pool for each sound
        foreach (Sound s in sounds)
        {
            Queue<AudioSource> sourceQueue = new Queue<AudioSource>();

            // Create multiple sources for each sound
            for (int i = 0; i < poolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = s.clip;
                source.volume = s.volume;
                source.pitch = s.pitch;
                source.loop = s.loop;
                source.playOnAwake = false;

                // Set the mixer group based on type
                if (audioMixer != null)
                {
                    source.outputAudioMixerGroup = s.isAmbient ? 
                        audioMixer.FindMatchingGroups("Ambient")[0] : 
                        audioMixer.FindMatchingGroups("SFX")[0];
                }

                sourceQueue.Enqueue(source);
            }

            soundPool.Add(s.name, sourceQueue);
        }
    }

    private void SetupAudioSources()
    {
        // Setup music source if not already set
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            if (audioMixer != null)
                musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        }

        // Setup ambient source if not already set
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.loop = true;
            if (audioMixer != null)
                ambientSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Ambient")[0];
        }
    }

    public void PlaySound(string name)
    {
        if (!soundPool.ContainsKey(name))
        {
            Debug.LogWarning($"Sound {name} not found!");
            return;
        }

        Queue<AudioSource> sourceQueue = soundPool[name];
        AudioSource source = sourceQueue.Dequeue();

        source.volume = sfxVolume;
        source.Play();

        // Put the source back in the queue
        sourceQueue.Enqueue(source);
    }

    public void PlaySoundAtPosition(string name, Vector3 position)
    {
        if (!soundPool.ContainsKey(name))
        {
            Debug.LogWarning($"Sound {name} not found!");
            return;
        }

        Queue<AudioSource> sourceQueue = soundPool[name];
        AudioSource source = sourceQueue.Dequeue();

        source.transform.position = position;
        source.volume = sfxVolume;
        source.spatialBlend = 1f; // Make the sound fully 3D
        source.Play();

        sourceQueue.Enqueue(source);
    }

    public void PlayMusic(AudioClip musicClip, float fadeTime = 1f)
    {
        if (musicSource.clip == musicClip && musicSource.isPlaying) return;

        StartCoroutine(FadeMusicCoroutine(musicClip, fadeTime));
    }

    private System.Collections.IEnumerator FadeMusicCoroutine(AudioClip newClip, float fadeTime)
    {
        float timeElapsed = 0;
        float startVolume = musicSource.volume;

        // Fade out current music
        while (timeElapsed < fadeTime)
        {
            timeElapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / fadeTime);
            yield return null;
        }

        // Change clip and start playing
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new music
        timeElapsed = 0;
        while (timeElapsed < fadeTime)
        {
            timeElapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, musicVolume, timeElapsed / fadeTime);
            yield return null;
        }
    }

    public void PlayAmbient(AudioClip ambientClip, float fadeTime = 1f)
    {
        if (ambientSource.clip == ambientClip && ambientSource.isPlaying) return;

        StartCoroutine(FadeAmbientCoroutine(ambientClip, fadeTime));
    }

    private System.Collections.IEnumerator FadeAmbientCoroutine(AudioClip newClip, float fadeTime)
    {
        float timeElapsed = 0;
        float startVolume = ambientSource.volume;

        // Fade out current ambient
        while (timeElapsed < fadeTime)
        {
            timeElapsed += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / fadeTime);
            yield return null;
        }

        // Change clip and start playing
        ambientSource.clip = newClip;
        ambientSource.Play();

        // Fade in new ambient
        timeElapsed = 0;
        while (timeElapsed < fadeTime)
        {
            timeElapsed += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(0, ambientVolume, timeElapsed / fadeTime);
            yield return null;
        }
    }

    // Volume Control Methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        audioMixer?.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
        audioMixer?.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer?.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SetAmbientVolume(float volume)
    {
        ambientVolume = volume;
        ambientSource.volume = volume;
        audioMixer?.SetFloat("AmbientVolume", Mathf.Log10(volume) * 20);
    }

    public void StopAllSounds()
    {
        foreach (var sourceQueue in soundPool.Values)
        {
            foreach (var source in sourceQueue)
            {
                source.Stop();
            }
        }
    }

    public void PauseAllSounds()
    {
        foreach (var sourceQueue in soundPool.Values)
        {
            foreach (var source in sourceQueue)
            {
                source.Pause();
            }
        }
        musicSource.Pause();
        ambientSource.Pause();
    }

    public void ResumeAllSounds()
    {
        foreach (var sourceQueue in soundPool.Values)
        {
            foreach (var source in sourceQueue)
            {
                if (!source.isPlaying)
                    source.UnPause();
            }
        }
        musicSource.UnPause();
        ambientSource.UnPause();
    }
}