using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    float axisH;
    float axisV;
    float cameraPitchAngle = 0f;
    GameObject lastTarget;

    [SerializeField]
    Transform camera;

    [SerializeField]
    float speed = 2f;

    [SerializeField]
    float sensibility = 10f;

    [SerializeField]
    float targetMaxDist = 3f;

    [SerializeField]
    LayerMask targetLayers; // contra que capas colisiona el rayo de la camara

    [SerializeField]
    TextMeshProUGUI txtTarget;

    [SerializeField]
    Image imgIcono;

    [SerializeField]
    ItemDB itemDB;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        UpdateSelectedInvItem();
    }

    string GetInvSelItemGUID()
    {
        if ((GameState.gameData.invSelItemIndex >= 0) && (GameState.gameData.invSelItemIndex < GameState.gameData.inventory.Count))
        {
            string itemGuid = GameState.gameData.inventory[GameState.gameData.invSelItemIndex];
            return itemGuid;
        }
        return "";
    }

    void UpdateSelectedInvItem()
    {
        string itemGuid = GetInvSelItemGUID();
        if (itemGuid != "")
        {
            ItemInfo info = itemDB.GetItemInfo(itemGuid);
            if (info)
            {
                imgIcono.sprite = info.icono;
            }
        }
    }


    void Update()
    {
        axisH = Input.GetAxis("Horizontal");
        axisV = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X") * sensibility;
        float mouseY = Input.GetAxis("Mouse Y") * sensibility;

        // rotacion de la capsula en el eje Y (yaw)
        Quaternion newRot = transform.rotation * Quaternion.Euler(0f, mouseX, 0f);
        rb.MoveRotation(newRot);

        // rotacion de la camara en el eje X (pitch)
        cameraPitchAngle = Mathf.Clamp(cameraPitchAngle - mouseY, -75f, 75f);
        camera.localRotation = Quaternion.Euler(cameraPitchAngle, 0f, 0f);

        // lanzamos un rayo desde la camara para ver si hay algo interesante delante nuestro
        lastTarget = null;
        string targetName = "";
        RaycastHit hitInfo;
        if (Physics.Raycast(camera.position, camera.forward, out hitInfo, targetMaxDist, targetLayers))
        {
            if (hitInfo.collider.gameObject.tag == "Item")
            {
                lastTarget = hitInfo.collider.gameObject;
                targetName = hitInfo.collider.gameObject.name;
            }

            if (hitInfo.collider.gameObject.tag == "Door")
            {
                lastTarget = hitInfo.collider.gameObject;
                targetName = "Puerta";
            }
        }
        if (targetName != txtTarget.text)
        {
            txtTarget.text = targetName;
        }

        // si pulsa el boton de accion, recoge el objeto
        if (Input.GetButtonDown("Fire1"))
        {
            if (lastTarget)
            {
                if (lastTarget.tag == "Item")
                {
                    Item item = lastTarget.GetComponent<Item>();
                    GameState.gameData.AddItemToInventory(item.info.guid);
                    GameState.gameData.AddPickedItem(item.instanceGUID);
                    UpdateSelectedInvItem();
                    Destroy(lastTarget);
                }

                if (lastTarget.tag == "Door")
                {
                    Door door = lastTarget.transform.parent.GetComponent<Door>();
                    door.TryToOpen(GetInvSelItemGUID());
                }
            }
        }

        // si pulsa otro boton de accion, que cambia el item seleccionado en el inventario
        if (Input.GetButtonDown("Fire2"))
        {
            GameState.gameData.invSelItemIndex = (GameState.gameData.invSelItemIndex + 1) % GameState.gameData.inventory.Count;
            UpdateSelectedInvItem();
        }
    }

    void FixedUpdate()
    {
        Vector3 newPos = transform.position + ((transform.forward * axisV) + (transform.right * axisH)) * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SavePoint")
        {
            // guardamos la partida
            GameState.Save();
        }
    }
}
