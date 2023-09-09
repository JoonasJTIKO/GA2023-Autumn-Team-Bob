using Pathfinding;
using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace TeamBobFPS
{
    public class MeleeEnemy : BaseUpdateListener
    {
        public Transform player;
        [SerializeField] private LayerMask targetMask;

        public float radius;
        [Range(0, 360)]
        public float angle;
        public bool canSee;
        [SerializeField] private float currentDistance;

        private bool isInCooldown = false;
        [SerializeField] private float hitTimer = 1;

        Path path;
        Seeker seeker;
        Rigidbody rb;
        private int currentWaypoint = 0;
        private float nextWaypointDistance = 3f;
        bool reachedEndOfPath = false;
        [SerializeField] private int speed = 25;

        public float timer;
        public bool noticed = false;


        void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody>();

            //seeker.StartPath(rb.position, player.position, OnPathComplete);
        }

        void UpdatePath()
        {
            if(seeker.IsDone())
            {
                seeker.StartPath(rb.position, player.position, OnPathComplete);
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
            if(noticed == false)
            {
                noticed = true;
                timer = 0;
                radius = radius * 2;
            }
        }

        void FixedUpdate()
        {
            timer += Time.deltaTime;

            currentDistance = Vector3.Distance(player.transform.position, transform.position);
            //Debug.Log(currentDistance);  

            if(currentDistance < radius)
            {
                Noticed();
            }
            else
            {
                canSee = false;
            }

            if (timer >= 10 && noticed)
            {
                radius = radius / 2;
                noticed = false;
            }
            else if(noticed && timer < 10)
            {
                InvokeRepeating("UpdatePath", 0f, .5f);
                Move();

                Quaternion lookOnLook =
                Quaternion.LookRotation(player.transform.position - transform.position);

                transform.rotation =
                Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 5f);
            }
            if(!noticed && timer >= 10)
            {
                Move();
            }

            LookForPlayer();
        }

        private void Move()
        {
            if (path == null)
                return;

            if(currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath =true;
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

            float speedChange = speed;
            if(currentDistance >= 3f && noticed)
            {
                Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector3 force = direction * speed * Time.deltaTime;
                rb.AddForce(force, ForceMode.VelocityChange);
            }
            else if(currentDistance <= 3f && noticed)
            {
                speedChange = speed * 0;
            }

            //rb.MovePosition(path.vectorPath[currentWaypoint] * speed * Time.deltaTime);
        }

        private void LookForPlayer()
        {
            if (currentDistance < radius)
            {
                Debug.Log("Player has been detected!");

                Quaternion lookOnLook =
                Quaternion.LookRotation(player.transform.position - transform.position);

                transform.rotation =
                Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 5f);

                FieldOfView();
            }
        }

        private void FieldOfView()
        {
            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
            float kulma = Mathf.Abs(Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up));

            if (kulma < angle / 2)
            {
                canSee = true;
                timer = 0;
                Attack();
                //noticed = true;
                //seeker.StartPath(rb.position, player.position, OnPathComplete);
            }
            else
            {
                canSee = false;
            }
        }
        public void Attack()
        {
            if (canSee && currentDistance < 5)
            {
                if (isInCooldown == false)
                {
                    StartCoroutine(Wait());
                    Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
                    if (Mathf.Abs(Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up)) < angle / 2)
                    {
                        //hit.GetComponent<Health>().TakeDamage(1, false);
                        Debug.Log("Player hit!");
                        StartCoroutine(Cooldown());
                    }
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
            yield return new WaitForSeconds(hitTimer);
            isInCooldown = false;
        }
    }
}
