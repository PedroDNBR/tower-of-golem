using UnityEngine;

namespace TW
{
    public class StopProjectileWhenCollide : MonoBehaviour
    {
        Projectile projectile;
        public Transform origin;
        public LayerMask ignoreLayer;
        [SerializeField]
        public float delayToStop = 0.01f;

        [SerializeField]
        Vector3 centerDetection;

        [SerializeField]
        Vector3 extentDetection;

        void OnEnable()
        {
            projectile = GetComponent<Projectile>();
            DealDamageWhenTriggerEnter damageWhenTriggerEnter = GetComponent<DealDamageWhenTriggerEnter>();
            if (damageWhenTriggerEnter != null)
                if (damageWhenTriggerEnter.CharacterBaseHealth != null)
                    origin = damageWhenTriggerEnter.CharacterBaseHealth.transform.root;

        }

        private void FixedUpdate()
        {
            if (projectile == null) return;
            if (origin == null)
            {
                DealDamageWhenTriggerEnter damageWhenTriggerEnter = GetComponent<DealDamageWhenTriggerEnter>();
                if (damageWhenTriggerEnter != null)
                    if (damageWhenTriggerEnter.CharacterBaseHealth != null)
                        origin = damageWhenTriggerEnter.CharacterBaseHealth.transform.root;
            }

            Vector3 worldCenter = transform.position + transform.rotation * centerDetection;

            Collider[] overlaps = Physics.OverlapBox(
                worldCenter,
                extentDetection / 2,
                transform.rotation,
                ignoreLayer
            );

            Transform hitboxTransform = null;
            Transform scenarioTransform = null;

            //Debug.Log("Overlaps: " + overlaps.Length);
            foreach (var other in overlaps)
            {
                //Debug.Log(LayerMask.NameToLayer("Hitbox"));
                //Debug.Log(LayerMask.NameToLayer("Ground"));
                //Debug.Log(LayerMask.NameToLayer("BossGround"));
                //Debug.Log("Overlap with: " + other.name + " (Layer: " + other.gameObject.layer + ")");

                if (other.transform.root == origin) continue;
                // if (((1 << other.gameObject.layer) & ignoreLayer.value) != 0) continue;

                if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
                    hitboxTransform = other.transform;
                else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("BossGround"))
                    scenarioTransform = other.transform;
            }

            if (hitboxTransform != null || scenarioTransform != null)
            {
                if (hitboxTransform != null)
                    transform.parent = hitboxTransform;
                else if (scenarioTransform != null)
                    transform.parent = scenarioTransform;

                Debug.Log(hitboxTransform);
                Debug.Log(scenarioTransform);


                if (delayToStop >= 0)
                    Invoke(nameof(StopProjectile), delayToStop);
                else
                    StopProjectile();

                Rigidbody rigid = GetComponent<Rigidbody>();
                if (rigid != null)
                    Destroy(rigid);

                enabled = false;
            }

            //RaycastHit[] collisions = Physics.BoxCastAll(transform.position + centerDetection, extentDetection/2, transform.TransformDirection(Vector3.forward), transform.rotation, 0, ignoreLayer);

            //RaycastHit hit;

            //if(Physics.BoxCast(transform.position + centerDetection, extentDetection / 2, transform.TransformDirection(Vector3.forward), out hit, transform.rotation, 0, ignoreLayer))
            //{
            //    Debug.Log("OUT HIT");
            //    Debug.Log(hit.collider.transform);
            //}


            //Transform hitboxTransform = null;
            //Transform scenarioTransform = null;

            //Debug.Log("BoxCastAll");
            //Debug.Log(collisions.Length);

            //for (int i = 0; i < collisions.Length; i++)
            //{
            //    Collider other = collisions[i].collider;

            //    Debug.Log(other.gameObject.layer);
            //    Debug.Log(LayerMask.NameToLayer("Hitbox"));
            //    Debug.Log(LayerMask.NameToLayer("Ground"));
            //    Debug.Log(LayerMask.NameToLayer("BossGround"));

            //    if (other.transform.root == origin) continue;
            //    // if (((1 << other.gameObject.layer) & ignoreLayer.value) != 0) continue;

            //    if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
            //        hitboxTransform = other.transform;
            //    else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("BossGround"))
            //        scenarioTransform = other.transform;
            //}

            //if(hitboxTransform != null || scenarioTransform != null)
            //{
            //    if (hitboxTransform != null)
            //        transform.parent = hitboxTransform;
            //    else if (scenarioTransform != null)
            //        transform.parent = scenarioTransform;

            //    if (delayToStop >= 0)
            //        Invoke(nameof(StopProjectile), delayToStop);
            //    else
            //        StopProjectile();

            //    Rigidbody rigid = GetComponent<Rigidbody>();
            //    if (rigid != null)
            //        Destroy(rigid);

            //    enabled = false;
            //}

        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 worldCenter = transform.position + transform.rotation * centerDetection;
            Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, extentDetection); // full size
        }

        //private void OnTriggerEnter(Collider other)
        //{
            //if (origin == null)
            //{
            //    DealDamageWhenTriggerEnter damageWhenTriggerEnter = GetComponent<DealDamageWhenTriggerEnter>();
            //    if (damageWhenTriggerEnter != null)
            //        if (damageWhenTriggerEnter.CharacterBaseHealth != null)
            //            origin = damageWhenTriggerEnter.CharacterBaseHealth.transform.root;
            //}

            //if (other.transform.root == origin) return;
            //if (((1 << other.gameObject.layer) & ignoreLayer.value) != 0) return;
            //if (projectile == null) return;
            //if (delayToStop >= 0)
            //    Invoke(nameof(StopProjectile), delayToStop);
            //else
            //    StopProjectile();

            //transform.parent = other.transform;
            //Debug.Log("OnTriggerEnter");
            //Debug.Log(other.transform);
            //Debug.Log(transform.parent);
            //Rigidbody rigid = GetComponent<Rigidbody>();
            //if (rigid != null)
            //    Destroy(rigid);
        //}

        private void StopProjectile()
        {
            Debug.Log("StopProjectile");
            projectile.Speed = 0;
            projectile.Gravity = 0;
            Destroy(projectile);
        }
    }
}
