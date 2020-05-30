using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public string MockStudentNumber; //TODO: remove this
    private bool m_isQuitting = false; //Used when the user presses the cancel axis to mimic GetButtonDown
    private bool assessmentMode;
    private bool isKeyboard = true;
    private bool prevIsKeyboard = true;
    public bool WillShowTutorials { set; get; }

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

    public bool IsUsingKeyboard()
    {
        return isKeyboard;
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

                if (SessionManager.Instance.GetCurrentSession().sessionType == Session.SESSION_TYPE.INSTRUMENT_POSITIONING)
                    SessionManager.Instance.ReadFinalInstrumentPositioning();

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

        assessmentMode = PlayerPrefs.GetInt("AssessmentMode", 0) == 1;
        GUIManager.Instance.GetMainCanvas().SetSceneSelectionGUIOn(false);
        GUIManager.Instance.GetMainCanvas().SetShowTutorialsPanelOn(true);
       
    }

    public void SetTurorialFlags(bool show)
    {
        WillShowTutorials = show;
        Player.Instance.SetTutorialsFlags(!show);
    }

    public void ShowTutorialsCallback(bool show)
    {
        GUIManager.Instance.GetMainCanvas().SetShowTutorialsPanelOn(false);
        SetTurorialFlags(show);
        //If assessment mode, do not allow screen selection
        if (assessmentMode)
        {
            //Read which mode they will be assessed on
            SessionManager.Instance.GenerateSelectByNameSession();
            SessionManager.Instance.GenerateSelectByPurposeSession();
            SessionManager.Instance.GenerateSelectInstrumentPositionSession();

            SessionManager.Instance.StartSessionSequence();
        }
        else
        {
            GUIManager.Instance.GetMainCanvas().SetSceneSelectionGUIOn(true);

        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if (isKeyboard)
            isKeyboard = !isControlerInput();
        else
            isKeyboard = isMouseKeyboard();
        
        if(isKeyboard != prevIsKeyboard)
        {
            if(isKeyboard)
                Debug.Log("Input changed to KEYBOARD");
            else
                Debug.Log("Input changed to GAMEPAD");


        }
        prevIsKeyboard = isKeyboard;
    }

    private bool isMouseKeyboard()
    {
        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2))
            return true;


        // mouse movement
        if (Input.GetAxis("Mouse X") != 0.0f ||
            Input.GetAxis("Mouse Y") != 0.0f)
        {
            return true;
        }

        if (Input.GetAxis("KeyboardX") != 0 || Input.GetAxis("KeyboardY") != 0)
        {
            return true;
        }

        return false;
    }

    private bool isControlerInput()
    {
        // joystick buttons
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
           Input.GetKey(KeyCode.Joystick1Button1) ||
           Input.GetKey(KeyCode.Joystick1Button2) ||
           Input.GetKey(KeyCode.Joystick1Button3) ||
           Input.GetKey(KeyCode.Joystick1Button4) ||
           Input.GetKey(KeyCode.Joystick1Button5) ||
           Input.GetKey(KeyCode.Joystick1Button6) ||
           Input.GetKey(KeyCode.Joystick1Button7) ||
           Input.GetKey(KeyCode.Joystick1Button8) ||
           Input.GetKey(KeyCode.Joystick1Button9) ||
           Input.GetKey(KeyCode.Joystick1Button10) ||
           Input.GetKey(KeyCode.Joystick1Button11) ||
           Input.GetKey(KeyCode.Joystick1Button12) ||
           Input.GetKey(KeyCode.Joystick1Button13) ||
           Input.GetKey(KeyCode.Joystick1Button14) ||
           Input.GetKey(KeyCode.Joystick1Button15) ||
           Input.GetKey(KeyCode.Joystick1Button16) ||
           Input.GetKey(KeyCode.Joystick1Button17) ||
           Input.GetKey(KeyCode.Joystick1Button18) ||
           Input.GetKey(KeyCode.Joystick1Button19))
        {
            return true;
        }

        // joystick axis
        if (Input.GetAxis("GamepadY") != 0  || Input.GetAxis("GamepadX") != 0 )
        {
            return true;
        }

        return false;
    }
}
