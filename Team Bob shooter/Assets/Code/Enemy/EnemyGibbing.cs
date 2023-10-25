using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamBobFPS
{
    public class EnemyGibbing : MonoBehaviour
    {
        public enum DeathType
        {
            Normal = 0,
            Head = 1,
            LeftArm = 2,
            RightArm = 3,
            LeftLeg = 4,
            RightLeg = 5,
            Explode = 6,
        }

        [SerializeField]
        private Transform[] bodyPieces;

        [SerializeField]
        private GameObject[] ragdollPieces;

        [SerializeField]
        private float gibStrength = 10;

        [SerializeField]
        private float lifeTime = 5f;

        [SerializeField]
        private LayerMask layerMask;

        private Vector3[] bodyPieceDefaultPositions;

        private RagdollBehavior ragdollBehavior;

        private DecalPaint decalPaint;

        public event Action<EnemyGibbing> Completed;

        private void Awake()
        {
            ragdollBehavior = GetComponent<RagdollBehavior>();
            decalPaint = GetComponent<DecalPaint>();
        }

        private void SaveInitialPositions()
        {
            bodyPieceDefaultPositions = new Vector3[bodyPieces.Length];
            int index = 0;
            foreach (Transform piece in bodyPieces)
            {
                bodyPieceDefaultPositions[index] = piece.localPosition;
                index++;
            }
        }

        private void ResetPositions()
        {
            if (bodyPieceDefaultPositions == null) return;

            for (int i = 0; i < bodyPieceDefaultPositions.Length; i++)
            {
                bodyPieces[i].localPosition = bodyPieceDefaultPositions[i];
                bodyPieces[i].gameObject.SetActive(false);
                Rigidbody rigidbody = bodyPieces[i].gameObject.GetComponent<Rigidbody>();
                rigidbody.useGravity = false;
            }

            foreach (var piece in ragdollPieces)
            {
                piece.SetActive(true);
            }
        }

        public void Activate(DeathType deathType = DeathType.Normal)
        {
            SaveInitialPositions();

            ragdollBehavior.EnableRagdoll();
            ragdollBehavior.PushRagdoll(transform.forward * 10);

            switch (deathType)
            {
                case DeathType.Normal:
                    break;
                case DeathType.Head:
                    ragdollPieces[0].SetActive(false);
                    ragdollPieces[1].SetActive(false);
                    ragdollPieces[2].SetActive(false);
                    ragdollPieces[3].SetActive(false);

                    bodyPieces[0].gameObject.SetActive(true);
                    break;
                case DeathType.LeftArm:
                    ragdollPieces[4].SetActive(false);
                    ragdollPieces[5].SetActive(false);
                    ragdollPieces[6].SetActive(false);

                    bodyPieces[1].gameObject.SetActive(true);
                    break;
                case DeathType.RightArm:
                    ragdollPieces[7].SetActive(false);
                    ragdollPieces[8].SetActive(false);

                    bodyPieces[2].gameObject.SetActive(true);
                    break;
                case DeathType.LeftLeg:
                    ragdollPieces[9].SetActive(false);
                    ragdollPieces[10].SetActive(false);

                    bodyPieces[3].gameObject.SetActive(true);
                    break;
                case DeathType.RightLeg:
                    ragdollPieces[11].SetActive(false);
                    ragdollPieces[12].SetActive(false);

                    bodyPieces[4].gameObject.SetActive(true);
                    break;
                case DeathType.Explode:
                    foreach (var piece in ragdollPieces)
                    {
                        piece.SetActive(false);
                    }

                    foreach (var piece in bodyPieces)
                    {
                        piece.gameObject.SetActive(true);
                    }
                    break;
            }

            foreach (var bodyPart in bodyPieces)
            {
                if (!bodyPart.gameObject.activeInHierarchy) continue;

                Rigidbody rigidbody = bodyPart.gameObject.GetComponent<Rigidbody>();
                rigidbody.useGravity = true;
                Vector3 angle = Vector3.up;
                angle = new Vector3(angle.x + UnityEngine.Random.Range(-0.5f, 0.5f), 
                    angle.y + UnityEngine.Random.Range(-0.5f, 0.5f), 
                    angle.z + UnityEngine.Random.Range(-0.5f, 0.5f));
                rigidbody.AddForce(angle * gibStrength, ForceMode.Impulse);
                rigidbody.AddTorque(angle * 0.1f, ForceMode.Impulse);
            }

            StartCoroutine(SplatterRoutine());
            StartCoroutine(Dissapear());
        }

        private IEnumerator SplatterRoutine()
        {
            int splatterCount = 0;
            while (splatterCount < 3)
            {
                float randomTime = Random.Range(0.1f, 0.3f);
                while (randomTime > 0)
                {
                    randomTime -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                    yield return null;
                }
                bool success = false;

                while (!success)
                {
                    Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, direction, out hit, 10, layerMask))
                    {
                        decalPaint.ApplyDecal(hit.point, hit.normal);
                        success = true;
                    }
                    yield return null;
                }
                splatterCount++;

                yield return null;
            }
        }

        private IEnumerator Dissapear()
        {
            float timer = lifeTime;

            while (timer > 0)
            {
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            ResetPositions();

            foreach (Transform piece in bodyPieces)
            {
                piece.gameObject.SetActive(false);
            }

            ragdollBehavior.DisableRagdoll();

            Completed?.Invoke(this);
        }
    }
}
