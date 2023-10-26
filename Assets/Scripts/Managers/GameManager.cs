// GameManager - Cameron, Halen
// Manages level loading, round flow, mapping controller inputs, and UI
// last edit 26/10/2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static reference
    public static GameManager Instance { get; private set; }
    
    //input related variables
    private PlayerController m_focusedPlayerController;
    private List<Gamepad> m_controllers;
    private List<PlayerController> m_activePlayerControllers;

    [Header("Canvases")]
    public StartUI startCanvas;
    public GameplayUI gameplayCanvas;
    public PauseUI pauseCanvas;
    public LeaderboardUI leaderboardCanvas;

    [Header("Player")]
    public GameObject playerPrefab;

    [Header("Game Info")]
    public GameObject[] stageList;
    public int numberOfRounds;

    // stage tracking
    private GameObject m_currentStageObject;
    private int m_roundNumber = 0;

    // score tracking
    [SerializeField] private List<int> m_leaderBoard;
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
                if (!IsGameOver())
                {
                    LoadStage();
                }
                else
                {
                    m_gameState = GameState.Ended;
                    EndGame();
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

    // initalizing
    void Start()
    {
        m_controllers = new List<Gamepad>();

        m_activePlayerControllers = new List<PlayerController>();

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log(Gamepad.all[i]);
        }

        // UI states
        startCanvas.gameObject.SetActive(true);
        gameplayCanvas.gameObject.SetActive(false);
        pauseCanvas.gameObject.SetActive(false);
        leaderboardCanvas.gameObject.SetActive(false);

        // Set start game state
        m_gameState = GameState.Start;
        m_isPaused = false;
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
                GameOverUpdate();
                break;
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

                // Since the list of connected controllers was updated, we need to update the StartUI to reflect that
                startCanvas.SetDisplayDetails(m_controllers);
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
            // disable the StartUI canvas and enable the GameplayUI canvas - Halen
            startCanvas.gameObject.SetActive(false);
            gameplayCanvas.gameObject.SetActive(true);
            
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
                //create a player object and assign their specific controller
                GameObject newPlayer = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: m_controllers[j]).gameObject;

                //set that player to a spawn
                newPlayer.transform.position = spawns[j].transform.position;

                // add player to list of players
                m_activePlayerControllers.Add(newPlayer.GetComponent<PlayerController>());
            }

            // deactivate start menu, activate gameplayUI - Halen
            startCanvas.gameObject.SetActive(false);
            gameplayCanvas.gameObject.SetActive(true);

            // Set correct game state
            m_gameState = GameState.Playing;
        }
    }

    /// <summary>
    /// enables and isables inputs for pressing pause
    /// </summary>
    /// <param name="pauser"></param>
    public void TogglePause(PlayerController pauser)
    {
        m_isPaused = !m_isPaused; // Halen
        pauseCanvas.gameObject.SetActive(m_isPaused);

        if (m_isPaused) //if the game paused
        {
            if (pauser != null) m_focusedPlayerController = pauser; // Can't pass a player through if pause is called by the button on the pause menu - Halen
            else m_focusedPlayerController = null;

            //disable input for every player except the pauser
            for (int i = 0; i < m_activePlayerControllers.Count; i++)
            {
                if (m_activePlayerControllers[i] != m_focusedPlayerController)
                {
                    m_activePlayerControllers[i].DisableInput();

                    // Update the PauseUI details - Halen
                    pauseCanvas.SetDisplayDetails(i);
                }
            }
            //give pauser menu controls instead of playing controls
            //m_focusedPlayerController.SetControllerMap("UI");

            //freeze time
            Time.timeScale = 0f;
        }
        else // if the game is unpaused
        {
            //enable the input on every player exept the unpauser
            for (int i = 0; i < m_activePlayerControllers.Count; i++)
            {
                m_activePlayerControllers[i].EnableInput();
            }
            //give unpauser playing controls
            // if (pauser != null) m_focusedPlayerController.SetControllerMap("Player");

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
                foreach (int score in m_leaderBoard)
                    if (score >= numberOfRounds) isGameOver = true;
                break;
            case GameMode.NumberOfRounds:
                if (m_roundNumber > numberOfRounds) isGameOver = true;
                break;
        }
        
        // Game has ended, deactivate gameplayUI and enable leaderboardUI - Halen
        if (isGameOver)
        {
            gameplayCanvas.gameObject.SetActive(false);
            leaderboardCanvas.gameObject.SetActive(true);
        }

        return isGameOver;
    }

    /// <summary>
    /// will delete old stage and instantiate and set up a new one
    /// </summary>
    public void LoadStage()
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
        m_roundNumber++;

        //keep track of dead players
        m_deadPlayers = 0;

        // Start round countdown, then enable all player input - Halen
        gameplayCanvas.StartCountdown();
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
    public void EndGame()
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

        // Enable and update leaderboard canvas - Halen
        leaderboardCanvas.gameObject.SetActive(true);
        leaderboardCanvas.SetDisplayDetails(winnerIndex + 1, m_leaderBoard);
    }
}
