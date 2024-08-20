using Board.Manager;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using Board.Elements.MatchElements;
using Player.Manager;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using ObjectPool;
public class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Scriptable Objects")]
    [SerializeField] private DTO dto;
    [SerializeField] private LevelController levelController;

    [Header("Script References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameUIManager uiManager;

    private int moveCount;
    private bool gameFinished;
    private Level currentLevel;

    private BoardElement[] elements;
    private Coroutine waitForBoardCoroutine;
    private Coroutine waitForSoundCoroutine;

    public bool GameFinished => gameFinished;
    public Level CurrentLevel => currentLevel;
    private ObjectPooling objectPooling => ObjectPooling.Instance;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private void Awake() => OnAwake();
    private void Start() => OnStart();

    private void OnAwake()
    {
        Application.targetFrameRate = 60;
        DOTween.Init(true, false);
        DOTween.SetTweensCapacity(5000, 100);

        currentLevel = levelController.CurrentLevel(dto.PlayerModel.Chapter);
        currentLevel.EndConditions.ForEach(x => x.ResetCompleted());
        moveCount = currentLevel.MoveCount;
    }

    private void OnStart()
    {
        gameFinished = false;
        boardManager.CreateBoard();
    }

    private void OnEnable()
    {
        SubscribeEvents();
        ClearCoroutines();
    }

    private void OnDisable()
    {
        DeSubscribeEvents();
        ClearCoroutines();
    }

    private void ClearCoroutines()
    {
        if (waitForBoardCoroutine != null)
            StopCoroutine(waitForBoardCoroutine);

        waitForBoardCoroutine = null;

        if (waitForSoundCoroutine != null)
            StopCoroutine(waitForSoundCoroutine);

        waitForSoundCoroutine = null;
    }

    private void SubscribeEvents()
    {
        boardManager.OnNormalElementMatch += OnNormalElementMatch;
        boardManager.OnPowerupPop += OnPowerupPop;
        boardManager.OnPowerupCombine += OnPowerupCombine;
        boardManager.OnObstacleDestroy += OnObstacleDestroy;
    }

    private void DeSubscribeEvents()
    {
        boardManager.OnNormalElementMatch -= OnNormalElementMatch;
        boardManager.OnPowerupPop -= OnPowerupPop;
        boardManager.OnPowerupCombine -= OnPowerupCombine;
        boardManager.OnObstacleDestroy -= OnObstacleDestroy;
    }

    private void DecreaseMoveCount()
    {
        if (gameFinished)
            return;

        moveCount--;

        uiManager.UpdateMoveCountText(moveCount);

        if (moveCount <= 0)
            OnMoveCountIsOver();
    }

    private void OnNormalElementMatch(List<BoardElement> _elements)
    {
        ControlEndConditions(_elements.Count, _elements[0].ElementType);
        DecreaseMoveCount();
    }

    private void OnPowerupPop(Powerup _powerup)
    {
        ControlEndConditions(1, _powerup.ElementType);
        DecreaseMoveCount();
    }

    private void OnPowerupCombine(BoardElement _first, BoardElement _second)
    {
        DecreaseMoveCount();
    }

    private void OnObstacleDestroy(Obstacle _obstacle)
    {
        ControlEndConditions(1, _obstacle.ElementType);
    }

    private void OnMoveCountIsOver()
    {
        if (gameFinished)
            return;

        GameEnd();

        waitForBoardCoroutine = StartCoroutine(WaitForBoard(() =>
        {
            if (currentLevel.EndConditionsCompleted())
            {
                OnEndConditionsCompleted();
                return;
            }
            AudioManager.Instance.PlaySound(SoundName.Lose);
            waitForSoundCoroutine = StartCoroutine(WaitForEndSound(SoundName.Lose));
        }));
    }

    private void OnEndConditionsCompleted()
    {
        if (gameFinished)
            return;

        GameEnd();

        waitForBoardCoroutine = StartCoroutine(WaitForBoard(() =>
        {
            dto.PlayerModel.Chapter++;
            dto.SavePlayerModel();
            AudioManager.Instance.PlaySound(SoundName.Win);
            waitForSoundCoroutine = StartCoroutine(WaitForEndSound(SoundName.Win));
        }));
    }

    private void GameEnd()
    {
        gameFinished = true;
        playerManager.CanPlay = false;
    }

    private void ControlEndConditions(int _amount, ElementType _elementType)
    {
        currentLevel.ControlEndConditions(_amount, _elementType);

        if (currentLevel.EndConditionsCompleted())
            OnEndConditionsCompleted();

        uiManager.ControlEndConditions(currentLevel);
    }

    private IEnumerator WaitForBoard(Action _afterBoardWait)
    {
        elements = boardManager.ElementsParent.GetComponentsInChildren<BoardElement>();

        if (elements == null || elements.Length == 0)
            yield break;

        while (elements.Where(x => x.Matching).Count() > 0 || elements.Where(x => x.PoweringUp).Count() > 0
        || elements.Where(x => x.WaitingToCreatePowerup).Count() > 0 || elements.Where(x => x.Moving).Count() > 0 || elements.Where(x => x.Destroying).Count() > 0)
        {
            elements = boardManager.ElementsParent.GetComponentsInChildren<BoardElement>();
            yield return null;
        }

        _afterBoardWait?.Invoke();
        waitForBoardCoroutine = null;
    }

    private IEnumerator WaitForEndSound(SoundName _name)
    {
        while (AudioManager.Instance.IsPlaying(_name))
            yield return null;

        objectPooling.ResetPool();
        yield return waitForEndOfFrame;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
