using Pathfinding;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace TeamBobFPS
{
    public class MeleeEnemy : BaseFixedUpdateListener
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

        private UnitHealth unitHealth;

        [SerializeField]
        private EnemyGibbing gibPrefab;

        private ComponentPool<EnemyGibbing> enemyGibbingPool;

        private EnemyGibbing activeGibbing = null;

        [SerializeField]
        private WaveData.EnemyType enemyType;

        public static event Action<WaveData.EnemyType, Transform> OnDefeated;

        private DropSpawner dropSpawner;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody>();
            dropSpawner = GetComponent<DropSpawner>();

            //seeker.StartPath(rb.position, player.position, OnPathComplete);
        }

        protected override void Awake()
        {
            base.Awake();

            enemyGibbingPool = new ComponentPool<EnemyGibbing>(gibPrefab, 2);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (unitHealth != null)
            {
                unitHealth.OnDied -= OnDie;
            }

            if (activeGibbing != null)
            {
                activeGibbing.Completed -= ReturnGibToPool;
                activeGibbing = null;
            }
        }

        public void Initialize()
        {
            player = FindObjectOfType<PlayerUnit>().transform;

            unitHealth = GetComponent<UnitHealth>();
            unitHealth.AddHealth(unitHealth.MaxHealth);
            unitHealth.OnDied += OnDie;
        }

        private void OnDie()
        {
            dropSpawner.SpawnThings();
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            OnDefeated?.Invoke(enemyType, transform);

            activeGibbing = enemyGibbingPool.Get();
            if (activeGibbing == null) return;
            activeGibbing.Completed += ReturnGibToPool;
            activeGibbing.transform.position = pos;
            activeGibbing.transform.rotation = rot;
            activeGibbing.Activate();
        }

        private void ReturnGibToPool(EnemyGibbing item)
        {
            activeGibbing = null;
            item.Completed -= ReturnGibToPool;
            enemyGibbingPool.Return(item);
        }

        void UpdatePath()
        {
            if (seeker.IsDone() && canSee)
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

            if (noticed && timer < 10)
            {
                Move();
            }
            //if(!noticed && timer >= 10)
            //{
            //    Move();
            //}

            LookForPlayer();

            if (noticed && !canSee)
            {
                Search();
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

            float speedChange = speed;
            if (currentDistance > 3f)
            {
                Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector3 force = direction * speed * Time.deltaTime;
                rb.AddForce(force, ForceMode.VelocityChange);
            }
            else if (currentDistance <= 3f)
            {
                speedChange = speed * 0;
            }

            //rb.MovePosition(path.vectorPath[currentWaypoint] * speed * Time.deltaTime);
        }

        private void LookForPlayer()
        {
            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;

            if (currentDistance < radius)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, radius, wallMask))
                {
                    //Debug.Log("Player has been detected!");

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

                    Attack();
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
        public void Attack()
        {
            InvokeRepeating("UpdatePath", 0f, .5f);

            if (canSee && currentDistance < 5)
            {
                if (isInCooldown == false)
                {
                    StartCoroutine(Wait());
                    Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
                    if (Mathf.Abs(Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up)) < angle / 2)
                    {
                        //hit.GetComponent<Health>().TakeDamage(1, false);
                        //Debug.Log("Player hit!");
                        StartCoroutine(Cooldown());
                    }
                }
            }
        }

        private void Search()
        {

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
