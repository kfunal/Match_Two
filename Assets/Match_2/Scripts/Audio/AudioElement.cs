using UnityEngine;

[System.Serializable]
public class AudioElement
{
    [SerializeField] private string name;
    [SerializeField] private SoundName soundName;
    [SerializeField] private SoundType soundType;
    [SerializeField] private AudioSource audioSource;

    public SoundName SoundName => soundName;
    public SoundType SoundType => soundType;
    public AudioSource AudioSource => audioSource;
}
