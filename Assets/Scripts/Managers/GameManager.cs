using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GameManager : MonoBehaviour
{
    private PlayerController m_focusedPlayerController;
    private int[] leaderBoard;
    [Tooltip("level prefabs")]
    public GameObject[] stageList;
    private int m_currentStage;

    public GameObject playerPrefab;
    public List<PlayerController> activePlayerControllers;
    private int m_deadPlayers;
    public bool isPaused;
    private bool m_started = false;

    [SerializeField]
    private List<Gamepad> m_controllers;


    /// the game managers start currently instantiates all the players
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

    // Update is called once per frame
    void Update()
    {
        if (!m_started)
        {
            CheckControllers();
            LoadFirst();
        }
    }

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

    private void LoadFirst()
    {
        for (int i = 0; m_controllers.Count > i; i++)
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
                m_started = true;
            }
        }
    }
}
