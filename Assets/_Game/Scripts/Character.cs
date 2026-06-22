using UnityEngine;

namespace Game
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterMovement _movement;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private CharacterAnimator _animator;

        private void Update()
        {
            _input.ManualUpdate();
            _movement.Move(_input.MouseWorldPosition);
            _animator.UpdateLocomotion(_movement.NormalizedSpeed);
        }

        private void LateUpdate()
        {
            _animator.UpdateAim(_input.AimEnabled, _input.MouseWorldPosition);

        }

    }
}