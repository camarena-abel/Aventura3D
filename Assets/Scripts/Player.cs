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
    bool userDialogAction = false;
    bool lockedMovement = false;

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

    void RemoveSelectedInvItem()
    {
        string itemGuid = GetInvSelItemGUID();
        if (itemGuid != "")
        {
            GameState.gameData.RemoveItemFromInventory(itemGuid);
            GameState.gameData.invSelItemIndex = 0;
            UpdateSelectedInvItem();
        }
    }

    public bool OnUserDialogAction()
    {
        return userDialogAction;
    }

    public void LockMovement(bool enambled)
    {
        lockedMovement = enambled;
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
                Item item = lastTarget.GetComponent<Item>();
                targetName = item.info.title; // hitInfo.collider.gameObject.name;
            }

            if (hitInfo.collider.gameObject.tag == "Door")
            {
                lastTarget = hitInfo.collider.gameObject;
                targetName = "Puerta";
            }

            if (hitInfo.collider.gameObject.tag == "Puzzle")
            {
                lastTarget = hitInfo.collider.gameObject;
                Puzzle puzzle = lastTarget.GetComponent<Puzzle>();
                targetName = puzzle.PuzzleName;
            }

            if (hitInfo.collider.gameObject.tag == "NPC")
            {
                lastTarget = hitInfo.collider.gameObject;
                NPC_X npc = lastTarget.GetComponent<NPC_X>();
                targetName = npc.NpcName;
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

                if (lastTarget.tag == "Puzzle")
                {
                    Puzzle puzzle = lastTarget.GetComponent<Puzzle>();
                    puzzle.TryToResolve(GetInvSelItemGUID());
                    if (puzzle.RemoveItemWhenUsed)
                    {
                        RemoveSelectedInvItem();
                    }
                }

                if (lastTarget.tag == "NPC")
                {
                    NPC_X npc = lastTarget.GetComponent<NPC_X>();
                    npc.StartDialog();
                }
            }
        }

        // si pulsa otro boton de accion, que cambia el item seleccionado en el inventario
        if (Input.GetButtonDown("Fire2"))
        {
            if (GameState.gameData.inventory.Count > 0)
            {
                GameState.gameData.invSelItemIndex = (GameState.gameData.invSelItemIndex + 1) % GameState.gameData.inventory.Count;
                UpdateSelectedInvItem();
            }
        }

        // si pulsa el boton de accion para cambiar de dialogo
        userDialogAction = false;
        if (Input.GetButtonDown("Fire3"))
        {
            userDialogAction = true;
        }
    }

    void FixedUpdate()
    {
        // si el movimiento esta bloqueado, salimos de aqui
        if (lockedMovement)
            return;

        // movimiento (translacion)
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
