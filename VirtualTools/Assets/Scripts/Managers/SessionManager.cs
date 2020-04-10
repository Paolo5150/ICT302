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

    public void CreateSession(bool setAsCurrent = true, bool startImmediately = false)
    {
        //Will create a session manager
        Session session = new Session(GenerateID());
        //Randomize? From external file?
        InstrumentSelectTask task1 = new InstrumentSelectTask(Instrument.INSTRUMENT_TAG.SUTURE_SCISSOR);
        InstrumentSelectTask task2 = new InstrumentSelectTask(Instrument.INSTRUMENT_TAG.ADDSON_BROWN_FORCEPS);

        session.AddTask(task1);
        session.AddTask(task2);
        m_sessionsRun.Add(session);
        if (setAsCurrent)
            m_currentSession = session;
        if(startImmediately)
        {
            m_currentSession = session;
            m_currentSession.Start();
        }

    }

    private int GenerateID()
    {
        //TODO: Change this
        return UnityEngine.Random.Range(0,5000);       
      
    }

    private void OnInstrumentSelected(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        if(m_currentSession != null)
        {
            Task.STATUS status = m_currentSession.GetCurrentTask().Evaluate(instrumentTag);
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
       // ExportResults(m_currentSession);
        //Thread.Sleep(1000);

    }

    public string CreateJSONString(Session s, string fileName)
    {
        JSONObject obj = new JSONObject();

        obj.AddField("Date", s.sessionResults.date.ToShortDateString());
        obj.AddField("StartTime", s.sessionResults.startTime.ToShortTimeString());


        if (s.sessionResults.completed)
        {
            obj.AddField("EndTime", s.sessionResults.endTime.ToShortTimeString());
            obj.AddField("Completed", "true");
        }
        else
        {
            obj.AddField("EndTime", "0");
            obj.AddField("Completed", "false");
        }

        obj.AddField("Retries", s.sessionResults.retries);
        obj.AddField("FileName", fileName);

        return obj.ToString();
    }

    
    public void ExportResults(Session s)
    {
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

        string dateString = s.sessionResults.startTime.Date.ToShortDateString();
        dateString = dateString.Replace('/','_');
        fileName += dateString + ".dat";

        string json = CreateJSONString(s,fileName);

        //Save to file, always
      /*  BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(fileName);
        bf.Serialize(file, json);
        file.Close();*/

        //Send to server
        WWWForm form = new WWWForm();
        form.AddField("MurdochUserNumber", GameManager.Instance.MockStudentNumber);
        form.AddField("SessionString", json);

       NetworkManager.Instance.SendRequest(form, "recordSession.php", 
            (string reply) => {
                Debug.Log("Server said: " + reply);

            }, 
            () => {
                Debug.Log("Failed to upload");
   
            });
        Debug.Log("Exported!");
    }
 
}
