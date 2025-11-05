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
}
