﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
        StartCoroutine(SendRequest());

        // Initialize other managers here
        GUIManager.Instance.Init();
        Player.Instance.Init();
        Cursor.visible = false;

        //Will create a session manager
        session = new Session();
        InstrumentSelectTask task1 = new InstrumentSelectTask(Instrument.INSTRUMENT_TAG.SCALP);
        InstrumentSelectTask task2 = new InstrumentSelectTask(Instrument.INSTRUMENT_TAG.SCISSOR);

        session.AddTask(task1);
        session.AddTask(task2);

        session.Start();

    }

    // Update is called once per frame
    void Update()
    {
        session.Update();
    }

    IEnumerator SendRequest()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", "shri");
        form.AddField("password", "Faila");

        UnityWebRequest www = UnityWebRequest.Post("https://unreckoned-worry.000webhostapp.com/login.php", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        { 
            Debug.Log("WWWForm: " + www.downloadHandler.text);
        }
    }
}
