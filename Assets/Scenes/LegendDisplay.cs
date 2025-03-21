using UnityEngine;
using UnityEngine.UI;

public class LegendDisplay : MonoBehaviour
{
    private int enemy1healthPoints = 100;
    private int enemy2healthPoints = 100;

    public Text infoText;

    [SerializeField] Player player;
    [SerializeField] Enemy enemy;

    void Start()
    {
        infoText = GetComponentInChildren<Text>();
        UpdateDisplay();
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Enemy1"))
        {
            enemy1healthPoints = enemy.EnemyHealth;
        }

        if (GameObject.FindGameObjectWithTag("Enemy2"))
        {
            enemy2healthPoints = enemy.EnemyHealth;
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {

        //infoText.text = $"Player\nHP: {player.PlayerHealth}\n\nEnemy 01:\nHP: {enemy1healthPoints}\n\nEnemy 02:\nHP: {enemy2healthPoints}";
    }
}
