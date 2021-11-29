using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerEnergyPoints = 100;
    public int score = 0;
    public int level = 1;

    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private Text conText;
    private GameObject levelImage;
    private GameObject continueImage;
    private bool lastLevel = false;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            if (lastLevel)
            {
                level++;
                InitGame();
            }
            else
                lastLevel = true;
        }
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();


        if (continueImage == null)
        {
            continueImage = GameObject.Find("ConImage");
        }

        levelText.text = "B " + level+"F";

        continueImage.SetActive(false);
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        score *= (int)Mathf.Log(level, 2f);
        levelText.text = "Entered the " + level + "F\nTotalScore : " + score;
        levelImage.SetActive(true);
        continueImage.SetActive(true);
        enabled = false;
    }

    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}