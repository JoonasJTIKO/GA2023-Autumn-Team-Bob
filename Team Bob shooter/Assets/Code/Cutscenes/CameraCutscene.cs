using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class CameraCutscene : CutsceneBase
    {
        [System.Serializable]
        public class CameraTarget
        {
            public Transform lookTarget;

            public AnimationCurve AnimationCurve;

            public float LookTime;
        }

        [SerializeField]
        private CameraTarget[] cameraTargets;

        [SerializeField]
        private float returnTime = 1f;

        private bool playCutscene = false;

        private int index = 0;

        private CameraTarget currentTarget;

        private Coroutine cameraRoutine;

        private Quaternion initialRotation;

        public override void StopCutscene()
        {
            if (cameraRoutine == null)
            {
                StopCoroutine(cameraRoutine);
            }
        }

        public override void StartCutscene()
        {
            playerUnit.LockControls(true, true, true);
            playCutscene = true;
            initialRotation = playerUnit.PlayerCam.transform.rotation;
            cameraRoutine = StartCoroutine(MoveCamera());
        }

        private IEnumerator MoveCamera()
        {
            currentTarget = cameraTargets[index];
            Quaternion startRot = playerUnit.PlayerCam.transform.rotation;
            float timer = 0;

            while (playCutscene)
            {
                if (timer >= currentTarget.LookTime)
                {
                    index++;
                    if (index >= cameraTargets.Length)
                    {
                        playCutscene = false;
                    }
                    else
                    {
                        currentTarget = cameraTargets[index];
                        startRot = playerUnit.PlayerCam.transform.rotation;
                        timer = 0;
                    }
                }

                float angle = currentTarget.AnimationCurve.Evaluate(timer / currentTarget.LookTime);
                Vector3 newDirection = Vector3.RotateTowards(playerUnit.PlayerCam.transform.forward, (currentTarget.lookTarget.position - playerUnit.PlayerCam.transform.position).normalized, angle, 0f);
                playerUnit.PlayerCam.transform.rotation = Quaternion.Lerp(startRot, Quaternion.LookRotation(newDirection), timer / currentTarget.LookTime);

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            cameraRoutine = StartCoroutine(ReturnToInitial());
        }

        private IEnumerator ReturnToInitial()
        {
            float timer = 0;
            Quaternion startRot = playerUnit.PlayerCam.transform.rotation;

            while (timer < returnTime)
            {
                playerUnit.PlayerCam.transform.rotation = Quaternion.Lerp(startRot, initialRotation, timer / returnTime);

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            playerUnit.LockControls(false, false, false);
        }
    }
}
