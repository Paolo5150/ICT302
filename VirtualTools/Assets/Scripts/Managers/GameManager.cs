using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
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


  
    // Start is called before the first frame update
    // GameManager is set to be compiled after Player.cs, so when setting the game mode, the player reference is valid (see Project Setting -> Script execution order)
    void Start()
    {
        // Initialize other managers here
        GUIManager.Instance.Init();

        Player.Instance.Init();

        session = new Session();
        InstrumentSelectTask task1 = new InstrumentSelectTask(Instrument.INSTRUMENT_TAG.SCALP);
        InstrumentSelectTask task2 = new InstrumentSelectTask(Instrument.INSTRUMENT_TAG.SCISSOR);

        session.AddTask(task1);
        session.AddTask(task2);

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
