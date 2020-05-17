using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;
using UnityStandardAssets.Characters.FirstPerson;

public class SessionManager : MonoBehaviour
{
    private static SessionManager m_instance;
    private List<Session> m_allSessions;
    private Session m_currentSession;
    private bool m_isCurrentSessionPaused = false;
    private bool m_previousCancelButtonStatus = false;
    public static SessionManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<SessionManager>();
            }

            return m_instance;
        }
    }

    public void Unregister()
    {
        Player.instrumentSelectedEvent -= OnInstrumentSelected;
    }

    private SessionManager()
    {
        m_allSessions = new List<Session>();
        Player.instrumentSelectedEvent += OnInstrumentSelected;

    }

    public Session GetCurrentSession()
    {
        return m_currentSession;
    }

    public Session GenerateSelectInstrumentPositionSession()
    {
        Session session = new Session(GenerateID());
        List<InstrumentPositionTask> allTasks = new List<InstrumentPositionTask>();
        session.SetSessionName("Instrument Positioning");
        session.sessionType = Session.SESSION_TYPE.INSTRUMENT_POSITIONING;
        session.sessionResults.isAssessed = GameManager.Instance.IsAssessmentMode();
        session.instructions = new string[] { "In this scenario, you are required to place the instruments on the tray in the correct order." };

        m_allSessions.Add(session);

        foreach (var tag in InstrumentLocManager.CurrentInstrumentOrder.Distinct())
        {
            if (tag != Instrument.INSTRUMENT_TAG.NONE)
            {
                foreach(InstrumentPositionTaskSlot slot in InstrumentPositionTaskLocManager.Instance.InstrumentSlots)
                {
                    if(slot.CorrectInstrument == tag)
                    {
                        allTasks.Add(new InstrumentPositionTask(slot, tag));
                        break;
                    }

                }
            }
        }

        while (allTasks.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, allTasks.Count);
            session.AddTask(allTasks[i]);
            allTasks.Remove(allTasks[i]);
        }

        return session;
    }

    public Session GenerateSelectByNameSession()
    {
        Session session = new Session(GenerateID());
        List<InstrumentSelectByNameTask> allTasks = new List<InstrumentSelectByNameTask>();
        session.SetSessionName("Select By Name");
        session.sessionType = Session.SESSION_TYPE.SELECT_BY_NAME;

        session.instructions = new string[] { "In this scenario, you are required to select intruments by their name." };

        session.sessionResults.isAssessed = GameManager.Instance.IsAssessmentMode();

        m_allSessions.Add(session);

        foreach (var tag in InstrumentLocManager.CurrentInstrumentOrder.Distinct())
        {
            if (tag != Instrument.INSTRUMENT_TAG.NONE)
            {
                allTasks.Add(new InstrumentSelectByNameTask(tag));
            }
        }

        while (allTasks.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, allTasks.Count);
            session.AddTask(allTasks[i]);
            allTasks.Remove(allTasks[i]);
        }

        return session;
    }

    public Session GenerateSelectByPurposeSession()
    {
        Session session = new Session(GenerateID());
        List<InstrumentSelectByPurpose> allTasks = new List<InstrumentSelectByPurpose>();
        session.SetSessionName("Select By Purpose");
        session.sessionType = Session.SESSION_TYPE.SELECT_BY_PURPOSE;

        session.instructions = new string[] { "In this scenario, you are required to select intruments by a description of their purpose." };
        session.sessionResults.isAssessed = GameManager.Instance.IsAssessmentMode();

        m_allSessions.Add(session);

        foreach (var tag in InstrumentLocManager.CurrentInstrumentOrder.Distinct())
        {
            if (tag != Instrument.INSTRUMENT_TAG.NONE)
            {
                allTasks.Add(new InstrumentSelectByPurpose(tag));
            }
        }

        while (allTasks.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, allTasks.Count);
            session.AddTask(allTasks[i]);
            allTasks.Remove(allTasks[i]);
        }

        return session;
    } 

    public void StartSessionSequence()
    {
        m_currentSession = m_allSessions[0];
        Player.Instance.FreezePlayer(true);
        m_currentSession.Start();
    }
    public void StartSessionByType(Session.SESSION_TYPE type)
    {
        Session session = null;
        switch(type)
        {
            case Session.SESSION_TYPE.INSTRUMENT_POSITIONING:
                session = GenerateSelectInstrumentPositionSession();
                break;

            case Session.SESSION_TYPE.SELECT_BY_NAME:
                session = GenerateSelectByNameSession();
                break;

            case Session.SESSION_TYPE.SELECT_BY_PURPOSE:
                session = GenerateSelectByPurposeSession();
                break;


        }

        m_currentSession = session;
        Player.Instance.FreezePlayer(true);
        m_currentSession.Start();
    } 


    private long GenerateID()
    {
        System.Random r = new System.Random();
        string id = "" +  DateTime.Today.DayOfYear  + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + m_allSessions.Count + r.Next(10000);

        long idLong = long.Parse(id);
       // Debug.Log("ID: " + idLong);
        Logger.LogToFile("ID: " + idLong);
        return idLong;
    }

    /// <summary>
    /// Called when the student confirms they've finished positioning instruments for an
    /// InstrumentPositionTask
    /// </summary>
    private void OnConfirmInstrumentPositions()
    {
    }

    /// <summary>
    /// Only for SelectByName or SelectByPurpose Task
    /// </summary>
    /// <param name="instrumentTag"></param>
    private void OnInstrumentSelected(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        if (m_currentSession != null)
        {
            // If current task is to position the instruments, select the chosen instrument.
            InstrumentPositionTask instrumentPositionTask = m_currentSession.GetCurrentTask() as InstrumentPositionTask;

            // If current task is to select by name or purpose, evaluate the task outcome.
            if (m_currentSession.GetCurrentTask() is InstrumentSelectByNameTask || 
                m_currentSession.GetCurrentTask() is InstrumentSelectByPurpose)
            {
                Task.STATUS status = m_currentSession.GetCurrentTask().Evaluate(instrumentTag, m_currentSession);

                if (status == Task.STATUS.COMPLETED_SUCCESS)
                {

                    Player.Instance.FreezePlayer(true);

                    if(!GameManager.Instance.IsAssessmentMode())
                    {
                        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Nicely done!" }, () => {
                            Player.Instance.ResetItemAndPlayerToFree();
                            Player.Instance.FreezePlayer(false);
                            Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start

                            // Next task
                            if (!m_currentSession.NextTask())
                                CompleteCurrentSession();
                        });
                    }
                    else
                    {
                        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "The item was selected." }, () => {
                            Player.Instance.ResetItemAndPlayerToFree();
                            Player.Instance.FreezePlayer(false);
                            Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start

                            // Next task
                            if (!m_currentSession.NextTask())
                                CompleteCurrentSession();
                        });
                    }
                    
                }
                else
                {
                    Player.Instance.FreezePlayer(true);
                    if(!GameManager.Instance.IsAssessmentMode())
                    {
                        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Oh no, wrong item!" }, () => {
                            Player.Instance.ResetItemAndPlayerToFree();
                            Player.Instance.FreezePlayer(false);
                            Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start
                            m_currentSession.sessionResults.retries++;
                            // Restart session
                            m_currentSession.NextTask();

                        });
                    }
                    else
                    {
                        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "The item was selected." }, () => {
                            Player.Instance.ResetItemAndPlayerToFree();
                            Player.Instance.FreezePlayer(false);
                            Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start
                            m_currentSession.sessionResults.retries++;
                            // Restart session
                            m_currentSession.NextTask();
                        });
                    }
                   

                }

            }

        }
        
    }

    public void OnInstrumentPlaced(Instrument.INSTRUMENT_TAG instrumentTag, InstrumentPositionTaskSlot slot)
    {
       if (m_currentSession != null)
        {
            // If current task is to position the instruments, select the chosen instrument.
            InstrumentPositionTask instrumentPositionTask = null;

            //Find the task related to slot
            foreach(InstrumentPositionTask t in m_currentSession.tasks)
            {
                if (t.m_instrument == instrumentTag)
                    instrumentPositionTask = t;
            }

            Task.STATUS status = instrumentPositionTask.Evaluate(instrumentTag, slot,m_currentSession);

            if (status == Task.STATUS.COMPLETED_SUCCESS && !GameManager.Instance.IsAssessmentMode())
            {
                GUIManager.Instance.GetMainCanvas().DogPopUp(2, "Correct placement");
            }
            else if(status == Task.STATUS.COMPLETED_FAIL && !GameManager.Instance.IsAssessmentMode())
            {
                GUIManager.Instance.GetMainCanvas().DogPopUp(2, "Wrong placement");
            }

            bool allGood = true;
            //Check if all slots are ok
            foreach (InstrumentPositionTask t in m_currentSession.tasks)
            {
                if (t.GetSlot().CurrentInstrument != t.GetSlot().CorrectInstrument)
                    allGood = false;
            }

            if (allGood)
            {

                CompleteCurrentSession();
            }

        }
    }

    public bool IsSessionPaused()
    {
        return m_isCurrentSessionPaused;
    }

    public void NextSession()
    {
        GUIManager.Instance.GetMainCanvas().HideResultPanel();
        int index = m_allSessions.IndexOf(m_currentSession);

        m_currentSession =  m_allSessions[index + 1];
        m_currentSession.Start();
    }

    public void CompleteCurrentSession()
    {
        //If this is reached there are no tasks left (write to report here?)
       // Player.Instance.SetPickingEnabled(false);
        m_currentSession.End();
        GUIManager.Instance.ConfigureCursor(true, CursorLockMode.None);

        if(m_currentSession.sessionResults.isAssessed)
        {
            int index = m_allSessions.IndexOf(m_currentSession);
            if(index < m_allSessions.Count - 1)
            {
                GUIManager.Instance.GetMainCanvas().EnableNextSessionBtn(true);
            }
            else
            {
                {
                    GUIManager.Instance.GetMainCanvas().EnableNextSessionBtn(false);
                }
            }
            GUIManager.Instance.GetMainCanvas().EnableRetryBtn(false);
        }
        else
        {
            GUIManager.Instance.GetMainCanvas().EnableNextSessionBtn(false);
            GUIManager.Instance.GetMainCanvas().EnableRetryBtn(true);
        }

        ExportResults(m_currentSession);

        GUIManager.Instance.GetMainCanvas().EnableResumeBtn(false);
        GUIManager.Instance.GetMainCanvas().SetResultsPanelTitle("Session complete");

        DisplayResults(m_currentSession);     
        Player.Instance.FreezePlayer(true);

    }

    public void OnQuit()
    {
        if (m_currentSession != null)
            m_currentSession.sessionResults.Log_SimulationClosedPrematurely();

        ExportResults(m_currentSession);
    }

    public string CreateJSONString(Session s)
    {
        JSONObject obj = new JSONObject();

        obj.AddField("UnityID", s.GetID());
        obj.AddField("SessionName", s.GetSessionName());
        obj.AddField("Date", s.sessionResults.date.ToShortDateString());
        obj.AddField("StartTime", s.sessionResults.startTime.ToShortTimeString());


        if (s.sessionResults.completed)
        {
            obj.AddField("EndTime", s.sessionResults.endTime.ToShortTimeString());
        }
        else
        {
            obj.AddField("EndTime", s.sessionResults.endTime.ToShortTimeString() + " (Incomplete)");
        }

        obj.AddField("Retries", s.sessionResults.retries);
        obj.AddField("IsAssessed", s.sessionResults.isAssessed);


        JSONObject logs = new JSONObject();
        int counter = 0;
        foreach(string log in s.sessionResults.logs)
        {
            logs.AddField("Log_" + counter, log);
            counter++;
        }

        obj.AddField("Logs", logs);

        return obj.ToString();
    }

    public void DisplayResults(Session s, bool isPause = false)
    {
        string name, studentNumber;

        // If first name is set, we can safely assume that all other keys are set
        if (PlayerPrefs.HasKey("FirstName"))
        {
            name = PlayerPrefs.GetString("FirstName") + " " + PlayerPrefs.GetString("LastName");
            
        }
        else
        {
            name = "Anonymous";
        }

        if (PlayerPrefs.HasKey("MurdochUserNumber"))
        {
            studentNumber = PlayerPrefs.GetString("MurdochUserNumber");
        }
        else
            studentNumber = GameManager.Instance.MockStudentNumber;


        GUIManager.Instance.GetMainCanvas().DisplayResults(name, studentNumber, s.sessionResults, isPause);
    }

    public WWWForm GetSessionForm(string json)
    {
        WWWForm form = new WWWForm();
        if (PlayerPrefs.HasKey("MurdochUserNumber"))
        {
            form.AddField("MurdochUserNumber", PlayerPrefs.GetString("MurdochUserNumber"));
        }
        else
            form.AddField("MurdochUserNumber", GameManager.Instance.MockStudentNumber);

        form.AddField("SessionString", json);
        return form;
    }
    
    public void SaveToFile()
    {
        DirectoryInfo info =  System.IO.Directory.CreateDirectory("SavedReports");
        string fileName = "";

        string fname = PlayerPrefs.GetString("FirstName", "Anonymous");
        string lname = PlayerPrefs.GetString("LastName", "");
        string mus = PlayerPrefs.GetString("MurdochUserNumber","");

        fileName += "SavedReports\\" +  fname + "_" + lname + "_" + mus + "_";

        string dateString = m_currentSession.sessionResults.startTime.Date.ToShortDateString() + "_" + m_currentSession.sessionResults.startTime.TimeOfDay.Hours + "_" + m_currentSession.sessionResults.startTime.TimeOfDay.Minutes;

         dateString = dateString.Replace('/','_');
         fileName += dateString + ".html";


        //Save to file
         
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(fileName, false))
        {
            file.WriteLine("<h2>Name: " + fname + " " + lname + "</h2>");
            file.WriteLineAsync("<h2>ID: " + mus + "</h2>");
            foreach(string log in m_currentSession.sessionResults.logs)
            {
                if(log.Contains("Failed"))
                    file.WriteLine("<p style='color: red'>" + log + "</p>");
                else if (log.Contains("Correctly"))
                    file.WriteLine("<p style='color: green'>" + log + "</p>");
                else
                    file.WriteLine("<p style='color: black'>" + log + "</p>");


            }

        }
         
    }

                
    public void ReadFinalInstrumentPositioning()
    {
        Debug.Log("Correct instruments");

        foreach (var slot in InstrumentPositionTaskLocManager.Instance.InstrumentSlots)
        {
            Debug.Log(slot.CorrectInstrument.ToString());
        }
    }

    public void ExportResults(Session s)
    {
        if(s != null && s.HasStarted())
        {
            Logger.LogToFile("Exporting session, id " + m_currentSession.GetID());


            if (m_currentSession.sessionType == Session.SESSION_TYPE.INSTRUMENT_POSITIONING)
                ReadFinalInstrumentPositioning();
            //Send to server

            string json = CreateJSONString(s);
            WWWForm form = GetSessionForm(json);
            Logger.LogToFile("Just about to send request, " + m_currentSession.GetID());
           NetworkManager.Instance.SendRequest(form, "recordSession.php", 
                (string reply) => {
                    //Debug.Log("Server said: " + reply);
                    Logger.LogToFile("Session recorded, id " + m_currentSession.GetID());
                    Logger.LogToFile("Reply" + reply);
                }, 
                () => {
                    // Debug.Log("Failed to upload");
                    Logger.LogToFile("Failed to upload results, id " + m_currentSession.GetID());
                },
                () => {
                    //If all attempts to connect fail
                }
                );
        }
    }

    /// <summary>
    /// Evaluate the task, used for InstrumentPositionTask
    /// </summary>
    public void EndMenu_ClickYes()
    {
        Player.Instance.HideEndMenu();

        // Passes None instrument because we don't use this parameter for an InstrumentPositionTask.
        m_currentSession.GetCurrentTask().Evaluate(Instrument.INSTRUMENT_TAG.NONE, m_currentSession);

        //If this is reached there are no tasks left (write to report here?)
        CompleteCurrentSession();
        /*Player.Instance.SetPickingEnabled(false);
        m_currentSession.End();
        Player.Instance.FreezePlayer(true);
        GUIManager.Instance.ConfigureCursor(true, CursorLockMode.None);
        DisplayResults(m_currentSession);
        ExportResults(m_currentSession);*/
    }

    // Called by GUI
    public void ResumeSession()
    {
        GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);

        GUIManager.Instance.GetMainCanvas().EnableResumeBtn(false);
        GUIManager.Instance.GetMainCanvas().HideResultPanel();
        if (Player.Instance.GetPlayerMode() == Player.PlayerMode.PICKING && m_currentSession.HasStarted())
            Player.Instance.FreezePlayer(false);
        else
            Player.Instance.enabled = true;
        m_isCurrentSessionPaused = false;
    }
    public void Update()
    {
        if (Input.GetAxis("Cancel") == 1 && !m_currentSession.sessionResults.completed)
        {
            //Disable retry option when assessment mode...? Ye why not
            if(GameManager.Instance.IsAssessmentMode())
            {
                GUIManager.Instance.GetMainCanvas().EnableRetryBtn(false);
            }
            else
            {
                GUIManager.Instance.GetMainCanvas().EnableRetryBtn(true);


            }

            if (!m_previousCancelButtonStatus)
            {
                m_previousCancelButtonStatus = true;

                if (!m_isCurrentSessionPaused)
                {
                    //Hide next session button, just in case
                    GUIManager.Instance.ConfigureCursor(true, CursorLockMode.None);

                    GUIManager.Instance.GetMainCanvas().EnableNextSessionBtn(false);
                    GUIManager.Instance.GetMainCanvas().EnableResumeBtn(true);
                    GUIManager.Instance.GetMainCanvas().SetResultsPanelTitle("Session paused");
                    DisplayResults(m_currentSession,true);
                    Player.Instance.FreezePlayer(true);
                    m_isCurrentSessionPaused = true;
                }
                else
                {
                    GUIManager.Instance.GetMainCanvas().HideResultPanel();
                    ResumeSession();
                    m_isCurrentSessionPaused = false;
                }
            }
        }
        else
            m_previousCancelButtonStatus = false;



    }

    /// <summary>
    /// Used for InstrumentPositionTask
    /// </summary>
    public void EndMenu_ClickNo()
    {
        Player.Instance.HideEndMenu();
    }

}
