using Spine;
using Spine.Unity;
using UnityEngine;

namespace Game
{
    public class CharacterAnimator : MonoBehaviour
    {
        private const int LocomotionTrack = 0;
        private const int AimTrack = 1;

        [SerializeField] private float _characterHeight = 6f;

        [SerializeField] private SkeletonAnimation _skeletonAnimation;
        [SerializeField] private LocomotionPhaseProvider _phaseProvider;

        [SpineAnimation, SerializeField] private string _idle;
        [SpineAnimation, SerializeField] private string _walk;
        [SpineAnimation, SerializeField] private string _run;
        [SpineAnimation, SerializeField] private string _aim;

        [SerializeField] private float _idleThreshold = 0.05f;
        [SerializeField] private float _runThreshold = 0.75f;

        [SpineBone(dataField: "skeletonAnimation"), SerializeField]
        private string _IKBoneName;
        [SerializeField] private float _bentArmDistance = 2.5f;

        [SpineBone(dataField: "skeletonAnimation"), SerializeField]
        private string _aimTorsoBoneName;
        [SerializeField] private float _maxTorsoUpAngle = 70f;
        [SerializeField] private float _maxTorsoDownAngle = 40f;

        private Bone _IKBone;
        private Bone _torsoAimBone;

        private Spine.AnimationState _animationState;

        private TrackEntry _currentTrack;
        private LocomotionState _currentState;

        private TrackEntry _aimTrack;
        public bool _isAiming;


        private void Awake()
        {
            _animationState = _skeletonAnimation.AnimationState;

            _IKBone = _skeletonAnimation.Skeleton.FindBone(_IKBoneName);
            _torsoAimBone = _skeletonAnimation.Skeleton.FindBone(_aimTorsoBoneName);

            ChangeState(LocomotionState.Idle);
        }

        public void UpdateLocomotion(float normalizedSpeed)
        {
            LocomotionState targetState = GetLocomotionState(normalizedSpeed);

            if (targetState != _currentState)
                ChangeState(targetState);

            if (_currentState != LocomotionState.Idle)
                ApplyCurrentPhase();
        }

        public void UpdateAim(bool aiming, Vector3 mouseWorldPos)
        {
            if (aiming)
            {
                UpdateIkBonePosition(mouseWorldPos);
                UpdateTorsoRotation(mouseWorldPos);
            }

            if (_isAiming == aiming)
                return;

            _isAiming = aiming;

            if (_isAiming)
                PlayAim();
            else
                StopAim();
        }

        private void UpdateTorsoRotation(Vector3 mouseWorldPos)
        {
            if (_torsoAimBone == null)
                return;

            Vector2 aimDirection = mouseWorldPos - CalculateCenterPoint();

            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -_maxTorsoDownAngle, _maxTorsoUpAngle);

            _torsoAimBone.Rotation = _torsoAimBone.Data.Rotation + angle;
        }

        private void UpdateIkBonePosition(Vector3 mouseWorldPos)
        {
            Vector2 mouseSkeletonSpacePoint = _skeletonAnimation.transform.InverseTransformPoint(mouseWorldPos);

            float heightDelta = mouseWorldPos.y - transform.position.y;

            if (heightDelta > _characterHeight * 2f)
            {
                _IKBone.SetLocalPosition(mouseSkeletonSpacePoint);
            }
            else
            {
                Vector3 centerPoint = CalculateCenterPoint();

                Vector2 aimDir = (mouseWorldPos - centerPoint).normalized;
                Vector2 targetPoint = (Vector2)centerPoint + aimDir * _bentArmDistance;

                Vector2 targetPos = _skeletonAnimation.transform.InverseTransformPoint(targetPoint);

                Debug.DrawLine(centerPoint, targetPoint);

                _IKBone.SetLocalPosition(targetPos);
            }
        }

        private Vector3 CalculateCenterPoint()
        {
            return transform.position + 0.5f * _characterHeight * Vector3.up;
        }

        private void PlayAim()
        {
            _aimTrack = _animationState.SetAnimation(AimTrack, _aim, true);
        }

        private void StopAim()
        {
            _animationState.SetEmptyAnimation(AimTrack, 0.1f);
        }

        private LocomotionState GetLocomotionState(float normalizedSpeed)
        {
            if (normalizedSpeed <= _idleThreshold)
                return LocomotionState.Idle;

            if (normalizedSpeed >= _runThreshold)
                return LocomotionState.Run;

            return LocomotionState.Walk;
        }

        private void ChangeState(LocomotionState state)
        {
            _currentState = state;
            
            string animationName = state switch
            {
                LocomotionState.Idle => _idle,
                LocomotionState.Walk => _walk,
                LocomotionState.Run => _run,
                _ => _idle
            };

            _currentTrack = _animationState.SetAnimation(LocomotionTrack, animationName, true);
        }

        private void ApplyCurrentPhase()
        {
            if (_currentTrack == null)
                return;

            float phase = _currentState switch
            {
                LocomotionState.Walk => _phaseProvider.GetWalkPhase(transform.position.x),
                LocomotionState.Run => _phaseProvider.GetRunPhase(transform.position.x),
                _ => 0f
            };

            _currentTrack.TrackTime = phase * _currentTrack.Animation.Duration;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Vector3 pos = transform.position + Vector3.up * _characterHeight;
            Gizmos.DrawSphere(pos, 0.25f);
        }

        private enum LocomotionState
        {
            Idle,
            Walk,
            Run
        }

    }
}