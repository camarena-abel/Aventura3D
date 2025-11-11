using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    int YesOrNo = 0; // 1 = si, -1 = no
    bool dialogsActive = false;

    [SerializeField]
    Image panDialog;

    [SerializeField]
    TextMeshProUGUI txtAvatar;

    [SerializeField]
    TextMeshProUGUI txtMessage;

    [SerializeField]
    Image panConfirm;

    public void ShowDialogs()
    {
        panDialog.gameObject.SetActive(true);
        dialogsActive = true;
    }

    public void HideDialogs()
    {
        panDialog.gameObject.SetActive(false);
        dialogsActive = false;
    }

    public bool IsDialogsActive()
    {
        return dialogsActive;
    }

    public void SetMessage(Color color, string avatar, string msg)
    {
        txtAvatar.color = color;
        txtAvatar.text = avatar;
        txtMessage.text = msg;
    }

    public void HideConfirm()
    {
        panConfirm.gameObject.SetActive(false);
    }

    public void ShowConfirm()
    {
        YesOrNo = 0;
        panConfirm.gameObject.SetActive(true);
    }

    public bool Confirmed()
    {
        return YesOrNo != 0;
    }

    public bool ConfirmResult()
    {
        return YesOrNo == 1;
    }

    public void ConfirmDialogYes()
    {
        YesOrNo = 1;
        HideConfirm();
    }

    public void ConfirmDialogNo()
    {
        YesOrNo = -1;
        HideConfirm();
    }

    void Start()
    {
        HideDialogs();
        HideConfirm();
    }

}
