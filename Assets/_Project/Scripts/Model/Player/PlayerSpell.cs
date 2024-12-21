using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace TW
{
    public class PlayerSpell : MonoBehaviour
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
        int equippedSpell = 0;

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

        public void Shoot()
        {
            InstantiateSpell(spells[equippedSpell].prefab);
        }

        public void InstantiateSpell(GameObject spell)
        {
            Instantiate(spell, spellcaster.position, spellcaster.rotation);
        }

        private void Update()
        {
            Vector3 newPosition = transform.position;
            newPosition.y += 1.6f;

            spellcasterPivot.position = Vector3.Lerp(spellcasterPivot.position, newPosition, .1f);
        }
    }
}
