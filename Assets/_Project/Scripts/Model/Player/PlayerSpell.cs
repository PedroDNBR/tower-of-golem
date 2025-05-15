using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.Netcode;

namespace TW
{
    public class PlayerSpell : NetworkBehaviour
    {
        [SerializeField]
        private Transform spellcasterPivot;
        [SerializeField]
        private Transform spellcaster;
        [SerializeField]
        private LayerMask layer;

        private Camera playerCamera;
        public Camera PlayerCamera { get => playerCamera; set => playerCamera = value; }

        [SerializeField]
        List<Spell> spells = new List<Spell>();

        [SerializeField]
        List<Spell> specialSpells = new List<Spell>();

        [SerializeField]
        NetworkVariable<int> equippedSpell = new NetworkVariable<int>(0);

        [SerializeField]
        private NetworkVariable<float> attackDelay = new NetworkVariable<float>(1.5f);

        [SerializeField]
        private NetworkVariable<float> lastShot = new NetworkVariable<float>(0);

        Dictionary<Spell, bool> specialSpellsUsageList = new Dictionary<Spell, bool>();


        private float timer = 0f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();  
            for (int i = 0; i < specialSpells.Count; i++)
                specialSpellsUsageList.Add(specialSpells[i], false);

            if (!IsServer) return;

            timer = Time.time;
            lastShot.Value = timer;
            UpdateTimerClientRpc(timer);
        }

        [ClientRpc]
        void UpdateTimerClientRpc(float newTime)
        {
            timer = newTime;
        }

        public void AimToPosition()
        {
            Vector3 pos = playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out hit, 1000, layer))
            {
                spellcaster.LookAt(hit.point);
            }
        }

        public void AimUsingAnalogic(Vector3 aim)
        {
            if (aim.x == 0 && aim.y == 0) return;
            Vector3 lookDirection = new Vector3(aim.x, 0, aim.y);
            spellcaster.rotation = Quaternion.LookRotation(lookDirection);
        }

        public void ShootInput()
        {
            if (timer > attackDelay.Value + lastShot.Value)
            {
                if (!IsServer)
                {
                    Shoot(); // visual imediato no client local
                    ShootServerRpc(); // envia pro server fazer o real
                }
                else
                {
                    Shoot(); // host tamb�m precisa ver o visual
                    lastShot.Value = timer;
                    ShootClientRpc(); // envia pra todos os outros clients
                }
            }
        }

        public void Shoot()
        {
            InstantiateSpell(spells[equippedSpell.Value]);
        }

        [ServerRpc]
        public void ShootServerRpc()
        {
            Shoot(); // servidor instancia o "real"
            ShootClientRpc(); // replicar pros outros
            lastShot.Value = timer;
            
        }

        [ClientRpc]
        public void ShootClientRpc()
        {
            if (IsServer || IsLocalPlayer) return; // evita duplicar no host
            Shoot(); // efeito visual nos outros clients
        }

        public void ShootSpecialInput()
        {
            if (timer > attackDelay.Value + lastShot.Value)
            {
                if (!IsServer)
                {
                    ShootSpecial(); // visual imediato no client local
                    ShootSpecialServerRpc(); // envia pro server fazer o real
                }
                else
                {
                    ShootSpecial(); // host tamb�m precisa ver o visual
                    ShootSpecialClientRpc(); // envia pra todos os outros clients
                }
            }
        }

        public void ShootSpecial()
        {
            if (specialSpellsUsageList[specialSpells[equippedSpell.Value]] == true) return;

            specialSpellsUsageList[specialSpells[equippedSpell.Value]] = true;
            InstantiateSpell(specialSpells[equippedSpell.Value]);
        }

        [ServerRpc]
        public void ShootSpecialServerRpc()
        {
            ShootSpecial(); // servidor instancia o "real"
            ShootSpecialClientRpc(); // replicar pros outros
        }

        [ClientRpc]
        public void ShootSpecialClientRpc()
        {
            if (IsServer || IsLocalPlayer) return; // evita duplicar no host
            ShootSpecial(); // efeito visual nos outros clients
        }

        public void InstantiateSpell(Spell spell)
        {
            GameObject spellCasted = Instantiate(spell.prefab, spellcaster.position, spellcaster.rotation);
            Projectile projectile = spellCasted.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Speed = spell.speed;
                projectile.Gravity = spell.gravity;
            }
        }

        private void Update()
        {
            if (IsServer) timer += Time.deltaTime;
            if (IsServer) UpdateTimerClientRpc(timer);

            Vector3 newPosition = transform.position;
            newPosition.y += 1.6f;

            spellcasterPivot.position = Vector3.Lerp(spellcasterPivot.position, newPosition, .1f);
        }
    }
}
