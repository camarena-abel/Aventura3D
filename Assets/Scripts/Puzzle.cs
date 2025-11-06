using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField]
    string puzzleName;

    [SerializeField]
    ItemInfo requiredItem;

    [SerializeField]
    Transform showHideObject;

    [SerializeField]
    bool removeItemWhenUsed = true;

    public string instanceGUID; // identificador de esta instancia del objeto

    public string PuzzleName { get => puzzleName; }
    public bool RemoveItemWhenUsed { get => removeItemWhenUsed;  }

    void Start()
    {
        // el puzzle ha sido resuelto en una partida anterior?
        if (GameState.gameData.CheckResolvedPuzzle(instanceGUID))
            return;

        // escondemos el objeto que aparecera al resolver el puzzle
        showHideObject.gameObject.SetActive(false);
    }

    public void TryToResolve(string selectedItemGUID)
    {
        // nos aseguramos que el player tiene el item seleccionado
        if (requiredItem.guid != selectedItemGUID)
           return;

        // mostramos el objeto
        showHideObject.gameObject.SetActive(true);

        // marcamos el puzzle como resuelto
        GameState.gameData.AddResolvedPuzzle(instanceGUID);
        
    }
}
