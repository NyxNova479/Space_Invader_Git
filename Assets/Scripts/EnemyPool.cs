using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField]
    public EnemyData enemyData;


    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();




    void Start()
    {
        // Initialiser un pool pour chaque type d'ennemi avec des quantités spécifiques
        for (int i = 0; i < enemyData.enemyTypes.Count; i++)
        {
            var enemyType = enemyData.enemyTypes[i];


            if (poolDictionary.ContainsKey(enemyType.prefab))
            {
                Debug.LogWarning($"Le prefab {enemyType.prefab.name} pour le type {enemyType.name} est déjà dans le pool");
                continue;
            }

            int poolSize = GetPoolSizeForEnemyType(i);

            Queue<GameObject> enemyQueue = new Queue<GameObject>();

            for (int j = 0; j < poolSize; j++) 
            {
                GameObject enemy = Instantiate(enemyType.prefab);
                enemy.SetActive(false);
                enemyQueue.Enqueue(enemy);
            }

            poolDictionary.Add(enemyType.prefab, enemyQueue);
        }  
    }

    public GameObject GetEnemy(GameObject prefab)
    {
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> enemyQueue) && enemyQueue.Count > 0)
        {
            GameObject enemy = enemyQueue.Dequeue();
            enemy.SetActive(true);

            return enemy;
        }

        return null;
    }

    public void ReturnEnemy(GameObject enemy, GameObject prefab)
    {
        enemy.SetActive(false);

        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> enemyQueue))
        {
            enemyQueue.Enqueue(enemy);
        }
        else
        {
            Debug.Log("Tentative de retourer un ennemi à un pool inexistant !");
        }
    }

    public List<EnemyData.EnemyType> GetEnemyType()
    {
        return enemyData.enemyTypes;
    }

    private int GetPoolSizeForEnemyType(int index)
    {
        switch (index)
        {
            case 0: // Type A
                return 22;
            case 1: // Type B
                return 22;
            case 2: // Type C
                return 11;
            default:
                return 0;
        }
    }

    void Update()
    {
        
    }
}
