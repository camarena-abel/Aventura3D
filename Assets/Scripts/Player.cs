using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    AudioSource audio;
    Rigidbody rb;
    float axisH;
    float axisV;
    float cameraPitchAngle = 0f;
    float footStepsTime = 0f;
    GameObject lastTarget;
    bool userDialogAction = false;
    bool lockedMovement = false;
    bool holdingSomething = false;
    bool isDead = false;
    Transform holdedObjectOriginalParent;

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
    GameOver gameOver;

    [SerializeField]
    ItemDB itemDB;

    [SerializeField]
    AudioClip soundShoot;

    [SerializeField]
    AudioClip soundPickItem;

    [SerializeField]
    AudioClip[] soundFootSteps;

    [SerializeField]
    AudioClip soundHit;

    [SerializeField]
    Transform holdPosition;

    [SerializeField]
    DialogManager dialogManager;

    [SerializeField]
    LayerMask shotLayers; // contra que capas colisiona el disparo

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        audio = GetComponent<AudioSource>();
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

    public void UpdateSelectedInvItem()
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
        Physics.queriesHitTriggers = true;
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

            if (hitInfo.collider.gameObject.tag == "Pickable")
            {
                lastTarget = hitInfo.collider.gameObject;
                targetName = lastTarget.gameObject.name;
            }
        }
        if (targetName != txtTarget.text)
        {
            txtTarget.text = targetName;
        }

        // si pulsa el boton de accion, recoge el objeto
        if (Input.GetButtonDown("Fire1"))
        {
            if (holdingSomething)
            {
                Transform ho = holdPosition.GetChild(0);
                Rigidbody rb = ho.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                Collider col = ho.GetComponent<Collider>();
                col.enabled = true;
                ho.transform.SetParent(holdedObjectOriginalParent);
                holdingSomething = false;
            }

            if (lastTarget)
            {
                // coger un item del escenario
                if (lastTarget.tag == "Item")
                {
                    Item item = lastTarget.GetComponent<Item>();
                    GameState.gameData.AddItemToInventory(item.info.guid);
                    GameState.gameData.AddPickedItem(item.instanceGUID);
                    UpdateSelectedInvItem();
                    Destroy(lastTarget);
                    // hacemos el sonido de recoger el item
                    audio.pitch = Random.Range(0.75f, 1.5f);
                    audio.PlayOneShot(soundPickItem);
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

                if (lastTarget.tag == "Pickable")
                {
                    if (!holdingSomething)
                    {
                        holdedObjectOriginalParent = lastTarget.transform.parent;
                        lastTarget.transform.SetParent(holdPosition);
                        Rigidbody rb = lastTarget.GetComponent<Rigidbody>();
                        rb.isKinematic = true;
                        Collider col = lastTarget.GetComponent<Collider>();
                        col.enabled = false;
                        lastTarget.transform.localPosition = Vector3.zero;
                        lastTarget.transform.localRotation = Quaternion.identity;
                        holdingSomething = true;
                    }
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
            if (dialogManager.IsDialogsActive())
            {
                // siguiente dialogo
                userDialogAction = true;
            } else
            {
                // disparo de arma de fuego
                RaycastHit shotInfo;
                Physics.queriesHitTriggers = false;
                if (Physics.Raycast(camera.position, camera.forward, out shotInfo, Mathf.Infinity, shotLayers))
                {
                    if (shotInfo.collider.gameObject.tag == "Enemy")
                    {
                        EnemyX enemy = shotInfo.collider.GetComponent<EnemyX>();
                        enemy.TakeDamage(25);

                        // hacemos el sonido de recoger el item
                        audio.pitch = Random.Range(0.75f, 1.5f);
                        audio.PlayOneShot(soundShoot);                        
                    }                        

                }
            }

        }
    }

    void FixedUpdate()
    {
        // si el movimiento esta bloqueado, salimos de aqui
        if (lockedMovement)
            return;

        // movimiento (translacion)
        Vector3 displacement = ((transform.forward * axisV) + (transform.right * axisH)) * speed * Time.fixedDeltaTime;
        Vector3 newPos = transform.position + displacement;
        rb.MovePosition(newPos);

        // contador para las pisadas
        footStepsTime -= displacement.magnitude;
        if (footStepsTime <= 0f)
        {
            int randomFootStepIndex = Random.Range(0, soundFootSteps.Length-1);
            audio.pitch = 1f;
            audio.PlayOneShot(soundFootSteps[randomFootStepIndex]);
            footStepsTime = 2f;
        }
        


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SavePoint")
        {
            // hacemos un sonido
            SavePoint sp = other.GetComponent<SavePoint>();
            sp.PlaySound();
            // guardamos la partida
            GameState.Save();
        }
    }

    public void ReceiveDamage(int amount)
    {
        GameState.gameData.life -= amount;
        audio.PlayOneShot(soundHit);
        if (GameState.gameData.life <= 0)
        {
            GameOver();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    IEnumerator FinalizarJuego()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MainMenu");
        yield break;
    }

    private void GameOver()
    {        
        lockedMovement = true;
        isDead = true;
        gameOver.Play();
        StartCoroutine(FinalizarJuego());
    }
}
