using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public EnemyData.EnemyType EnemyType;
    public int ScoreData;

    public Sprite sprite01;
    public Sprite sprite02;
    private SpriteRenderer spriteRenderer;

    private bool isSprite01;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite01;
            isSprite01 = true;
        }
        else
        {
            Debug.LogError("[EnnemyScript] SpriteRenderer is not assigned");
        }
    }

    public void ChangeSprite()
    {
        if (sprite01 == null && sprite02 == null) return;
        isSprite01 = !isSprite01;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.invaderMove);
        // spriteRenderer.sprite = (condition) ? (si vrai) : (si faux);
        spriteRenderer.sprite = isSprite01 ? sprite01 : sprite02;

    }



}
