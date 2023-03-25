using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ScreenManager : SingletonComponent<ScreenManager>
{
	#region Inspector Variables

	[Tooltip("The home screen to of the app, ei. the first screen that shows when the app starts.")]
	[SerializeField] private string homeScreenId = "main";

	[Tooltip("The list of Screen components that are used in the game.")]
	[SerializeField] private List<IScreen> screens = null;

	[SerializeField] public Canvas canvas;


	
	#endregion

	#region Member Variables

	// Screen id back stack
	private List<string> backStack;

	// The screen that is currently being shown
	private IScreen	currentScreen;
	private bool	isAnimating;

	// Used internally - set by InputfieldFocused.cs
	public bool InputFieldActive = false;
	public RectTransform childRectTransform;
	#endregion

	#region Properties

	public string HomeScreenId		{ get { return homeScreenId; } }
	public string CurrentScreenId	{ get { return currentScreen == null ? "" : currentScreen.Id; } }
	public string PreviousScreenId { get { return backStack.Count == 0 ? "" : backStack[backStack.Count - 1]; } }

	#endregion

	#region Properties

	/// <summary>
	/// Invoked when the ScreenController is transitioning from one screen to another. The first argument is the current showing screen id, the
	/// second argument is the screen id of the screen that is about to show (null if its the first screen). The third argument id true if the screen
	/// that is being show is an overlay
	/// </summary>
	public System.Action<string, string> OnSwitchingScreens;

	/// <summary>
	/// Invoked when ShowScreen is called
	/// </summary>
	public System.Action<string> OnShowingScreen;

	#endregion

	#region Unity Methods

	private void Start()
	{
		UnityEngine.Screen.orientation = ScreenOrientation.Portrait;

		backStack = new List<string>();

		// Initialize and hide all the screens
		for (int i = 0; i < screens.Count; i++)
		{
			IScreen screen = screens[i];

			// Add a CanvasGroup to the screen if it does not already have one
			if (screen.gameObject.GetComponent<CanvasGroup>() == null)
			{
				screen.gameObject.AddComponent<CanvasGroup>();
			}

			// Force all screens to hide right away
			screen.Initialize();
			screen.gameObject.SetActive(true);
			screen.Hide(false, true);
		}

		// Show the home screen when the app starts up
		Show(homeScreenId, false, true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// First try and close an active popup (If there are any)
			//if (!PopupManager.Instance.CloseActivePopup())
			//{
			//	// No active popups, if we are on the home screen close the app, else go back one screen
			//	if (CurrentScreenId == HomeScreenId)
			//	{
			//		Application.Quit();
			//	}
			//	else
			//	{
			//		Back();
			//	}
			//}
		}
	}

    #endregion

    #region Public Methods

    public void Show(string screenId)
	{
		if (CurrentScreenId == screenId)
		{
			return;
		}

		Show(screenId, false, false);
	}


	public void Back()
	{
		if (backStack.Count <= 0)
		{
			Debug.LogWarning("[ScreenController] There is no screen on the back stack to go back to.");

			return;
		}

		// Get the screen id for the screen at the end of the stack (The last shown screen)
		string screenId = backStack[backStack.Count - 1];

		// Remove the screen from the back stack
		backStack.RemoveAt(backStack.Count - 1);

		// Show the screen
		Show(screenId, true, false);
	}

	/// <summary>
	/// Navigates to the screen in the back stack with the given screen id
	/// </summary>
	public void BackTo(string screenId)
	{
		for (int i = backStack.Count - 1; i >= 0; i--)
		{
			if (screenId == backStack[i])
			{
				Back();

				return;
			}
			else
			{
				backStack.RemoveAt(i);
			}
		}

		// If we get here then the screen was not found to just go to home
		Home();
	}

	public void Home()
	{
		if (CurrentScreenId == homeScreenId)
		{
			return;
		}

		Show(homeScreenId, true, false);
		ClearBackStack();
	}

	#endregion

	#region Private Methods

	private void Show(string screenId, bool back, bool immediate)
	{
//		UIManager.LogError("[ScreenController] Showing screen " + screenId);

		// Get the screen we want to show
		IScreen screen = GetScreenById(screenId);

		if (screen == null)
		{
	//		UIManager.LogError("[ScreenController] Could not find screen with the given screenId: " + screenId);

			return;
		}

		// Check if there is a current screen showing
		if (currentScreen != null)
		{
			// Hide the current screen
			currentScreen.Hide(back, immediate);

			if (!back)
			{
				// Add the screens id to the back stack
				backStack.Add(currentScreen.Id);
			}

			if (OnSwitchingScreens != null)
			{
				OnSwitchingScreens(currentScreen.Id, screenId);
			}
		}

		// Show the new screen
		screen.Show(back, immediate);

		// Set the new screen as the current screen
		currentScreen = screen;

		if (screenId == "login" || screenId == "register")
        {
			AudioManager.Instance.PlayBackgroundSound(AudioManager.Instance.firstClip);
		}

		if (OnShowingScreen != null)
		{
			OnShowingScreen(screenId);
		}
	}

	private void ClearBackStack()
	{
		backStack.Clear();
	}

	private IScreen GetScreenById(string id)
	{
		for (int i = 0; i < screens.Count; i++)
		{
			//UIManager.Instance.ShowErrorDlg(screens[i].Id);
			if (id == screens[i].Id)
			{
				return screens[i];
			}
		}

		UIManager.Instance.ShowErrorDlg("[ScreenTransitionController] No Screen exists with the id " + id);
		
		return null;
	}

	#endregion

	void LateUpdate()
	{
#if UNITY_IOS
		if (CurrentScreenId.Equals(""))
		{
			return;
		}

		GameObject currentScreenObj = currentScreen.gameObject;
		currentScreenObj.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

		if ((InputFieldActive) && ((TouchScreenKeyboard.visible)))
		{
			

			Vector3[] corners = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
			Rect rect = RectTransformExtension.GetScreenRect(childRectTransform, canvas);
			float keyboardHeight = TouchScreenKeyboard.area.height;

			float heightPercentOfKeyboard = keyboardHeight / UnityEngine.Screen.height * 100f;
			float heightPercentOfInput = (UnityEngine.Screen.height - (rect.y + rect.height)) / UnityEngine.Screen.height * 100f;


			if (heightPercentOfKeyboard > heightPercentOfInput)
			{
				// keyboard covers input field so move screen up to show keyboard
				float differenceHeightPercent = heightPercentOfKeyboard - heightPercentOfInput;
				float newYPos = transform.GetComponent<RectTransform>().rect.height / 100f * differenceHeightPercent;

				Vector2 newAnchorPosition = Vector2.zero;
				newAnchorPosition.y = newYPos;
				currentScreenObj.transform.GetComponent<RectTransform>().anchoredPosition = newAnchorPosition;
			}
			else
			{
				// Keyboard top is below the position of the input field, so leave screen anchored at zero
				currentScreenObj.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
		}
		else
		{
			// No focus or touchscreen invisible, set screen anchor to zero
			currentScreenObj.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}
		InputFieldActive = false;
#endif
	}

}

public static class RectTransformExtension
{

	public static Rect GetScreenRect(this RectTransform rectTransform, Canvas canvas)
	{

		Vector3[] corners = new Vector3[4];
		Vector3[] screenCorners = new Vector3[2];

		rectTransform.GetWorldCorners(corners);

		if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
		{
			screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
			screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
		}
		else
		{
			screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
			screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
		}

		screenCorners[0].y = UnityEngine.Screen.height - screenCorners[0].y;
		screenCorners[1].y = UnityEngine.Screen.height - screenCorners[1].y;

		return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
	}

}
