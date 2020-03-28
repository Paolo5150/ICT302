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

    public MainCanvas GetMainCanvas()
    {
        return m_mainCanvas;
    }

    public void Init()
    {
        m_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvas>();
        m_mainCanvas.Init();

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
