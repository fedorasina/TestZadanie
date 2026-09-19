using UnityEngine;

public class RewardTrigger : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private int moneyReward = 20;
    [SerializeField] private int scoreReward = 20;

    [Header("Money Collector")]
    [SerializeField] private MoneyCollector moneyCollector;

    [Header("Player Mesh")]
    [SerializeField] private GameObject playerMeshPrefab;

    [Header("VFX")]
    [SerializeField] private ParticleSystem rewardVFX;

    [Header("Trigger Group")]
    [SerializeField] private RewardTriggerGroup triggerGroup;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used)
            return;

        if (!other.CompareTag("Player"))
            return;

        used = true;

        // Награда / штраф
        if (moneyCollector != null)
        {
            moneyCollector.AddReward(
                moneyReward,
                scoreReward
            );
        }

        PlayVFX(other.transform);

        ChangePlayerMesh(other.gameObject);

        if (triggerGroup != null)
        {
            triggerGroup.SelectTrigger();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayVFX(Transform player)
    {
        if (rewardVFX == null)
            return;

        ParticleSystem vfx = Instantiate(
            rewardVFX,
            player.position,
            Quaternion.identity
        );

        vfx.Play();

        float destroyTime =
            vfx.main.duration +
            vfx.main.startLifetime.constantMax +
            0.2f;

        Destroy(vfx.gameObject, destroyTime);
    }

    private void ChangePlayerMesh(GameObject player)
    {
        if (playerMeshPrefab == null)
            return;

        MeshFilter playerMesh =
            player.GetComponentInChildren<MeshFilter>();

        SkinnedMeshRenderer playerSkinnedMesh =
            player.GetComponentInChildren<SkinnedMeshRenderer>();

        if (playerMesh != null)
        {
            MeshFilter prefabMesh =
                playerMeshPrefab.GetComponentInChildren<MeshFilter>();

            if (prefabMesh != null)
            {
                playerMesh.sharedMesh =
                    prefabMesh.sharedMesh;
            }

            Renderer prefabRenderer =
                prefabMesh != null
                    ? prefabMesh.GetComponent<Renderer>()
                    : null;

            if (prefabRenderer != null)
            {
                Renderer playerRenderer =
                    playerMesh.GetComponent<Renderer>();

                if (playerRenderer != null)
                {
                    playerRenderer.sharedMaterials =
                        prefabRenderer.sharedMaterials;
                }
            }
        }

        if (playerSkinnedMesh != null)
        {
            SkinnedMeshRenderer prefabSkinnedMesh =
                playerMeshPrefab
                    .GetComponentInChildren<SkinnedMeshRenderer>();

            if (prefabSkinnedMesh != null)
            {
                playerSkinnedMesh.sharedMesh =
                    prefabSkinnedMesh.sharedMesh;

                playerSkinnedMesh.sharedMaterials =
                    prefabSkinnedMesh.sharedMaterials;
            }
        }
    }
}