using UnityEngine;
using UnityEngine.InputSystem;

public class StartGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;

    [Header("Swipe")]
    [SerializeField] private float swipeThreshold = 50f;

    private Vector2 touchStartPosition;
    private bool gameStarted;

    private void Awake()
    {
        Time.timeScale = 0f;

        if (startCanvas != null)
            startCanvas.SetActive(true);

        if (gameCanvas != null)
            gameCanvas.SetActive(false);
    }

    private void Update()
    {
        if (gameStarted)
            return;

        CheckTouch();
        CheckKeyboard();
    }

    private void CheckTouch()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            touchStartPosition = touch.position.ReadValue();
        }

        if (touch.press.wasReleasedThisFrame)
        {
            Vector2 touchEndPosition = touch.position.ReadValue();

            Vector2 swipe = touchEndPosition - touchStartPosition;

            if (Mathf.Abs(swipe.x) >= swipeThreshold &&
                Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
            {
                StartGameNow();
            }
        }
    }

    private void CheckKeyboard()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.aKey.wasPressedThisFrame ||
            Keyboard.current.dKey.wasPressedThisFrame ||
            Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            StartGameNow();
        }
    }

    private void StartGameNow()
    {
        if (gameStarted)
            return;

        gameStarted = true;

        Time.timeScale = 1f;

        if (startCanvas != null)
            startCanvas.SetActive(false);

        if (gameCanvas != null)
            gameCanvas.SetActive(true);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}