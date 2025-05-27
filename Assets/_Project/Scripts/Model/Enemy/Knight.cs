using UnityEngine;
using UnityEngine.AI;

namespace TW
{
    public class Knight : BaseAI
    {
        [SerializeField] bool debug = false;

        // Behavious
        public bool avoidHit = false;
        public bool isBackingOff = false;
        public bool drawGizmo = false;
        bool forceEscape = false;

        // Destinations
        public Vector3 circlePos = Vector3.zero;
        public Vector3 backoffPos = Vector3.zero;
        Vector3 forceEscapePos = Vector3.zero;

        // Timers
        public float backoffCooldown = 0f;
        float stuckTimer = 0f;
        float stuckThreshold = 3.5f;

        // Debug
        public BaseAI FUCKINGBLOCKER;

        protected override void AttackPlayer()
        {
            if (currentPlayerInsight == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, currentPlayerInsight.transform.position);

            if (distanceToPlayer > 6f)
            {
                ResetStates();
                return;
            }

            if (HandleForceEscapeLogic()) return;
            if (HandleBackoffLogic()) return;
            if (CheckAndTriggerBackoff(distanceToPlayer)) return;

            BaseAI blocker = GetBlockingEnemy();
            FUCKINGBLOCKER = blocker;
            drawGizmo = true;

            if (blocker == null)
            {
                HandleClearAttackPath(distanceToPlayer);
            }
            else
            {
                HandleBlockedAttackPath(blocker);
                return;
            }

            TryAttack(distanceToPlayer);
        }

        void ResetStates()
        {
            avoidHit = false;
            isBackingOff = false;
            forceEscape = false;
            stuckTimer = 0f;
        }

        bool HandleForceEscapeLogic()
        {
            if (!isBackingOff && !avoidHit) return false;

            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckThreshold && !forceEscape)
            {
                HandleForceEscape();
                return true;
            }

            if (forceEscape)
            {
                if (Vector3.Distance(transform.position, forceEscapePos) <= 1.5f)
                {
                    forceEscape = false;
                    forceEscapePos = Vector3.zero;
                    stuckTimer = 0;
                }
                else
                {
                    agent.SetDestination(forceEscapePos);
                    return true;
                }
            }
            return false;
        }

        bool HandleBackoffLogic()
        {
            if (!isBackingOff) return false;

            backoffCooldown -= Time.deltaTime;

            if (Vector3.Distance(transform.position, backoffPos) < 0.75f || backoffCooldown <= 0)
            {
                isBackingOff = false;
                backoffPos = Vector3.zero;
            }
            else
            {
                agent.SetDestination(backoffPos);
                return true;
            }
            return false;
        }

        bool CheckAndTriggerBackoff(float distanceToPlayer)
        {
            if (distanceToPlayer < 1f && !isBackingOff)
            {
                HandleBackoff();
                return true;
            }
            return false;
        }

        void HandleClearAttackPath(float distanceToPlayer)
        {
            if (enemyController.AnimatorController.GetCanRotate()) HandleRotation(true);

            agent.avoidancePriority = 10;
            avoidHit = false;
            circlePos = Vector3.zero;

            if (agent.destination != currentPlayerInsight.transform.position)
                agent.SetDestination(currentPlayerInsight.transform.position);
        }

        void HandleBlockedAttackPath(BaseAI blocker)
        {
            if (avoidHit && Vector3.Distance(transform.position, circlePos) <= 1.3f)
            {
                circlePos = Vector3.zero;
                avoidHit = false;
                agent.SetDestination(currentPlayerInsight.transform.position);
                return;
            }

            agent.avoidancePriority = 50;

            if (circlePos == Vector3.zero)
                circlePos = GetOppositePosition(blocker);

            if (agent.destination != circlePos)
                agent.SetDestination(circlePos);

            avoidHit = true;
        }

