using Pathfinding;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static TeamBobFPS.WaveData;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

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

        private float currentDistance;

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

        private UnitHealth unitHealth;

        [SerializeField]
        private EnemyGibbing gibPrefab;

        private ComponentPool<EnemyGibbing> enemyGibbingPool;

        private EnemyGibbing activeGibbing = null;

        [SerializeField]
        private WaveData.EnemyType enemyType;

        public static event Action<WaveData.EnemyType, Transform> OnDefeated;

        private DropSpawner dropSpawner;

        private EnemyFireAtPlayer shootComponent;

        private Mover mover;

        private Vector3 changePosDirection = Vector3.zero;

        [SerializeField]
        private float fireRange = 10f;

        private enum ActionState
        {
            idle = 0,
            chase = 1,
            fire = 2
        }

        private ActionState currentState = ActionState.idle;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody>();
            dropSpawner = GetComponent<DropSpawner>();
            shootComponent = GetComponent<EnemyFireAtPlayer>();
            mover = GetComponent<Mover>();
            mover.Setup(speed);
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

                //if (!posChange)
                //{
                //    if (UnityEngine.Random.value > 0.4)
                //    {
                //        ChangePosition();
                //    }
                //    if (UnityEngine.Random.value > 0.6)
                //    {
                //        Shoot();
                //    }
                //}
            }
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            timer += fixedDeltaTime;

            currentDistance = Vector3.Distance(player.transform.position, transform.position);

            if (currentDistance < radius)
            {
                currentState = ActionState.chase;

                if (canSee)
                {
                    Noticed();
                }

                if (currentDistance < fireRange || posChange)
                {
                    currentState = ActionState.fire;
                }
            }
            else
            {
                currentState = ActionState.idle;
                canSee = false;
            }

            if (timer >= 10 && noticed)
            {
                radius = radius / 5;
                noticed = false;
                angle = 90;
            }

            switch (currentState)
            {
                case ActionState.chase:
                    UpdatePath();
                    Move();
                    break;
                case ActionState.fire:
                    TurnTowardsPlayer(fixedDeltaTime);
                    if (!Shoot())
                    {
                        if (changePosDirection == Vector3.zero)
                        {
                            changePosDirection = (transform.position - player.transform.position).normalized;
                            changePosDirection = new Vector3(changePosDirection.x, 0, changePosDirection.z);
                            changePosDirection = Quaternion.Euler(0, UnityEngine.Random.Range(-90f, 90f), 0) * changePosDirection;
                            StartCoroutine(ChangePositionRoutine(changePosDirection));
                        }
                    }
                    else
                    {
                        mover.Move(Vector3.zero);
                        changePosDirection = Vector3.zero;
                    }
                    break;
            }

            //if (posChange)
            //{
            //    Move();
            //    Shoot();
            //    if (reachedEndOfPath)
            //    {
            //        posChange = false;
            //    }
            //}

            //if ((currentDistance > 15f && noticed && canSee) || (currentDistance > 15f && noticed && !canSee))
            //{
            //    posChange = false;
            //    InvokeRepeating("UpdatePath", 0f, .5f);
            //    Move();
            //}
            //else if (currentDistance <= 15f && noticed && !canSee)
            //{
            //    UpdatePath();
            //    Move();
            //}

            LookForPlayer();
        }

        private void TurnTowardsPlayer(float deltaTime)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            targetDirection.y = 0f;
            Vector3.Normalize(targetDirection);

            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, deltaTime * 100, 0));
        }

        private void ChangePosition()
        {
            if (!posChange)
            {
                float posRange;
                posRange = radius / 3.5f;
                var point = UnityEngine.Random.insideUnitSphere * posRange;
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

            Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;
            mover.Move(direction);
            //Vector3 force = direction * speed * Time.deltaTime;
            //rb.AddForce(force, ForceMode.VelocityChange);
        }

        private void LookForPlayer()
        {
            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
            if (currentDistance < radius)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, currentDistance, wallMask))
                {
                    FieldOfView();
                }
                else if (Physics.Raycast(transform.position, directionToTarget, currentDistance, wallMask))
                {
                    //if (path != null)
                    //{
                    //    var lookPos = (Vector3)path.vectorPath[currentWaypoint] - transform.position;
                    //    lookPos.y = 0;

                    //    Quaternion lookForward = Quaternion.LookRotation(lookPos);
                    //    transform.rotation = Quaternion.Slerp(transform.rotation, lookForward, Time.deltaTime * 5f);
                    //}

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
            var lookPos = player.transform.position - transform.position;
            lookPos.y = 0;

            Quaternion lookOnLook = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 5f);

            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
            float kulma = Mathf.Abs(Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up));

            if (kulma < angle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, currentDistance, wallMask))
                {
                    canSee = true;
                    timer = 0;

                    //Shoot();
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
        public bool Shoot()
        {
            if (canSee)
            {
                if (isInCooldown == false)
                {
                    Vector3 toPlayer = (player.transform.position - transform.position).normalized;
                    Vector3 lookDirection = transform.forward;
                    Vector3 shootDirection = new Vector3(lookDirection.x, toPlayer.y, lookDirection.z);
                    
                    return shootComponent.Fire(shootDirection);

                    //Wait();
                    //GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
                    //if (bullet != null)
                    //{
                    //    bullet.transform.position = projectilePos.transform.position;
                    //    bullet.transform.rotation = projectilePos.transform.rotation;
                    //    bullet.SetActive(true);
                    //}
                    //StartCoroutine(Cooldown());
                }
            }
            return false;
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

        private IEnumerator ChangePositionRoutine(Vector3 direction)
        {
            posChange = true;
            float timer = 2;
            while (timer > 0)
            {
                mover.Move(mover.GetSlopeDirection(direction));
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            posChange = false;
        }
    }
}
