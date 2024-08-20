using UnityEngine;

[CreateAssetMenu(fileName = "DataTransferObject", menuName = "ScriptableObjects/Data/DTO")]
public class DTO : ScriptableObject
{
    [Header("Player Model")]
    [SerializeField] private PlayerModel playerModel;
    [Header("Scriptable Objects")]
    [SerializeField] private LevelController levelController;

    [Header("Prefs Names")]
    [SerializeField] private string PlayerModelPrefsName = "PlayerModel";

    public PlayerModel PlayerModel => playerModel;

    public void SavePlayerModel()
    {
        ControlChapter();
        PlayerPrefs.SetString(PlayerModelPrefsName, JsonUtility.ToJson(playerModel));
    }
    public void LoadPlayerModel()
    {
        if (!PlayerPrefs.HasKey(PlayerModelPrefsName))
            playerModel = new PlayerModel();
        else
        {
            playerModel = JsonUtility.FromJson<PlayerModel>(PlayerPrefs.GetString(PlayerModelPrefsName));
            ControlChapter();
        }
    }

    private void ControlChapter()
    {
        if (playerModel.Chapter > levelController.Levels.Count)
            playerModel.Chapter = levelController.Levels.Count;
    }
}
