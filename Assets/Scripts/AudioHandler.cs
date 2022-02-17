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
        float volume = PlayerPrefs.GetFloat("achtergrondMuziekVolume", 0.25f);
        audioSource.mute = volume == 0.001f;
        audioMixerGroup.audioMixer.SetFloat("Muziek", Mathf.Log10(volume) * 20f);
    }
}
