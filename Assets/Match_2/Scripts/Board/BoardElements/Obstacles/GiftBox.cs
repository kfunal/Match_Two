using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBox : Obstacle
{
    [SerializeField] private float CollapseCallWaitTime = 1f;
    private Coroutine timerCoroutine;
    private WaitForSeconds collapseCallWait;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        collapseCallWait = new WaitForSeconds(CollapseCallWaitTime);
    }

    public override void OnReturnToPool()
    {
        ClearCoroutine();
        base.OnReturnToPool();
    }

    private void ClearCoroutine()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = null;
    }

    public override void OnObstacleHealthDecrease()
    {
        base.OnObstacleHealthDecrease();
        AudioManager.Instance.PlaySound(SoundName.GiftBox);
    }

    public override void OnObstacleDestroy()
    {
        if (destroying)
            return;

        destroying = true;
        AudioManager.Instance.PlaySound(SoundName.GiftBox);
        spriteRenderer.enabled = false;
        destroyParticle.PlayParticle();
        boardManager.OnObstacleDestroy(this);
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        yield return collapseCallWait;
        CallCollapse();
        timerCoroutine = null;
    }
}
