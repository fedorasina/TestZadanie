using UnityEngine;

public class MoneyMultiplier : MonoBehaviour
{
    [SerializeField] private float moneyMultiplier = 2f;

    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform effectPoint;

    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource audioSource;

    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used)
            return;

        if (!other.CompareTag("Player"))
            return;

        MoneyCollector collector =
            other.GetComponent<MoneyCollector>();

        if (collector == null)
        {
            collector =
                other.GetComponentInParent<MoneyCollector>();
        }

        if (collector == null)
            return;

        used = true;

        collector.SetMoneyMultiplier(moneyMultiplier);

        PlayEffect();
        PlaySound();

        Destroy(gameObject);
    }

    private void PlayEffect()
    {
        if (effectPrefab == null)
            return;

        Vector3 position = transform.position;

        if (effectPoint != null)
            position = effectPoint.position;

        GameObject effect =
            Instantiate(
                effectPrefab,
                position,
                Quaternion.identity
            );

        ParticleSystem particles =
            effect.GetComponentInChildren<ParticleSystem>();

        if (particles != null)
        {
            particles.Play();

            float destroyTime =
                particles.main.duration +
                particles.main.startLifetime.constantMax +
                0.2f;

            Destroy(effect, destroyTime);
        }
        else
        {
            Destroy(effect, 3f);
        }
    }

    private void PlaySound()
    {
        if (sound == null)
            return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(sound);
        }
        else
        {
            AudioSource.PlayClipAtPoint(
                sound,
                transform.position
            );
        }
    }
}