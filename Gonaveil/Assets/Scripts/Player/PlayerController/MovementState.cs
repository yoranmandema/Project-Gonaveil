using UnityEngine;

public partial class PlayerController : MonoBehaviour {
    public abstract class MovementState {
        protected PlayerController _movement;

        public MovementState(PlayerController movement) {
            _movement = movement;
        }

        public abstract void OnStateEnter();
        public abstract void OnStateUpdate();
        public abstract void OnStateExit();
    }
}