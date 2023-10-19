using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private int gibCount = 3;

        private Vector3[] bodyPieceDefaultPositions;

        private RagdollBehavior ragdollBehavior;

        public event Action<EnemyGibbing> Completed;

        private void Awake()
        {
            ragdollBehavior = GetComponent<RagdollBehavior>();
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
        }

        public void Activate(DeathType deathType = DeathType.Normal)
        {
            ResetPositions();
            SaveInitialPositions();

            ragdollBehavior.EnableRagdoll();

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

            StartCoroutine(Dissapear());
        }

        private IEnumerator Dissapear()
        {
            yield return new WaitForSeconds(2);

            foreach (Transform piece in bodyPieces)
            {
                piece.gameObject.SetActive(false);
            }
            Completed?.Invoke(this);
        }
    }
}
