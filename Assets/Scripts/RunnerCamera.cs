using UnityEngine;

public class RunnerCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);

    [Header("Smooth")]
    [SerializeField] private float positionSmooth = 0.05f;
    [SerializeField] private float rotationSmooth = 8f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        FollowPlayer();
        FollowRotation();
    }

    private void FollowPlayer()
    {
        Vector3 desiredPosition =
            target.position +
            target.rotation * offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            positionSmooth
        );
    }

    private void FollowRotation()
    {
        // Камера смотрит туда же, куда смотрит игрок
        Quaternion targetRotation =
            target.rotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );
    }
}