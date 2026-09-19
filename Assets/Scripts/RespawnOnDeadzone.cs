using UnityEngine;

public class RespawnOnDeadzone : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private Vector3 respawnDirection;

    private RunnerController runner;

    private void Awake()
    {
        runner = GetComponent<RunnerController>();

        respawnDirection = transform.forward;
        respawnDirection.y = 0f;
        respawnDirection.Normalize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Point"))
        {
            Transform[] children =
                other.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                if (child.CompareTag("Spawn"))
                {
                    respawnPoint = child;

                    // Запоминаем текущее направление игрока
                    respawnDirection = transform.forward;
                    respawnDirection.y = 0f;
                    respawnDirection.Normalize();

                    break;
                }
            }

            return;
        }

        if (other.CompareTag("Deadzone"))
        {
            if (respawnPoint == null)
                return;

            transform.position = respawnPoint.position;

            if (runner != null)
            {
                runner.SetMovementDirection(respawnDirection);
            }
            else
            {
                transform.rotation =
                    Quaternion.LookRotation(respawnDirection);
            }
        }
    }
}