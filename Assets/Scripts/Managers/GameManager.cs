using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Static reference
    public static GameManager Instance { get; private set; }
    
    private PlayerController m_focusedPlayerController;
    [SerializeField]
    private int[] m_leaderBoard;
    [Tooltip("level prefabs")]
    public GameObject[] stageList;
    private int m_currentStage;

    public TMP_Text controllerCount;

    public GameObject playerPrefab;
    public List<PlayerController> activePlayerControllers;
    private int m_deadPlayers;
    private bool m_isPaused;
    private bool m_started = false;

    public Canvas pauseCanvas;

    public int deadPlayers
    {
        get { return m_deadPlayers; }
        set
        {
            m_deadPlayers = value;
            if (m_deadPlayers > activePlayerControllers.Count - 1)
            {
                // round is over
            }
        }
    }

    [SerializeField]
    private List<Gamepad> m_controllers;

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
        
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log(Gamepad.all[i]);
        }
            //activePlayerControllers.Add(FindObjectOfType<PlayerController>());

            //Debug.Log(InputSystem.devices.Count);

            //for (int i = 0; i < activePlayerControllers[0].GetComponent<PlayerInput>().devices.Count; i++)
            //{
            //    Debug.Log(activePlayerControllers[0].GetComponent<PlayerInput>().devices[i]);
            //}

            //for (int i = 0; i < Gamepad.all.Count; i++)
            //{
            //    Debug.Log(Gamepad.all[i]);
            //    
            //    GameObject newPlayer = Instantiate(playerPrefab, spawns[i].position, Quaternion.identity);
            //    activePlayerControllers.Add(newPlayer.GetComponent<PlayerController>());
            //}
    }

    /// <summary>
    ///  at the moment update is just incharge of input for the start screen and is calling some blank functions for later
    /// </summary>
    void Update()
    {
        DebugLeaderBoard();
        if (!m_started)
        {
            controllerCount.text = "active players: " + m_controllers.Count.ToString();
            CheckControllers();
            LoadFirst();
        } else
        {

        }
    }

    /// <summary>
    /// checks if any controller has pressed east and stores them if so
    /// </summary>
    private void CheckControllers()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            //check if the current gamepad has pressed east
            if (Gamepad.all[i].buttonEast.isPressed && !m_controllers.Contains(Gamepad.all[i]))
            {
                m_controllers.Add(Gamepad.all[i]);
            }
        }
    }

    /// <summary>
    /// checks if any stored controller has pressed start and if so loads the level and creates the appropriate players
    /// </summary>
    private void LoadFirst()
    {
        
         if (m_controllers.Count > 0 && m_controllers[0].startButton.isPressed)
         {
             Instantiate(stageList[0]);
             Transform[] spawns = FindObjectOfType<Stage>().spawns;
             for (int j = 0; m_controllers.Count > j; j++)
             {
                 GameObject newPlayer = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: Gamepad.all[j]).gameObject;
                 newPlayer.transform.position = spawns[j].transform.position;
                 activePlayerControllers.Add(newPlayer.GetComponent<PlayerController>());
             }
            Destroy(controllerCount.gameObject);
             m_started = true;
         }
        
    }

    /// <summary>
    /// enables and isables inputs for pressing pause
    /// </summary>
    /// <param name="pauser"></param>
    public void TogglePause(PlayerController pauser)
    {
        if (m_isPaused == false)
        {
            m_focusedPlayerController = pauser;

            for (int i = 0; i < activePlayerControllers.Count; i++)
            {
                if (activePlayerControllers[i] != m_focusedPlayerController)
                {
                    activePlayerControllers[i].DisableInput();
                }
            }
            //m_focusedPlayerController.SetControllerMap("UI");

            pauseCanvas.enabled = true;

            Time.timeScale = 0f;
            m_isPaused = true;

        }
        else
        {
            for (int i = 0; i < activePlayerControllers.Count; i++)
            {
                if (activePlayerControllers[i] != m_focusedPlayerController)
                {
                    activePlayerControllers[i].EnableInput();
                }
            }
            //m_focusedPlayerController.SetControllerMap("Player");

            pauseCanvas.enabled = false;

            Time.timeScale = 1f;
            m_isPaused = false;
        }
            
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool IsRoundOver()
    {
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool IsGameOver()
    {
        return false;
    }

    /// <summary>
    /// will delete old scen and instantiate and set up a new one
    /// </summary>
    /// <param name="newGame"></param>
    private void LoadStage(bool newGame)
    {

    }

    /// <summary>
    /// calls reset players on all players
    /// </summary>
    private void ResetPlayers()
    {
        for (int i = 0; i < activePlayerControllers.Count; i++)
        {
            activePlayerControllers[i].ResetPlayer();
        }
    }

    /// <summary>
    /// just loads the scene from the top again
    /// </summary>
    public void ResetGame()
    {
        //SceneManager.LoadScene();
    }

    public void DebugLeaderBoard()
    { 
        if (Keyboard.current.digit1Key.isPressed)
        {
            m_leaderBoard[0]++;
        }
        if (Keyboard.current.digit2Key.isPressed)
        {
            m_leaderBoard[1]++;
        }
        if (Keyboard.current.digit3Key.isPressed)
        {
            m_leaderBoard[2]++;
        }
        if (Keyboard.current.digit4Key.isPressed)
        {
            m_leaderBoard[3]++;
        }

    }
}
