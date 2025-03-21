using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] Player player;

    public int treassureValue = 10000;

    public float m_Amplitude = 1.0f;
    public float m_Period = 1.0f;
    public Vector3 m_Direction = Vector3.up;
    Vector3 m_StartPosition;
    void Start()
    {
        m_StartPosition = new Vector3(55f, 4f, 82f);

        var pos = m_StartPosition + m_Direction * m_Amplitude * Mathf.Sin(2.0f * Mathf.PI * Time.time / m_Period);
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.totalTreassure += treassureValue;
            Destroy(gameObject);
        }
    }
}
