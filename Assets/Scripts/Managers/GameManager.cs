// GameManager - Cameron, Halen
// Manages level loading, round flow, mapping controller inputs, and UI
// last edit 1/11/2023

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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
    private List<Gamepad> m_controllers; // list of connected controllers
    private List<PlayerController> m_activePlayerControllers; // currently instantiated players
    private PlayerController m_focusedPlayerController; // for pause controls

    [Header("Canvas Prefabs")]
    public StartUI startCanvasPrefab;
    public GameplayUI gameplayCanvasPrefab;
    public PauseUI pauseCanvasPrefab;
    public LeaderboardUI leaderboardCanvasPrefab;
    public DisconnectUI disconnectCanvasPrefab;
    public DangerUI dangerCanvasPrefab;

    // canvas references
    private StartUI m_startCanvas;
    private GameplayUI m_gameplayCanvas;
    private PauseUI m_pauseCanvas;
    private LeaderboardUI m_leaderboardCanvas;
    private DisconnectUI m_disconnectCanvas;
    private DangerUI m_dangerCanvas;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera staticCamera;
    public CinemachineVirtualCamera gameplayCamera;

    [Header("Player")]
    public GameObject playerPrefab;

    public GameObject controlCube;
    private GameObject endController;

    [Header("Game Info")]
    public GameObject[] stageList;
    public int numberOfRounds;

    // stage tracking
    private GameObject m_currentStageObject;
    private int m_roundNumber = 0;

    [Space(10)]

    [SerializeField] private List<int> m_leaderboard;

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

                // Only need one player when debugging - Halen
                int requiredPlayers = 2;
#if UNITY_EDITOR
                requiredPlayers = 1;
