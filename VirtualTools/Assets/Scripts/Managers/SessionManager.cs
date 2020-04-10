using System.Collections;
using System.Collections.Generic;
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
 
}
