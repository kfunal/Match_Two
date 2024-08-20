using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private DTO dto;
    [SerializeField] List<AudioElement> audioElements;

    public void PlaySound(SoundName _name)
    {
        if (GetAudioElement(_name).SoundType == SoundType.SoundEffect && dto.PlayerModel.SoundEffects)
            GetAudioElement(_name).AudioSource.Play();

        else if (GetAudioElement(_name).SoundType == SoundType.Music && dto.PlayerModel.Musics)
            GetAudioElement(_name).AudioSource.Play();
    }

    public void Stop(SoundName _name) => GetAudioElement(_name).AudioSource.Stop();

    public AudioElement GetAudioElement(SoundName _name)
    {
        for (int i = 0; i < audioElements.Count; i++)
        {
            if (audioElements[i].SoundName == _name)
                return audioElements[i];
        }

        return null;
    }

    public bool IsPlaying(SoundName _name) => GetAudioElement(_name).AudioSource.isPlaying;
}
