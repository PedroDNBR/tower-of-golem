using UnityEngine;
using UnityEngine.AI;
using System;
using Unity.Netcode;

namespace TW
{
    public abstract class BaseAI : NetworkBehaviour
    {
        [SerializeField]
        protected string enemyName = "Enemy test";

        [SerializeField]
        protected float stoppingDistance = 1f;

        [SerializeField]
        protected float walkSpeed = 5f;

        [SerializeField]
        protected float walkRadius = 10f;

        [SerializeField]
        protected float rotationSpeed = 10f;

        [SerializeField]
        protected float minSightRangeRadius = 4f;

        [SerializeField]
        protected float medSightRangeRadius = 3f;

        [SerializeField]
        protected float maxSightRangeRadius = 10f;

        [SerializeField]
        protected LayerMask detectionLayer;

        [SerializeField]
        protected ActionSnapshot[] actionSnapshots;

        protected PlayerController currentPlayerInsight;
        protected ActionSnapshot currentSnapshot;

        protected EnemyController enemyController;

        protected Vector3 finalDestination;

        protected NavMeshAgent agent;

        protected bool isBusy = false;

        protected bool actionFlag = false;

        protected float recoveryTimer;

        public string EnemyName { get => enemyName; }

        public EnemyController EnemyController { set => enemyController = value; }
        public NavMeshAgent Agent { get => agent; }

        public Action<PlayerController> playerFound;

        public void Init()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = walkSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
            agent.avoidancePriority = 50;
        }

        private void Update()
        {
            if (!IsServer || agent == null) return;

            Roaming();
            FollowPlayer();
            AttackPlayer();
            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;
            TrackPlayer();
        }

        protected virtual void UpdateAnimation()
        {
            isBusy = enemyController.AnimatorController.GetIsBusyBool();
            float speed = agent.velocity.magnitude;
            enemyController.AnimatorController.SetMovementValue(Mathf.Clamp01(speed / walkSpeed));
        }

        protected virtual void TrackPlayer()
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

        protected virtual void AttackPlayer()
        {
            if (currentPlayerInsight == null) return;

            if (enemyController.AnimatorController.GetCanRotate()) HandleRotation();

            Vector3 dir = currentPlayerInsight.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();

            float dis = Vector3.Distance(transform.position, currentPlayerInsight.transform.position);
            float angle = Vector2.Angle(transform.position, dir);
            float dot = Vector3.Dot(transform.right, dir);
            if (dot < 0)
                angle *= -1;

            if (!isBusy)
            {
                if (actionFlag)
                {
                    recoveryTimer -= Time.deltaTime;
                    if (recoveryTimer <= 0)
                    {
                        actionFlag = false;
                    }
                }

                if (!isBusy && actionFlag == false)
                {
                    currentSnapshot = GetAction(dis, angle);

                    if (currentSnapshot != null)
                    {
                        if (enemyController.AnimatorController.GetCanRotate()) HandleRotation(true);

                        agent.avoidancePriority = 10;
                        enemyController.AnimatorController.PlayTargetAnimation(currentSnapshot.anim, true);
                        actionFlag = true;
                        recoveryTimer = currentSnapshot.recoveryTime;
                    }
                }

            }
        }

        protected virtual void FollowPlayer()
        {
            if (currentPlayerInsight == null) return;
            if (enemyController.AnimatorController.GetCanRotate()) HandleRotation();
            agent.avoidancePriority = 50;
            if (isBusy)
            {
                agent.speed = 0;
                return;
            }
            else
            {
                float dis = Vector3.Distance(transform.position, currentPlayerInsight.transform.position);
                agent.speed = walkSpeed;

                if (dis < 2)
                {
                    float rotationLess = rotationSpeed / 4;
                }
            }

            if (agent.destination != currentPlayerInsight.transform.position)
                agent.SetDestination(currentPlayerInsight.transform.position);
        }

        protected virtual void Roaming()
        {
            if (currentPlayerInsight != null) return;

            if (finalDestination == null || finalDestination == Vector3.zero) finalDestination = FindRoamingSpot();

            if (Vector3.Distance(transform.position, finalDestination) < .5f)
                finalDestination = FindRoamingSpot();
            
            HandleRotation();
            if (agent.destination != finalDestination)
                agent.SetDestination(finalDestination);
        }

        protected virtual Vector3 FindRoamingSpot()
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

        protected Vector3 GetLocationAroundPlayer()
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

        protected void HandleRotation(bool isAttacking = false)
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