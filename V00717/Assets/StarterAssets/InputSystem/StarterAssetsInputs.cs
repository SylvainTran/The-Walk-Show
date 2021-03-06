using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
		// public delegate void TriggerPlayerInteracted();
		// public static event TriggerPlayerInteracted _OnPlayerInteracted;

		// Player opens dashboard OS
		public delegate void TriggerOpenDashboardOS();
		public static event TriggerOpenDashboardOS _OnTriggerOpenDashboardOS;
		public static GameObject previouslyActiveCanvas = null;
		public static GameObject activeMenuCanvas = null; // If any menu is open, close it first to open another
		public static Canvas activeOverlayScreen = null; // Any overlay screen notification

		// Player closes active menu
		public delegate void TriggerCloseActiveMenu();
		public static event TriggerCloseActiveMenu _OnTriggerCloseActiveMenu;
		// Player closes active overlay
		public delegate void TriggerCloseActiveOverlay();
		public static event TriggerCloseActiveOverlay _OnTriggerCloseOverlay;

		// Camera views in "On Air" page in the dashboard OS
		public GameObject lane1Subquadrants;
		public GameObject lane2Subquadrants;
		public GameObject lane3Subquadrants;
		public GameObject lane4Subquadrants;
		public Image blackMask;

		public GameObject specialEventsWindow;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif
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
			//_OnPlayerInteracted();
		}

		public void OnOpenDashboardOS(InputValue value)
        {
			// if there is no active menu canvas or it's currently inactive, we can open the dashboard OS
			if(!activeMenuCanvas || !activeMenuCanvas.activeInHierarchy)
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
			if (specialEventsWindow.gameObject.activeInHierarchy)
            {
				specialEventsWindow.gameObject.SetActive(false);
				return;
			}
			//if (activeMenuCanvas != null)
            //{
			//_OnTriggerCloseActiveMenu();				
            //}
			Cursor.visible = false;
		}

		public void OnLane1QuadrantToggle(InputValue value)
        {
			bool active = lane1Subquadrants.gameObject.activeInHierarchy;
			lane1Subquadrants.gameObject.SetActive(!active);
		}

		public void OnLane2QuadrantToggle(InputValue value)
		{
			bool active = lane2Subquadrants.gameObject.activeInHierarchy;
			lane2Subquadrants.gameObject.SetActive(!active);
		}

		public void OnLane3QuadrantToggle(InputValue value)
		{
			bool active = lane3Subquadrants.gameObject.activeInHierarchy;
			lane3Subquadrants.gameObject.SetActive(!active);
		}

		public void OnLane4QuadrantToggle(InputValue value)
		{
			bool active = lane4Subquadrants.gameObject.activeInHierarchy;
			lane4Subquadrants.gameObject.SetActive(!active);
		}

		public void OnSpecialCameraToggle(InputValue value)
        {
			specialEventsWindow.SetActive(!specialEventsWindow.activeInHierarchy);
		}

		public static void ClearActiveMenu()
        {
			activeMenuCanvas = null;
        }

		// There's only one active menu at any state in the game hence static
		public static void SetActiveMenuCanvas(GameObject canvas)
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
			if (activeMenuCanvas.activeInHierarchy)
            {
				Cursor.visible = true;
            }
        }
	// old input sys if we do decide to have it (most likely wont)...

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