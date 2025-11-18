using UnityEngine;

public class Lever : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    public LeverPuzzle puzzle;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        Reset();
    }

    public bool IsDown()
    {
        return animator.GetBool("Down");
    }

    public void MoveLever()
    {
        // activamso la palanca
        bool down = IsDown();
        animator.SetBool("Down", !down);

        // si hay un puzzle relacionado, avisamos que la palanca se ha cambiado
        if (puzzle)
        {
            puzzle.CheckPuzzle(this);
        }
    }

    public void Reset()
    {
        animator.SetBool("Down", false);
    }
}
