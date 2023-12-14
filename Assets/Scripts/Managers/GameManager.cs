// GameManager - Cameron, Halen
// Manages level loading, round flow, mapping controller inputs, and UI
// last edit 14/12/2023

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
    private int gamepadCount;

    [Header("Canvas Prefabs")]
    public StartUI startCanvasPrefab;
    public GameplayUI gameplayCanvasPrefab;
    public PauseUI pauseCanvasPrefab;
    public LeaderboardUI leaderboardCanvasPrefab;
    public DisconnectUI disconnectCanvasPrefab;

    // canvas references
    private StartUI m_startCanvas;
    private GameplayUI m_gameplayCanvas;
    private PauseUI m_pauseCanvas;
    private LeaderboardUI m_leaderboardCanvas;
    private DisconnectUI m_disconnectCanvas;

    [Header("Cinemachine Prefabs")]
    [SerializeField] private CinemachineVirtualCamera m_staticCamera;
    [SerializeField] private CinemachineVirtualCamera m_gameplayCamera;
    [SerializeField] private CinemachineTargetGroup m_targetGroup;
    public float playerRadius;
    public float playerWeight;
    public float anchorRadius;
    public float anchorWeight;

    [Header("Player")]
    public GameObject playerPrefab;
    public Color[] playerColours;

    [Header("Game Info")]
    public GameObject[] stageList;
    private GameObject[] m_orderedStageList;
    public int numberOfRounds;

    [Header("Announcer Display Screen")]
    [SerializeField] private AnnouncerSubtitleDisplay m_announcerSubtitleDisplay;
    public AnnouncerCamera announcerCameraPrefab;
    private AnnouncerCamera m_announcerCamera;
    public float timeToNextPlayer;
    [Space(5)]
    [SerializeField] private List<int> m_leaderboard;

    // stage tracking
    private GameObject m_currentStageObject;
    private int m_roundNumber = 0;

    // Game mode tracking
    private enum GameMode
    {
        NumberOfWins = 0,
        NumberOfRounds = 1
    }
    private readonly GameMode m_gameMode;

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
        m_announcerCamera = Instantiate(announcerCameraPrefab);

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

    public void Init()
    {
        // Set timescale in case game was paused when reset
        Time.timeScale = 1.0f;

        // set all private variables to default values
        m_controllers = new List<Gamepad>();
        m_focusedPlayerController = null;
        m_activePlayerControllers = new List<PlayerController>();
        gamepadCount = Gamepad.all.Count;

        if (m_currentStageObject) Destroy(m_currentStageObject);
        m_roundNumber = 0;

        m_leaderboard = new List<int>();

        // Set start game state
        m_gameState = GameState.Start;
        m_isPaused = false;

        // Setup Cinemachine cameras and target group
        m_staticCamera.gameObject.SetActive(true);
        m_gameplayCamera.gameObject.SetActive(false);
        m_targetGroup.gameObject.SetActive(false);

        // Initialise canvases
        m_startCanvas = Instantiate(startCanvasPrefab);
        m_gameplayCanvas = Instantiate(gameplayCanvasPrefab);
        m_pauseCanvas = Instantiate(pauseCanvasPrefab);
        m_leaderboardCanvas = Instantiate(leaderboardCanvasPrefab);
        m_disconnectCanvas = Instantiate(disconnectCanvasPrefab);

        m_startCanvas.gameObject.SetActive(true);
        m_gameplayCanvas.gameObject.SetActive(false);
        m_pauseCanvas.gameObject.SetActive(false);
        m_leaderboardCanvas.gameObject.SetActive(false);
        m_disconnectCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// checks if any controller has pressed east and stores them if so
    /// </summary>
    public void CheckControllers()
    {
        //for all of the controllers connected to the computer, while there aren't already 4 players connected
        if (m_controllers.Count < 4)
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                //check if the current gamepad has a button pressed and is not stored
                if (Gamepad.all[i].allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic) && !m_controllers.Contains(Gamepad.all[i]))
                {
                    //store controller and add player to leaderboard
                    m_controllers.Add(Gamepad.all[i]);

                    // Since the list of connected controllers was updated, we need to update the StartUI to reflect that
                    m_startCanvas.SetDisplayDetails(m_controllers);
                }
            }
        }

        // if the number of gamepads has changed
        if (gamepadCount != Gamepad.all.Count)
        {
            List<Gamepad> dissconnected = new();
            
            foreach (Gamepad controller in m_controllers)
            {
                bool notFound = true;
            
                foreach (Gamepad gamepad in Gamepad.all)
                {
                    if (gamepad == controller)
                    {
                        notFound = false;
                        break;
                    }
                
                }
                if (notFound)
                {
                    dissconnected.Add(controller);
                    
                }
            }
            foreach(Gamepad controller in dissconnected)
            {
                m_controllers.Remove(controller);
                m_startCanvas.SetDisplayDetails(m_controllers);
            }
        }

        // update gamepad count
        gamepadCount = Gamepad.all.Count;

        for (int i = 0; i < m_controllers.Count; i++)
        {

            if (m_controllers[i].allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic))
            {
                m_startCanvas.ShowPlayerInput(true, i);
            }
            else
            {
                m_startCanvas.ShowPlayerInput(false, i);
            }
        }
        m_startCanvas.ShowControls(m_controllers);
    }

    /// <summary>
    /// checks if any stored controller has pressed start and if so loads the first level and instantiates all connected players
    /// </summary>
    public void LoadFirst()
    {
        if (m_currentStageObject != null)
        {
            Destroy(m_currentStageObject);
        }
        foreach (PlayerController player in m_activePlayerControllers)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }
        for (int i = 0; i < m_controllers.Count; i++)
        {
            bool found = false;
            for(int j = 0; j < Gamepad.all.Count; j++)
            {
                if (m_controllers[i].deviceId == Gamepad.all[j].deviceId)
                    found = true;
            }

            if (!found)
            {
                return;
            }
        }

        m_activePlayerControllers = new List<PlayerController>();
        m_leaderboard = new List<int>();

        //for every player
        for (int j = 0; m_controllers.Count > j; j++)
        {
            //create a player object and assign their specific controller
            GameObject newPlayer = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: m_controllers[j]).gameObject;

            // set auto swap controls to false
            newPlayer.GetComponent<PlayerInput>().neverAutoSwitchControlSchemes = true;

            // get the PlayerController component from the newly instantiated player
            PlayerController playerController = newPlayer.GetComponent<PlayerController>();

            // Set player details
            playerController.Init(m_controllers[j], j, playerColours[j]);

            // make their controller rumble
            //playerController.Rumble(.25f, .85f, 3f);

            // add player to list of players
            m_activePlayerControllers.Add(playerController);

            // add slot to leaderboard
            m_leaderboard.Add(0);
        }

        // reset variables in case players chose to play again - halen
        if (m_currentStageObject) Destroy(m_currentStageObject);
        m_roundNumber = 0;

        // deactivate start menu/leaderboardUI, activate gameplayUI - Halen
        m_startCanvas.gameObject.SetActive(false);
        m_leaderboardCanvas.gameObject.SetActive(false);
        m_gameplayCanvas.gameObject.SetActive(true);

        // Set correct game state
        m_gameState = GameState.Playing;

        // Set current active camera
        m_staticCamera.gameObject.SetActive(false);
        m_gameplayCamera.gameObject.SetActive(true);
        m_targetGroup.gameObject.SetActive(true);

        // update cinemachine camera
        UpdateCameraTargetGroup();

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

        // Destroy any remaining powerup indicators in the scene
        PickupIndicator[] remainingIndicators = FindObjectsOfType<PickupIndicator>();
        foreach (PickupIndicator indicator in remainingIndicators) Destroy(indicator.gameObject);

        // destroy any remaining powerups in the scene
        PowerUp[] allPowerUps = FindObjectsOfType<PowerUp>();
        foreach (Object powerup in allPowerUps) Destroy(powerup);

        // destroy any remaining spawned objects in the scene
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Spawned");
        foreach (Object spawnedObject in spawnedObjects) Destroy(spawnedObject);

        //destroy the current stage and load the next one
        if (m_currentStageObject) Destroy(m_currentStageObject);

        // modulus with 0 will cause errors
        int index = m_roundNumber % stageList.Length;
        if (index == 0) m_orderedStageList = ShuffleList(stageList).ToArray(); 
        m_currentStageObject = Instantiate(m_orderedStageList[index]);

        // Reset camera to default position
        CameraManager.Instance.SetCameraPosition(m_currentStageObject.GetComponent<Stage>().cameraDefaultTransform);

        //reset and disable all players
        ResetPlayers();
        DisablePlayers();

        //randomize spawn order
        Stage currentStage = m_currentStageObject.GetComponent<Stage>();
        currentStage.playerSpawns = ShuffleList(currentStage.playerSpawns).ToArray();

        // set each player to a spawn - Halen
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            m_activePlayerControllers[i].transform.position = currentStage.playerSpawns[i].position;
        }

        //keep track of what stage we are on
        m_roundNumber++;

        UpdateCameraTargetGroup();

        // Start round countdown, then enable all player input - Halen
        m_leaderboardCanvas.gameObject.SetActive(false);
        m_gameplayCanvas.gameObject.SetActive(true);
        m_gameplayCanvas.StartCountdown();
        m_isPaused = false;
        StartAnnouncement(AnnouncerSubtitleDisplay.AnnouncementType.BeforeRound);
        StopCoroutine(ChangeDisplayLater());
        ChangeAnnouncerDisplay();
    }

    /// <summary>
    /// Called by a player when it dies to check if the round has ended.
    /// Will either progress to the next round or end the game.
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
    private bool IsRoundOver()
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

    private void EndRound(int winningPlayerID)
    {
        Time.timeScale = 0.1f;
        StartAnnouncement(AnnouncerSubtitleDisplay.AnnouncementType.EndRound); // this needs to be in subtitleUI

        // Disable players and toggle relevant canvases
        DisablePlayers();
        m_leaderboardCanvas.gameObject.SetActive(true);
        m_leaderboardCanvas.SetDisplayDetails(winningPlayerID, m_leaderboard, false);
        m_gameplayCanvas.SetDisplayDetails(winningPlayerID + 1);

        m_gameplayCanvas.StartRoundEnd(winningPlayerID);
    }

    /// <summary>
    /// finds the winner and destroys the game
    /// </summary>
    public void EndGame()
    {
        // Set which button the player defaults to in the leaderboard menu
        EventSystemManager.Instance.SetCurrentSelectedGameObject(m_leaderboardCanvas.defaultSelectedObject);

        // Set correct gamestate
        m_gameState = GameState.Ended;
        // End Halen

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
        m_leaderboardCanvas.SetDisplayDetails(winnerIndex, m_leaderboard, true);

        // disable players and set UI controls
        DisablePlayers();

        Time.timeScale = 1f;
        EventSystemManager.Instance.SetPlayerToControl(m_activePlayerControllers[winnerIndex]);
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
            m_pauseCanvas.SetDisplayDetails(m_focusedPlayerController.id); // Update the PauseUI details
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

    /// <summary>
    /// Enables the laser warning image on the GameplayUI.
    /// </summary>
    public void ShowEndLaserWarning()
    {
        m_gameplayCanvas.ShowLaserWarning();
    }

    public void StartAnnouncement(AnnouncerSubtitleDisplay.AnnouncementType announcementType)
    {
        m_announcerSubtitleDisplay.StartAnnouncement(announcementType);
    }

    public void StopAnnouncer()
    {
        m_announcerSubtitleDisplay.StopText();
    }

    /// <summary>
    /// Updates the array of targets that the CinemachineTargetGroup object tracks for the Gameplay Camera to follow.
    /// </summary>
    public void UpdateCameraTargetGroup()
    {
        // Add the players to the target group target array
        List<CinemachineTargetGroup.Target> targets = new();
        for (int i = 0; i < m_activePlayerControllers.Count; i++)
        {
            if (!m_activePlayerControllers[i].isDead)
            {
                CinemachineTargetGroup.Target target;
                target.target = m_activePlayerControllers[i].transform;
                target.weight = playerWeight;
                target.radius = playerRadius;
                targets.Add(target);
            }
        }

        GameObject[] anchors = GameObject.FindGameObjectsWithTag("Anchor");
        for (int i = 0; i < anchors.Length; i++)
        {
            CinemachineTargetGroup.Target target;
            target.target = anchors[i].transform;
            target.weight = anchorWeight;
            target.radius = anchorRadius;
            targets.Add(target);
        }

        m_targetGroup.m_Targets = targets.ToArray<CinemachineTargetGroup.Target>();
        // End Cinemachine camera setup
    }

    public void ChangeAnnouncerDisplay()
    {
        int loopCount = 0;
        int randomIndex = Random.Range(0, m_activePlayerControllers.Count);
        while (loopCount < m_activePlayerControllers.Count)
        {
            int realIndex = randomIndex + loopCount;
            if (realIndex >= m_activePlayerControllers.Count) realIndex -= m_activePlayerControllers.Count;

            if (!m_activePlayerControllers[realIndex].isDead)
            {
                m_announcerCamera.SetNewParent(m_activePlayerControllers[realIndex].transform);
            }
            loopCount++;
        }
        
        StartCoroutine(ChangeDisplayLater());
    }

    private IEnumerator ChangeDisplayLater()
    {
        yield return new WaitForSeconds(timeToNextPlayer);
        ChangeAnnouncerDisplay();

    }

    public PlayerController GetPlayer(int id)
    {
        return m_activePlayerControllers[id];
    }

    /// <summary>
    /// Randomises the order of a list and returns it as a new list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    private IList<T> ShuffleList<T>(IList<T> list)
    {
        IList<T> shuffledList = list.ToList();
        int count = shuffledList.Count;
        while (count > 1)
        {
            count--;
            int index = Random.Range(0, count + 1);
            T value = shuffledList[index];
            shuffledList[index] = shuffledList[count];
            shuffledList[count] = value;
        }
        return shuffledList;
    }

    /// <summary>
    /// When a controller disconnects.
    /// </summary>
    /// <param name="playerID"></param>
    public void Disconnected(int playerID)
    {
        m_disconnectCanvas.gameObject.SetActive(true);
        m_disconnectCanvas.SetDisplayDetails(playerID);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// When a disconnected controller reconnects.
    /// </summary>
    public void Reconnected()
    {
        m_disconnectCanvas.gameObject.SetActive(false);
        // only reset timescale if game isn't paused
        if (!m_isPaused) Time.timeScale = 1f;
    }

    /// <summary>
    /// Enable all active players.
    /// </summary>
    public void EnablePlayers()
    {
        foreach (PlayerController player in m_activePlayerControllers)
        {
            player.gameObject.SetActive(true);
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
        for (int i = 0; i < m_activePlayerControllers.Count; i ++)
        {
            m_activePlayerControllers[i].gameObject.SetActive(true);
            m_activePlayerControllers[i].GetComponent<PlayerInput>().SwitchCurrentControlScheme(m_controllers[i]);
            m_activePlayerControllers[i].ResetPlayer();
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
            //m_activePlayerControllers[0].TakeDamage(1f, new string["hgf", "tgufhg"]);
        }
        if (Keyboard.current.digit2Key.isPressed)
        {
            
        }
        if (Keyboard.current.digit3Key.isPressed)
        {
            EndGame();
        }
    }

    public void StartControllerRumbleRoutine(int id, float lowFrequncy, float highFrequency, float time)
    {
        
        StartCoroutine(ControllerRumbleRoutine(id, highFrequency, lowFrequncy, time));

    }

    private IEnumerator ControllerRumbleRoutine(int id, float lowFrequncy, float highFrequency, float time)
    {
        m_controllers[id].SetMotorSpeeds(lowFrequncy, highFrequency);
        yield return new WaitForSeconds(time);
        m_controllers[id].SetMotorSpeeds(0, 0);
    }
}
