using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    Image panDialog;

    [SerializeField]
    TextMeshProUGUI txtAvatar;

    [SerializeField]
    TextMeshProUGUI txtMessage;

    public void ShowDialogs()
    {
        panDialog.gameObject.SetActive(true);
    }

    public void HideDialogs()
    {
        panDialog.gameObject.SetActive(false);
    }

    public void SetMessage(Color color, string avatar, string msg)
    {
        txtAvatar.color = color;
        txtAvatar.text = avatar;
        txtMessage.text = msg;
    }

    void Start()
    {
        HideDialogs();
    }

}
