//stage - Cameron
//in charge of level loading and start screen and end dcreen inputs
// last edit 20/10/2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static reference
    public static GameManager Instance { get; private set; }
    
    //input related variables
    private PlayerController m_focusedPlayerController;
    private List<Gamepad> m_controllers;
    private List<PlayerController> m_activePlayerControllers;

    //level loading variables
    [Header("Level loading")]
    private GameObject m_currentStageObject;
    [Tooltip("level prefabs")]
    public GameObject[] stageList;
    private int m_currentStage = 0;
    public int roundsPerGame;

    //keeps score
    [SerializeField]
    private List<int> m_leaderBoard;
    
    //bools
    private int m_deadPlayers;
    private bool m_isPaused;
    private bool m_started = false;
    private bool m_gameOver;   
    
    [Header("Player")]
    public GameObject playerPrefab;

    //UI
    [Header("UI")]
    public Canvas pauseCanvas;
    public GameObject gameOverScreen;
    public TMP_Text controllerCount;
    public TMP_Text gameOverText;

    public int deadPlayers
    {
        get { return m_deadPlayers; }
        set
        {
            m_deadPlayers = value;
            if (m_deadPlayers == m_activePlayerControllers.Count - 1)
            {
                if (!IsGameOver())
                {
                    LoadStage();
                } else
                {
                    StartGameOver();
                    m_gameOver = true;

                }
            }
        }
    }
    

    // Singleton instantiation
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    /// initalizing
    void Start()
    {
        m_controllers = new List<Gamepad>();

        m_activePlayerControllers = new List<PlayerController>();

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log(Gamepad.all[i]);
        }
    }

    /// <summary>
    ///  takes in input for the start screen and end screen
    /// </summary>
    void Update()
    {
        DebugUpdate();
        if (!m_started)
        {
            controllerCount.text = "active players: " + m_controllers.Count.ToString();
            CheckControllers();
            LoadFirst();
        } 
        else if (!m_gameOver)
        {

        } 
        else if (m_gameOver)
        {
            GameOverUpdate();
        }
    }

    /// <summary>
    /// checks if any controller has pressed east and stores them if so
    /// </summary>
    private void CheckControllers()
    {
        //for all of the controllers connected to the computer
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            //check if the current gamepad has pressed east and is not stored
            if (Gamepad.all[i].buttonEast.isPressed && !m_controllers.Contains(Gamepad.all[i]))
            {
                //store controller and add player to leaderboard
                m_controllers.Add(Gamepad.all[i]);
                m_leaderBoard.Add(0);
            }
        }
    }

    /// <summary>
    /// checks if any stored controller has pressed start and if so loads the level and creates the appropriate players
    /// </summary>
    private void LoadFirst()
    {
        //if there are enogh players and player 1 presses start
         if (m_controllers.Count > 0 && m_controllers[0].startButton.isPressed)
         {
             //load new stage
             int random = Random.Range(0, stageList.Length);
             m_currentStageObject = Instantiate(stageList[random]);
             //keep the spawn locations in the stage
             Transform[] spawns = FindObjectOfType<Stage>().spawns;
             //randomize the spawns order
             ShuffleSpawns(spawns);
             //for every player
             for (int j = 0; m_controllers.Count > j; j++)
             {
                 //create a player object and assign there specific controller
                 GameObject newPlayer = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: Gamepad.all[j]).gameObject;
                 //move that player to a spawn
                 newPlayer.transform.position = spawns[j].transform.position;
                // add player to list of players
                m_activePlayerControllers.Add(newPlayer.GetComponent<PlayerController>());
             }
             // destroy start screen
             Destroy(controllerCount.gameObject);
             // say that the game ahs begun
             m_started = true;
         }
        
    }

    /// <summary>
    /// enables and isables inputs for pressing pause
    /// </summary>
    /// <param name="pauser"></param>
    public void TogglePause(PlayerController pauser)
    {
        //if the game is not paused
        if (m_isPaused == false)
        {
            //keep track of the pauser
            m_focusedPlayerController = pauser;

            //disable input in ever player exept the pauser
            for (int i = 0; i < m_activePlayerControllers.Count; i++)
            {
                if (m_activePlayerControllers[i] != m_focusedPlayerController)
                {
                    m_activePlayerControllers[i].DisableInput();
                }
            }
            //give pauser menu controls instead of playing controls
            //m_focusedPlayerController.SetControllerMap("UI");

            //show pause screen
            pauseCanvas.enabled = true;

            //freeze time
            Time.timeScale = 0f;
            //keep track of the fact the game is paused
            m_isPaused = true;

        }
        else // if the game is paused
        {
            //enable the input on every player exept the unpauser
            for (int i = 0; i < m_activePlayerControllers.Count; i++)
            {
                if (m_activePlayerControllers[i] != m_focusedPlayerController)
                {
                    m_activePlayerControllers[i].EnableInput();
                }
            }
            //give unpauser playing controls
            //m_focusedPlayerController.SetControllerMap("Player");

            //stop teh puase menu being visible
            pauseCanvas.enabled = false;

            //unfreeze time
            Time.timeScale = 1f;
            //keep track that the game is unpaused
            m_isPaused = false;
        }
            
    }


    /// <summary>
    /// returns true if the amount of rounds hass reached the rounds per game
    /// </summary>
    /// <returns></returns>
    private bool IsGameOver()
    {
        if (m_currentStage <= roundsPerGame)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// will delete old stage and instantiate and set up a new one
    /// </summary>
    /// <param name="newGame"></param>
    private void LoadStage()
    {
        //finds the only living player and adds score to its position in the leaderboard
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            if (m_activePlayerControllers[i].GetComponent<PlayerInput>().inputIsActive)
            {
                m_leaderBoard[i]++;
                break;
            }
        }
        //destroy the current stage and load a new one
        Destroy(m_currentStageObject);
        int random = Random.Range(0, stageList.Length);
        m_currentStageObject = Instantiate(stageList[random]);
        //reset player stats
        ResetPlayers();
        //randomize spawn order
        ShuffleSpawns(m_currentStageObject.GetComponent<Stage>().spawns);
        //for each player move it to a spawn
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            m_activePlayerControllers[i].gameObject.transform.position = m_currentStageObject.GetComponent<Stage>().spawns[i].position;
        }
        //keep track of what stage we are on
        m_currentStage++;
        //keep track of dead players
        m_deadPlayers = 0;
    }

    /// <summary>
    /// calls reset players on all players
    /// </summary>
    private void ResetPlayers()
    {
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            m_activePlayerControllers[i].ResetPlayer();
        }
    }

    /// <summary>
    /// just loads the scene from the top again
    /// </summary>
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// makes P1 take damage when you press 1
    /// </summary>
    public void DebugUpdate()
    { 
        if (Keyboard.current.digit1Key.isPressed)
        {
            m_activePlayerControllers[0].TakeDamage(1f);
        }

    }

    /// <summary>
    /// randomizes the spawns in the array to spawn players at random locations
    /// </summary>
    /// <param name="spawns"></param>
    private void ShuffleSpawns(Transform[] spawns)
    {
        // Knuth shuffle algorithm
        for (int i = 0; i < spawns.Length; i++)
        {
            Transform tmp = spawns[i];
            int r = Random.Range(i, spawns.Length);
            spawns[i] = spawns[r];
            spawns[r] = tmp;
        }
    }

    /// <summary>
    /// update for when the game is over just waits for player 1 to press a button
    /// </summary>
    public void GameOverUpdate()
    {
        //if p1 press start or east
        if (m_controllers[0].startButton.isPressed || m_controllers[0].buttonEast.isPressed)
        {
            //reload scene
            ResetGame();
        }
    }

    /// <summary>
    /// finds the winner and destroys the game
    /// </summary>
    public void StartGameOver()
    {
        //destroy the stage and players
        Destroy(m_currentStageObject);
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            Destroy(m_activePlayerControllers[i].gameObject);
        }

        //find the winner and that score
        int winnerIndex = 0;
        int topScore = 0;
        for (int i = 0; i < m_leaderBoard.Count; i++)
        {
            if (m_leaderBoard[i] > topScore)
            {
                winnerIndex = i;
                topScore = m_leaderBoard[i];
            }
        }

        //this line wont be here at the end
        gameOverText.text = "the winner is player " + (winnerIndex + 1) + " there score was " + topScore;
        
        //show game over screen
        gameOverScreen.SetActive(true);
    }
}
