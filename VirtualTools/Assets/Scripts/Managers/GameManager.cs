using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public Player m_player;
    Session session;

    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject coreGameObject = GameObject.Find("Managers");
                m_instance = coreGameObject.AddComponent<GameManager>();
            }

            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
            m_instance = GetComponent<GameManager>();
        else
            DestroyImmediate(this);
    }
  
    public enum GAME_MODE
    {
        PLAYING,
        INSTRUCTION,

    }

    private GAME_MODE m_gameMode;

    public void SetGameMode(GAME_MODE mode)
    {
        switch(mode)
        {
            case GAME_MODE.INSTRUCTION:
                m_player.SetMovementEnabled(false);
                m_player.enabled = false;
                break;
            case GAME_MODE.PLAYING:
                m_player.SetMovementEnabled(true);
                m_player.enabled = true;

                break;
        }
    }
    // Start is called before the first frame update
    // GameManager is set to be compiled after Player.cs, so when setting the game mode, the player reference is valid (see Project Setting -> Script execution order)
    void Start()
    {
        // Initialize other managers here
        GUIManager.Instance.Init();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_player.Init();

        session = new Session();
        InstrumentSelectTask task = new InstrumentSelectTask(Instrument.INSTRUMENT_TAG.SCALP);

        session.AddTask(task);
        session.Start();
       /* SetGameMode(GAME_MODE.INSTRUCTION);

        string[] instructions = { "Hi, I'm Gustaf Von Horbert. Press the Fire button to dismiss my messages.",
                                    "How are you?",
                                    "Nice!" };

        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(instructions, ()=> {
            SetGameMode(GAME_MODE.PLAYING);
        });*/
    }

    // Update is called once per frame
    void Update()
    {
        session.Update();
    }
}
