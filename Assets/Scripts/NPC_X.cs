using UnityEngine;
using System.Collections;

/*
 
CoRutinas

  yield break;       // finalizar la corutina
  yield return null; // esperar 1 fotograma
  yield return new WaitForSeconds(n);          // esperar n segundos (si el juego esta pausado durará más)
  yield return new WaitForSecondsRealtime(n);  // esperar n segundos (da igual si el juego esta pausado)
  yield return StartCoroutine(OtraCorutina()); // espera a que termine otra corutina  
  yield return new WaitUntil(OtraFuncion); // OtraFuncion debe de ser una funcion normal que devuelva true o false
  yield return new WaitWhile(OtraFuncion); // es lo mismo que antes pero al reves

*/

public abstract class NPC_X : MonoBehaviour
{
    public static Color PlayerColor = Color.green;

    [SerializeField]
    string npcName;

    [SerializeField]
    protected Color color = Color.red;

    [SerializeField]
    protected DialogManager dlgMan;

    [SerializeField]
    protected Player player;

    [SerializeField]
    float rotSpeed = 90f;

    [SerializeField]
    float playerLookAtDistance = 3f;

    public string NpcName { get => npcName; }

    public abstract void StartDialog();

    protected IEnumerator PlayerDialog(string msg)
    {
        dlgMan.SetMessage(PlayerColor, "Player", msg);
        yield return new WaitUntil(() => player.OnUserDialogAction());
        yield return null;
    }

    protected IEnumerator NPCDialog(string msg)
    {
        dlgMan.SetMessage(color, NpcName, msg);
        yield return new WaitUntil(() => player.OnUserDialogAction());
        yield return null;
    }

    private void Update()
    {
        // calculamos la distancia entre el NPC y el player
        Vector3 diff = player.transform.position - transform.position;
        float d = diff.magnitude;
        if (d < playerLookAtDistance) // si esta cerca del  player...
        {
            // que se gire en dirección al player
            Quaternion q = Quaternion.LookRotation(diff);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotSpeed * Time.deltaTime);
        }

    }
}
