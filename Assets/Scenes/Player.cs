using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : MonoBehaviour
{
    [SerializeField] Bullet Bullet;
    [SerializeField] Transform Enemy1;
    [SerializeField] Transform Enemy2;
    [SerializeField] Enemy Enemy1Health;
    [SerializeField] Enemy Enemy2Health;

    private int playerHealth = 100;
    public int totalTreassure = 0;
    public int totalCoinsAmount = 0;
    public int magicalPowers = 0;

    private float move, moveSpeed, rotation, rotateSpeed;

    NavMeshAgent m_Agent;

    float AttackRange = 4f;
    float FireRate = 2f;
    float nextShootTime = 0;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        moveSpeed = 5f;
        rotateSpeed = 100f;
        Player player = new Player();
        player.PlayerHealth = playerHealth;
    }

    void Update()
    {
        move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        rotation = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
        

    }

    private void LateUpdate()
    {
        transform.Translate(0f, 0f, move);
        transform.Rotate(0f, rotation, 0f);

        float enemyRange1 = Vector3.Distance(m_Agent.transform.position, Enemy1.transform.position);
        float enemyRange2 = Vector3.Distance(m_Agent.transform.position, Enemy2.transform.position);
        if (enemyRange1 <= AttackRange && Enemy1Health.EnemyHealth >=0)
        {
            Vector3 targetDirection = Enemy1.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1.0f * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            Attack();
        }
        if (enemyRange2 <= AttackRange && Enemy2Health.EnemyHealth >= 0)
        {
            Vector3 targetDirection = Enemy2.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1.0f * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            Attack();
        }
    }

    private void Attack()
    {
        if (Time.time > nextShootTime)
        {
            float space = 2.0f;
            Bullet.transform.position = transform.position + transform.forward * space;
            Bullet bulletClone = Instantiate(Bullet, Bullet.transform.position, Bullet.transform.rotation);
            bulletClone.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * Bullet.Speed);
            nextShootTime = Time.time + FireRate;

        }
    }

    public int PlayerHealth   
    {
        get { return playerHealth; }
        set { playerHealth = value; }
    }
}
