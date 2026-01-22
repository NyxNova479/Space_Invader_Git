using Unity.VisualScripting;
using UnityEngine;

public class PixelPerfectCollision : MonoBehaviour
{

    public SpriteRenderer shelterSprite; // Sprite renderer du shelter
    private Texture2D shelterTexture;    // Texture assoscié au sprite du shelter
    public GameObject maskPrefab;        // Prefab du MissileSplash avec Sprite mask
    public float yOffset = 0f;           // Offset vertical en espace monde

    private void Start()
    {
        // Fait une copie de la texture pour la rendre modifiable
        shelterTexture = Instantiate(shelterSprite.sprite.texture);

        shelterSprite.sprite = Sprite.Create(shelterTexture, shelterSprite.sprite.rect, new Vector2(0.5f, 0.5f), shelterSprite.sprite.pixelsPerUnit);

        if (!shelterTexture.isReadable)
        {
            Debug.LogError("⚠️ La texture de la protection doit être lisible !");
        }
       
       
       
       
    }

    private void OnTriggerStay2D(Collider2D collision)

    {

        if (collision.CompareTag("Missile") || collision.CompareTag("Enemy"))
        {
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("Le missile n'a pas de Collider2D");
                return;
            }

            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {

                // Pooling à créer 
                InstantiateMaskAtPosition(worldImpactPoint);

                // ResetMissile
                collision.gameObject.GetComponent<PlayerMissile>()?.ResetMissile();
                collision.gameObject.GetComponent<EnemyMissile>()?.ResetMissile();
            }

        }

    }


    private bool IsPixelHitAndModify(BoxCollider2D missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint)
    {

        worldImpactPoint = Vector2.zero;
        uvImpactPoint = Vector2.zero;

        // .bounds vous permet de récupérer les limites d'un collider
        Bounds missileBounds = missileCollider.bounds;

        Vector3 bottomLeft = shelterSprite.transform.InverseTransformPoint(missileBounds.min);

        Vector3 topRight = shelterSprite.transform.InverseTransformPoint(missileBounds.max);

        bottomLeft.y += yOffset;
        topRight.y += yOffset;

        Bounds spriteBounds = shelterSprite.sprite.bounds;

        // Prendre compte des dimensions de la texture et le rect du sprite
        Rect textureRect = shelterSprite.sprite.textureRect;

        // Normaliser les coordonnées du missile dans l'espace UV
        float uMin = (bottomLeft.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMin = (bottomLeft.y - spriteBounds.min.y) / spriteBounds.size.y;
        float uMax = (topRight.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMax = (topRight.y - spriteBounds.min.y) / spriteBounds.size.y;

        // Vérifier si les UV du missile sont dans les limites de la texture
        uMin = Mathf.Clamp01(uMin);
        vMin = Mathf.Clamp01(vMin);
        uMax = Mathf.Clamp01(uMax);
        vMax = Mathf.Clamp01(vMax);

        // Déterminer le point d'impact
        uvImpactPoint = new Vector2((uMin + uMax) / 2, (vMin + vMax) / 2);

        worldImpactPoint = shelterSprite.transform.TransformPoint(new Vector3(spriteBounds.min.x + uvImpactPoint.x * spriteBounds.size.x,spriteBounds.min.y + uvImpactPoint.y * spriteBounds.size.y, 0 ));

        bool pixelModified = false;

        // Parcourir les pixels touchés par la texture missile
        for(float u  = uMin; u <= uMax; u += 1.0f / shelterTexture.width)
        {
            for(float v = vMin; v <= vMax; v += 1.0f / shelterTexture.height)
            {

                int x = Mathf.FloorToInt(textureRect.x + u * textureRect.width);
                int y = Mathf.FloorToInt(textureRect.y + v * textureRect.height);

                // Vérifier si les coordonnées de la texture sont valides aka actifs
                if (x >= 0 && x < shelterTexture.width && y >= 0 && y < shelterTexture.height)
                {
                    // Lire la couleur du pixel dans la texture
                    Color pixel = shelterTexture.GetPixel(x, y);

                    if (pixel.a > 0)
                    {
                        shelterTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
                        pixelModified = true;
                    }
                }

            }
        }

        if(pixelModified)
        {
            shelterTexture.Apply();
        }

        return pixelModified;

    }

    private void InstantiateMaskAtPosition(Vector2 worldPosition)
    {
        // TODO : Implémenter un système de pooling
        GameObject maskInstance = Instantiate(maskPrefab, worldPosition, Quaternion.identity);

        // Positionne le mask sur le même z que le shelter
        maskInstance.transform.position = new Vector3(worldPosition.x, worldPosition.y, shelterSprite.transform.position.z);

    }
}
