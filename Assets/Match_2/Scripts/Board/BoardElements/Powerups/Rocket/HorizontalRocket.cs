using UnityEngine;

public class HorizontalRocket : Rocket
{
    public override void OnPop()
    {
        if (poweringUp)
            return;

        poweringUp = true;
        spriteRenderer.enabled = false;
        ResetPieceTransforms();
        PieceSpriteRenderers(true);
        ControlFirstPiece(true);
        ControlSecondPiece(true);
        base.OnPop();
    }
}
