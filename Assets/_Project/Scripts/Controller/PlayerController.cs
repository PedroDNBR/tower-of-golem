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

        private Vector2 analogicAim;
        private Vector2 mouseAim;
        private Vector2 mouseMovement;
        private Vector2 lastMouseAim;

        public void OnEnable()
        {
            if (DialogueMenu.instance == null)
                SetupPlayerInputs();
            else
            {
                DialogueMenu.instance.OnDialogStarted += DisableInput;
                DialogueMenu.instance.OnDialogEnded += SetupPlayerInputs;
            }
        }

        private void SetupPlayerInputs()
        {
            if (playerInput == null)
            {
                playerInput = new PlayerInput();
                playerInput.Motion.Movement.performed += playerInput => movement = playerInput.ReadValue<Vector2>();
                playerInput.Motion.Jump.performed += playerInput => playerMovement.Jump();
                playerInput.Motion.Dash.performed += playerInput => playerMovement.Dash(movement.x, movement.y, playerCamera.cameraReference);
                playerInput.Actions.PrimarySpell.performed += playerInput => playerSpell.ShootInput();
                playerInput.Actions.SpecialSpell.performed += playerInput => playerSpell.ShootSpecialInput();
                playerInput.Settings.Menu.performed += playerInput => TogglePauseMenu();
                playerInput.Actions.AnalogicAim.performed += playerInput => analogicAim = playerInput.ReadValue<Vector2>();
                playerInput.Actions.MouseAim.performed += playerInput => mouseAim = playerInput.ReadValue<Vector2>();
                playerInput.Actions.MouseMovement.performed += playerInput => mouseMovement = playerInput.ReadValue<Vector2>();
            }
            playerInput.Enable();
        }

        private void TogglePauseMenu()
        {
            playerUI.TogglePauseMenu();
        }

        private void OnDisable()
        {
            DisableInput();
            if (DialogueMenu.instance != null)
            {
                DialogueMenu.instance.OnDialogStarted -= DisableInput;
                DialogueMenu.instance.OnDialogEnded -= SetupPlayerInputs;
            }
        }

        private void DisableInput()
        {
            if(playerInput != null) playerInput.Disable();
        }

        public void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerCamera = GetComponent<PlayerCamera>();
            playerSpell = GetComponent<PlayerSpell>();
            playerHealth = GetComponent<PlayerHealth>();
            playerUI = GetComponentInParent<PlayerUI>();
            playerUI.playerController = this;
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
            if (playerInput == null) return;
            float delta = Time.deltaTime;
            playerMovement.Movement(movement.x, movement.y, delta, playerCamera.cameraReference);
            playerCamera.MoveCamera(mouseMovement);
        }   

        bool usingAnalogic = false;
        bool usingMouse = true;

        private void LateUpdate()
        {
            if (playerInput == null) return;
            float analogAmout = Mathf.Clamp01(Mathf.Abs(analogicAim.x) + Mathf.Abs(analogicAim.y));
            if (analogAmout > 0 && !usingAnalogic)
            {
                usingAnalogic = true;
                usingMouse = false;
            }
            else if (mouseAim != lastMouseAim)
            {
                usingAnalogic = false;
                usingMouse = true;
            }

            if (usingAnalogic)
                playerSpell.AimUsingAnalogic(analogicAim);

            if (usingMouse)
                playerSpell.AimToPosition();

            lastMouseAim = mouseAim;
        }
    }
}