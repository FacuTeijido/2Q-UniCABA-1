/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]

public abstract class Enemy : Character
{
    [SerializeField] protected EnemyData data;
    [SerializeField] float eyeHeight = 1.5f; // altura desde la que "mira"

    public static event Action<Enemy> OnEnemyKilled;
    public static event Action<Enemy> OnEnemySpawned;

    // ---- Acceso para los estados ----
    public EnemyData Data => data;
    public Player Target => target;
    public Vector3 HomePosition { get; private set; }
    public Vector3 LastKnownPosition { get; private set; }
    public bool CanSeeTarget { get; private set; }
    public bool IsBusy => isBusy;

    public IdleState StateIdle { get; private set; }
    public PatrolState StatePatrol { get; private set; }
    public ChaseState StateChase { get; private set; }
    public AttackState StateAttack { get; private set; }

    // Solo para depurar: muestra el estado actual en el Inspector
    [field: SerializeField] public string CurrentStateName { get; private set; }

    protected Rigidbody rb;
    protected Player target;
    protected bool isBusy; // true mientras ejecuta un ataque largo (ej: embestida)

    Collider myCollider;
    Collider targetCollider;
    StateMachine stateMachine;
    float nextAttackTime;
    Vector3 desiredVelocity;
    Vector3 faceDirection;
    bool started; // true tras el primer Start: desde ahí, OnEnable reinicia al enemigo

    protected override void Awake()
    {
        if (data != null) maxHealth = data.maxHealth; // antes de base.Awake()
        else Debug.LogError($"{name}: falta asignar el EnemyData.");

        base.Awake();
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        // La FSM se arma una sola vez por instancia
        StateIdle = new IdleState(this);
        StatePatrol = new PatrolState(this);
        StateChase = new ChaseState(this);
        StateAttack = new AttackState(this);
        stateMachine = new StateMachine();
        OnHealthChanged += HandleHealthChanged; // una sola suscripción por instancia
    }

    protected virtual void Start()
    {
        started = true;
        ResetEnemy();
    }

    void OnEnable()
    {
        NoiseSystem.OnNoise += HandleNoise;

        // Cuando el pool reactiva un enemigo ya usado, vuelve a empezar de cero
        if (started) ResetEnemy();
    }

    void OnDisable()
    {
        // Importante con el pool: un enemigo desactivado no debe seguir escuchando
        NoiseSystem.OnNoise -= HandleNoise;
    }

    // Deja al enemigo como recién creado
    void ResetEnemy()
    {
        target = GameManager.Instance.player;
        targetCollider = target != null ? target.GetComponent<Collider>() : null;

        HomePosition = transform.position;     // su "casa" es donde lo spawnearon
        LastKnownPosition = transform.position;

        Revive(); // vida completa e IsDead = false

        isBusy = false;
        nextAttackTime = 0f;
        CanSeeTarget = false;
        desiredVelocity = Vector3.zero;
        faceDirection = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        stateMachine.Clear();
        ChangeState(StateIdle);

        OnSpawned();
        OnEnemySpawned?.Invoke(this);
        
        path.Clear();
        pathIndex = 0;
        repathTimer = 0f;
        useDirectRoute = true;
               
    }

    // Hook para que cada tipo reinicie lo suyo (se sobreescribe en las clases hijas)
    protected virtual void OnSpawned() { }

    

    void Update()
    {
        if (IsDead || data == null || target == null)
        {
            Stop();
            return;
        }

        // Percepción: una vez por frame, antes de que los estados decidan
        CanSeeTarget = EvaluateVision();
        if (CanSeeTarget) LastKnownPosition = target.transform.position;

        stateMachine.Tick();
    }

