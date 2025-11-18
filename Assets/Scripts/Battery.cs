using UnityEngine;

public class Battery : LeverPuzzleTarget
{
    Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void PuzzleResolved()
    {
        animator.SetTrigger("Small");
    }


}
