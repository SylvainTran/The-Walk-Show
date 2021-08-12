using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		// Player interacts with something
		public delegate void TriggerPlayerInteracted();
		public static event TriggerPlayerInteracted _OnPlayerInteracted;

		// Player opens dashboard OS
		public delegate void TriggerOpenDashboardOS();
		public static event TriggerOpenDashboardOS _OnTriggerOpenDashboardOS;
		public static Canvas previouslyActiveCanvas = null;
		public static Canvas activeMenuCanvas = null; // If any menu is open, close it first to open another
		public static Canvas activeOverlayScreen = null; // Any overlay screen notification

		// Player closes active menu
		public delegate void TriggerCloseActiveMenu();
		public static event TriggerCloseActiveMenu _OnTriggerCloseActiveMenu;
		// Player closes active overlay
		public delegate void TriggerCloseActiveOverlay();
		public static event TriggerCloseActiveOverlay _OnTriggerCloseOverlay;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnInteract()
        {
			_OnPlayerInteracted();
		}

		public void OnOpenDashboardOS(InputValue value)
        {
			// if there is no active menu canvas or it's currently inactive, we can open the dashboard OS
			if(!activeMenuCanvas || !activeMenuCanvas.isActiveAndEnabled)
            {
				_OnTriggerOpenDashboardOS();
			}
		}

		public void OnCloseActiveMenu(InputValue value)
        {
			// Close overlays first, if it it's now null also close any active menu canvas
			if (activeOverlayScreen != null)
			{
				_OnTriggerCloseOverlay();
				return;
			}
			//if (activeMenuCanvas != null)
            //{
				_OnTriggerCloseActiveMenu();				
            //}
			Cursor.visible = false;
		}

		public static void ClearActiveMenu()
        {
			activeMenuCanvas = null;
        }

		// There's only one active menu at any state in the game hence static
		public static void SetActiveMenuCanvas(Canvas canvas)
        {
			activeMenuCanvas = canvas;
		}

        private void Start()
        {
			//Cursor.lockState = CursorLockMode.Confined;
		}

        private void Update()
        {
			// Force cursor mode to be visible on the UI while in any menu
			if(activeMenuCanvas == null)
            {
				return;
            }
			if (activeMenuCanvas.isActiveAndEnabled)
            {
				Cursor.visible = true;
            }
        }
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			//SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			//Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}