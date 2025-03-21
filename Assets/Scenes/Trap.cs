using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] Player Player;
    public int trapDamage = 2;

    Vector3 m_StartPosition;
    void Start()
    {
        m_StartPosition = new Vector3(49.1f, 3.16f, 56.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player.PlayerHealth -= trapDamage;
            if (Player.PlayerHealth < 0)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(other.gameObject);
                Debug.Log("GAME OVER");
            }

        }
    }
}
