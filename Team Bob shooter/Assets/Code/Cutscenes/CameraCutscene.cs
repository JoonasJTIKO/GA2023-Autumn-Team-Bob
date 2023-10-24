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
            public Vector3 TargetDirection;

            public AnimationCurve AnimationCurve;

            public float LookTime;

            public float LookSpeed;
        }

        [SerializeField]
        private CameraTarget[] cameraTargets;

        private bool playCutscene = false;

        private int index = 0;

        private CameraTarget currentTarget;

        private Coroutine cameraRoutine;

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
            cameraRoutine = StartCoroutine(MoveCamera());
        }

        private IEnumerator MoveCamera()
        {
            currentTarget = cameraTargets[index];
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
                        timer = 0;
                    }
                }

                float angle = currentTarget.AnimationCurve.Evaluate(timer) * currentTarget.LookSpeed * Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                Vector3 newDirection = Vector3.RotateTowards(playerUnit.PlayerCam.transform.forward, currentTarget.TargetDirection, angle, 0f);
                playerUnit.PlayerCam.transform.rotation = Quaternion.Lerp(playerUnit.PlayerCam.transform.rotation, Quaternion.LookRotation(newDirection), timer * currentTarget.LookSpeed);

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            playerUnit.LockControls(false, false, false);
        }
    }
}
