using UnityEngine;
using UnityEngine.AI;
using TMPro;
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
    int maxLife;

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

    [SerializeField]
    protected float patrolStoppingDistance = 0.1f;

    [SerializeField]
    protected float attackStoppingDistance = 1.25f;

    [SerializeField]
    TextMeshProUGUI txtLife;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        proximityTriggerZone = GetComponent<SphereCollider>();
        setDestinationTime = setDestMaxTime;
        InitPatrolMode();
        maxLife = life;
        UpdateUI();
    }

    public bool IsDead()
    {
        return life <= 0;
    }

    public void KillEnemy()
    {
        skinnedMeshRenderer.materials = new Material[] { dissolveMaterial, dissolveMaterial };
        animator.SetTrigger("Death");
        agent.enabled = false;
        Destroy(gameObject, dissolveTime);
    }

    public void UpdateUI()
    {
        if (txtLife)
        {
            txtLife.text = life.ToString();

            // cambio de aspecto segun la cantida de vida
            float d = (float)life / (float)maxLife;
            txtLife.color = Color.Lerp(Color.red, Color.green, d);
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead())
        {
            // lo que esta muerto no puede morir!
            return;
        }

        // reducimos la cantidad de vida
        life -= amount;

        // actualizamos contador de vida
        UpdateUI();

        // esta muerto?
        if (life <= 0)
        {
            txtLife.enabled = false;
            KillEnemy(); // muerto!
        } else
        {
            // animacion de impacto
            animator.SetTrigger("Hit");
            // iniciar modo ataque
            InitAttackMode();
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
        tieneDestino = true;
    }

    protected void InitAttackMode()
    {
        targetFound = true;
        patrolMode = false;
        agent.stoppingDistance = attackStoppingDistance;
        fotgotTarget = 10f; // en 10 segundos olviadrá al player si no lo vuelve a ver
    }

    protected void InitPatrolMode()
    {
        targetFound = false;
        patrolMode = true;
        agent.stoppingDistance = patrolStoppingDistance;
    }

    private void Update()
    {
        // ajustamos el UI para que mire hace el player
        if (txtLife)
        {
            Vector3 tgtpos = new Vector3(target.transform.position.x, txtLife.gameObject.transform.position.y, target.transform.position.z);
            txtLife.gameObject.transform.parent.LookAt(tgtpos);
        }

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

        // pillamos el estado actual de la maquina de estados (animation controller)
        var current = animator.GetCurrentAnimatorStateInfo(0);
        agent.isStopped = (current.IsName("Hit") || current.IsName("Attack"));

        if ((targetFound) && (!target.IsDead()))
        {
            // esta cerca para atacar?
            // la distancia al target debe de ser > 0 y menor que la distancia de ataque!
            if ((agent.remainingDistance > 0f) && (agent.remainingDistance <= attackDistance))
            {
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
                InitPatrolMode();
            }
            
        }

        // estamos en modo patrulla?
        if (patrolMode)
        {
            // si ya ha llegado, buscamos el siguiente destino
            if (AgentLlegoAlDestino())
            {
                SetDestination(patrolRoute[patrolNextPoint].position);
                patrolNextPoint = (patrolNextPoint + 1) % patrolRoute.Length;
            }
        }

    }

}
