using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Threading;
using System;

public class SessionManager
{
    private static SessionManager m_instance;
    private List<Session> m_sessionsRun;
    private Session m_currentSession;

    public static SessionManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SessionManager();
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
        m_sessionsRun = new List<Session>();
        Player.instrumentSelectedEvent += OnInstrumentSelected;

    }

    public Session GetCurrentSession()
    {
        return m_currentSession;
    }

    private Session SelectByNameSession()
    {
        //Will create a session manager
        Session session = new Session(GenerateID());
        //Randomize? From external file?
        List<InstrumentSelectByNameTask> allTasks = new List<InstrumentSelectByNameTask>();

        allTasks.Add(new InstrumentSelectByNameTask(Instrument.INSTRUMENT_TAG.SUTURE_SCISSOR));
        allTasks.Add(new InstrumentSelectByNameTask(Instrument.INSTRUMENT_TAG.ADDSON_BROWN_FORCEPS));
        allTasks.Add(new InstrumentSelectByNameTask(Instrument.INSTRUMENT_TAG.MAYO_SCISSOR));
        allTasks.Add(new InstrumentSelectByNameTask(Instrument.INSTRUMENT_TAG.METZEMBAUM_SCISSOR));
        allTasks.Add(new InstrumentSelectByNameTask(Instrument.INSTRUMENT_TAG.ROCHESTER_CARMALT_FORCEPS));

        while(allTasks.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, allTasks.Count);
            session.AddTask(allTasks[i]);
            allTasks.Remove(allTasks[i]);
        }

        return session;
    }

    private Session SelectByPurposeSession()
    {
        //Will create a session manager
        Session session = new Session(GenerateID());
        //Randomize? From external file?
        List<InstrumentSelectByPurpose> allTasks = new List<InstrumentSelectByPurpose>();

        allTasks.Add(new InstrumentSelectByPurpose(Instrument.INSTRUMENT_TAG.SUTURE_SCISSOR));
        allTasks.Add(new InstrumentSelectByPurpose(Instrument.INSTRUMENT_TAG.ADDSON_BROWN_FORCEPS));
        allTasks.Add(new InstrumentSelectByPurpose(Instrument.INSTRUMENT_TAG.MAYO_SCISSOR));
        allTasks.Add(new InstrumentSelectByPurpose(Instrument.INSTRUMENT_TAG.METZEMBAUM_SCISSOR));
        allTasks.Add(new InstrumentSelectByPurpose(Instrument.INSTRUMENT_TAG.ROCHESTER_CARMALT_FORCEPS));

        while (allTasks.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, allTasks.Count);
            session.AddTask(allTasks[i]);
            allTasks.Remove(allTasks[i]);
        }

        return session;
    }

    public void CreateSelectByNameSession()
    {
        Session session = SelectByNameSession();
        m_sessionsRun.Add(session);

        m_currentSession = session;
        m_currentSession.Start();
    }

    public void CreateSelectByPurposeSession()
    {
        Session session = SelectByPurposeSession();
        m_sessionsRun.Add(session);

        m_currentSession = session;
        m_currentSession.Start();
    }
    
    private long GenerateID()
    {
        string id = "" +  DateTime.Today.DayOfYear  + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

        long idLong = long.Parse(id);
        Debug.Log("ID: " + idLong);
        Logger.LogToFile("ID: " + idLong);
        return idLong;
    }

    private void OnInstrumentSelected(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        if(m_currentSession != null)
        {
            Task.STATUS status = m_currentSession.GetCurrentTask().Evaluate(instrumentTag, m_currentSession);
            if (status == Task.STATUS.COMPLETED_SUCCESS)
            {
                Player.Instance.FreezePlayer(true);
                

                GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Nicely done!" }, () => {
                    Player.Instance.ResetItemAndPlayerToFree();
                    Player.Instance.FreezePlayer(false);
                    Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start

                    // Next task
                    if (!m_currentSession.NextTask())
                    {
                        //If this is reached there are no tasks left (write to report here?)
                        Player.Instance.SetPickingEnabled(false);
                        GUIManager.Instance.GetMainCanvas().DogPopUp(5.0f, "SESSION COMPLETE!");
                        m_currentSession.End();
                        Player.Instance.FreezePlayer(true);
                        GUIManager.Instance.ConfigureCursor(true, CursorLockMode.None);
                        DisplayResults(m_currentSession);
                        ExportResults(m_currentSession);
                         
                    }
                });
            }
            else
            {
                Player.Instance.FreezePlayer(true);
                GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Oh no, wrong item!" }, () => {
                    Player.Instance.ResetItemAndPlayerToFree();
                    Player.Instance.FreezePlayer(false);
                    Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start
                    
                    // Restart session
                    m_currentSession.Restart();

                });

            }
        }
        
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

    public void DisplayResults(Session s)
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

        bool completed = s.sessionResults.completed;
        string date = s.sessionResults.date.ToShortDateString();
        string startDateString = s.sessionResults.startTime.ToShortTimeString();
        string endDateString = s.sessionResults.endTime.ToShortTimeString();
        int retries = s.sessionResults.retries;

        GUIManager.Instance.GetMainCanvas().DisplayResults(completed, name, studentNumber, date, startDateString, endDateString, retries);
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
    
    public void ExportResults(Session s)
    {
        if(s != null && s.HasStarted())
        {
            Logger.LogToFile("Exporting session, id " + m_currentSession.GetID());
            string fileName = "";

            // If first name is set, we can safely assume that all other keys are set
            if (PlayerPrefs.HasKey("FirstName"))
            {
                fileName += PlayerPrefs.GetString("FirstName") + "_" + PlayerPrefs.GetString("LastName") + "_" + PlayerPrefs.GetString("MurdochUserNumber") + "_";
            }
            else
            {
                fileName += "Anonymous_";
            }

           /* string dateString = s.sessionResults.startTime.Date.ToShortDateString();
            dateString = dateString.Replace('/','_');
            fileName += dateString + ".dat";*/


            //Save to file
            /* BinaryFormatter bf = new BinaryFormatter();
             FileStream file = File.Create(fileName);
             bf.Serialize(file, json);
             file.Close();
             */
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

}
