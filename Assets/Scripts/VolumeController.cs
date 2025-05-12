using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] public AudioMixer audioMixer; // Reference to the AudioMixer
    [SerializeField] public Slider volumeSlider; // Reference to the volume slider


    // Start is called before the first frame update
    public void SetAudioLevel()
    {
        float volumeLevel = volumeSlider.value; // Get the value from the slider
        // Set the volume level in the AudioMixer
        audioMixer.SetFloat("volume", Mathf.Log10(volumeLevel) * 20);
    }
}
