using UnityEngine;
using UnityEngine.Playables;

public class Cinematica01 : LeverPuzzleTarget
{
    PlayableDirector animacion;
    public override void PuzzleResolved()
    {
        animacion.Play();
    }

    void Start()
    {
        animacion = GetComponent<PlayableDirector>();
    }

}
