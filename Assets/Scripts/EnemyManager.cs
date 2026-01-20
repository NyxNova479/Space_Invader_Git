using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private float playerBoundaryX;


    public EnemyPool EnemyPool;
    public int rows = 5; // Nb de rangées
    public int columns = 11; // Nb de colonnes
    public float spacing = 1.5f; // Espacement entre les ennemies
    public float stepDistance = 0.5f; // Distance de déplacement par frame
    public float stepDistanceVertical = 1f; // Distance de déplacement vertical par frame

    public Vector2 startPosition = new Vector2(-6.5f, 7.5f);


    private GameObject[,] enemies;
    private bool isPaused = false;
    private bool isExploding = false;

    private enum MoveState {MoveRight, MoveLeft}
    private MoveState currentState = MoveState.MoveRight;

    public GameObject missilePrefab;
    public Transform missilePoint;
    public float missileInterval = 2.0f; // Intervalle minimum entre les tirs


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        enemies = new GameObject[rows, columns];

        SpawnEnemies();

    }

    private void SpawnEnemies()
    {
        var enemyTypes = EnemyPool.GetEnemyType();

        for (int row = 0; row < rows; row++)
        {
            var enemyType = GetEnemyTypeForRow(row, enemyTypes);

            for (int col = 0; col < columns; col++)
            {
                GameObject enemy = EnemyPool.GetEnemy(enemyType.prefab);

                if (enemy != null)
                {
                    float xPos = startPosition.x + (col * spacing);
                    float yPos = startPosition.y - (row * spacing);

                 //   Debug.Log($"[EnemyManager] {enemy.name} est à la position X : {xPos}; Y : {yPos}");

                    enemy.transform.position = new Vector3(xPos, yPos, 0);

                    EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.EnemyType = enemyType;
                        enemyScript.ScoreData = enemyType.points;
                    }
                    enemies[row, col] = enemy;
                }
            }
        }

    }

    private EnemyData.EnemyType GetEnemyTypeForRow(int row, List<EnemyData.EnemyType> enemyTypes)
    {
        if (row == 0) // 1er ligne : Type C
        {
            return enemyTypes[2];
        }
        else if (row < 2) // 2e et 3e lignes : Type B
        {
            return enemyTypes[1];
            
        }
        else // 4e et 5e lignes : Type A
        {
            return enemyTypes[0];
        }
    }
}
