using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyA_Attack : MonoBehaviour
{
    EnemyA enemy;
    private void Start()
    {
        enemy = GetComponentInParent<EnemyA>();
    }

    public void Attack()
    {
        enemy.Attack();
    }
}
