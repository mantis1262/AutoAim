using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class HeadBob : MonoBehaviour
    {
        public Camera Camera;
        public CurveControlledBob MotionBob = new CurveControlledBob();
        public LerpControlledBob JumpAndLandingBob = new LerpControlledBob();
        public RigidbodyFirstPersonController RigidbodyFirstPersonController;
        public float StrideInterval;
        [Range(0f, 1f)] public float RunningStrideLengthen;

        private CameraRefocus _cameraRefocus;
        private bool _previouslyGrounded;
        private Vector3 _originalCameraPosition;


        private void Start()
        {
            MotionBob.Setup(Camera, StrideInterval);
            _originalCameraPosition = Camera.transform.localPosition;
            _cameraRefocus = new CameraRefocus(Camera, transform.root.transform, Camera.transform.localPosition);
        }


        private void Update()
        {
            _cameraRefocus.GetFocusPoint();
            Vector3 newCameraPosition;
            if (RigidbodyFirstPersonController.Velocity.magnitude > 0 && RigidbodyFirstPersonController.Grounded)
            {
                Camera.transform.localPosition = MotionBob.DoHeadBob(RigidbodyFirstPersonController.Velocity.magnitude*(RigidbodyFirstPersonController.Running ? RunningStrideLengthen : 1f));
                newCameraPosition = Camera.transform.localPosition;
                newCameraPosition.y = Camera.transform.localPosition.y - JumpAndLandingBob.Offset();
            }
            else
            {
                newCameraPosition = Camera.transform.localPosition;
                newCameraPosition.y = _originalCameraPosition.y - JumpAndLandingBob.Offset();
            }
            Camera.transform.localPosition = newCameraPosition;

            if (!_previouslyGrounded && RigidbodyFirstPersonController.Grounded)
            {
                StartCoroutine(JumpAndLandingBob.DoBobCycle());
            }

            _previouslyGrounded = RigidbodyFirstPersonController.Grounded;
          //  m_CameraRefocus.SetFocusPoint();
        }
    }
}
