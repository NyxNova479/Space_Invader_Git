using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCfg", menuName = "Space Invaders/EnemyConfiguration")]

public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public int points;
        public Color color;
        public GameObject prefab;
    }

    public List<EnemyType>enemyTypes; 

}
