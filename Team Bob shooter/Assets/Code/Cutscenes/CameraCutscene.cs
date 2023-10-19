using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class CameraCutscene : BaseUpdateListener
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

        [SerializeField]
        private int cutsceneIndex = 0;

        private PlayerUnit playerUnit;

        private bool changingCamera = false;

        private bool playCutscene = false;

        private int index = 0;

        private CameraTarget currentTarget;

        private Coroutine cameraRoutine;

        protected override void Awake()
        {
            base.Awake();

            playerUnit = FindObjectOfType<PlayerUnit>();
        }

        private void Start()
        {
            StartCutscene(0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            WaveManager.OnLevelCleared += StartCutscene;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            WaveManager.OnLevelCleared -= StartCutscene;
            if (cameraRoutine != null )
            {
                StopCoroutine(cameraRoutine);
            }
        }

        public void StartCutscene(int cutsceneIndex)
        {
            if (cutsceneIndex == this.cutsceneIndex)
            {
                playerUnit.LockControls(true, true, true);
                playCutscene = true;
                cameraRoutine = StartCoroutine(MoveCamera());
            }
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
