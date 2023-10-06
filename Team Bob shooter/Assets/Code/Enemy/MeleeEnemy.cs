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
        public int CurrentMapArea = 0;

        public bool pathUpdating = false;

        public Transform player;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask wallMask;

        [SerializeField]
        private float attackRange = 5f;

        private EnemyLungeAttack enemyLungeAttack;

        private bool attacking = false;

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

        private Mover mover;

        private MapAreaManager mapAreaManager;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody>();
            dropSpawner = GetComponent<DropSpawner>();
            mover = GetComponent<Mover>();
            mover.Setup(speed);

            mapAreaManager = GameInstance.Instance.GetMapAreaManager();

            //seeker.StartPath(rb.position, player.position, OnPathComplete);
        }

        protected override void Awake()
        {
            base.Awake();

            enemyGibbingPool = new ComponentPool<EnemyGibbing>(gibPrefab, 2);
            enemyLungeAttack = GetComponent<EnemyLungeAttack>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (enemyLungeAttack != null)
            {
                enemyLungeAttack.AttackEnd += OnAttackEnd;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            CancelInvoke("UpdatePath");
            pathUpdating = false;

            if (unitHealth != null)
            {
                unitHealth.OnDied -= OnDie;
            }

            if (activeGibbing != null)
            {
                activeGibbing.Completed -= ReturnGibToPool;
                activeGibbing = null;
            }

            if (enemyLungeAttack != null)
            {
                enemyLungeAttack.AttackEnd -= OnAttackEnd;
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

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;

            if (!pathUpdating && mapAreaManager.PlayerInArea(CurrentMapArea))
            {
                InvokeRepeating("UpdatePath", 0f, 0.5f);
                pathUpdating = true;
            }
            else if (pathUpdating && !mapAreaManager.PlayerInArea(CurrentMapArea))
            {
                CancelInvoke("UpdatePath");
                pathUpdating = false;
            }

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
                if (currentDistance < attackRange && !attacking)
                {
                    Attack();
                }
                else if (!attacking)
                {
                    Move();
                }
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
            {
                return;
            }

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
            if (currentDistance > 3f && currentWaypoint < path.vectorPath.Count)
            {
                Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;
                direction = new Vector3(direction.x, 0, direction.z);
                mover.Move(mover.GetSlopeDirection(direction));
                //Vector3 force = direction * speed * Time.deltaTime;
                //rb.AddForce(force, ForceMode.VelocityChange);
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
            float distance = (player.transform.position - transform.position).magnitude;

            if (currentDistance < radius)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distance, wallMask))
                {
                    //Debug.Log("Player has been detected!");

                    FieldOfView();

                    Vector3 toPlayer = player.transform.position - transform.position;
                    toPlayer = new Vector3(toPlayer.x, 0, toPlayer.z);

                    Quaternion lookOnLook =
                    Quaternion.LookRotation(toPlayer);

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
            float distance = (player.transform.position - transform.position).magnitude;
            float kulma = Mathf.Abs(Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up));

            if (kulma < angle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distance, wallMask))
                {
                    canSee = true;
                    timer = 0;

                    //Attack();
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
            if (enemyLungeAttack.Lunge())
            {
                mover.enabled = false;
                attacking = true;

                //if (isInCooldown == false)
                //{
                //    StartCoroutine(Wait());
                //    Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
                //    if (Mathf.Abs(Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up)) < angle / 2)
                //    {
                //        //hit.GetComponent<Health>().TakeDamage(1, false);
                //        //Debug.Log("Player hit!");
                //        StartCoroutine(Cooldown());
                //    }
                //}
            }
        }

        private void OnAttackEnd()
        {
            mover.enabled = true;
            attacking = false;
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
