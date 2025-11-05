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

    public string PuzzleName { get => puzzleName; }
    public bool RemoveItemWhenUsed { get => removeItemWhenUsed;  }

    void Start()
    {
        showHideObject.gameObject.SetActive(false);
    }

    public void TryToResolve(string selectedItemGUID)
    {
        // nos aseguramos que el player tiene el item seleccionado
        if (requiredItem.guid != selectedItemGUID)
           return;

        // mostramos el objeto
        showHideObject.gameObject.SetActive(true);

        
        
    }
}
