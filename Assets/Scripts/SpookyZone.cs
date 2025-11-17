using UnityEngine;

public class SpookyZone : MonoBehaviour
{
    AudioSource audioSrc;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            audioSrc.Play();
        }
    }
}
