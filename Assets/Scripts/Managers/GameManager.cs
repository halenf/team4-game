// GameManager - Cameron, Halen
// Manages level loading, round flow, mapping controller inputs, and UI
// last edit 27/10/2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static reference
    public static GameManager Instance { get; private set; }

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

    //input variables
    private List<Gamepad> m_controllers;
    private List<PlayerController> m_activePlayerControllers;
    private PlayerController m_focusedPlayerController;

    [Header("Canvas Prefabs")]
    public StartUI startCanvasPrefab;
    public GameplayUI gameplayCanvasPrefab;
    public PauseUI pauseCanvasPrefab;
    public LeaderboardUI leaderboardCanvasPrefab;

    // canvas references
    private StartUI m_startCanvas;
    private GameplayUI m_gameplayCanvas;
    private PauseUI m_pauseCanvas;
    private LeaderboardUI m_leaderboardCanvas;

    [Header("Player")]
    public GameObject playerPrefab;

    [Header("Game Info")]
    public GameObject[] stageList;
    public int numberOfRounds;

    // stage tracking
    private GameObject m_currentStageObject;
    private int m_roundNumber = 0;

    // score tracking
    [SerializeField] private List<int> m_leaderboard;
    private int m_deadPlayers;

    // Game mode tracking
    private enum GameMode
    {
        NumberOfWins = 0,
        NumberOfRounds = 1
    }
    private GameMode m_gameMode;

    // Game state tracking
    private enum GameState
    {
        Start = 0,
        Playing = 1,
        Ended = 2
    }
    private GameState m_gameState;
    private bool m_isPaused;

    public int deadPlayers
    {
        get { return m_deadPlayers; }
        set
        {
            m_deadPlayers = value;
            if (m_deadPlayers == m_activePlayerControllers.Count - 1) // if there is only one player left alive
            {
                // Add to the last living player's score
                for (int i = 0; i < m_activePlayerControllers.Count; i++)
                {
                    if (m_activePlayerControllers[i].GetComponent<PlayerInput>().inputIsActive)
                    {
                        m_leaderboard[i]++;
                        break;
                    }
                }

                if (!IsGameOver())
                {
                    LoadStage();
                }
                else
                {
                    EndGame();
                }
            }
        }
    }

    // initalizing
    void Start()
    {
        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialise game manager
        Init();
    }

    /// <summary>
    ///  takes in input for the start screen and end screen
    /// </summary>
    void Update()
    {
# if UNITY_EDITOR
        DebugUpdate();
# endif
        // Change behaviour based on game state - Halen
        switch (m_gameState)
        {
            case GameState.Start:
                CheckControllers();
                LoadFirst();
                break;
            case GameState.Playing:
                break;
            case GameState.Ended:
                break;
        }
    }

    private void Init()
    {
        // Set timescale in case game was paused when reset
        Time.timeScale = 1.0f;

        // set all private variables to default values
        m_controllers = new List<Gamepad>();
        m_focusedPlayerController = null;
        m_activePlayerControllers = new List<PlayerController>();

        m_currentStageObject = null;
        m_roundNumber = 0;

        m_leaderboard = new List<int>();
        m_deadPlayers = 0;

        // Set start game state
        m_gameState = GameState.Start;
        m_isPaused = false;

        // Initialise canvases
        m_startCanvas = Instantiate(startCanvasPrefab);
        m_gameplayCanvas = Instantiate(gameplayCanvasPrefab);
        m_pauseCanvas = Instantiate(pauseCanvasPrefab);
        m_leaderboardCanvas = Instantiate(leaderboardCanvasPrefab);

        m_startCanvas.gameObject.SetActive(true);
        m_gameplayCanvas.gameObject.SetActive(false);
        m_pauseCanvas.gameObject.SetActive(false);
        m_leaderboardCanvas.gameObject.SetActive(false);
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
                m_leaderboard.Add(0);

                // Since the list of connected controllers was updated, we need to update the StartUI to reflect that
                m_startCanvas.SetDisplayDetails(m_controllers);
            }
        }
    }

    /// <summary>
    /// checks if any stored controller has pressed start and if so loads the level and creates the appropriate players
    /// </summary>
    public void LoadFirst()
    {
        // Only need one player when debugging - Halen
        int requiredPlayers = 2;
# if UNITY_EDITOR
        requiredPlayers = 1;
# endif
        //if there are enough players and player 1 presses start
        if (m_controllers.Count >= requiredPlayers && m_controllers[0].startButton.isPressed)
        {            
            //for every player
            for (int j = 0; m_controllers.Count > j; j++)
            {
                //create a player object and assign their specific controller
                GameObject newPlayer = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: m_controllers[j]).gameObject;

                // add player to list of players
                m_activePlayerControllers.Add(newPlayer.GetComponent<PlayerController>());
            }

            // deactivate start menu, activate gameplayUI - Halen
            m_startCanvas.gameObject.SetActive(false);
            m_gameplayCanvas.gameObject.SetActive(true);

            // Set correct game state
            m_gameState = GameState.Playing;

            // Load the first stage
            LoadStage();
        }
    }

    /// <summary>
    /// enables and isables inputs for pressing pause
    /// </summary>
    /// <param name="pauser"></param>
    public void TogglePause(PlayerController pauser)
    {
        m_isPaused = !m_isPaused; // Halen
        m_pauseCanvas.gameObject.SetActive(m_isPaused);

        if (m_isPaused) // if the game paused
        {
            m_focusedPlayerController = pauser;

            // Halen
            DisablePlayers(); // Disable all player inputs
            m_focusedPlayerController.EnableInput(); // Re-enable input for the pauser
            m_focusedPlayerController.SetControllerMap("UI"); // Let the pauser control the UI
            m_pauseCanvas.SetDisplayDetails(GetPlayerID(m_focusedPlayerController) + 1); // Update the PauseUI details
            EventSystemManager.Instance.SetCurrentSelectedGameObject(m_pauseCanvas.defaultSelectedObject);
            // end Halen

            //freeze time
            Time.timeScale = 0f;
        }
        else // if the game is unpaused
        {
            EnablePlayers(); // Enable input for all players - Halen
            m_focusedPlayerController.SetControllerMap("Player");

            //unfreeze time
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// returns true if the amount of rounds hass reached the rounds per game
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        bool isGameOver = false;
        
        // Check end conditions based on current game mode - Halen
        switch (m_gameMode)
        {
            case GameMode.NumberOfWins:
                foreach (int score in m_leaderboard)
                    if (score >= numberOfRounds) isGameOver = true;
                break;
            case GameMode.NumberOfRounds:
                if (m_roundNumber > numberOfRounds) isGameOver = true;
                break;
        }

        return isGameOver;
    }

    /// <summary>
    /// will delete old stage and instantiate and set up a new one
    /// </summary>
    public void LoadStage()
    {
        // Destroy any bullets that might remain in the level from the last level
        GameObject[] allRemainingBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (Object bullet in allRemainingBullets) Destroy(bullet);
        
        //destroy the current stage and load a new one
        if (m_currentStageObject) Destroy(m_currentStageObject);
        int random = Random.Range(0, stageList.Length);
        m_currentStageObject = Instantiate(stageList[random]);

        //reset and disable all players
        ResetPlayers();
        DisablePlayers();

        //randomize spawn order
        ShuffleSpawns(m_currentStageObject.GetComponent<Stage>().spawns);

        // set each player to a spawn - Halen
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            m_activePlayerControllers[i].gameObject.transform.position = m_currentStageObject.GetComponent<Stage>().spawns[i].position;
        }

        //keep track of what stage we are on
        m_roundNumber++;

        //keep track of dead players
        m_deadPlayers = 0;

        // Start round countdown, then enable all player input - Halen
        m_gameplayCanvas.gameObject.SetActive(true);
        m_gameplayCanvas.StartCountdown();
    }

    /// <summary>
    /// finds the winner and destroys the game
    /// </summary>
    public void EndGame()
    {
        // Halen
        m_pauseCanvas.gameObject.SetActive(false);
        m_gameplayCanvas.gameObject.SetActive(false);
        m_leaderboardCanvas.gameObject.SetActive(true);
        EventSystemManager.Instance.SetCurrentSelectedGameObject(m_leaderboardCanvas.defaultSelectedObject);
        m_gameState = GameState.Ended;
        // Halen

        //destroy the stage and players
        Destroy(m_currentStageObject);
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            Destroy(m_activePlayerControllers[i].gameObject);
        }

        //find the winner and that score
        int winnerIndex = 0;
        int topScore = 0;
        for (int i = 0; i < m_leaderboard.Count; i++)
        {
            if (m_leaderboard[i] > topScore)
            {
                winnerIndex = i;
                topScore = m_leaderboard[i];
            }
        }

        // Enable and update leaderboard canvas - Halen
        m_leaderboardCanvas.gameObject.SetActive(true);
        m_leaderboardCanvas.SetDisplayDetails(winnerIndex + 1, m_leaderboard);
    }

    /// <summary>
    /// Enable all active players.
    /// </summary>
    public void EnablePlayers()
    {
        foreach (PlayerController player in m_activePlayerControllers)
        {
            player.EnableInput();
        }
    }    

    /// <summary>
    /// Disable all active players.
    /// </summary>
    public void DisablePlayers()
    {
        foreach (PlayerController player in m_activePlayerControllers)
        {
            player.DisableInput();
        }
    }

    /// <summary>
    /// calls reset players on all players
    /// </summary>
    public void ResetPlayers()
    {
        foreach (PlayerController player in m_activePlayerControllers)
        {
            player.ResetPlayer();
        }
    }

    /// <summary>
    /// just loads the scene from the top again
    /// </summary>
    public void ResetGame()
    {
        Init();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// makes P1 take damage when you press 1
    /// </summary>
    private void DebugUpdate()
    { 
        if (Keyboard.current.digit1Key.isPressed)
        {
            m_activePlayerControllers[0].TakeDamage(1f);
        }
        if (Keyboard.current.digit2Key.isPressed)
        {
            deadPlayers = 0;
        }
        if (Keyboard.current.digit3Key.isPressed)
        {
            EndGame();
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
    /// Returns the ID (or player number) of a specified player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private int GetPlayerID(PlayerController player)
    {
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            if (player == m_activePlayerControllers[i]) return i;
        }
        return -1;
    }
}
