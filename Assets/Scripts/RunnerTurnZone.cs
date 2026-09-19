using UnityEngine;

public class RunnerTurnZone : MonoBehaviour
{
    public enum TurnType
    {
        Left,
        Right,
        Around
    }

    [SerializeField] private TurnType turnType;

    private void OnTriggerEnter(Collider other)
    {
        RunnerController runner =
            other.GetComponent<RunnerController>();

        if (runner == null)
        {
            runner =
                other.GetComponentInParent<RunnerController>();
        }

        if (runner == null)
            return;

        switch (turnType)
        {
            case TurnType.Left:
                runner.TurnLeft();
                break;

            case TurnType.Right:
                runner.TurnRight();
                break;

            case TurnType.Around:
                runner.TurnAround();
                break;
        }
    }
}