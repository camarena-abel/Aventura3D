using UnityEngine;

public class GameOver : MonoBehaviour
{

    public void Play()
    {
        Animator animator = GetComponent<Animator>();
        transform.gameObject.SetActive(true);
        animator.SetTrigger("Play");
    }
}
