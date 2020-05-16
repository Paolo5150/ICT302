using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionResults
{
    public DateTime startTime;
    public DateTime endTime;
    public DateTime date;
    public int retries;
    public bool completed = false;
    public List<string> logs;
    public bool isAssessed = false;

    public SessionResults()
    {
        logs = new List<string>();
    }

    public void Log_SessionStart()
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Session starts");
    }

    public void Log_SessionEnd()
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Session ends");
    }

    public void Log_Instruction(string[] instructions)
    {
        for(int i=0; i< instructions.Length; i++)
            logs.Add(DateTime.Now.ToShortTimeString() + " - " + instructions[i]);


    }
    public void Log_FailedToSelectByName(Instrument.INSTRUMENT_TAG toSelectinstrumentTag, Instrument.INSTRUMENT_TAG selectedInstrumentTag)
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Failed to select by name " + Instrument.GetName(toSelectinstrumentTag) + ". Selected: " + Instrument.GetName(selectedInstrumentTag));
    }

    public void Log_FailedToPositionInstrument(Instrument.INSTRUMENT_TAG correctInstrumentTag, Instrument.INSTRUMENT_TAG actualInstrumentTag)
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Failed to place instrument, put " + Instrument.GetName(actualInstrumentTag) + " where the " + Instrument.GetName(correctInstrumentTag) + " should have gone.");
    }

    public void Log_FailedToSelectByPurpose(Instrument.INSTRUMENT_TAG toSelectinstrumentTag, Instrument.INSTRUMENT_TAG selectedInstrumentTag)
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Failed to select by purpose " + Instrument.GetName(toSelectinstrumentTag) + ". Selected: " + Instrument.GetName(selectedInstrumentTag));
    }

    public void Log_CorrectlyPositionedInstrument(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Correctly placed " + Instrument.GetName(instrumentTag));
    }


    public void Log_CorrectlySelectedInstrumentByPurpose(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Correctly selected by purpose " + Instrument.GetName(instrumentTag));
    }

    public void Log_CorrectlySelectedInstrumentByName(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Correctly selected by name " + Instrument.GetName(instrumentTag));
    }

    public void Log_SimulationClosedPrematurely()
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Simulation closed prematurely");
    }
}
    public class Session
    {
   
    public List<Task> tasks;
    public SESSION_TYPE sessionType { get; set; }
    private Task m_currentTask;
    private bool m_isStarted;    
    private long m_id;
    private String m_sessionName;
    public SessionResults sessionResults;
    public string[] instructions { get; set; }

    public Session(long id)
    {
        sessionResults = new SessionResults();
        tasks = new List<Task>();
        m_isStarted = false;
        m_id = id;
    }


    public String GetSessionName()
    {
        return m_sessionName;
    }

    public void SetSessionName(string name)
    {
        m_sessionName = name;
    }
    public bool HasStarted()
    {
        return m_isStarted;
    }
    public long GetID()
    {
        return m_id;
    }

   public Task GetCurrentTask()
    {
        return m_currentTask;
    }

    public void AddTask(Task task)
    {
        tasks.Add(task);
    }

    public void Start()
    {
        if(!m_isStarted)
        {
            m_isStarted = true;
            Player.Instance.ResetPosition();
            GUIManager.Instance.GetMainCanvas().DogInstructionSequence(instructions, () => {

                if (GameManager.Instance.IsAssessmentMode())
                {
                    GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "This is an assessment." }, () => {
                        GUIManager.Instance.GetMainCanvas().SetAssessmentModePanel(true);

                        m_currentTask = tasks[0];
                        sessionResults.startTime = DateTime.Now;
                        sessionResults.date = DateTime.Today;
                        sessionResults.Log_SessionStart();
                        StartCurrentTask();

                    });
                }            
                else
                {
                    m_currentTask = tasks[0];
                    sessionResults.startTime = DateTime.Now;
                    sessionResults.date = DateTime.Today;
                    sessionResults.Log_SessionStart();
                    StartCurrentTask();
                }
            });



            
        }
    }

    public void End()
    {
        if (m_isStarted)
        {          
            sessionResults.endTime = DateTime.Now;
            sessionResults.completed = true;
            sessionResults.Log_SessionEnd();
        }
    }

    
    public void Restart()
    {
        //Set all tasks to pending
        foreach (Task t in tasks)
            t.taskStatus = Task.STATUS.PENDING;

        if (!m_isStarted)
        {
            m_isStarted = true;
        }

        m_currentTask = tasks[0];
        
        StartCurrentTask();

    }

    public bool NextTask()
    {
        if(m_currentTask.taskStatus == Task.STATUS.COMPLETED_SUCCESS)
        {
            int indexOfCurrent = tasks.IndexOf(m_currentTask);
            tasks.RemoveAt(indexOfCurrent);
        }

        if (tasks.Count == 0)
            return false;

        System.Random r = new System.Random();
        m_currentTask = tasks[r.Next(tasks.Count)];
        StartCurrentTask();
        return true;
    }

    private void StartCurrentTask()
    {
        m_currentTask.taskStatus = Task.STATUS.INSTRUCTING;

        Player.Instance.FreezePlayer(true);
        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(m_currentTask.instructions.ToArray(), () => {
            Player.Instance.FreezePlayer(false);
            Player.Instance.SetPickingEnabled(true);
            m_currentTask.taskStatus = Task.STATUS.STARTED;
            sessionResults.endTime = DateTime.Now;
            sessionResults.Log_Instruction(m_currentTask.instructions.ToArray());

            //Keep first instruction on screen
            GUIManager.Instance.GetMainCanvas().DogSpeak(m_currentTask.instructions[0]);

        });
    }

    public void Update()
    {
      
    }

    public enum SESSION_TYPE
    {
        SELECT_BY_NAME,
        SELECT_BY_PURPOSE,
        INSTRUMENT_POSITIONING
    }

}
