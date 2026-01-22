using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    public float Speed = 10f;
    public float MinHeight = -10f;



    void Update()
    {
        transform.Translate(Vector3.down * Speed * Time.deltaTime);

        if (transform.position.y < MinHeight)
        {
            Destroy(gameObject);
        }


    }

    public void ResetMissile()
    {
        // gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);

            ResetMissile();
        }


    }
}
