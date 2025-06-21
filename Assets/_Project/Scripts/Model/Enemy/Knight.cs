using UnityEngine;
using UnityEngine.AI;

namespace TW
{
    public class Knight : BaseAI
    {
        // Destinations
        public Vector3 backoffPos = Vector3.zero;
        Vector3 forceEscapePos = Vector3.zero;

        // Timers
        public float backoffCooldown = 0f;
        float stuckTimer = 0f;
        float stuckThreshold = 3.5f;

        public override void Init()
        {
            base.Init();
            enemyController.EnemyHealth.Dead += () => AICommander.Instance.Unregister(this);
            playerFound += (PlayerController playerController) =>
            {
                if(enemyController.BaseAI.currentPlayerInsight != playerController)
                {
                    enemyController.AnimatorController.PlayTargetAnimation("DrawSword", true);
                }
            };
        }

        protected override void SetFollowPlayerState() => followPlayerState = KnightStates.followPlayerState;

        //bool HandleForceEscapeLogic()
        //{
        //    if (!isBackingOff && !avoidHit) return false;

        //    stuckTimer += Time.deltaTime;
        //    if (stuckTimer > stuckThreshold && !forceEscape)
        //    {
        //        HandleForceEscape();
        //        return true;
        //    }

        //    if (forceEscape)
        //    {
        //        if (Vector3.Distance(transform.position, forceEscapePos) <= 1.5f)
        //        {
        //            forceEscape = false;
        //            forceEscapePos = Vector3.zero;
        //            stuckTimer = 0;
        //        }
        //        else
        //        {
        //            agent.SetDestination(forceEscapePos);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public BaseAI GetBlockingEnemy()
        {
            Vector3 toPlayer = (currentPlayerInsight.transform.position - transform.position).normalized;

            // lateral offset (right from character)
            Vector3 rightOffset = Vector3.Cross(Vector3.up, toPlayer) * 0.45f; // quanto maior, mais pra direita

            // origin point dislocated to the right
            Vector3 detectionPosition = Vector3.Lerp(currentPlayerInsight.transform.position, transform.position, 0.5f) + rightOffset;

            float middleDistance = Vector3.Distance(currentPlayerInsight.transform.position, transform.position) * 0.6f;

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

        public Vector3 GetOppositePosition(BaseAI blocker)
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
            if (NavMesh.SamplePosition(targetPos, out hit, 4f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return transform.position; // fallback
        }

        //public void HandleForceEscape()
        //{
        //    Vector3 toPlayer = (transform.position - currentPlayerInsight.transform.position).normalized;
        //    Vector3 escapeDir = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * toPlayer;
        //    Vector3 escapePos = transform.position + escapeDir * Random.Range(4f, 6f);

        //    if (NavMesh.SamplePosition(escapePos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        //    {
        //        forceEscapePos = hit.position;
        //        agent.SetDestination(forceEscapePos);
        //        forceEscape = true;
        //        stuckTimer = 0f;
        //        avoidHit = false;
        //        isBackingOff = false;
        //        backoffCooldown = 0;
        //        if (debug) Debug.Log($"{gameObject.name} FOR�ANDO ESCAPE", gameObject);
        //    }
        //}

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (currentPlayerInsight != null)
            {
                Vector3 toPlayer = (currentPlayerInsight.transform.position - transform.position).normalized;

                // lateral offset (right from character)
                Vector3 rightOffset = Vector3.Cross(Vector3.up, toPlayer) * 0.45f; // quanto maior, mais pra direita

                // origin point dislocated to the right
                Vector3 detectionPosition = Vector3.Lerp(currentPlayerInsight.transform.position, transform.position, 0.5f) + rightOffset;

                float middleDistance = Vector3.Distance(currentPlayerInsight.transform.position, transform.position) * 0.65f;

                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(detectionPosition, middleDistance);
            }
        }
    }
}
