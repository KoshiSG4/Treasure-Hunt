using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 10f;

    [SerializeField] Player player;
    [SerializeField] Enemy enemy1;
    [SerializeField] Enemy enemy2;

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);

        if (collision.gameObject.tag == "Player")
        {
            player.PlayerHealth -= 1;            
        }

        if (collision.gameObject.tag == "Enemy1")
        {
            enemy1.EnemyHealth -= 1;
        }

        if (collision.gameObject.tag == "Enemy2")
        {
            enemy2.EnemyHealth -= 1;
        }

    }


}
