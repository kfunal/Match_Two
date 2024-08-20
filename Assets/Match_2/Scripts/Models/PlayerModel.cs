using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    public int Chapter;
    public bool SoundEffects;
    public bool Musics;

    public PlayerModel()
    {
        Chapter = 1;
        SoundEffects = true;
        Musics = true;
    }

    public void ResetChapter() => Chapter = 1;
}
