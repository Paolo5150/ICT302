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

    public void Log_FailedToSelectByPurpose(Instrument.INSTRUMENT_TAG toSelectinstrumentTag, Instrument.INSTRUMENT_TAG selectedInstrumentTag)
    {
        logs.Add(DateTime.Now.ToShortTimeString() + " - Failed to select by purpose " + Instrument.GetName(toSelectinstrumentTag) + ". Selected: " + Instrument.GetName(selectedInstrumentTag));
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

    private Task m_currentTask;
    private bool m_isStarted;    
    private long m_id;

    public SessionResults sessionResults;

    public Session(long id)
    {
        sessionResults = new SessionResults();
        tasks = new List<Task>();
        m_isStarted = false;
        m_id = id;
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
            m_currentTask = tasks[0];
            sessionResults.startTime = DateTime.Now;
            sessionResults.date = DateTime.Today;
            sessionResults.Log_SessionStart();
            StartCurrentTask();
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
        sessionResults.retries++;
        StartCurrentTask();

    }

    public bool NextTask()
    {
        int indexOfCurrent = tasks.IndexOf(m_currentTask);
        if (indexOfCurrent == tasks.Count - 1)
            return false;

        m_currentTask = tasks[indexOfCurrent + 1];
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
  
}
