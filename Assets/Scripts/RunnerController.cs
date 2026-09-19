using UnityEngine;
using UnityEngine.InputSystem;

public class RunnerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 6f;
    [SerializeField] private float horizontalSpeed = 5f;

    [Header("Touch Control")]
    [SerializeField] private float touchSensitivity = 0.02f;
    [SerializeField] private float maxHorizontalSpeed = 8f;

    [Header("Rotation")]
    [SerializeField] private float turnSpeed = 360f;

    private Vector3 moveDirection = Vector3.forward;

    private float horizontalInput;

    private bool isTurning;
    private Vector3 turnDirection;

    private Vector2 lastTouchPosition;
    private bool isTouching;

    private void Update()
    {
        ReadInput();

        if (isTurning)
            PerformTurn();
        else
            Move();
    }

    private void ReadInput()
    {
        horizontalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                horizontalInput = -1f;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                horizontalInput = 1f;
            }
        }



        if (Touchscreen.current == null)
            return;

        var touch =
            Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            lastTouchPosition =
                touch.position.ReadValue();

            isTouching = true;
        }

        if (touch.press.isPressed && isTouching)
        {
            Vector2 currentTouchPosition =
                touch.position.ReadValue();

            float deltaX =
                currentTouchPosition.x -
                lastTouchPosition.x;

            horizontalInput =
                Mathf.Clamp(
                    deltaX * touchSensitivity,
                    -1f,
                    1f
                );

            lastTouchPosition =
                currentTouchPosition;
        }

        if (touch.press.wasReleasedThisFrame)
        {
            isTouching = false;
            horizontalInput = 0f;
        }
    }

    private void Move()
    {
        Vector3 forwardMovement =
            moveDirection * forwardSpeed;

        Vector3 right =
            Vector3.Cross(
                Vector3.up,
                moveDirection
            );

        Vector3 sideMovement =
            right *
            horizontalInput *
            horizontalSpeed;

        transform.position +=
            (forwardMovement + sideMovement) *
            Time.deltaTime;
    }

    private void PerformTurn()
    {
        transform.position +=
            moveDirection *
            forwardSpeed *
            Time.deltaTime;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                turnDirection
            );

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );

        if (Quaternion.Angle(
                transform.rotation,
                targetRotation
            ) < 0.5f)
        {
            transform.rotation =
                targetRotation;

            moveDirection =
                turnDirection;

            isTurning = false;
        }
    }

    public void TurnLeft()
    {
        if (isTurning)
            return;

        turnDirection =
            Quaternion.Euler(
                0f,
                -90f,
                0f
            ) * moveDirection;

        turnDirection.y = 0f;
        turnDirection.Normalize();

        isTurning = true;
    }

    public void TurnRight()
    {
        if (isTurning)
            return;

        turnDirection =
            Quaternion.Euler(
                0f,
                90f,
                0f
            ) * moveDirection;

        turnDirection.y = 0f;
        turnDirection.Normalize();

        isTurning = true;
    }

    public void TurnAround()
    {
        if (isTurning)
            return;

        turnDirection =
            -moveDirection;

        turnDirection.y = 0f;
        turnDirection.Normalize();

        isTurning = true;
    }


    public void SetMovementDirection(
        Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        moveDirection =
            direction.normalized;

        transform.rotation =
            Quaternion.LookRotation(
                moveDirection
            );

        isTurning = false;
    }

    public void ResetMovement()
    {
        isTurning = false;

        turnDirection =
            Vector3.zero;

        moveDirection =
            transform.forward;

        moveDirection.y = 0f;
        moveDirection.Normalize();
    }
}