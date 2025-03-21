using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    MeshRenderer meshRenderer;
    public enum EnemyStates
    {
        Patrol,
        Chase,
        Attack
    }
    public Transform target;
    Vector3 targetDirection;
    Vector3[] path;
    int targetIndex;
    
    [SerializeField] Vector3[] PatrolPoints;
    [SerializeField] Transform Player;
    [SerializeField] Bullet Bullet;
    [SerializeField] Material PatrolMaterial;
    [SerializeField] Material ChaseMaterial;
    [SerializeField] Material AttackMaterial;
    [SerializeField] float ChaseRange = 7f;
    [SerializeField] float AttackRange = 4f;

    public float moveSpeed = 3.0f;
    float FireRate = 2f;
    int nextPatrolPoint = 0;    
    float nextShootTime = 0;
    EnemyStates currentState = EnemyStates.Patrol;

   [SerializeField] Player playerHealthScore;

    private int enemyHealth = 100;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        EnemyHealth = 100;
        targetDirection = target.position - transform.position;
        target.position = PatrolPoints[nextPatrolPoint];
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }
    void Update()
    {
        SwitchState();
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;            
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position.x == currentWaypoint.x || Vector3.Distance(transform.position, currentWaypoint) < 1f)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            Debug.Log(currentWaypoint);

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1.0f * Time.deltaTime, 0.0f);
            this.transform.rotation = Quaternion.LookRotation(newDirection);
            this.transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
   

    private void SwitchState()
    {
        switch (currentState)
        {
            case EnemyStates.Patrol:
                Patrol();
                break;
            case EnemyStates.Chase:
                Chase();
                break;
            case EnemyStates.Attack:
                Attack();
                break;
            default:
                Patrol();
                break;
        }
    }

    private void Attack()
    {
        meshRenderer.material = AttackMaterial;

        if (Time.time > nextShootTime)
        {
            float space = 2.0f;
            Bullet.transform.position = navMeshAgent.transform.position + navMeshAgent.transform.forward * space;
            Bullet bulletClone = Instantiate(Bullet, Bullet.transform.position, Bullet.transform.rotation);
            bulletClone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * Bullet.Speed);
            nextShootTime = Time.time + FireRate;
        }


        if (playerHealthScore.PlayerHealth <= 0)
        {
            currentState = EnemyStates.Patrol;
        }

        float playerRange = Vector3.Distance(transform.position, Player.transform.position);
        if (playerRange > AttackRange)
        {
            currentState = EnemyStates.Chase;
        }
    }

    private void Chase()
    {
        targetDirection = Player.position - transform.position;
        PathRequestManager.RequestPath(transform.position, Player.position, OnPathFound);
        meshRenderer.material = ChaseMaterial;

        float playerRange = Vector3.Distance(transform.position, Player.transform.position);
        if (playerRange < AttackRange && playerHealthScore.PlayerHealth > 0)
        {
            currentState = EnemyStates.Attack;
        }
        if (playerRange > ChaseRange && playerHealthScore.PlayerHealth > 0)
        {
            target.position = PatrolPoints[nextPatrolPoint];
            targetDirection = target.position - transform.position;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            currentState = EnemyStates.Patrol;
        }
    }

    private void Patrol()
    {
        meshRenderer.material = PatrolMaterial;

        float distanceToNextPatrolPoint = Vector3.Distance(transform.position, PatrolPoints[nextPatrolPoint]);
        float playerRange = Vector3.Distance(transform.position, Player.transform.position);
        if (playerRange < ChaseRange)
        {
            currentState = EnemyStates.Chase;
        }
        
        else if (distanceToNextPatrolPoint <= 10f)
        {
            meshRenderer.material = PatrolMaterial;
            nextPatrolPoint++;
            if (nextPatrolPoint > PatrolPoints.Length - 1)
            {
                nextPatrolPoint = 0;
            }

            target.position = PatrolPoints[nextPatrolPoint];
            targetDirection = target.position - transform.position;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        }
        
    }

    public int EnemyHealth   
    {
        get { return enemyHealth; }
        set { enemyHealth = value; }
    }

}
