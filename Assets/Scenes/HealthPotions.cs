using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotions : MonoBehaviour
{
    [SerializeField] Player Player;
    public int healingAmount = 50;

    public float m_Amplitude = 1.0f;
    public float m_Period = 1.0f;
    public Vector3 m_Direction = Vector3.up;
    Vector3 m_StartPosition;
    void Start()
    {
        m_StartPosition = new Vector3(30f, 7.52f, 34.49f);

        var pos = m_StartPosition + m_Direction * m_Amplitude * Mathf.Sin(2.0f * Mathf.PI * Time.time / m_Period);
        transform.position = pos;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(Player.PlayerHealth < 100 || Player.PlayerHealth != 0)
            {
                Player.PlayerHealth += healingAmount;

                if(Player.PlayerHealth > 100)
                {
                    Player.PlayerHealth = 100;
                }

                Destroy(gameObject);
            }
            
            
        }
    }

}
