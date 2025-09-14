using UnityEngine;
using UnityEngine.AI;
using System;
using Unity.Netcode;
using System.Collections.Generic;
using System.Buffers.Text;

namespace TW
{
    public abstract class BaseAI : NetworkBehaviour
    {
        [SerializeField]
        public string enemyName = "Enemy test";

        [SerializeField]
        public float maxAttackDistance = 20;

        [SerializeField]
        public float minAttackDistance = 2;

        [SerializeField]
        public float stoppingDistance = 1f;

        [SerializeField]
        public float walkSpeed = 5f;

        [SerializeField]
        public float walkRadius = 10f;

        [SerializeField]
        public float rotationSpeed = 10f;

        [SerializeField]
        public float minSightRangeRadius = 4f;

        [SerializeField]
        public float medSightRangeRadius = 3f;

        [SerializeField]
        public float maxSightRangeRadius = 10f;

        [SerializeField]
        public LayerMask detectionLayer;

        [SerializeField]
        public ActionSnapshot[] actionSnapshots;

        public Vector3 circlePos = Vector3.zero;

        public IAIState CurrentState { get => currentState; }

        public PlayerController currentPlayerInsight;
        public ActionSnapshot currentSnapshot;

        public EnemyController enemyController;

        public Vector3 finalDestination;

        public NavMeshAgent agent;

        public bool actionFlag = false;

        public float recoveryTimer;

        public string EnemyName { get => enemyName; }

        public EnemyController EnemyController { set => enemyController = value; }
        public NavMeshAgent Agent { get => agent; }

        public Action<PlayerController> playerFound;

        protected IAIState currentState;

        public IAIState followPlayerState;

        public bool debugtest;

        public string currentStateName;

        protected Dictionary<PlayerController, ThreatData> threatTable = new();

        protected float decayRate = .2f;


        // FSM API
        public void SwitchState(IAIState newState)
        {
            Debug.Log($"SwitchState from {currentState} to {newState}");
            currentState?.Exit(this);
            currentState = newState;
            currentState?.Enter(this);
            currentStateName = newState.ToString();
        }

        public virtual void Init()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = 0;
            SetFollowPlayerState();
            SwitchState(States.roamingState);
        }

        protected virtual void SetFollowPlayerState() => followPlayerState = States.followPlayerState;

        private void Update()
        {
            if (!IsServer || !enabled) return;

            UpdateAnimation();
            SetSpeedBasedOnIfIsBusy();
            currentState?.Execute(this);

            foreach (var kvp in threatTable)
            {
                kvp.Value.recentDamage = Mathf.Max(0, kvp.Value.recentDamage - Time.deltaTime * decayRate);
            }
        }

        public virtual void UpdateAnimation()
        {
            Vector3 velocity = agent.velocity;

            velocity.y = 0;

            Vector3 moveDir = velocity.normalized;

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            float forwardAmount = Vector3.Dot(moveDir, forward);
            float strafeAmount = Vector3.Dot(moveDir, right);

            float speed = velocity.magnitude;
            forwardAmount *= Mathf.Clamp01(speed / walkSpeed);
            strafeAmount *= Mathf.Clamp01(speed / walkSpeed);

            enemyController.AnimatorController.SetMovementValue(strafeAmount, forwardAmount);
        }

