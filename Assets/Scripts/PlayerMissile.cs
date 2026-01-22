using System;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{

    public float speed = 10f;
    public float maxHeight = 10f;



    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);   

        if (transform.position.y > maxHeight)
        {
            ResetMissile();
        }


    }

    public void ResetMissile()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyManager enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.ReturnEnemy(collision.gameObject, collision.gameObject);
            }

            ResetMissile();
        }

    }
}
