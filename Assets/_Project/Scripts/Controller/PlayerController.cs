using UnityEngine;

namespace TW
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerMovement playerMovement;
        private PlayerCamera playerCamera;
        private PlayerSpell playerSpell;
        private PlayerHealth playerHealth;
        private PlayerUI playerUI;
        private PlayerDealDamageOnCollision playerDealDamage;

        public PlayerUI PlayerUI { get => playerUI; }


        [Header("References")]
        public PlayerInput playerInput;

        private Vector2 movement;

        public void OnEnable()
        {
            if (playerInput == null)
            {
                playerInput = new PlayerInput();
                playerInput.Motion.Movement.performed += playerInput => movement = playerInput.ReadValue<Vector2>();
                playerInput.Motion.Jump.performed += playerInput => playerMovement.Jump();
                playerInput.Motion.Dash.performed += playerInput => playerMovement.Dash(movement.x, movement.y);
                playerInput.Actions.PrimarySpell.performed += playerInput => playerSpell.Shoot();
                playerInput.Actions.SpecialSpell.performed += playerInput => playerSpell.ShootSpecial();
                // playerInput.Actions.Aim.performed += playerInput => playerInput.ReadValue<Vector2>());
            }
            playerInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.Disable();
        }

        public void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerCamera = GetComponent<PlayerCamera>();
            playerSpell = GetComponent<PlayerSpell>();
            playerHealth = GetComponent<PlayerHealth>();
            playerUI = GetComponentInParent<PlayerUI>();
            playerDealDamage = GetComponentInParent<PlayerDealDamageOnCollision>();
            playerSpell.PlayerCamera = playerCamera.PlayerCameraObj;
        }

        public void Start()
        {
            playerMovement.StaminaChanged += playerUI.StaminaTimeToSliderValue;
            playerHealth.HealthChanged += playerUI.HealthValueToSliderValue;

            playerMovement.Init();

            playerDealDamage.Type = playerHealth.Type;
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            playerMovement.Movement(movement.x, movement.y, delta);
        }

        private void LateUpdate()
        {
            playerSpell.AimToPosition();
        }
    }
}