        public virtual void TrackPlayer()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, minSightRangeRadius, Vector3.forward, minSightRangeRadius, detectionLayer);
            if (hits.Length > 0)
            {
                for (uint i = 0; i < hits.Length; i++)
                {
                    PlayerController player = hits[i].transform.GetComponent<PlayerController>();
                    if (player == null) continue;
                    currentPlayerInsight = player;
                    playerFound?.Invoke(currentPlayerInsight);
                }
            }

            hits = Physics.SphereCastAll(transform.position, medSightRangeRadius, Vector3.forward, medSightRangeRadius, detectionLayer);
            if (hits.Length > 0)
            {
                for (uint i = 0; i < hits.Length; i++)
                {
                    PlayerController player = hits[i].transform.GetComponent<PlayerController>();
                    if (player == null) continue;
                    currentPlayerInsight = player;
                    playerFound?.Invoke(currentPlayerInsight);
                }
            }

            hits = Physics.SphereCastAll(transform.position, maxSightRangeRadius, Vector3.forward, maxSightRangeRadius, detectionLayer);
            if (hits.Length > 0)
            {
                for (uint i = 0; i < hits.Length; i++)
                {
                    PlayerController player = hits[i].transform.GetComponent<PlayerController>();
                    if (player == null) continue;
                    currentPlayerInsight = player;
                    playerFound?.Invoke(currentPlayerInsight);
                }
            }
        }

        public virtual Vector3 FindRoamingSpot()
        {
            Vector3 finalPosition = transform.position;
            NavMeshPath path = new NavMeshPath();
            int attempts = 3;

            for (int i = 0; i < attempts; i++)
            {
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
                {
                    // check if path is valid within navmesh
                    if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        finalPosition = hit.position;
                        break;
                    }
                }
            }

            return finalPosition;
        }

        public Vector3 GetLocationAroundPosition(float min = 1.1f, float max = 1.6f)
        {
            // Gera um offset fixo em torno do jogador (aleat�rio dentro de um arco)
            float angle = UnityEngine.Random.Range(-170f, 170f);
            float radius = UnityEngine.Random.Range(min, max);

            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 relativeOffset = rotation * Vector3.forward * radius;

            // fallback
            return relativeOffset;
        }

        public ActionSnapshot GetAction(float distance, float angle)
        {
            int maxScore = 0;
            for (int i = 0; i < actionSnapshots.Length; i++)
            {
                ActionSnapshot a = actionSnapshots[i];

                if (distance <= a.maxDist && distance >= a.minDist)
                    if (angle <= a.maxAngle && angle >= a.minAngle)
                        maxScore += a.score;
            }

            int ran = UnityEngine.Random.Range(0, maxScore + 1);
            int temp = 0;

            for (int i = 0; i < actionSnapshots.Length; i++)
            {
                ActionSnapshot a = actionSnapshots[i];

                if (a.score == 0)
                    continue;

                if (distance <= a.maxDist && distance >= a.minDist)
                    if (angle <= a.maxAngle && angle >= a.minAngle)
                        temp += a.score;
                if (temp > ran)
                    return a;
            }
            return null;
        }

        public void HandleRotation(bool isAttacking = false)
        {
            if (!enemyController.AnimatorController.GetCanRotate()) return;
            if (isAttacking)
            {
                Vector3 objectivePosition = finalDestination;
                if (currentPlayerInsight != null)
                    objectivePosition = currentPlayerInsight.transform.position;

                Vector3 dir = objectivePosition - transform.position;
                dir.y = 0;
                dir.Normalize();

                if (dir == Vector3.zero)
                {
                    dir = transform.forward;
                }

                Quaternion targetRot = Quaternion.LookRotation(dir);
                Debug.Log("ROTACIONANDO ENQUANTO ATACA (teoricamente)");
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
            }
            else
            {
                Vector3 moveDirection = agent.velocity;

                if (moveDirection.sqrMagnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }

        public void Die()
        {
            if(agent != null) agent.enabled = false;
            currentState = null;
        }

        public void SetSpeedBasedOnIfIsBusy()
        {
            if (enemyController.AnimatorController.GetIsBusyBool())
            {
                if (agent.speed != 0)
                    agent.speed = 0;
            }
            else
            {
                if (agent.speed != walkSpeed)
                    agent.speed = walkSpeed;
            }
        }

        public void RegisterDamage(PlayerController player, float damage)
        {
            if (!threatTable.ContainsKey(player))
                threatTable[player] = new ThreatData();

            var data = threatTable[player];
            data.totalDamage += damage;
            data.recentDamage += damage;
            data.lastDamageTime = Time.time;
        }

        public PlayerController GetBestTarget()
        {
            PlayerController bestTarget = null;
            float bestScore = float.MinValue;

            foreach (var kvp in threatTable)
            {
                var p = kvp.Key;
                var d = kvp.Value;


                if (p == null || p.PlayerHealth.Health <= 0) // <- Checa se é válido
                    continue;

                float score = d.totalDamage * 1.5f;

                if (Time.time - d.lastDamageTime <= 5f)
                    score += d.recentDamage * 2f;

                float dist = Vector3.Distance(transform.position, p.transform.position);
                score += Mathf.Clamp(15f - dist, 0f, 15f); // <- Clamp da distância

                score += UnityEngine.Random.Range(0f, 2f);

                // Debug.Log($"checking if {p} is a good threat {d.totalDamage} {d.recentDamage} {Time.time - d.lastDamageTime <= 5f} {dist}");


                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = p;
                }
            }

            return bestTarget;
        }

        protected virtual void OnDrawGizmos()
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(transform.position, minSightRangeRadius);

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, medSightRangeRadius);

            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(transform.position, maxSightRangeRadius);
        }
    }
}