using UnityEngine;

public class Item : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material material;
    public string instanceGUID; // identificador de esta instancia del objeto
    public ItemInfo info;

    void Start()
    {
        // si el objeto ya fue recogido en otra partida anterior, destruirlo
        if (GameState.gameData.pickedItems.Contains(instanceGUID))
        {
            Destroy(gameObject);
            return;
        }

        // accedemos al mesh renderer del primer hijo
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        material.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        float v = Mathf.PingPong(Time.time, 1f) * 2f; 
        material.SetColor("_EmissionColor", new Color(v, v, v));
        //material.color = new Color(v, v, v);
    }

}
