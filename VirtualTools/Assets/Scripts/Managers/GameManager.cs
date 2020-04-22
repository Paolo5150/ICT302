using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public string MockStudentNumber; //TODO: remove this
    private bool m_isQuitting = false; //Used when the user presses the cancel axis to mimic GetButtonDown

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

    private void ProcessTest()
    {
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        // You can start any process, HelloWorld is a do-nothing example.
        p.StartInfo.FileName = "W://HelloWorld.exe";
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.UseShellExecute = false;
        p.ErrorDataReceived += (s, a) => {
            Logger.LogToFile("Wtf is this", "process.txt");
        };
        p.Start();


    }

    public void Quit()
    {
        if (!m_isQuitting)
        {
            m_isQuitting = true;
            Player.Instance.FreezePlayer(true);
            GUIManager.Instance.GetMainCanvas().DogSpeak("Quitting...");

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
                 ()=> {
                     Logger.LogToFile("Quitting now");
                     Application.Quit();
                 });
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
