using UnityEngine;
using UnityEngine.AI;

public class EnemyX : MonoBehaviour
{
    NavMeshAgent agent;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Animator animator;
    float setDestinationTime;
    float killedTime;
    protected float fotgotTarget = 0f;
    protected bool targetFound = false;
    protected SphereCollider proximityTriggerZone;
    protected bool patrolMode = true;
    protected int patrolNextPoint = 0;
    bool tieneDestino = false;

    [SerializeField]
    int life = 100;

    [SerializeField]
    float setDestMaxTime = 2f;

    [SerializeField]
    float forgotTime = 10f;

    [SerializeField]
    protected Player target; // target del enemigo

    [SerializeField]
    protected LayerMask attackLayerMask;

    [SerializeField]
    float attackDistance = 1.2f;

    [SerializeField]
    protected int attackDamage = 10;

    [SerializeField]
    protected Material dissolveMaterial;

    [SerializeField]
    protected float dissolveTime = 5f;

    [SerializeField]
    protected Transform[] patrolRoute;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        proximityTriggerZone = GetComponent<SphereCollider>();
        setDestinationTime = setDestMaxTime;        
    }

    public bool IsDead()
    {
        return life <= 0;
    }

    public void KillEnemy()
    {
        skinnedMeshRenderer.materials = new Material[] { dissolveMaterial, dissolveMaterial };
        animator.SetTrigger("Death");
        agent.isStopped = true;
        agent.enabled = false;
        Destroy(gameObject, dissolveTime);
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
            KillEnemy(); // muerto!
        } else
        {
            animator.SetTrigger("Hit");
        }
    }

    bool AgentLlegoAlDestino()
    {
        if (!tieneDestino) return true;

        if (agent.pathPending)
            return false;

        if (agent.remainingDistance > agent.stoppingDistance)
            return false;

        if (agent.hasPath && agent.velocity.sqrMagnitude > 0f)
            return false;

        return true;
    }

    private void SetDestination(Vector3 v)
    {
        agent.SetDestination(v);
        agent.isStopped = false;
        tieneDestino = true;
    }

    private void Update()
    {
        // si esta muerto, no se puede mover
        if (IsDead())
        {
            killedTime += Time.deltaTime;
            dissolveMaterial.SetFloat("_Strength", killedTime / dissolveTime);
            return;
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
            }
            
            // busca el target (el sitio en el que esta)
            setDestinationTime -= Time.deltaTime;
            if (setDestinationTime <= 0f)
            {
                patrolMode = false;
                SetDestination(target.transform.position);                
                setDestinationTime = setDestMaxTime;
                fotgotTarget = forgotTime;
            }

            // vamos olvidando al player
            fotgotTarget -= Time.deltaTime;
            if (fotgotTarget <= 0f)
            {
                targetFound = false;
                patrolMode = true;
            }
            
        }

        // estamos en modo patrulla?
        if (patrolMode)
        {
            // si ya ha llegado, buscamos el siguiente destino
            if (AgentLlegoAlDestino())
            {
                print(patrolNextPoint);
                print(patrolRoute[patrolNextPoint].position);
                SetDestination(patrolRoute[patrolNextPoint].position);
                patrolNextPoint = (patrolNextPoint + 1) % patrolRoute.Length;
            }
        }

    }

}
