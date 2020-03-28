using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    private static GUIManager m_instance;
    private MainCanvas m_mainCanvas;
    public static GUIManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<GUIManager>();
            }
            return m_instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvas>();

        string[] instructions = { "Hi, I'm Gustaf Von Horbert. Press the Fire button to dismiss my messages.", "How are you?", "Fuck off then!" };
        m_mainCanvas.DogInstructionSequence(instructions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
