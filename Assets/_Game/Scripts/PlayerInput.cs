using UnityEngine;

namespace Game
{
    public class PlayerInput : MonoBehaviour
    {
        private Camera _camera;

        public Vector2 MouseWorldPosition { get; private set; }
        public bool AimEnabled { get; private set; }

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void ManualUpdate()
        {
            MouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            AimEnabled = Input.GetMouseButton(1);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(MouseWorldPosition, 0.25f);
        }

    }
}