using System;
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
        [Range(1f, 2f)] public float FastSpeed = 1.0f; 


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera)
        {
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

            Debug.Log("przed switchem");
            Debug.Log(AimGrade);


            switch (AimGrade)
            { 
                case 0:
                    break;
                case 1:
                    Collider collider = character.GetComponent<Collider>();
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // zmieni� na pozycie celownika
                    RaycastHit hit;
                    Debug.Log("1 TRYB");
                        if(collider.Raycast(ray, out hit, 50.0f))
                    {
                        Debug.Log("Trafiony");
                        Debug.Log(hit.collider.tag);
                        if (hit.collider.tag == "Aimable")
                            rotationSpeed = SlowSpeed;
                    }
                    break;
                case 2:

                    break;
                case 3:

                    break;
                default:
                    break;
            }

            m_CharacterTargetRot *= Quaternion.Euler (0f, yRot * rotationSpeed, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot * rotationSpeed, 0f, 0f);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
           }
            else
           {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
           }

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

    }
}
