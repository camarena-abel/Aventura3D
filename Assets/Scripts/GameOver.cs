using UnityEngine;

public class GameOver : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Play()
    {
        transform.gameObject.SetActive(true);
        animator.SetTrigger("Play");
    }
}
