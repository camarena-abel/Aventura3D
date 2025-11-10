using System.Collections;
using UnityEngine;

public class NPC_A : NPC_X
{
    private static string DLG_PRESENTACION = "2a0a0a3f-9b5f-40c5-ba11-e322b901b171";
    private static string ITEM_POCION = "6de14066-aaeb-4172-838e-ab7edf4939e9";

    IEnumerator Dialogo_Presentacion()
    {
        yield return StartCoroutine(PlayerDialog("hola como estas?"));
        yield return StartCoroutine(NPCDialog("muy bien y tu que tal"));
        yield return StartCoroutine(PlayerDialog("estupendamente, gracias"));
        GameState.gameData.AddProcessedDialog(DLG_PRESENTACION);
    }

    IEnumerator Dialogo_PostPresentacion()
    {
        yield return StartCoroutine(PlayerDialog("alguna novedad?"));
        yield return StartCoroutine(NPCDialog("todo muy tranquilo aqui"));
        yield return StartCoroutine(PlayerDialog("muy bien"));
    }

    IEnumerator Dialogo_DarLaPocionABob()
    {
        yield return StartCoroutine(NPCDialog("anda, que pocion mas bonita, me la das?"));
        yield return StartCoroutine(ConfirmDialog());
        if (dlgMan.ConfirmResult())
        {
            yield return StartCoroutine(PlayerDialog("toma, te la regalo"));
            yield return StartCoroutine(NPCDialog("gracias!"));
            GameState.gameData.RemoveItemFromInventory(ITEM_POCION);
            player.UpdateSelectedInvItem();
            animator.SetTrigger("Happy");
        } else
        {
            yield return StartCoroutine(PlayerDialog("uy! no se, le he cogido cariño"));
            yield return StartCoroutine(NPCDialog("Ohhhh"));
        }        
    }

    IEnumerator BeginDialog()
    {
        dlgMan.ShowDialogs();
        player.LockMovement(true);

        // se nos ha presentado ya?
        if (GameState.gameData.CheckProcessedDialog(DLG_PRESENTACION))
        {
            // tenemos la pocion
            if (GameState.gameData.CheckInventoryItem(ITEM_POCION))
            {
                // bob ve que tenemos la pocion, nos la pide y se la damos
                yield return StartCoroutine(Dialogo_DarLaPocionABob());
            } else
            {
                // conversacion posterior a la presentacion            
                yield return StartCoroutine(Dialogo_PostPresentacion());
            }

        } else
        {
            // presentacion
            yield return StartCoroutine(Dialogo_Presentacion());
        }

        player.LockMovement(false);
        dlgMan.HideDialogs();

        yield break;
    }

    public override void StartDialog()
    {
        
        StartCoroutine(BeginDialog());
        
    }

}
