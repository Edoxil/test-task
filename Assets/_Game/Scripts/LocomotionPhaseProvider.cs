using UnityEngine;

namespace Game
{
    public class LocomotionPhaseProvider : MonoBehaviour
    {
        [SerializeField] private int _walkStepsAcrossScreen = 12;
        [SerializeField] private int _runStepsAcrossScreen = 8;

        [SerializeField] private float _gizmoHeight = 1f;
        [SerializeField] private float _gizmoRadius = 0.06f;

        private Camera _camera;

        private float _leftEdge;
        private float _rightEdge;

        private float _walkStepLength;
        private float _runStepLength;

        private void Awake()
        {
            _camera = Camera.main;
            Recalculate();
        }

        private void Recalculate()
        {
            if (_camera == null)
                _camera = Camera.main;

            if (_camera == null)
                return;

            _leftEdge = _camera.ViewportToWorldPoint(new Vector3(0f, 0f)).x;
            _rightEdge = _camera.ViewportToWorldPoint(new Vector3(1f, 0f)).x;

            float screenWidth = _rightEdge - _leftEdge;

            _walkStepLength = screenWidth / _walkStepsAcrossScreen;
            _runStepLength = screenWidth / _runStepsAcrossScreen;
        }

        public float GetWalkPhase(float worldPositionX)
        {
            return Mathf.Repeat((worldPositionX - _leftEdge) / _walkStepLength, 1f);
        }

        public float GetRunPhase(float worldPositionX)
        {
            return Mathf.Repeat((worldPositionX - _leftEdge) / _runStepLength, 1f);
        }

        private void OnDrawGizmos()
        {
            DrawWalkSteps();
            DrawRunSteps();
        }

        private void DrawWalkSteps()
        {
            Gizmos.color = Color.green;

            float y = transform.position.y;

            for (int i = 0; i <= _walkStepsAcrossScreen; i++)
            {
                float x =
                    _leftEdge +
                    i * _walkStepLength;

                Vector3 position = new(x, y, 0f);

                Gizmos.DrawSphere(
                    position,
                    _gizmoRadius);

                Gizmos.DrawLine(
                    position + Vector3.down * 0.25f,
                    position + Vector3.up * 0.25f);
            }
        }

        private void DrawRunSteps()
        {
            Gizmos.color = Color.cyan;

            float y = transform.position.y + 0.35f;

            for (int i = 0; i <= _runStepsAcrossScreen; i++)
            {
                float x =
                    _leftEdge +
                    i * _runStepLength;

                Vector3 position = new(x, y, 0f);

                Gizmos.DrawSphere(
                    position,
                    _gizmoRadius);

                Gizmos.DrawLine(
                    position + Vector3.down * 0.2f,
                    position + Vector3.up * 0.2f);
            }
        }

    }
}