using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MisssileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject missilePrefab;

    [SerializeField]
    private Transform firePoint; // Position d'où le missile spawn

    public int poolSize = 1;
    private GameObject[] missilePool;
    private int currentMissileIndex = 0; // Permet de commencer la recherche à cet index

    private InputSystem_Actions controls;

    public EnemyManager enemyManager;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Fire.performed += ctx => OnFire(ctx);
        
    }

    private void Start()
    {
        missilePool = new GameObject[poolSize]; 

        for (int i = 0; i < poolSize; i++)
        {
            missilePool[i] = Instantiate(missilePrefab);
            missilePool[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        controls.Player.Fire.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Fire.Disable();
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && (!GameManager.Instance.IsPaused && !enemyManager.isExploding))
        { 

            // Vérifier si un missile inactif est disponible
            for (int i = 0; i < poolSize; i++)
            {
                
                int index = (currentMissileIndex + i) % poolSize;

                if (!missilePool[index].activeSelf)
                {
                    missilePool[index].transform.position = firePoint.position;
                    missilePool[index].transform.rotation = firePoint.rotation;
                    missilePool[index].SetActive(true);


                    currentMissileIndex = (index + 1) % poolSize;
                    return; // Sortir de la loop après avoir trouvé un missile
                }

            }

            Debug.Log("⚠️ Aucun missile disponible !");

        } 
    }


}