        void TryAttack(float distanceToPlayer)
        {
            Vector3 dir = currentPlayerInsight.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();

            float angle = Vector2.Angle(transform.position, dir);
            float dot = Vector3.Dot(transform.right, dir);
            if (dot < 0) angle *= -1;

            if (!isBusy)
            {
                if (actionFlag)
                {
                    recoveryTimer -= Time.deltaTime;
                    if (recoveryTimer <= 0) actionFlag = false;
                }

                if (!actionFlag)
                {
                    currentSnapshot = GetAction(distanceToPlayer, angle);

                    if (currentSnapshot != null)
                    {
                        agent.avoidancePriority = 10;
                        enemyController.AnimatorController.PlayTargetAnimation(currentSnapshot.anim, true);
                        actionFlag = true;
                        recoveryTimer = currentSnapshot.recoveryTime;
                    }
                }
            }
        }

        BaseAI GetBlockingEnemy()
        {
            Vector3 toPlayer = (currentPlayerInsight.transform.position - transform.position).normalized;

            // lateral offset (right from character)
            Vector3 rightOffset = Vector3.Cross(Vector3.up, toPlayer) * 0.45f; // quanto maior, mais pra direita

            // origin point dislocated to the right
            Vector3 detectionPosition = Vector3.Lerp(currentPlayerInsight.transform.position, transform.position, 0.5f) + rightOffset;

            float middleDistance = Vector3.Distance(currentPlayerInsight.transform.position, transform.position) * 0.65f;

            RaycastHit[] hits = Physics.SphereCastAll(detectionPosition, middleDistance, detectionPosition, middleDistance, detectionLayer);
            foreach (var hit in hits)
            {
                BaseAI otherAI = hit.collider.GetComponentInParent<BaseAI>();
                if (otherAI != null && otherAI != this)
                    return otherAI;
                otherAI = hit.collider.GetComponentInChildren<BaseAI>();
                if (otherAI != null && otherAI != this)
                    return otherAI;
                otherAI = hit.collider.GetComponent<BaseAI>();
                if (otherAI != null && otherAI != this)
                    return otherAI;
            }
            return null;
        }

        protected Vector3 GetOppositePosition(BaseAI blocker)
        {
            Vector3 playerPos = currentPlayerInsight.transform.position;
            Vector3 blockerPos = blocker.transform.position;

            // blocker direction in relation to the player
            Vector3 toBlocker = (blockerPos - playerPos).normalized;

            // oposite direction
            Vector3 oppositeDir = -toBlocker;

            Vector3 lateralOffset = (Random.value < 0.7f)
                ? Vector3.Cross(Vector3.up, toBlocker).normalized     // left
                : Vector3.Cross(toBlocker, Vector3.up).normalized;    // right

            // combines the opposite direction with a slight dislocation to the left
            Vector3 offsetDir = (oppositeDir + lateralOffset * 0.8f).normalized;

            // defines distance
            float distance = UnityEngine.Random.Range(2.75f, 3.75f);
            Vector3 targetPos = playerPos + offsetDir * distance;

            // Make sure position is within navmesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas))
            {
                if (debug) Debug.Log("GetOppositePosition (LEFT OFFSET)", gameObject);
                return hit.position;
            }

            return transform.position; // fallback
        }

        void HandleBackoff()
        {
            float backoffDistance = 2.5f;

            Vector3 toPlayer = currentPlayerInsight.transform.position - transform.position;
            Vector3 backDirection = -toPlayer.normalized;
            Vector3 targetPos = transform.position + backDirection * backoffDistance;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                backoffPos = hit.position;
                agent.SetDestination(backoffPos);
                isBackingOff = true;
                backoffCooldown = 1.25f;
                agent.avoidancePriority = 60;
                if (debug) Debug.Log("BACKING OFF", gameObject);
            }
        }

        void HandleForceEscape()
        {
            Vector3 toPlayer = (transform.position - currentPlayerInsight.transform.position).normalized;
            Vector3 escapeDir = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * toPlayer;
            Vector3 escapePos = transform.position + escapeDir * Random.Range(4f, 6f);

            if (NavMesh.SamplePosition(escapePos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                forceEscapePos = hit.position;
                agent.SetDestination(forceEscapePos);
                forceEscape = true;
                stuckTimer = 0f;
                avoidHit = false;
                isBackingOff = false;
                backoffCooldown = 0;
                if (debug) Debug.Log($"{gameObject.name} FORÇANDO ESCAPE", gameObject);
            }
        }
    }
}
