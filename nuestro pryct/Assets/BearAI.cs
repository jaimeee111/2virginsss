using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Necesario para movimiento si usas NavMesh

public class BearAI : MonoBehaviour
{
    public float detectionRange = 10f; // Rango de detección del jugador
    public float attackRange = 2f; // Rango de ataque
    public float patrolSpeed = 2f; // Velocidad en modo patrullaje
    public float chaseSpeed = 5f; // Velocidad en modo persecución
    public int damage = 20; // Daño que inflige al jugador
    public float attackCooldown = 2f; // Tiempo entre ataques

    private Transform player; // Referencia al jugador
    private NavMeshAgent agent; // Componente para navegación
    private Animator animator; // Animador para las animaciones del oso
    private float attackCooldownTimer; // Temporizador para el ataque
    private bool isChasing = false; // Indicador de si está persiguiendo

    // Estados de la IA
    private enum BearState { Idle, Patrol, Chase, Attack }
    private BearState currentState = BearState.Patrol;

    private Vector3 patrolTarget; // Objetivo al patrullar

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attackCooldownTimer = 0f;

        // Configurar primer objetivo de patrulla
        SetRandomPatrolTarget();
    }

    void Update()
    {
        attackCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BearState.Patrol:
                Patrol();
                break;
            case BearState.Chase:
                Chase();
                break;
            case BearState.Attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        // Movimiento hacia el objetivo de patrulla
        agent.speed = patrolSpeed;
        agent.SetDestination(patrolTarget);

        // Cambiar objetivo si llega al punto actual
        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            SetRandomPatrolTarget();
        }

        // Detectar al jugador
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = BearState.Chase;
            isChasing = true;
        }

        // Actualizar animaciones
        if (animator) animator.SetBool("isMoving", true);
    }

    private void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        // Si el jugador está dentro del rango de ataque
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = BearState.Attack;
        }
        else if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            // Perder al jugador y volver a patrullar
            isChasing = false;
            currentState = BearState.Patrol;
            SetRandomPatrolTarget();
        }

        // Actualizar animaciones
        if (animator) animator.SetBool("isMoving", true);
    }

    private void Attack()
    {
        agent.SetDestination(transform.position); // Detener al oso
        if (attackCooldownTimer <= 0f)
        {
            // Asume que tienes un script de daño en el jugador
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            // Reiniciar temporizador de ataque
            attackCooldownTimer = attackCooldown;

            // Animación de ataque (si tienes un animador configurado)
            if (animator) animator.SetTrigger("attack");
        }

        // Volver a perseguir si el jugador está fuera del rango de ataque
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = BearState.Chase;
        }
    }

    private void SetRandomPatrolTarget()
    {
        patrolTarget = transform.position + new Vector3(
            Random.Range(-5f, 5f),
            0,
            Random.Range(-5f, 5f)
        );
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el rango de detección y ataque para debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
