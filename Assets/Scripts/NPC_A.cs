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
    private void Update()
    {
        // calculamos la distancia entre el NPC y el player
        float d = Vector3.Distance(transform.position, player.transform.position);
        if (d < 3f) // si esta cerca del  player...
        {
            Transform t = transform;
            t.LookAt(player.transform);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, t.rotation, Time.deltaTime);
        } 
        
    }
}
