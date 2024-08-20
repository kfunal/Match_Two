using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private float settingsButtonChangeDuration = 0.1f;
    [SerializeField] private Vector3 settingsButtonOffPosition;
    [SerializeField] private Vector3 settingsButtonOnPosition;

    [Header("Canvas")]
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    [Header("Rect Transforms")]
    [SerializeField] private RectTransform effectsButtonRect;
    [SerializeField] private RectTransform musicsButtonRect;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI chapterButtonText;

    [Header("Scriptable Objects")]
    [SerializeField] private DTO dto;

    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private Vector3 targetPos;

    private void Awake()
    {
        dto.LoadPlayerModel();
        chapterButtonText.SetText($"CHAPTER {dto.PlayerModel.Chapter}");
    }

    private IEnumerator Start()
    {
        //To prevent the animation from freezing
        yield return waitForEndOfFrame;
        InitializeSettingsButtons();
    }

    public void InitializeSettingsButtons()
    {
        targetPos = dto.PlayerModel.SoundEffects ? settingsButtonOnPosition : settingsButtonOffPosition;
        effectsButtonRect.DOKill();
        effectsButtonRect.DOLocalMove(targetPos, settingsButtonChangeDuration);

        targetPos = dto.PlayerModel.Musics ? settingsButtonOnPosition : settingsButtonOffPosition;
        musicsButtonRect.DOKill();
        musicsButtonRect.DOLocalMove(targetPos, settingsButtonChangeDuration);
    }

    public void OnChapterButtonClicked()
    {
        graphicRaycaster.enabled = false;
        dto.SavePlayerModel();
        AudioManager.Instance.PlaySound(SoundName.ButtonClick);
        effectsButtonRect.DOKill();
        musicsButtonRect.DOKill();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnResetChapterButtonClicked()
    {
        dto.PlayerModel.ResetChapter();
        AudioManager.Instance.PlaySound(SoundName.ButtonClick);
    }

    public void ChangeSoundEffectsStatus() => ChangeSettingsButtonStatus(effectsButtonRect, ref dto.PlayerModel.SoundEffects);
    public void ChangeMusicStatus() => ChangeSettingsButtonStatus(musicsButtonRect, ref dto.PlayerModel.Musics);

    private void ChangeSettingsButtonStatus(RectTransform _buttonRect, ref bool _modelField)
    {
        targetPos = _modelField ? settingsButtonOffPosition : settingsButtonOnPosition;
        _modelField = !_modelField;
        AudioManager.Instance.PlaySound(SoundName.ButtonClick);
        _buttonRect.DOKill();
        _buttonRect.DOLocalMove(targetPos, settingsButtonChangeDuration);
    }

    private void OnApplicationFocus(bool _focusStatus)
    {
        if (!_focusStatus)
            dto.SavePlayerModel();
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        dto.SavePlayerModel();
    }
#endif
}
