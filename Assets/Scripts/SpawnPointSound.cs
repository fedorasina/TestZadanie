using UnityEngine;

public class SpawnPointSound : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource audioSource;

    private bool played;

    private void OnTriggerEnter(Collider other)
    {
        if (played)
            return;

        if (!other.CompareTag("Player"))
            return;

        played = true;

        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
        else if (sound != null)
        {
            AudioSource.PlayClipAtPoint(
                sound,
                transform.position
            );
        }
    }
}