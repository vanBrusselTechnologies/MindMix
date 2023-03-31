using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        float volume = 0.001f;//PlayerPrefs.GetFloat("achtergrondMuziekVolume", 0.25f);
        SetVolume(volume);
    }

    public void SetVolume(float volume)
    {
        audioSource.mute = Math.Abs(volume - 0.001f) < 0.0001f;
        //audioMixerGroup.audioMixer.SetFloat("Muziek", Mathf.Log10(volume) * 20f);
    }
}
