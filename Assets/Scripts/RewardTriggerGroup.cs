using UnityEngine;

public class RewardTriggerGroup : MonoBehaviour
{
    private bool selected = false;

    public void SelectTrigger()
    {
        if (selected)
            return;

        selected = true;
        Destroy(gameObject);
    }
}