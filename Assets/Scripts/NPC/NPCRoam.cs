using UnityEngine;
using UnityEngine.AI;

public class NPCRoam : MonoBehaviour
{
    private NavMeshAgent agent;
    private Renderer rend;
    private Transform targetLeader;
    private bool isFollowing = false;
    private Material originalMaterial;
    private Vector3 roamCenter;
    
    [Header("Roam Settings")]
    public float roamRadius = 15f;
    public float roamSpeed = 8f;
    public float directionChangeInterval = 3f;
    private float directionTimer;
    
    [Header("Detection Settings")]
    public float detectionRange = 15f;
    public float followSpeed = 15f;
    public Material playerFollowMaterial;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();
        roamCenter = transform.position;
        
        if (rend != null) originalMaterial = rend.material;
        
        if (agent != null)
        {
            agent.speed = roamSpeed;
            agent.acceleration = 10000f;
            agent.angularSpeed = 10000f;
            agent.stoppingDistance = 2f;
            agent.updateRotation = false;
        }
        
        SetNewRoamDestination();
        directionTimer = directionChangeInterval;
    }
    
    void Update()
    {
        if (!isFollowing)
        {
            agent.speed = roamSpeed;
            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0f || agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewRoamDestination();
                directionTimer = directionChangeInterval;
            }
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            float playerDistance = player != null ? Vector3.Distance(transform.position, player.transform.position) : Mathf.Infinity;
            float closestEnemyDistance = Mathf.Infinity;
            Transform closestEnemy = null;
            
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < closestEnemyDistance)
                {
                    closestEnemyDistance = dist;
                    closestEnemy = enemy.transform;
                }
            }
            
            if (player != null && playerDistance < detectionRange)
            {
                if (closestEnemy != null && closestEnemyDistance < playerDistance && closestEnemyDistance < detectionRange)
                    StartFollowing(closestEnemy, false);
                else
                    StartFollowing(player.transform, true);
            }
            else if (closestEnemy != null && closestEnemyDistance < detectionRange)
            {
                StartFollowing(closestEnemy, false);
            }
        }
        else
        {
            agent.speed = followSpeed;
            if (targetLeader != null) agent.SetDestination(targetLeader.position);
            else StopFollowing();
        }
    }
    
    void SetNewRoamDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius + roamCenter;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }
    
    void StartFollowing(Transform leader, bool isPlayer)
    {
        if (isFollowing && targetLeader != null)
        {
            NPCCounter oldCounter = targetLeader.GetComponent<NPCCounter>();
            if (oldCounter != null) oldCounter.RemoveNPC(); 
        }
        
        isFollowing = true;
        targetLeader = leader;
        agent.stoppingDistance = 3f;
        
        NPCCounter newCounter = leader.GetComponent<NPCCounter>();
        if (newCounter == null) newCounter = leader.gameObject.AddComponent<NPCCounter>();
        newCounter.AddNPC();
        
        if (rend != null)
        {
            if (isPlayer)
            {
                PlayerMovement pMove = leader.GetComponent<PlayerMovement>();
                if (pMove != null && pMove.playerMaterial != null)
                {
                    rend.material = pMove.playerMaterial;
                }
                else if (playerFollowMaterial != null)
                {
                    rend.material = playerFollowMaterial;
                }
            }
            else
            {
                Enemy enemyScript = leader.GetComponent<Enemy>(); 
                if (enemyScript != null && enemyScript.NPCMaterial != null) 
                {
                    rend.material = enemyScript.NPCMaterial;
                }
            }
        }
    }
    
    void StopFollowing()
    {
        if (isFollowing && targetLeader != null)
        {
            NPCCounter npcCounter = targetLeader.GetComponent<NPCCounter>();
            if (npcCounter != null) npcCounter.RemoveNPC(); 
        }
        
        isFollowing = false;
        targetLeader = null;
        agent.stoppingDistance = 2f;
        if (rend != null && originalMaterial != null) rend.material = originalMaterial;
        SetNewRoamDestination();
    }
    
    void OnDestroy()
    {
        if (isFollowing && targetLeader != null)
        {
            NPCCounter npcCounter = targetLeader.GetComponent<NPCCounter>();
            if (npcCounter != null) npcCounter.RemoveNPC();
        }
    }
}