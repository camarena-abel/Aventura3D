using UnityEngine;
using UnityEngine.AI;

public class EnemyX : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    float setDestinationTime;
    protected bool targetFound = false;

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

    private void Update()
    {
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
