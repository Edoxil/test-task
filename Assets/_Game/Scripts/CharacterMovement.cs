using UnityEngine;

namespace Game
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private float _maxSpeed = 5f;
        [SerializeField] private float _minSpeed = 1.5f;

        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 15f;

        [SerializeField] private float _distanceResponse = 2f;
        [SerializeField] private float _deadZone = 0.1f;

        private float _currentVelocity;

        private float CurrentVelocity => Mathf.Abs(_currentVelocity);
        public float NormalizedSpeed => CurrentVelocity / _maxSpeed;

        public void Move(Vector3 targetPosition)
        {
            float currentX = transform.position.x;
            float targetX = targetPosition.x;

            float distance = targetX - currentX;
            float absDistance = Mathf.Abs(distance);

            float direction = Mathf.Sign(distance);

            float desiredSpeed = _maxSpeed;
            float brakingSpeed = Mathf.Sqrt(2f * _deceleration * absDistance);
            desiredSpeed = Mathf.Min(desiredSpeed, brakingSpeed);

            if (absDistance < _deadZone)
                desiredSpeed = 0f;

            float desiredVelocity = desiredSpeed * direction;

            float velocityChangeSpeed = Mathf.Abs(desiredVelocity) > Mathf.Abs(_currentVelocity)
                ? _acceleration
                : _deceleration;

            _currentVelocity = Mathf.MoveTowards(
                _currentVelocity,
                desiredVelocity,
                velocityChangeSpeed * Time.deltaTime);

            transform.position += Vector3.right * (_currentVelocity * Time.deltaTime);
        }
    }
}