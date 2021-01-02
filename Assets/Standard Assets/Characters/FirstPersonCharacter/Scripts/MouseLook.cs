using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;

        [Range(0, 4)] public int AimGrade;
        [Range(0.1f, 1f)] public float SlowSpeed = 1.0f;

        [SerializeField] private float AimDistance = 20.0f;        
        [SerializeField] private GameObject m_aim;
        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;
        private bool _isLockedOn = false;
        private Transform _target;
        private GameObject[] _aimableObject;
        private int _indexAimableObject = 0;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera)
        {

            if (_isLockedOn)
            {
                if(CrossPlatformInputManager.GetButtonUp("Fire2"))
                {
                    _isLockedOn = false;
                    return;
                }

                if (AimGrade == 3)
                {
                    if (CrossPlatformInputManager.GetButtonUp("Fire1"))
                    {
                        _indexAimableObject++;
                        if (_indexAimableObject >= _aimableObject.Length)
                            _indexAimableObject = 0;
                        _target = _aimableObject[_indexAimableObject].transform;
                    }

                        if (Vector3.Distance(_target.position, character.position) > AimDistance)
                    {
                        _indexAimableObject++;
                        if (_indexAimableObject >= _aimableObject.Length)
                            _indexAimableObject = 0;
                        _target = _aimableObject[_indexAimableObject].transform;
                    }
                }

                if (AimDistance < Vector3.Distance(character.position, _target.position))
                {
                    _isLockedOn = false;

                //    m_CharacterTargetRot = character.rotation;
                  //  m_CameraTargetRot = camera.rotation;
                }
                character.transform.LookAt(_target);
                camera.transform.LookAt(_target);
                return;
            }


            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            float rotationSpeed = 1.0f;

            if (Mathf.Abs(yRot) < 0.1f)
            {
                if (Mathf.Abs(xRot) < 0.1f)
                {
                    yRot = 0.0f;
                    xRot = 0.0f;
                }
            }

            switch (AimGrade)
            { 
                case 0:
                    break;
                case 1:
                    RaycastHit hit;
                        if(Physics.Raycast(character.transform.position, character.transform.forward, out hit, AimDistance))
                    {
                        if (hit.collider.tag == "Aimable")
                            rotationSpeed = SlowSpeed;
                    }
                    break;
                case 2:
                    if (!_isLockedOn && CrossPlatformInputManager.GetButton("Fire2"))
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(character.transform.position, character.transform.forward, out hit2, AimDistance))
                        {
                            if (hit2.collider.tag == "Aimable")

                            {
                                _isLockedOn = true;
                                _target = hit2.collider.gameObject.transform;
                            }
                        }
                    }
                    break;
                case 3:
                    if (!_isLockedOn && CrossPlatformInputManager.GetButton("Fire2"))
                    {
                        _aimableObject = GameObject.FindGameObjectsWithTag("Aimable");
                        _isLockedOn = true;
                        _target = _aimableObject[0].transform;

                    }
                        break;
                default:
                    break;
            }

            m_CharacterTargetRot *= Quaternion.Euler (0f, yRot * rotationSpeed, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot * rotationSpeed, 0f, 0f);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

           // if(smooth)
           // {
           //     character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
           //         smoothTime * Time.deltaTime);
           //     camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
           //         smoothTime * Time.deltaTime);
           //}
           // else
           //{
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
       //    }

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
        
        public void Adujst_Target(bool isLocked, Transform target)
        {
            _isLockedOn = isLocked;
            _target = target;
        }
    }
}
