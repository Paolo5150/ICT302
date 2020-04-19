﻿using System.Collections;
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
                GameObject coreGameObject = GameObject.Find("Managers");
                m_instance = coreGameObject.AddComponent<GUIManager>();
            }

            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
            m_instance = GetComponent<GUIManager>();
        else
            DestroyImmediate(this);
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

    public void ConfigureCursor(bool visible, CursorLockMode lockstate)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockstate;
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
