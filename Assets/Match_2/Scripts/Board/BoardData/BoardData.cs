using DG.Tweening;
using UnityEngine;

namespace Board.Parameters
{
    [CreateAssetMenu(fileName = "BoardData", menuName = "ScriptableObjects/Board/BoardData")]
    public class BoardParameters : ScriptableObject
    {
        [field: Header("Match Parameters")]
        [field: SerializeField] public int MinimumElementCountToMatch { get; private set; }
        [field: SerializeField] public int RocketMatchElementCount { get; private set; }
        [field: SerializeField] public int TNTMatchElementCount { get; private set; }
        [field: SerializeField] public int DiscoBallMatchElementCount { get; private set; }

        [field: Header("Move Parameters")]
        [field: SerializeField] public float MoveDuration { get; private set; }
        [field: SerializeField] public float MoveEaseAmplitude { get; private set; }
        [field: SerializeField] public Ease MoveEase { get; private set; }

        [field: Header("Powerup Create Parameters")]
        [field: SerializeField] public float PowerupCreateMoveSpeed { get; private set; }
        [field: SerializeField] public Ease PowerupCreateMoveEase { get; private set; }

        [field: Header("Disco Ball Parameters")]
        [field: SerializeField] public float LineDuration { get; private set; }
        [field: SerializeField] public float DelayBetweenLines { get; private set; }
        [field: SerializeField] public Ease LineEase { get; private set; }

        [field: Header("Horizontal Rocket Parameters")]
        [field: SerializeField] public float PieceDuration { get; private set; }
        [field: SerializeField] public Ease PieceEase { get; private set; }
    }
}
