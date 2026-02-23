using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 roamCenter;
    private float directionTimer;
    private Transform player;
    private bool isChasing = false;
    private bool isFleeing = false;
    
    [Header("Roam Settings")]
    public float roamRadius = 20f;
    public float roamSpeed = 10f;
    public float directionChangeInterval = 4f;
    
    [Header("NPC Settings")]
    public Material NPCMaterial; 
    
    [Header("Combat Behavior")]
    public float chaseRange = 20f;
    public float fleeRange = 25f;
    public float chaseSpeed = 15f;
    public float fleeSpeed = 18f;
    public int npcAdvantageThreshold = 2; 
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        roamCenter = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (agent != null)
        {
            agent.speed = roamSpeed;
            agent.acceleration = 10000f;
            agent.angularSpeed = 10000f;
            agent.stoppingDistance = 2f;
            agent.autoBraking = true;
            agent.updateRotation = false;
        }
        
        SetNewRoamDestination();
        directionTimer = directionChangeInterval;
    }
    
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) return;
        }
        
        NPCCounter myCounter = GetComponent<NPCCounter>();
        NPCCounter playerCounter = player.GetComponent<NPCCounter>();
        
        if (myCounter != null && playerCounter != null)
        {
            int myNPCs = myCounter.GetNPCCount();
            int playerNPCs = playerCounter.GetNPCCount();
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (myNPCs >= playerNPCs + npcAdvantageThreshold && distanceToPlayer < chaseRange)
            {
                ChasePlayer();
                return;
            }
            else if (playerNPCs >= myNPCs + npcAdvantageThreshold && distanceToPlayer < fleeRange)
            {
                FleeFromPlayer();
                return;
            }
        }
        
        if (isChasing || isFleeing)
        {
            isChasing = false;
            isFleeing = false;
            agent.speed = roamSpeed;
            SetNewRoamDestination();
            directionTimer = directionChangeInterval;
        }
        
        Roam();
    }

    // --- BEHAVIOR METHODS ---

    void ChasePlayer()
    {
        isChasing = true;
        isFleeing = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }
    
    void FleeFromPlayer()
    {
        isChasing = false;
        isFleeing = true;
        agent.speed = fleeSpeed;
        
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleePosition = transform.position + fleeDirection * fleeRange;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, fleeRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(transform.position + fleeDirection * 10f);
        }
    }
    
    void Roam()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f || agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNewRoamDestination();
            directionTimer = directionChangeInterval;
        }
    }
    
    void SetNewRoamDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius + roamCenter;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}