using UnityEngine;
using UnityEngine.UI;

public class CanvasDamage : MonoBehaviour
{
    Animator animator;
    Material damage;
    float lastStr = 0f;

    [SerializeField]
    float strength = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        Image img = GetComponent<Image>();
        damage = img.material;
        UpdateShader();
    }

    void UpdateShader()
    {
        damage.SetFloat("_Strength", strength);
    }

    // Update is called once per frame
    void Update()
    {
        if (lastStr != strength)
            UpdateShader();
        lastStr = strength;
    }
    public void PlayAnimation()
    {
        animator.SetTrigger("Damage");
    }
}
