using UnityEngine;
using UnityEngine.AI;
using System;
using Unity.Netcode;

namespace TW
{
    public abstract class BaseAI : NetworkBehaviour
    {
        [SerializeField]
        public string enemyName = "Enemy test";

        [SerializeField]
        public float maxAttackDistance = 20;

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

        public PlayerController currentPlayerInsight;
        public ActionSnapshot currentSnapshot;

        public EnemyController enemyController;

        public Vector3 finalDestination;

        public NavMeshAgent agent;

        public bool isBusy = false;

        public bool actionFlag = false;

        public float recoveryTimer;

        public string EnemyName { get => enemyName; }

        public EnemyController EnemyController { set => enemyController = value; }
        public NavMeshAgent Agent { get => agent; }

        public Action<PlayerController> playerFound;

        protected IAIState currentState;

        public IAIState followPlayerState;

        public bool debugtest;


        // FSM API
        public void SwitchState(IAIState newState)
        {
            if(debugtest) Debug.Log($"Exiting {currentState} and Entering {newState}");
            currentState?.Exit(this);
            currentState = newState;
            currentState?.Enter(this);
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
            if (!IsServer && !enabled) return;

            UpdateAnimation();
            SetSpeedBasedOnIfIsBusy();
            currentState?.Execute(this);
        }

        public virtual void UpdateAnimation()
        {
            isBusy = enemyController.AnimatorController.GetIsBusyBool();
            float speed = agent.velocity.magnitude;
            enemyController.AnimatorController.SetMovementValue(Mathf.Clamp01(speed / walkSpeed));
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

        public Vector3 GetLocationAroundPlayer()
        {
            Vector3 randomDir = Quaternion.Euler(0, UnityEngine.Random.Range(-90, 90), 0) * Vector3.forward;
            Vector3 rawPos = currentPlayerInsight.transform.position + randomDir * UnityEngine.Random.Range(1.1f, 1.6f);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(rawPos, out hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            // fallback
            return transform.position;
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
            agent.enabled = false;
        }

        public void SetSpeedBasedOnIfIsBusy()
        {
            if (isBusy)
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

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, minSightRangeRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, medSightRangeRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxSightRangeRadius);
        }
    }
}