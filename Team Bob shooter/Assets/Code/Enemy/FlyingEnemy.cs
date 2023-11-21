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
    public class FlyingEnemy : BaseFixedUpdateListener
    {
        public int CurrentMapArea = 0;

        public Transform player;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask wallMask;

        public float radius;
        [Range(0, 360)]
        public float angle;
        public bool canSee;

        private float currentDistance;

        private bool isInCooldown = false;

        Path path;
        Seeker seeker;
        Rigidbody rb;
        private int currentWaypoint = 0;
        private float nextWaypointDistance = 3f;
        public bool reachedEndOfPath = false;
        [SerializeField] private int speed = 25;

        public float timer;
        public bool noticed = false;

        private bool posChange = false;

        private UnitHealth unitHealth;

        [SerializeField]
        private EnemyGibbing gibPrefab;

        private ComponentPool<EnemyGibbing> enemyGibbingPool;

        private EnemyGibbing activeGibbing = null;

        [SerializeField]
        private WaveData.EnemyType enemyType;

        public static event Action<WaveData.EnemyType, Transform> OnDefeated;

        private DropSpawner dropSpawner;

        private FlyingEnemyAttack shootComponent;

        private Mover mover;

        private Vector3 changePosDirection = Vector3.zero;

        [SerializeField]
        private float fireRange = 10f;

        public bool roam = false;

        private MapAreaManager mapAreaManager;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float damageLockoutTime = 0.5f;

        [SerializeField]
        private float heightFromGround = 4f;

        private bool damageLockout = false;

        private EnemySpawnEffect spawnEffect;

        private FlyingEnemyDash enemyDash;

        private enum ActionState
        {
            idle = 0,
            fire = 1
        }

        private ActionState currentState = ActionState.idle;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody>();
            dropSpawner = GetComponent<DropSpawner>();
            shootComponent = GetComponent<FlyingEnemyAttack>();
            mover = GetComponent<Mover>();
            enemyDash = GetComponent<FlyingEnemyDash>();
            mover.Setup(speed);

            mapAreaManager = GameInstance.Instance.GetMapAreaManager();
        }

        protected override void Awake()
        {
            base.Awake();

            enemyGibbingPool = new ComponentPool<EnemyGibbing>(gibPrefab, 2);
            spawnEffect = GetComponentInChildren<EnemySpawnEffect>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (unitHealth != null)
            {
                unitHealth.OnDied -= OnDie;
                unitHealth.OnTakeDamage -= OnTakeDamage;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (activeGibbing != null)
            {
                activeGibbing.Completed -= ReturnGibToPool;
                activeGibbing = null;
            }
        }

        public void Initialize()
        {
            canSee = false;
            noticed = false;
            posChange = false;
            roam = false;

            player = FindObjectOfType<PlayerUnit>().transform;

            unitHealth = GetComponent<UnitHealth>();
            unitHealth.AddHealth(unitHealth.MaxHealth);
            unitHealth.OnDied += OnDie;
            unitHealth.OnTakeDamage += OnTakeDamage;

            damageLockout = false;
            spawnEffect.PlayEffect();
        }

        private void OnDie(float explosionStrength, Vector3 explosionPoint, EnemyGibbing.DeathType deathType = EnemyGibbing.DeathType.Normal)
        {
            EnemyAggroState.aggro = true;

            dropSpawner.SpawnThings();
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - 0.9f, transform.position.z);
            Vector3 vRot = transform.rotation.eulerAngles;

            Quaternion rot = Quaternion.Euler(new Vector3(vRot.x, vRot.y + 180f, vRot.z));

            OnDefeated?.Invoke(enemyType, transform);

            activeGibbing = enemyGibbingPool.Get();
            if (activeGibbing == null) return;
            activeGibbing.Completed += ReturnGibToPool;
            activeGibbing.transform.position = pos;
            activeGibbing.transform.rotation = rot;
            activeGibbing.Activate(explosionPoint, explosionStrength, deathType);
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

            currentState = ActionState.idle;

            if (currentDistance < fireRange && canSee && noticed|| posChange)
            {
                currentState = ActionState.fire;
            }

            if (currentState == ActionState.idle)
            {
                canSee = false;
            }

            if (damageLockout)
            {
                return;
            }

            if (timer >= 10 && noticed)
            {
                radius = radius / 5;
                noticed = false;
                angle = 90;
            }

            switch (currentState)
            {
                case ActionState.idle:
                    break;
                case ActionState.fire:
                    TurnTowardsPlayer(fixedDeltaTime);
                    if (StartAttack())
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
            MaintainGroundDistance();
        }

        private void TurnTowardsPlayer(float deltaTime)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            targetDirection.y = 0f;
            Vector3.Normalize(targetDirection);

            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, deltaTime * 10, 0));
        }

        private void Move()
        {
            if (path == null) return;

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
            if (currentWaypoint < path.vectorPath.Count)
            {
                Vector3 direction = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;
                direction = new Vector3(direction.x, 0, direction.z);
                mover.Move(mover.GetSlopeDirection(direction));

                animator.SetBool("Moving", true);
            }
            else
            {
                animator.SetBool("Moving", false);
            }
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
                    //if (path != null && currentWaypoint < path.vectorPath.Count)
                    //{
                    //    var lookPos = (Vector3)path.vectorPath[currentWaypoint] - transform.position;
                    //    lookPos.y = 0;

                    //    Quaternion lookForward = Quaternion.LookRotation(lookPos);
                    //    transform.rotation = Quaternion.Slerp(transform.rotation, lookForward, Time.deltaTime * 10f);
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
                    Noticed();
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

        private bool StartAttack()
        {
            if (canSee && noticed && shootComponent.Ready && !posChange)
            {
                animator.SetTrigger("Attack");
                Shoot();
                StartCoroutine(TempWait());
                return true;
            }
            return false;
        }

        public void Shoot()
        {
            shootComponent.Fire(player);

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

        private IEnumerator TempWait()
        {
            yield return new WaitForSeconds(0.7f);
            DashAfterAttack();
        }

        public void DashAfterAttack()
        {
            changePosDirection = (player.position - transform.position).normalized;

            if (Vector3.Distance(player.transform.position, transform.position) < fireRange * 0.66f)
            {
                changePosDirection = new Vector3(changePosDirection.x, 0, changePosDirection.z);
                changePosDirection = Quaternion.Euler(0, UnityEngine.Random.Range(-180f, 180f), 0) * changePosDirection;
            }
            
            enemyDash.Dash(changePosDirection);
        }

        private void MaintainGroundDistance()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, wallMask))
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(hit.point.x, hit.point.y + 4f, hit.point.z), speed * Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale);
            }
        }

        private void OnTakeDamage()
        {
            EnemyAggroState.aggro = true;
            animator.SetTrigger("Damage");
            StartCoroutine(DamageLockout());
        }

        private IEnumerator DamageLockout()
        {
            damageLockout = true;
            float timer = damageLockoutTime;
            while (timer > 0)
            {
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            damageLockout = false;
        }

        private IEnumerator Cooldown(float time)
        {
            isInCooldown = true;
            timer = 0;
            while (timer < time)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            isInCooldown = false;
        }
    }
}

