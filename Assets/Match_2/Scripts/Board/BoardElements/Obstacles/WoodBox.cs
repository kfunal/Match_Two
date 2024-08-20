using System.Collections;
using UnityEngine;

public class WoodBox : Obstacle
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

    public override void OnObstacleDestroy()
    {
        if (destroying)
            return;

        destroying = true;

        AudioManager.Instance.PlaySound(SoundName.WoodBox);
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
