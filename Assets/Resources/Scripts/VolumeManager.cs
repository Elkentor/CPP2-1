using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    private void Awake()
    {
        Debug.Log("VolumeManager Awake on: " + gameObject.name);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load saved volume or default to 1.0
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;

        // Apply immediately
        SetVolume(savedVolume);

        // Add listener for runtime changes
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float sliderValue)
    {
        // Clamp to avoid log10(0) errors
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);

        // Convert slider (0.0001 → 1) to decibels
        float volumeInDb = Mathf.Log10(clampedValue) * 20f;

        audioMixer.SetFloat("MasterVolume", volumeInDb);

        // Save for persistence
        PlayerPrefs.SetFloat("MasterVolume", clampedValue);

        Debug.Log("SetVolume called with value: " + clampedValue);
    }

}
