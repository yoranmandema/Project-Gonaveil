using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour {
    private class GroundedState : MovementState {
        public GroundedState(PlayerController movement) : base(movement) {

        }

        public override void OnStateEnter() {
            _movement.velocity.y = 0;
        }

        public override void OnStateUpdate() {

            if (_movement.wishJump)
                _movement.ApplyFriction(0f);
            else
                _movement.ApplyFriction(_movement.friction);

            var rayCast = Physics.Raycast(_movement.transform.position, Vector3.down, out RaycastHit hit, _movement.characterController.height / 2 + _movement.characterController.skinWidth + 1f);

            if (!_movement.isGrounded) _movement.SetState(new AirState(_movement));

            _movement.characterController.Move(Vector3.down * hit.distance);

            _movement.ApplyFriction(_movement.friction);

            var movementDir = _movement.transform.TransformDirection(_movement.desiredMovement);
            movementDir.y += Vector3.Dot(movementDir, -_movement.groundedNormal);

            _movement.DoAcceleration(movementDir, _movement.acceleration, !_movement.isCrouching ? _movement.maxVelocity : _movement.crouchVelocity);

            if (_movement.wishJump) {
                _movement.velocity += Vector3.up * Mathf.Sqrt(_movement.jumpHeight * 2f * Physics.gravity.magnitude * (_movement.fallSpeedMultiplier));
                _movement.SetState(new AirState(_movement));
                _movement.wishJump = false;
                return;
            }
        }

        public override void OnStateExit() {
        }
    }
}