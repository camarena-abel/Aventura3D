using System.Collections;
using UnityEngine;

public class NPC_A : NPC_X
{
    IEnumerator BeginDialog()
    {
        dlgMan.ShowDialogs();
        player.LockMovement(true);

        yield return StartCoroutine(PlayerDialog("hola como estas?"));
        yield return StartCoroutine(NPCDialog("muy bien y tu que tal"));
        yield return StartCoroutine(PlayerDialog("estupendamente, gracias"));

        player.LockMovement(false);
        dlgMan.HideDialogs();

        yield break;
    }

    public override void StartDialog()
    {
        
        StartCoroutine(BeginDialog());
        
    }

}
