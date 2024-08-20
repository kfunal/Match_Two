using System;
using System.Collections.Generic;
using Board.Elements.MatchElements;
using Helpers;
using Player.Manager;
using UnityEngine;

namespace Player.State
{
    [Serializable]
    public class PMPlayState : StateBase
    {
        private PlayerManager playerManager;
        private IClickable clickedBoardElement;
        private BoardElement clickedElement;

        RaycastHit2D[] rayHit = new RaycastHit2D[1];
        [SerializeField] private List<BoardElement> matchElements = new List<BoardElement>();

        public PMPlayState(PlayerManager _stateMachine) : base("PMPlayState", _stateMachine) => playerManager = _stateMachine;

        public override void EnterState()
        {
            base.EnterState();
            matchElements.Clear();
            playerManager.CanPlay = true;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (!playerManager.CanPlay)
                return;

            if (Input.GetMouseButtonDown(0))
                OnMouseDown();
        }

        private void OnMouseDown()
        {
            clickedBoardElement = BoardElementClicked();

            if (clickedBoardElement == null)
                return;

            clickedElement = clickedBoardElement.Element;

            if (clickedElement == null)
                return;

            matchElements.Clear();
            clickedBoardElement.OnClick(clickedBoardElement.ClickedElementType, clickedBoardElement.ClickedElementCategory, ref matchElements);
            clickedBoardElement.CheckMatch(ref matchElements);

            ConsoleHelper.PrintLogWithColor($"Element -> {clickedElement.ElementType} at {clickedElement.Row},{clickedElement.Column} is clicked", "aqua");
        }

        private IClickable BoardElementClicked()
        {
            rayHit = new RaycastHit2D[1];

            Physics2D.GetRayIntersectionNonAlloc(playerManager.MainCamera.ScreenPointToRay(Input.mousePosition), rayHit);

            if (rayHit[0].collider != null && rayHit[0].collider.TryGetComponent(out IClickable boardElement) && boardElement.Clickable)
                return boardElement;

            return null;
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}
