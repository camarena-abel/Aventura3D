using UnityEngine;

public class SavePoint : MonoBehaviour
{
    AudioSource audioSrc;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSrc.Play();
    }
}
