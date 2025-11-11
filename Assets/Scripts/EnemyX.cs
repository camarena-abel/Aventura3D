using UnityEngine;
using UnityEngine.AI;

public class EnemyX : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    float setDestinationTime;
    protected bool targetFound = false;

    [SerializeField]
    int life = 100;

    [SerializeField]
    float setDestMaxTime = 2f;

    [SerializeField]
    Transform target; // target del enemigo

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        setDestinationTime = setDestMaxTime;
    }

    public bool IsDead()
    {
        return life <= 0;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead())
        {
            // lo que esta muerto no puede morir!
            return;
        }

        life -= amount;
        if (life <= 0)
        {
            // muerto!
            animator.SetTrigger("Death");
            agent.isStopped = true;
            agent.enabled = false;
            Destroy(gameObject, 5f);
        } else
        {
            animator.SetTrigger("Hit");
        }
    }

    private void Update()
    {
        // si esta muerto, no se puede mover
        if (IsDead())
            return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (targetFound)
        {
            setDestinationTime -= Time.deltaTime;
            if (setDestinationTime <= 0f)
            {
                agent.SetDestination(target.position);
                setDestinationTime = setDestMaxTime;
            }
        }


    }

}
