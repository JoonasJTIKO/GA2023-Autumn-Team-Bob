using Pathfinding;
using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace TeamBobFPS
{
    public class RangeEnemy : BaseFixedUpdateListener
    {
        public Transform player;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask wallMask;

        public float radius;
        [Range(0, 360)]
        public float angle;
        public bool canSee;
        [SerializeField] private float currentDistance;

        private bool isInCooldown = false;
        [SerializeField] private float shootTimer = 1;

        Path path;
        Seeker seeker;
        Rigidbody rb;
        private int currentWaypoint = 0;
        private float nextWaypointDistance = 3f;
        private bool reachedEndOfPath = false;
        [SerializeField] private int speed = 25;

        public float timer;
        public bool noticed = false;

        public Transform projectilePos;

        public bool posChange = false;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody>();

            //seeker.StartPath(rb.position, player.position, OnPathComplete);
        }

        void UpdatePath()
        {
            if (seeker.IsDone())
            {
                seeker.StartPath(rb.position, player.position, OnPathComplete);
            }
            else if (!canSee)
            {
                return;
            }
        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        void Noticed()
        {
            if (noticed == false)
            {
                noticed = true;
                timer = 0;
                radius = radius * 5;
                angle = 360;

                if (!posChange)
                {
                    if (Random.value > 0.4)
                    {
                        ChangePosition();
                    }
                    if (Random.value > 0.6)
                    {
                        Shoot();
                    }
                }
            }
        }

        void FixedUpdate()
        {
            timer += Time.deltaTime;

            currentDistance = Vector3.Distance(player.transform.position, transform.position);
            //Debug.Log(currentDistance);  

            if (currentDistance < radius && canSee)
            {
                Noticed();
            }
            else
            {
                canSee = false;
            }

            if (timer >= 10 && noticed)
            {
                radius = radius / 5;
                noticed = false;
                angle = 90;
            }
            //if (!noticed && timer >= 10)
            //{
            //    Move();
            //}

            if (posChange)
            {
                Move();
                Shoot();
                if (reachedEndOfPath)
                {
                    posChange = false;
                }
            }

            if (currentDistance > 15f && noticed)
            {
                InvokeRepeating("UpdatePath", 0f, .5f);
                Move();
            }

            LookForPlayer();
        }

        private void ChangePosition()
        {
            if (!posChange)
            {
                float posRange;
                posRange = radius / 3.5f;
                var point = Random.insideUnitSphere * posRange;
                point.y = 0;
                point += transform.position;
                seeker.StartPath(rb.position, point, OnPathComplete);
                
                posChange = true;
            }
            else
            {
                return;
            }
        }
    
        private void Move()
        {
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            float distance = Vector3.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            //float speedChange = speed;
            //if (currentDistance > 5f && noticed)
            //{
                Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector3 force = direction * speed * Time.deltaTime;
                rb.AddForce(force, ForceMode.VelocityChange);
            //}
            //else if (currentDistance <= 5f && noticed)
            //{
            //    speedChange = speed * 0;
            //}

            //rb.MovePosition(path.vectorPath[currentWaypoint] * speed * Time.deltaTime);
        }

        private void LookForPlayer()
        {
            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;

            if (currentDistance < radius)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, radius, wallMask))
                {
                    FieldOfView();

                    Quaternion lookOnLook =
                    Quaternion.LookRotation(player.transform.position - transform.position);

                    transform.rotation =
                    Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 5f);
                }
                else
                {
                    canSee = false;
                }
            }
            else
            {
                canSee = false;
            }
        }

        private void FieldOfView()
        {
            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
            float kulma = Mathf.Abs(Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up));

            if (kulma < angle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, radius, wallMask))
                {
                    canSee = true;
                    timer = 0;

                    Shoot();
                }
                else
                {
                    canSee = false;
                }
                //noticed = true;
                //seeker.StartPath(rb.position, player.position, OnPathComplete);
            }
            else
            {
                canSee = false;
            }
        }
        public void Shoot()
        {
            if (canSee)
            {
                if (isInCooldown == false)
                {
                    Wait();
                    GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
                    if (bullet != null)
                    {
                        bullet.transform.position = projectilePos.transform.position;
                        bullet.transform.rotation = projectilePos.transform.rotation;
                        bullet.SetActive(true);
                    }
                    StartCoroutine(Cooldown());
                }
            }
        }
        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.75f);
        }

        private IEnumerator Cooldown()
        {
            isInCooldown = true;
            yield return new WaitForSeconds(shootTimer);
            isInCooldown = false;
        }
    }
}
