using UnityEngine;

public class LeverPuzzle : MonoBehaviour
{
    Lever[] activatedOrder; // solo para los puzzles de tipo orden
    int activationCounter;

    [SerializeField]
    bool ordered; // indica si es un puzzle de activar palancas en un orden o si lo importante es la combinacion

    [SerializeField]
    Lever[] levers;

    [SerializeField]
    bool[] down;  // solo para los puzzles de tipo combinacion (no para los puzzles de orden)

    [SerializeField]
    LeverPuzzleTarget target;
    
    void Start()
    {
        if ((!ordered) && (levers.Length != down.Length))
        {
            Debug.LogError("Los dos arrays deben de tener la misma longitud!");
        }

        activatedOrder = new Lever[levers.Length];  

        // indicamos a cada palance su puzzle
        foreach (var lever in levers)
        {
            lever.puzzle = this;
        }
    }

    private bool PuzzleCombinationIsOk()
    {
        bool isOk = true;
        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i].IsDown() != down[i])
            {
                isOk = false;
                break;
            }
        }
        return isOk;
    }

    private bool PuzzleOrderIsOk()
    {
        bool isOk = true;
        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i] != activatedOrder[i])
            {
                isOk = false;
                break;
            }
        }
        return isOk;
    }

    private bool PuzzleIsOk()
    {
        if (ordered)
        {
            return PuzzleOrderIsOk();
        } else
        {
            return PuzzleCombinationIsOk();
        }
    }

    public void CheckPuzzle(Lever lever)
    {
        // si es un puzzle de tipo ordered, vamos añadiendo elementos al array que indica el orden en el que las palancas han sido activadas
        if (ordered)
        {
            // añadimos la palanca al array
            activatedOrder[activationCounter] = lever;
            activationCounter++;

            // se han activado todas las palancas?
            if (activationCounter < levers.Length)
                return;
        }

        // comprobamos si el puzzle esta ok
        if (PuzzleIsOk())
        {
            if (target)
                target.PuzzleResolved();
        } else
        {
            // resetear las palancas
            if (ordered)
            {
                foreach (var l in levers)
                {
                    l.Reset();
                }
                activationCounter = 0;
            }
        }

    }
}
