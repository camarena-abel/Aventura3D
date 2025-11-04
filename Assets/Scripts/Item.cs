using UnityEngine;

public class Item : MonoBehaviour
{
    public string instanceGUID; // identificador de esta instancia del objeto
    public ItemInfo info;

    void Start()
    {
        if (GameState.gameData.pickedItems.Contains(instanceGUID))
        {
            Destroy(gameObject);
        }
    }

}
