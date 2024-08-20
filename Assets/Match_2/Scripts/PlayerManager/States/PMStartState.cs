using System.Collections;
using System.Collections.Generic;
using Player.Manager;
using UnityEngine;

namespace Player.State
{
    public class PMStartState : StateBase
    {
        private PlayerManager playerManager;

        public PMStartState(PlayerManager _stateMachine) : base("PMStartState", _stateMachine) => playerManager = _stateMachine;
    }
}
