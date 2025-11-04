using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    ItemInfo requiredItem;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TryToOpen(string selectedItemGUID)
    {
        // si es necesario un item para abrir la puerta
        if (requiredItem)
        {
            // nos aseguramos que el player lo tiene seleccionado
            if (requiredItem.guid != selectedItemGUID)
                return;
        }

        // abrimos la puerta
        animator.SetTrigger("Open");
    }
}
