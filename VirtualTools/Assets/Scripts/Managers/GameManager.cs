using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    private Player m_player;

    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<GameManager>();
            }
            return m_instance;
        }
    }

    public enum GAME_MODE
    {
        PLAYING,
        INSTRUCTION
    }

    private GAME_MODE m_gameMode;

    public void SetGameMode(GAME_MODE mode)
    {
        switch(mode)
        {
            case GAME_MODE.INSTRUCTION:
                m_player.SetMovementEnabled(false);
                break;
            case GAME_MODE.PLAYING:
                m_player.SetMovementEnabled(true);

                break;
        }
    }
    // Start is called before the first frame update
    // GameManager is set to be compiled after Player.cs, so when setting the game mode, the player reference is valid (see Project Setting -> Script execution order)
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        SetGameMode(GAME_MODE.INSTRUCTION);

        GUIManager.Instance.Init();

        string[] instructions = { "Hi, I'm Gustaf Von Horbert. Press the Fire button to dismiss my messages.", "How are you?", "Fuck off then!" };
        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(instructions, ()=> {
            SetGameMode(GAME_MODE.PLAYING);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
