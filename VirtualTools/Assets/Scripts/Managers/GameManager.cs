using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using System.Diagnostics;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public string MockStudentNumber; //TODO: remove this
    private bool m_isQuitting = false; //Used when the user presses the cancel axis to mimic GetButtonDown
    private bool assessmentMode;
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

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsAssessmentMode()
    {
        return assessmentMode;
    }

    public void Quit()
    {
        if (!m_isQuitting)
        {
            m_isQuitting = true;
            Player.Instance.FreezePlayer(true);
            GUIManager.Instance.GetMainCanvas().DogSpeak("Quitting...");



            //Wait until data is pushed to server before quitting
            if(SessionManager.Instance.GetCurrentSession() != null)
            {
                if (!SessionManager.Instance.GetCurrentSession().sessionResults.completed)
                    SessionManager.Instance.GetCurrentSession().sessionResults.Log_SimulationClosedPrematurely();

                string json = SessionManager.Instance.CreateJSONString(SessionManager.Instance.GetCurrentSession());
                WWWForm form = SessionManager.Instance.GetSessionForm(json);
                Logger.LogToFile("Just about to send request, " + SessionManager.Instance.GetCurrentSession().GetID());
                NetworkManager.Instance.SendRequest(form, "recordSession.php",
                     (string reply) => {

                         Logger.LogToFile("Session recorded, id " + SessionManager.Instance.GetCurrentSession().GetID());
                         Logger.LogToFile("Reply" + reply);
                         Logger.LogToFile("Quitting now");
                         Application.Quit();
                     },
                     () => {

                         Logger.LogToFile("Failed to upload results, id " + SessionManager.Instance.GetCurrentSession().GetID());
                     },
                     () => {
                         Logger.LogToFile("Quitting now");
                         Application.Quit();
                     });
            }
            else
            {
                Application.Quit();
            }
           
        }
    }
  


    // Start is called before the first frame update
    // GameManager is set to be compiled after Player.cs, so when setting the game mode, the player reference is valid (see Project Setting -> Script execution order)
    void Start()
    {                
        // Initialize other managers here
        GUIManager.Instance.Init();
        Player.Instance.Init();
        GUIManager.Instance.ConfigureCursor(true, CursorLockMode.None);
        Player.Instance.FreezePlayer(true);

        string order = PlayerPrefs.GetString("InstrumentOrder");

        if (!order.Equals(""))
        {
            InstrumentLocManager.Instance.PlaceInstrumentsInOrder(order);
        }
        else
        {
            // Read insturment order
            List<Instrument.INSTRUMENT_TAG> allTags = new List<Instrument.INSTRUMENT_TAG>();

            foreach(Instrument.INSTRUMENT_TAG tag in Enum.GetValues(typeof(Instrument.INSTRUMENT_TAG)))
            {
                if(tag != Instrument.INSTRUMENT_TAG.NONE)
                    allTags.Add(tag);
            }

            //Shuffle
            System.Random rand = new System.Random();
            int n = allTags.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n);
                Instrument.INSTRUMENT_TAG value = allTags[k];
                allTags[k] = allTags[n];
                allTags[n] = value;
            }

            InstrumentLocManager.Instance.PlaceInstrumentsInOrder(allTags);
        }

        if (PlayerPrefs.HasKey("AssessmentMode"))
        {
            assessmentMode = PlayerPrefs.GetInt("AssessmentMode") == 1;
        }
        

        //If assessment mode, do not allow screen selection
        if(assessmentMode)
        {
            GUIManager.Instance.GetMainCanvas().HideSceneSelectionGUI();
            GUIManager.Instance.GetMainCanvas().SetAssessmentModePanel(true);

            //Read which mode they will be assessed on
            SessionManager.Instance.StartSessionByType(Session.SESSION_TYPE.SELECT_BY_NAME);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
