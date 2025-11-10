using UnityEngine;

public class Door : MonoBehaviour
{
    AudioSource audio;
    Animator animator;

    [SerializeField]
    ItemInfo requiredItem;

    void Start()
    {
        audio = GetComponent<AudioSource>();
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
        audio.Play();
    }
}
