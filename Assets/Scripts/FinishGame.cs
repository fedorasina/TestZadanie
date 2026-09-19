using UnityEngine;

public class FinishGame : MonoBehaviour
{
    [Header("Finish UI")]
    [SerializeField] private GameObject finishCanvas;

    private bool finished;

    private void OnTriggerEnter(Collider other)
    {
        if (finished)
            return;

        if (!other.CompareTag("Finish"))
            return;

        finished = true;

        Time.timeScale = 0f;

        if (finishCanvas != null)
            finishCanvas.SetActive(true);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}