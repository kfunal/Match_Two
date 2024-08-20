using System.Collections;
using System.Collections.Generic;
using Board.Manager;
using Player.State;
using UnityEngine;

namespace Player.Manager
{
    public class PlayerManager : StateManager
    {
        #region States
        private PMStartState startState;
        private PMPlayState playState;

        public PMPlayState PlayState => playState;

        #endregion

        #region Inspector

        [field: Header("Scripts")]
        [field: SerializeField] public BoardManager BoardManager { get; private set; }

        [field: Header("Camera")]
        [field: SerializeField] public Camera MainCamera { get; private set; }

        #endregion

        #region Variables
        private bool canPlay = false;
        #endregion

        #region Props
        public bool CanPlay { get { return canPlay; } set { canPlay = value; } }
        #endregion

        private void Awake()
        {
            InitStates();
        }

        private void InitStates()
        {
            startState = new PMStartState(this);
            playState = new PMPlayState(this);
        }

        public override StateBase InitState()
        {
            return startState;
        }
    }
}
