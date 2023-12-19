using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TeamBobFPS.CameraCutscene;

namespace TeamBobFPS
{
    public class FlyingCameraCutscene : CutsceneBase
    {
        [System.Serializable]
        public class CameraMovement
        {
            public bool flatGoToTarget = false;
            public Vector3 TargetPosition;
            public Transform LookAtTarget;
            public AnimationCurve AnimationCurve;
            public float Time;
            public float Speed;
        }

        [SerializeField]
        private CameraMovement[] cameraMovements;

        private CameraMovement currentMovement;

        private Coroutine cutsceneRoutine;

        private Vector3 initialPosition;

        private Quaternion initialRotation;

        [SerializeField]
        private float returnTime;

        [SerializeField]
        private Camera cutsceneCamera;

        public override void StopCutscene()
        {
            if (cutsceneRoutine != null)
            {
                StopCoroutine(cutsceneRoutine);
            }
        }

        public override void StartCutscene()
        {
            cutsceneCamera.enabled = true;

            cutsceneCamera.transform.position = playerUnit.PlayerCam.transform.position;
            initialPosition = cutsceneCamera.transform.position;
            cutsceneCamera.transform.rotation = playerUnit.PlayerCam.transform.rotation;
            initialRotation = cutsceneCamera.transform.rotation;
            cutsceneCamera.gameObject.tag = "MainCamera";
            playerUnit.PlayerCam.gameObject.tag = "Untagged";
            playerUnit.PlayerCam.enabled = false;
            playerUnit.LockControls(true, true, true);
            cutsceneRoutine = StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            EnemyAggroState.aggro = false;

            float timer = 0;
            int index = 0;
            currentMovement = cameraMovements[index];
            Vector3 startPos = cutsceneCamera.transform.position;
            Vector3 startLookDir = cutsceneCamera.transform.forward;

            while (index < cameraMovements.Length)
            {
                if (currentMovement.LookAtTarget != null)
                {
                    if (currentMovement.flatGoToTarget)
                    {
                        Vector3 lookDir = Vector3.Lerp(startLookDir, (currentMovement.LookAtTarget.position - currentMovement.TargetPosition).normalized, timer / currentMovement.Time);

                        cutsceneCamera.transform.rotation = Quaternion.LookRotation(lookDir);
                    }
                    else
                    {
                        cutsceneCamera.transform.LookAt(currentMovement.LookAtTarget);
                    }
                }

                if (timer >= currentMovement.Time || Vector3.Distance(cutsceneCamera.transform.position, currentMovement.TargetPosition) < 0.01f)
                {
                    index++;
                    if (!(index >= cameraMovements.Length))
                    {
                        currentMovement = cameraMovements[index];
                        startPos = cutsceneCamera.transform.position;
                        timer = 0;
                    }
                }
                if (!currentMovement.flatGoToTarget)
                {
                    cutsceneCamera.transform.position = Vector3.Lerp(cutsceneCamera.transform.position, currentMovement.TargetPosition, Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale * currentMovement.Speed * currentMovement.AnimationCurve.Evaluate(timer / currentMovement.Time));
                }
                else
                {
                    cutsceneCamera.transform.position = Vector3.Lerp(startPos, currentMovement.TargetPosition, timer / currentMovement.Time);
                }

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            cutsceneRoutine = StartCoroutine(ReturnToInitial());
        }

        private IEnumerator ReturnToInitial()
        {
            float timer = 0;
            Vector3 startPos = cutsceneCamera.transform.position;
            Quaternion startRot = cutsceneCamera.transform.rotation;

            while (timer < returnTime)
            {
                cutsceneCamera.transform.position = Vector3.Lerp(startPos, initialPosition, timer / returnTime);
                cutsceneCamera.transform.rotation = Quaternion.Lerp(startRot, initialRotation, timer / returnTime);

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            cutsceneCamera.gameObject.tag = "Untagged";
            cutsceneCamera.enabled = false;
            playerUnit.PlayerCam.enabled = true;
            playerUnit.PlayerCam.gameObject.tag = "MainCamera";
            playerUnit.LockControls(false, false, false);

            EnemyAggroState.aggro = true;
        }
    }
}
