using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyData;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private float playerBoundaryX;

    [SerializeField] EnemyData enemyData;


    public EnemyPool EnemyPool;
    public int rows = 5; // Nb de rangées
    public int columns = 11; // Nb de colonnes
    public float spacing = 1.5f; // Espacement entre les ennemies
    public float _stepDistance = 0.5f; // Distance de déplacement par frame
    public float _stepDistanceVertical = 1f; // Distance de déplacement vertical par frame

    public Vector2 startPosition = new Vector2(-7.5f, 6f);


    private GameObject[,] enemies;
    private int remainingEnemies = 0;

    private bool isPaused = false;
    public bool isExploding = false;

    [SerializeField]
    private MisssileManager missileManager;
    public int shootLimit = 22;

    private enum MoveState {MoveRight, MoveLeft}
    private MoveState currentState = MoveState.MoveRight;

    public GameObject[] missilePrefabs;
    public Transform missilePoint;
    public float missileInterval = 2.0f; // Intervalle minimum entre les tirs
    public int missileType = 0;

    private int explosionDuration = 17;
    [SerializeField]
    private GameObject explosionPrefab;

    private bool spawnedUFO = false;
    private GameObject ufoInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        enemies = new GameObject[rows, columns];

        SpawnEnemies();

        StartCoroutine(HandheldEnemyMovement());

        StartCoroutine(EnemyShooting());

    }
    private void Update()
    {


    }

    private void SpawnUFO()
    {
        GameObject ufoPrefab = enemyData.enemyTypes[3].prefab;

        if (ufoInstance == null)
        {
            // Première apparition
            ufoInstance = Instantiate(ufoPrefab,new Vector2(-5.5f, 7.5f),Quaternion.identity);
            spawnedUFO = false;
        }
        else
        {
            // Réapparition
            ufoInstance.transform.position = new Vector2(-5.5f, 7.5f);
            ufoInstance.SetActive(true);
        }
    }

    public void SpawnEnemies()
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
                    remainingEnemies++;
                }
            }

        }


    }

    IEnumerator HandheldEnemyMovement()
    {
        while (remainingEnemies > 0)
        {


            bool boundaryReached = false;

            // Boucle pour déplacer chaque ennemie l'un après l'autre en commençant par la dernière ligne
            for (int row = rows - 1; row >= 0; row--)
            {

                
                for (int col = 0;  col < columns; col++)
                {
                    if (GameManager.Instance.IsPaused || isExploding)
                    {
                        yield return new WaitUntil(() => !GameManager.Instance.IsPaused && !isExploding);
                    }

                    if (enemies[row, col] != null && enemies[row, col].activeSelf)
                    {
                        // Déplacer l'ennemi dans la currentDirection

                        Vector3 direction = currentState == MoveState.MoveRight ? Vector3.right : Vector3.left; ;

                        MoveEnemy(enemies[row, col], direction, _stepDistance);

                        if (enemies[row, col] == null) continue;

                        // Alterne le sprite de l'ennemi
                        EnemyScript enemyScript = enemies[row, col].GetComponent<EnemyScript>();
                        if (enemyScript != null) enemyScript.ChangeSprite();

                        if (ReachedBoundary(enemies[row, col])) boundaryReached = true;

                        yield return null;

                    }

                }

                
            }
            if (missileManager.shootCount >= shootLimit)
            {
                Debug.Log("caca");
                missileManager.shootCount = 0;
                SpawnUFO();
            }


            if (boundaryReached)
            {
                yield return MoveAllEnemiesDown();
                currentState = currentState == MoveState.MoveRight ? MoveState.MoveLeft : MoveState.MoveRight;
            }
        }
    }

    IEnumerator MoveAllEnemiesDown()
    {
        
        
       for (int row = rows - 1; row >= 0; row--)
       {

            for (int col = 0; col < columns; col++)
            {

                if (enemies[row, col] != null && enemies[row, col].activeSelf)
                {

                    Vector3 direction = Vector3.down;

                    MoveEnemy(enemies[row, col], direction, _stepDistanceVertical);

                    yield return null;
                }

            }

        }
        
    }

    IEnumerator EnemyShooting()
    {
        while (true)
        {
            yield return new WaitUntil(() => !GameManager.Instance.IsPaused && !isExploding);

            yield return new WaitForSeconds(Random.Range(missileInterval, missileInterval * 2));

            List<GameObject> shooters = GetBottomEnemies();

            if (shooters.Count > 0 && !GameManager.Instance.IsPaused && !isExploding && enemyData.enemyTypes[3].prefab)
            {
                GameObject shooter = shooters[Random.Range(0, shooters.Count)];
                
                FireMissile(shooter, missileType);
                missileType = 1 + missileType % missilePrefabs.Length;

            }
        }
    }

    private List<GameObject> GetBottomEnemies()
    {
        List<GameObject> bottomEnemies = new List<GameObject>();

        for (int col = 0; col < columns; col++)
        {
            for (int row = rows -1; row >= 0; row--)
            {
                if (enemies[row,col] != null && enemies[row, col].activeSelf)
                {
                    bottomEnemies.Add(enemies[row, col]);
                    break;
                }   
            }
        }

        

        return bottomEnemies;
    }

    private void FireMissile(GameObject shooter, int missileType)
    {
        // Rechercher le FirePoint dans les enfants de l'ennemi
        Transform firePoint = shooter.transform.Find("FirePoint");

        if (firePoint != null)
        {
            if (missileType == 0)
            {
                Instantiate(missilePrefabs[0], firePoint.position, Quaternion.identity);
            }
            if (missileType == 1)
            {
                Instantiate(missilePrefabs[1], firePoint.position, Quaternion.identity);
            }
            if (missileType == 2)
            {
                Instantiate(missilePrefabs[2], firePoint.position, Quaternion.identity);
                missileType = 0;
            }


        }
        else
        {
            Debug.Log($"FirePoint non trouvé pour l'ennemi : {shooter.name}");
        }
    }


    public void ReturnEnemy(GameObject enemy, GameObject prefab)
    {
        for (int row = 0; row < rows; row++)
        {

            for (int col = 0; col < columns; col++)
            {

                if (enemies[row,col] == enemy)
                {
                    enemies[row, col] = null;
                }

            }


        }

        GameManager.Instance.AddScore(enemy.GetComponent<EnemyScript>().ScoreData);

        EnemyPool.ReturnToPool(enemy, prefab);

        remainingEnemies--;
        
        if (isExploding != true)
        {
            StartCoroutine(ExplosionCoroutine(enemy));
        }
        
        if (remainingEnemies <= 0)
        {
            GameManager.Instance.CompletedLevel();
        }


    }

    IEnumerator ExplosionCoroutine(GameObject enemy)
    {
        isExploding = true;
        int duration = explosionDuration;

        GameObject explosion = Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
        while (duration > 0)
        {
            
            duration--;
            yield return new WaitForEndOfFrame();
        }

        explosion.SetActive(false);

        isExploding = false;
    }

    private void MoveEnemy(GameObject enemy, Vector3 direction, float stepDistance)
    {

        if (enemy == null) return;

        Vector3 newPosition = enemy.transform.position + direction * stepDistance;

        newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
        newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
        newPosition.z = Mathf.Round(newPosition.z * 100f) / 100f;

        enemy.transform.position = newPosition;
    }

    private bool ReachedBoundary(GameObject enemy)
    {
        float xPos = enemy.transform.position.x;

        if (currentState == MoveState.MoveRight && xPos >= playerBoundaryX)
        {
            
            return true;
        }
        if (currentState == MoveState.MoveLeft && xPos <= -playerBoundaryX)
        {
            
            return true;
        }
        
        return false;
        
    }


    private EnemyData.EnemyType GetEnemyTypeForRow(int row, List<EnemyData.EnemyType> enemyTypes)
    {
        if (row == 0) // 1er ligne : Type C
        {
            return enemyTypes[2];
        }
        else if (row <= 2) // 2e et 3e lignes : Type B
        {
            return enemyTypes[1];
            
        }
        else // 4e et 5e lignes : Type A
        {
            return enemyTypes[0];
        }
    }
}
