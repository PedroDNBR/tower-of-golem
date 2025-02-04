using UnityEngine;
using UnityEngine.AI;
using System;

namespace TW
{
    public abstract class BaseAI : MonoBehaviour
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

        PlayerController currentPlayerInsight;
        protected ActionSnapshot currentSnapshot;

        private EnemyController enemyController;

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
        }

        private void Update()
        {
            Roaming();
            FollowPlayer();
            AttackPlayer();
            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            TrackPlayer();
        }

        protected virtual void UpdateAnimation()
        {
            isBusy = enemyController.AnimatorController.GetIsBusyBool();
            Vector3 relativeVelocity = transform.InverseTransformDirection(agent.desiredVelocity);
            enemyController.AnimatorController.SetMovementValue(Mathf.Clamp(relativeVelocity.z, 0, 1));
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
                    Debug.Log(currentSnapshot);

                    if (currentSnapshot != null)
                    {
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

            if (isBusy)
            {
                agent.speed = 0;
                return;
            }
            else
            {
                agent.speed = walkSpeed;

                float dis = Vector3.Distance(transform.position, currentPlayerInsight.transform.position);
                if (dis < 2)
                {
                    float rotationLess = rotationSpeed / 4;
                    HandleRotation();

                }
            }

            agent.SetDestination(currentPlayerInsight.transform.position);
        }

        protected virtual void Roaming()
        {
            if (currentPlayerInsight != null) return;

            if (finalDestination == null || finalDestination == Vector3.zero) finalDestination = FindRoamingSpot();

            if (Vector3.Distance(transform.position, finalDestination) < .5f)
                finalDestination = FindRoamingSpot();
            
            HandleRotation();

            agent.SetDestination(finalDestination);
        }

        protected virtual Vector3 FindRoamingSpot()
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
            Vector3 finalPosition = hit.position;
            return finalPosition;
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

        void HandleRotation()
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

        public void Die()
        {
            agent.enabled = false;
        }

        private void OnDrawGizmos()
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