    void FixedUpdate()
    {
        if (IsDead) return;

        Vector3 v = desiredVelocity;
        v.y = rb.linearVelocity.y;
        rb.linearVelocity = v;

        if (faceDirection.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(faceDirection);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, look, data.turnSpeed * Time.fixedDeltaTime));
        }
    }

    // ---- FSM ----

    public void ChangeState(EnemyState next)
    {
        stateMachine.ChangeState(next);
        CurrentStateName = stateMachine.Current.GetType().Name;
    }

    // Avisa al enemigo de que algo pasó en una posición (daño, y más adelante, ruido)
    public void Alert(Vector3 position)
    {
        LastKnownPosition = position;

        if (stateMachine.Current == StateIdle || stateMachine.Current == StatePatrol)
            ChangeState(StateChase);
        else if (stateMachine.Current == StateChase)
            StateChase.RefreshSearch(); // oyó algo nuevo: reinicia el tiempo de búsqueda
    }
    
    void HandleHealthChanged(float current, float max)
    {
        if (current >= max) return; // reinicio o curación, no es daño
        if (IsDead || target == null) return;

        Alert(target.transform.position); // "me pegaron": voy a ver quién fue
    }

    // ---- Visión: distancia + ángulo + línea de visión ----

    bool EvaluateVision()
    {
        if (target.IsDead) return false;

        // 1. Distancia
        if (DistanceToTarget() > data.detectionRange) return false;

        // 2. Ángulo: ¿está dentro del cono de visión?
        Vector3 toTarget = target.transform.position - transform.position;
        toTarget.y = 0f;
        if (Vector3.Angle(transform.forward, toTarget) > data.viewAngle * 0.5f) return false;

        // 3. Línea de visión: ¿hay algo en el medio?
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 aimPoint = targetCollider != null
            ? targetCollider.bounds.center
            : target.transform.position + Vector3.up;

        if (Physics.Linecast(eye, aimPoint, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.GetComponentInParent<Player>() == target;

        return true;
    }

    // Distancia entre las superficies de ambos colliders (0 = se están tocando)
    public float DistanceToTarget()
    {
        if (targetCollider == null || myCollider == null)
            return Vector3.Distance(transform.position, target.transform.position);

        Vector3 onTarget = targetCollider.ClosestPoint(myCollider.bounds.center);
        Vector3 onMe = myCollider.ClosestPoint(onTarget);
        return Vector3.Distance(onMe, onTarget);
    }



        // ---- Navegación con pathfinding ----

    const float RepathInterval = 0.25f; // recalcula la ruta 4 veces por segundo, no cada frame

    readonly List<Vector3> path = new();
    int pathIndex;
    float repathTimer;
    bool useDirectRoute = true;
    const float WaypointTolerance = 1.5f;
    NavMeshPath navPath;


   public void NavigateTo(Vector3 goal, float speed)
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = RepathInterval;
            RecalculateRoute(goal);
        }

        // Camino libre: en línea recta
        if (useDirectRoute)
        {
            Move(goal - transform.position, speed);
            return;
        }

        // Hay obstáculo y el NavMesh no encontró ruta
        if (path.Count == 0)
        {
            Stop();
            return;
        }

        Vector3 waypoint = path[Mathf.Min(pathIndex, path.Count - 1)];
        Vector3 toWaypoint = waypoint - transform.position;
        toWaypoint.y = 0f;

        if (toWaypoint.magnitude < WaypointTolerance && pathIndex < path.Count - 1)
            pathIndex++;

        Move(toWaypoint, speed);
    }

    void RecalculateRoute(Vector3 goal)
    {
        path.Clear();
        pathIndex = 0;
        useDirectRoute = false;

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit from, 10f, NavMesh.AllAreas)) return;
        if (!NavMesh.SamplePosition(goal, out NavMeshHit to, 10f, NavMesh.AllAreas)) return;

        // Si no hay obstáculos en el medio, no hace falta calcular ruta
        if (!NavMesh.Raycast(from.position, to.position, out _, NavMesh.AllAreas))
        {
            useDirectRoute = true;
            return;
        }

        if (NavMesh.CalculatePath(from.position, to.position, NavMesh.AllAreas, navPath)
            && navPath.status == NavMeshPathStatus.PathComplete)
        {
            for (int i = 1; i < navPath.corners.Length; i++) // la esquina 0 es donde ya está
                path.Add(navPath.corners[i]);
        }
    }


    // ---- Herramientas que usan los estados ----

    protected void Move(Vector3 direction, float speed)
    {
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
        {
            Stop();
            return;
        }

        direction.Normalize();
        desiredVelocity = direction * speed;
        faceDirection = direction;
    }

    public void MoveTowards(Vector3 point)
    {
        Move(point - transform.position, data.moveSpeed);
    }

    public void MoveTowards(Vector3 point, float speed)
    {
        Move(point - transform.position, speed);
    }

    public void FaceTowards(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        dir.y = 0f;
        faceDirection = dir.normalized;
    }

    public void Stop()
    {
        desiredVelocity = Vector3.zero;
    }

    // El cooldown es común a todos; lo que cambia es CÓMO se ataca
    public void TryAttack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + data.attackCooldown;
        Attack();
    }

    // Cada tipo de enemigo define su ataque (polimorfismo)
    protected abstract void Attack();

    protected override void Die()
    {
        Stop();
        OnEnemyKilled?.Invoke(this);
        PoolManager.Instance.Release(gameObject); // vuelve al pool en vez de destruirse
    }

    void OnDrawGizmosSelected()
    {
        if (data == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);

        // Cono de visión
        Gizmos.color = Color.cyan;
        Vector3 left = Quaternion.Euler(0f, -data.viewAngle * 0.5f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, data.viewAngle * 0.5f, 0f) * transform.forward;
        Gizmos.DrawRay(transform.position, left * data.detectionRange);
        Gizmos.DrawRay(transform.position, right * data.detectionRange);


        // Ruta de A* (verde), solo cuando está siguiendo un camino
        if (!useDirectRoute)
        {
            Gizmos.color = Color.green;
            Vector3 prev = transform.position;
            for (int i = pathIndex; i < path.Count; i++)
            {
                Gizmos.DrawLine(prev + Vector3.up, path[i] + Vector3.up);
                prev = path[i];
            }
        }
    }
    void HandleNoise(Vector3 position, float radius)
    {
        if (IsDead || data == null) return;

        Vector3 flat = position - transform.position;
        flat.y = 0f;

        float heardRadius = radius * data.hearingSensitivity;
        if (flat.magnitude > heardRadius) return;

        Alert(position); // el mismo punto de entrada que usa el daño
    }
}