using UnityEngine;
using UnityEngine.AI;

public class EnemyX : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    float setDestinationTime;
    protected float fotgotTarget = 0f;
    protected bool targetFound = false;
    protected SphereCollider proximityTriggerZone;
    float attackTimeCounter = 0f;

    [SerializeField]
    int life = 100;

    [SerializeField]
    float setDestMaxTime = 2f;

    [SerializeField]
    protected Player target; // target del enemigo

    [SerializeField]
    float attackDistance = 1.2f;

    [SerializeField]
    float attackTime = 0.2f;

    [SerializeField]
    float attackRadius = 0.5f;

    [SerializeField]
    LayerMask attackLayerMask;

    [SerializeField]
    int attackDamage = 10;

    [SerializeField]
    float attackForward = 1f; // para calcular la posicion donde hará daño

    [SerializeField]
    float attackUp= 1f;       // para calcular la posicion donde hará daño

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        proximityTriggerZone = GetComponent<SphereCollider>();
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

        // esta atacando?
        if (attackTimeCounter > 0f)
        {
            attackTimeCounter -= Time.deltaTime;
            if (attackTimeCounter <= 0f)
            {
                // atacamos 1m hacia adelante y 1m hacia arriba de la posicion del enemigo
                Vector3 attackPos = transform.position + transform.forward * attackForward + transform.up * attackUp;
                Collider[] col = Physics.OverlapSphere(attackPos, attackRadius, attackLayerMask);
                if (col.Length > 0)
                {
                    // hemos golpeado al player!
                    Player play = target.GetComponent<Player>();
                    play.ReceiveDamage(attackDamage);
                }
                attackTimeCounter = 0f;
            }
        }

        // obtenemos la velocidad (para la animacion de caminar
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if ((targetFound) && (!target.IsDead()))
        {
            // esta cerca para atacar?
            // la distancia al target debe de ser > 0 y menor que la distancia de ataque!
            if ((agent.isStopped == false) && (agent.remainingDistance > 0f) && (agent.remainingDistance <= attackDistance))
            {
                // le decimos que se pare
                agent.isStopped = true;
                // le decimos que ataque
                animator.SetTrigger("Attack");
                attackTimeCounter = attackTime;
            }

            // busca el target (el sitio en el que esta)
            setDestinationTime -= Time.deltaTime;
            if (setDestinationTime <= 0f)
            {
                agent.SetDestination(target.transform.position);
                agent.isStopped = false;
                setDestinationTime = setDestMaxTime;
            }

            // vamos olvidando al player
            fotgotTarget -= Time.deltaTime;
            if (fotgotTarget <= 0f)
            {
                targetFound = false;
            }



        }


    }

}
