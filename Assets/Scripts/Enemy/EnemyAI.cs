using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    // how often to move
    [SerializeField]
    private float maxWanderInterval = 5f;
    [SerializeField]
    private float minWanderInterval = 15f;
    // how close to be considerd as arrived
    [SerializeField]
    private float arriveDistance = 0.6f;
    [SerializeField]
    private int roomPadding = 1;

    // distance to start chasing player
    [SerializeField]
    private float aggroRadius = 6f;
    // keep chasing until this range
    [SerializeField]
    private float chaseRadius = 10f;

    // toggle option to keep chasing once aggroed
    [SerializeField]
    private bool keepChasingOnceAggro = false;

    private RectInt room;

    private Transform player;
    private bool aggro = false;
    private float nextWanderTime;

    private Animator animator;
    private float moveAmount;

    // keeps track of how many enmies are chasing the player
    private static int aggroCount = 0;
    // ensure enemy is only counted once when aggroed
    private bool isAggroed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void InRoom(RectInt room)
    {
        this.room = room;
        // randomiz wander times
        nextWanderTime = Time.time + Random.Range(minWanderInterval, maxWanderInterval);
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            player = p.transform;

        }

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            // set enemy to agro state when player is within aggro radius
            if (aggro == false && dist <= aggroRadius)
            {
                EnterAggro();
                aggro = true;
            }

            if (aggro == true)
            {
                // lose aggro if player is out of chase radius (only if keepChasingOnceAggro option is off)
                if (keepChasingOnceAggro == false && dist > chaseRadius)
                {
                    ExitAggro();
                    aggro = false;
                }

                // chase player
                agent.SetDestination(player.position);

                // handle move animation
                moveAmount = agent.velocity.magnitude;
                animator.SetFloat("Speed", moveAmount);

                return;
            }
        }

        // check if agent has reached its target or is still on route
        bool arrived = agent.pathPending == false && agent.remainingDistance <= arriveDistance;

        // wander within room
        if (arrived == true && Time.time >= nextWanderTime)
        {
            Vector3 target;
            if (TryGetRandomPointInRoomOnNavMesh(out target))
            {
                agent.SetDestination(target);
            }
            nextWanderTime = Time.time + Random.Range(minWanderInterval, maxWanderInterval);
        }

        // handle move animation
        moveAmount = agent.velocity.magnitude;
        animator.SetFloat("Speed", moveAmount);
    }

    bool TryGetRandomPointInRoomOnNavMesh(out Vector3 result)
    {
        // Convert RectInt room (grid coords) to world x/z positions.
        // We assume 1 tile = 1 unit, centered at x+0.5, z+0.5.

        int minX = room.x + roomPadding;
        int maxX = room.x + room.width - roomPadding - 1;

        int minY = room.y + roomPadding;
        int maxY = room.y + room.height - roomPadding - 1;

        // Safety
        if (minX > maxX || minY > maxY)
        {
            result = transform.position;
            return false;
        }

        // Try a few random samples
        for (int tries = 0; tries < 12; tries++)
        {
            int x = Random.Range(minX, maxX + 1);
            int y = Random.Range(minY, maxY + 1);

            Vector3 candidate = new Vector3(x + 0.5f, transform.position.y, y + 0.5f);

            // Snap candidate to nearest NavMesh point
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = transform.position;
        return false;
    }

    void OnDisable()
    {
        ExitAggro();
    }

    void EnterAggro()
    {
        if (isAggroed == true)
        {
            return;
        }
        aggroCount++;
        isAggroed = true;
    }

    void ExitAggro()
    {
        if (isAggroed == false)
        {
            return;
        }
        aggroCount--;
        isAggroed = false;
    }

    // returns true if 1 or more enemies are currently aggroed to player
    public static bool IsAnyEnemyAggro()
    {
        return aggroCount > 0;
    }
}
