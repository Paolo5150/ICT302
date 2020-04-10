using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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
        return Random.Range(0,5000);       
      
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
        ExportResults(m_currentSession);
    }
    
    public void ExportResults(Session s)
    {
        string fileName = s.GetID() + "_" + ".txt";

        JSONObject obj = new JSONObject();
        obj.AddField("StartTime", s.sessioResults.startTime.ToShortTimeString());
        if(!s.sessioResults.endTime.ToShortTimeString().Equals(s.sessioResults.startTime.ToShortTimeString()))
            obj.AddField("EndTime", s.sessioResults.endTime.ToShortTimeString());
        else
            obj.AddField("Completed", "false");

        obj.AddField("Attempts", s.sessioResults.retries);

        // Create the Binary Formatter.
        BinaryFormatter bf = new BinaryFormatter();
        // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
        FileStream file = File.Create(Application.persistentDataPath + fileName);
        // Create a new Player_Data.
        if(file != null)
        {
             bf.Serialize(file,obj.ToString());
        }
        else
        {
            Debug.Log("Didn't serialize");
        }
        // Serialize the file so the contents cannot be manipulated.
       
        // Close the file to prevent any corruptions
        file.Close();

    }
 
}