#endif
                //if there are enough players and player 1 presses start
                if (m_controllers.Count >= requiredPlayers && m_controllers[0].startButton.isPressed)
                {
                    LoadFirst();
                }
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

        // Set start game state
        m_gameState = GameState.Start;
        m_isPaused = false;

        // Setup cameras
        staticCamera.gameObject.SetActive(true);
        gameplayCamera.gameObject.SetActive(false);

        // Initialise canvases
        m_startCanvas = Instantiate(startCanvasPrefab);
        m_gameplayCanvas = Instantiate(gameplayCanvasPrefab);
        m_pauseCanvas = Instantiate(pauseCanvasPrefab);
        m_leaderboardCanvas = Instantiate(leaderboardCanvasPrefab);
        m_disconnectCanvas = Instantiate(disconnectCanvasPrefab);
        m_dangerCanvas = Instantiate(dangerCanvasPrefab);

        m_startCanvas.gameObject.SetActive(true);
        m_gameplayCanvas.gameObject.SetActive(false);
        m_pauseCanvas.gameObject.SetActive(false);
        m_leaderboardCanvas.gameObject.SetActive(false);
        m_disconnectCanvas.gameObject.SetActive(false);
        m_dangerCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// checks if any controller has pressed east and stores them if so
    /// </summary>
    public void CheckControllers()
    {
        //for all of the controllers connected to the computer
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            //check if the current gamepad has a button pressed and is not stored
            if (Gamepad.all[i].allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic) && !m_controllers.Contains(Gamepad.all[i]))
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
    /// checks if any stored controller has pressed start and if so loads the first level and instantiates all connected players
    /// </summary>
    public void LoadFirst()
    {
        m_activePlayerControllers = new List<PlayerController>();
        //for every player
        for (int j = 0; m_controllers.Count > j; j++)
        {
            //create a player object and assign their specific controller
            GameObject newPlayer = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: m_controllers[j]).gameObject;

            // Set the player's colour based on their id
            newPlayer.gameObject.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Player/Player" + (j + 1).ToString());

            // get the PlayerController component from the newly instantiated player
            PlayerController playerController = newPlayer.GetComponent<PlayerController>();

            // Set player details
            playerController.controller = m_controllers[j];
            playerController.id = j;

            // make their controller rumble
            playerController.Rumble(.25f, .85f, 3f);

            // add player to list of players
            m_activePlayerControllers.Add(playerController);
        }

        // deactivate start menu/leaderboardUI, activate gameplayUI - Halen
        m_startCanvas.gameObject.SetActive(false);
        m_leaderboardCanvas.gameObject.SetActive(false);
        m_gameplayCanvas.gameObject.SetActive(true);

        // Set correct game state
        m_gameState = GameState.Playing;

        // Cinemachine camera setup
        // Set current active camera
        staticCamera.gameObject.SetActive(false);
        gameplayCamera.gameObject.SetActive(true);

        // Add the players to the target group target array
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>();
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            CinemachineTargetGroup.Target target;
            target.target = m_activePlayerControllers[i].transform;
            target.weight = m_activePlayerControllers[i].GetComponent<Rigidbody>().mass;
            target.radius = 5f;
            targets.Add(target);
        }
        GameObject.FindGameObjectWithTag("TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets = targets.ToArray<CinemachineTargetGroup.Target>();
        // End Cinemachine camera setup

        // Load the first stage
        LoadStage();
    }

    /// <summary>
    /// will delete the old stage and instantiate and set up a new one
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

        // Reset camera to default position
        CameraManager.Instance.SetCameraPosition(m_currentStageObject.GetComponent<Stage>().cameraDefaultTransform);

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

        // Start round countdown, then enable all player input - Halen
        m_gameplayCanvas.gameObject.SetActive(true);
        m_gameplayCanvas.StartCountdown();
        Time.timeScale = 0f;
    }
    /// <summary>
    /// A public method to be called by a player before it dies
    /// </summary>
    public void CheckIsRoundOver()
    {
        if (IsRoundOver())
        {
            // id of the winning player
            int playerID = 0;
            
            // increase the score of the last living player
            foreach (PlayerController player in m_activePlayerControllers)
            {
                if (!player.isDead)
                {
                    playerID = player.id;
                    m_leaderboard[player.id]++;
                    break;
                }
            }

            if (!IsGameOver()) EndRound(playerID);
            else EndGame();
        }
    }

    /// <summary>
    /// Returns if enough players have died to end the round
    /// </summary>
    /// <returns></returns>
    public bool IsRoundOver()
    {
        // track the number of dead players
        int deadPlayers = 0;

        // check how many players are dead
        foreach (PlayerController player in m_activePlayerControllers)
        {
            if (player.isDead) deadPlayers++;
        }

        if (deadPlayers == m_activePlayerControllers.Count - 1) return true;
        return false;
    }

    /// <summary>
    /// returns true if the end condition has been met
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
    /// finds the winner and destroys the game
    /// </summary>
    public void EndGame()
    {
        // Halen
        // Disable canvases
        m_pauseCanvas.gameObject.SetActive(false);
        m_gameplayCanvas.gameObject.SetActive(false);
        m_disconnectCanvas.gameObject.SetActive(false);

        // Set which button the player defaults to in the leaderboard menu
        EventSystemManager.Instance.SetCurrentSelectedGameObject(m_leaderboardCanvas.defaultSelectedObject);

        // Set correct gamestate
        m_gameState = GameState.Ended;
        // End Halen

        //destroy the stage and players
        Destroy(m_currentStageObject);
        foreach (PlayerController player in m_activePlayerControllers)
        {
            Destroy(player.gameObject);
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

        // Change to the static camera from the gameplay camera - Halen
        gameplayCamera.gameObject.SetActive(false);
        staticCamera.gameObject.SetActive(true);

        // Enable and update leaderboard canvas - Halen
        m_leaderboardCanvas.gameObject.SetActive(true);
        m_leaderboardCanvas.SetDisplayDetails(winnerIndex + 1, m_leaderboard);

        GameObject newPlayer = PlayerInput.Instantiate(controlCube, controlScheme: "Gamepad", pairWithDevice: m_controllers[0]).gameObject;

        EventSystemManager.Instance.SetPlayerToControl(controlCube.GetComponent<PlayerController>());
    }

    /// <summary>
    /// randomizes the spawns in the array to spawn players at random locations
    /// </summary>
    /// <param name="spawns"></param>
    public void ShuffleSpawns(Transform[] spawns)
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
    /// toggles the game's pause menu
    /// </summary>
    /// <param name="pauser"></param>
    public void TogglePause(int playerID)
    {
        m_isPaused = !m_isPaused; // Halen
        m_pauseCanvas.gameObject.SetActive(m_isPaused);

        if (m_isPaused) // if the game paused
        {
            m_focusedPlayerController = m_activePlayerControllers[playerID];

            // Halen
            DisablePlayers(); // Disable all player inputs
            m_focusedPlayerController.EnableInput(); // Re-enable input for the pauser
            m_focusedPlayerController.SetControllerMap("UI"); // Let the pauser control the UI
            m_pauseCanvas.SetDisplayDetails(m_focusedPlayerController.id + 1); // Update the PauseUI details
            EventSystemManager.Instance.SetCurrentSelectedGameObject(m_pauseCanvas.defaultSelectedObject);
            EventSystemManager.Instance.SetPlayerToControl(m_focusedPlayerController);
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

    public void Disconnected(int disconnectedPlayerID)
    {
        int playerID = disconnectedPlayerID + 1;
        m_disconnectCanvas.gameObject.SetActive(true);
        m_disconnectCanvas.SetText(playerID);
        Time.timeScale = 0f;
    }

    public void Reconnected()
    {
        m_disconnectCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void EndRound(int winningPlayerID)
    {
        DisablePlayers();
        m_gameplayCanvas.StartRoundEnd(winningPlayerID);
    }

    public void ShowDanger(float displayTime)
    {
        m_dangerCanvas.gameObject.SetActive(true);
        StartCoroutine(TurnOffDanger(displayTime));
    }

    private IEnumerator TurnOffDanger(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);
        m_dangerCanvas.gameObject.SetActive(false);
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
            player.gameObject.SetActive(true);
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
    /// various tools for debugging
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
        if (Keyboard.current.digit3Key.isPressed)
        {
            EndGame();
        }
    }

    
}
