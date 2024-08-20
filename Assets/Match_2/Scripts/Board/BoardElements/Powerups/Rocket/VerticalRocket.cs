using UnityEngine;

public class VerticalRocket : Rocket
{
    public override void OnPop()
    {
        if (poweringUp)
            return;

        poweringUp = true;
        spriteRenderer.enabled = false;
        ResetPieceTransforms();
        PieceSpriteRenderers(true);
        ControlFirstPiece(false);
        ControlSecondPiece(false);
        base.OnPop();
    }
}
