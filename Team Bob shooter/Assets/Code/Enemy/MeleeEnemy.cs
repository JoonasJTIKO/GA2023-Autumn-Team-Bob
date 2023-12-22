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
        private bool reachedEndOfPath = false;
        [SerializeField] private int speed = 25;

        public float timer;
        public bool noticed = false;

        private UnitHealth unitHealth;

        [SerializeField]
        private EnemyGibbing gibPrefab;

        private ComponentPool<EnemyGibbing> enemyGibbingPool;

        private EnemyGibbing activeGibbing = null;

        private EnemySpawnEffect spawnEffect;

        [SerializeField]
        private WaveData.EnemyType enemyType;

        public static event Action<WaveData.EnemyType, Transform> OnDefeated;

        private DropSpawner dropSpawner;

        private Mover mover;

        private MapAreaManager mapAreaManager;

        [SerializeField]
        private Animator animator;

        public bool roam = false;

        private Vector3 changePosDirection = Vector3.zero;

        private float idleWalkTimer = 0;

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
            spawnEffect = GetComponentInChildren<EnemySpawnEffect>();
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
                unitHealth.OnTakeDamage -= OnTakeDamage;
            }

            if (enemyLungeAttack != null)
            {
                enemyLungeAttack.AttackEnd -= OnAttackEnd;
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
            if (mover != null)
            {
                mover.enabled = true;
            }

            player = FindObjectOfType<PlayerUnit>().transform;

            unitHealth = GetComponent<UnitHealth>();
            unitHealth.AddHealth(unitHealth.MaxHealth);
            unitHealth.OnDied += OnDie;
            unitHealth.OnTakeDamage += OnTakeDamage;

            animator.SetBool("Moving", false);
            spawnEffect.PlayEffect();

            attacking = false;

            if (noticed)
            {
                radius = radius / 5;
                noticed = false;
                angle = 90;
            }
        }

        private void OnDie(float explosionStrength, Vector3 explosionPoint, EnemyGibbing.DeathType deathType = EnemyGibbing.DeathType.Normal)
        {
            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_LILGUY_DIE, transform.position, 0.5f);

            EnemyAggroState.aggro = true;

            dropSpawner.SpawnThings();
            Vector3 pos = transform.position;
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
                //timer = 0f;
                radius = radius * 5;
                angle = 360;
            }
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);

            animator.speed = GameInstance.Instance.GetUpdateManager().fixedTimeScale;

            //timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;

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

            if (/*currentDistance < radius && canSee*/ EnemyAggroState.aggro)
            {
                Noticed();
            }
            else
            {
                canSee = false;
                IdleRoam(fixedDeltaTime);
                return;
            }

            //if (timer >= 10 && noticed)
            //{
            //    radius = radius / 5;
            //    noticed = false;
            //    angle = 90;
            //}
            if (noticed /*&& timer < 10*/)
            {
                if (currentDistance < attackRange)
                {
                    if (attacking)
                    {
                        FacePlayer();
                    }
                    else
                    {
                        Attack();
                        animator.SetBool("Moving", false);
                    }
                }

                if (!attacking && pathUpdating)
                {
                    Move();
                }
                else
                {
                    animator.SetBool("Moving", false);
                }
            }
            //if(!noticed && timer >= 10)
            //{
            //    Move();
            //}

            if (!attacking) LookForPlayer();

            if (noticed && !canSee)
            {
                Search();
            }
        }

        private void IdleRoam(float deltaTime)
        {
            if (!roam && !isInCooldown)
            {
                changePosDirection = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0) * Vector3.forward;


                //float posRange;
                //posRange = radius;
                //var point = UnityEngine.Random.insideUnitSphere * posRange;
                //point.y = 0;
                //point += transform.position;
                //seeker.StartPath(rb.position, point, OnPathComplete);
                roam = true;
                idleWalkTimer = 2.5f;
            }
            else if (roam)
            {
                if (idleWalkTimer <= 0 || Physics.SphereCast(new Ray(transform.position, mover.GetSlopeDirection(changePosDirection)), 1, 1, LayerMask.GetMask("Ground", "Environment", "LevelBorder")))
                {
                    roam = false;
                    isInCooldown = true;
                    StartCoroutine(Cooldown(3f));
                }
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, changePosDirection, deltaTime * 10, 0));
                mover.Move(mover.GetSlopeDirection(changePosDirection));
                animator.SetBool("Moving", true);

                idleWalkTimer -= deltaTime;
                return;
            }

            animator.SetBool("Moving", false);
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
                animator.SetBool("Moving", true);
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

                    FacePlayer();
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

        private void FacePlayer()
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            toPlayer = new Vector3(toPlayer.x, 0, toPlayer.z);

            Quaternion lookOnLook =
                    Quaternion.LookRotation(toPlayer);

            transform.rotation =
            Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 10f);
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
            if (!enemyLungeAttack.OnCooldown && !attacking)
            {
                animator.SetTrigger("Attack");
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

        public void DoAttack()
        {
            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_LILGUY_ATTACK, transform.position, 0.5f);
            enemyLungeAttack.Lunge();
        }

        private void OnAttackEnd()
        {
            mover.enabled = true;
            attacking = false;
        }

        private void OnTakeDamage(float amount)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= 0.33f)
            {
                GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_LILGUY_TAKE_DAMAGE, transform.position, 0.5f);
            }
            EnemyAggroState.aggro = true;
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
