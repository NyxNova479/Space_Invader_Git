using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance;

    public bool IsPaused = false;

    private InputSystem_Actions controls;

    [SerializeField]
    private int score = 0;

    public int highScore = 0;

    [SerializeField]
    private int _lives = 2;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private TextMeshProUGUI _scoreUI;
    [SerializeField]
    private TextMeshProUGUI _highScore;
    [SerializeField]
    private TextMeshProUGUI _livesText;
    [SerializeField]
    private GameObject[] _livesImage;

    public EnemyManager enemyManager;
    [SerializeField]
    private TextMeshProUGUI gameOverUI;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this; ;
        }
        else
        {
            Destroy(gameObject);
        }

        controls = new InputSystem_Actions();
        controls.UI.Pause.performed += ctx => Pause();


    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("Score", score);

        if (highScore.ToString().Length == 0)
        {
            _highScore.text = "000" + highScore.ToString();
            return;
        }

        if (highScore.ToString().Length <= 2)
        {
            _highScore.text = "00" + highScore.ToString();
            return;
        }

        if (highScore.ToString().Length == 3)
        {
            _highScore.text = "0" + highScore.ToString();
            return;
        }


        _highScore.text =   highScore.ToString();
    }

    private void OnEnable()
    {
        controls.UI.Pause.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Pause.Disable();
    }

    private void Pause()
    {
        IsPaused = !IsPaused;
    }



    public void AddScore(int points)
    {

        score += points;

        

        if (score.ToString().Length <= 2)
        {
            _scoreUI.text = "00" + score.ToString();
            return;
        }

        if (score.ToString().Length == 3)
        {
            _scoreUI.text = "0" + score.ToString();
            return;
        }

        
        _scoreUI.text = score.ToString();
        
    }
    
        

    
    private void RestScore() => score = 0;
    
        
    public void LoseLife()
    {
        _lives--;
        
        
        if (_lives == 0)
        {
            GameOver();
        }
        else
        {
            

            playerPrefab.transform.position = new Vector2(0,-9.24f);
            playerPrefab.SetActive(true);
            _livesText.text = (_lives + 1).ToString();
            
        }
        if (_livesImage == null) return;
        else if(_lives == 2)
        {
            _livesImage[1].SetActive(false);
        }
        else _livesImage[0].SetActive(false);
    }

    public void GameOver()
    {
        // TODO : Implémenter le game over
        gameOverUI.text = "Game Over";
        SaveScore();
    }

    public void CompletedLevel()
    {
        // TODO : Implémenter le CompletedLevel
        enemyManager.SpawnEnemies();
        enemyManager._stepDistance += 0.05f;
        enemyManager.missileInterval -= 0.05f;
        enemyManager.shootLimit = 14;
        _lives = 2;
    }

    private void SaveScore()
    {
        if (score < highScore) return;
        
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
        
    }


}


