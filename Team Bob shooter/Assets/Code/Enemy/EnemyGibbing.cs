using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemyGibbing : MonoBehaviour
    {
        [SerializeField]
        private Transform[] bodyPieces;

        [SerializeField]
        private float gibStrength = 10;

        [SerializeField]
        private int gibCount = 3;

        private Vector3[] bodyPieceDefaultPositions;

        public event Action<EnemyGibbing> Completed;

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
                Rigidbody rigidbody = bodyPieces[i].gameObject.GetComponent<Rigidbody>();
                rigidbody.useGravity = false;
            }
        }

        public void Activate()
        {
            ResetPositions();
            SaveInitialPositions();

            int[] piecesToActivate = new int[gibCount];
            for (int i = 0; i < gibCount; i++)
            {
                bool completed = false;
                while (!completed)
                {
                    int index = UnityEngine.Random.Range(0, bodyPieces.Length);
                    foreach (int item in piecesToActivate)
                    {
                        if (item == index)
                        {
                            completed = false;
                            break;
                        }
                        completed = true;
                    }
                    if (completed)
                    {
                        piecesToActivate[i] = index;
                    }
                }
            }

            foreach (int index in piecesToActivate)
            {
                bodyPieces[index].gameObject.SetActive(true);
                Rigidbody rigidbody = bodyPieces[index].gameObject.GetComponent<Rigidbody>();
                rigidbody.useGravity = true;
                Vector3 angle = (bodyPieces[index].position - transform.position).normalized;
                angle = new Vector3(angle.x + UnityEngine.Random.Range(-0.1f, 0.1f), 
                    angle.y + UnityEngine.Random.Range(-0.1f, 0.1f), 
                    angle.z + UnityEngine.Random.Range(-0.1f, 0.1f));
                rigidbody.AddForce(angle * gibStrength, ForceMode.Impulse);
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
