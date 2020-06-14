using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SessionResults
{
    public DateTime startTime;
    public DateTime endTime;
    public DateTime date;
    public int errors;
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
    private bool m_isStarted = false;    
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

    private void StartSession()
    {
        Player.Instance.ResetPosition();
        //Player.Instance.FreezePlayer(true);
        GUIManager.Instance.GetMainCanvas().SetAssessmentModePanel(GameManager.Instance.IsAssessmentMode());

        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(instructions, () => {

            m_currentTask = tasks[0];
            sessionResults.startTime = DateTime.Now;
            sessionResults.date = DateTime.Today;
            sessionResults.Log_SessionStart();
            m_isStarted = true;

            StartCurrentTask();
        });
    }

    public void Start()
    {
        if(!m_isStarted)
        {
            Logger.LogToFile("Started session " + GetID());
            GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
            GUIManager.Instance.GetMainCanvas().SetPauseIconOn(true);
            if (GameManager.Instance.WillShowTutorials)
            {

                string[] newInst = new string[] {"Hi, I'm you assistant! Press the fire button to dismiss my messages.",
                    "Move the player using the keys WASD on your keyboard. Use the mouse to look around.",
                    "Use the fire button to interact with instruments, more control hints will appear as you go.",
                    "The session will start now!" };

                newInst = newInst.Concat(instructions).ToArray();
                instructions = newInst;

            }

            StartSession();          
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
        if(m_currentTask.taskStatus == Task.STATUS.COMPLETED_SUCCESS || GameManager.Instance.IsAssessmentMode())
        {
            int indexOfCurrent = tasks.IndexOf(m_currentTask);
            tasks.RemoveAt(indexOfCurrent);
        }

        if (tasks.Count == 0)
            return false;

        // Assign next task
        if (m_currentTask.taskStatus == Task.STATUS.COMPLETED_FAIL && !GameManager.Instance.IsAssessmentMode() && m_currentTask.AttemptNumber < 2)
        {
            // From Isobel's feedback: Students should be given two chances to select the correct instrument
            m_currentTask.Restart();
        }
        else
        {
            System.Random r = new System.Random();
            m_currentTask = tasks[r.Next(tasks.Count)];
            m_currentTask.Reset(); // Must reset the number of attempts so they get 2 chances again since the task might have been failed before.
        }
        StartCurrentTask();
        return true;
    }

    /// <summary>
    /// Start current task.
    /// </summary>
    private void StartCurrentTask()
    {

        m_currentTask.taskStatus = Task.STATUS.INSTRUCTING;

        bool isInstrumentPositionTask = m_currentTask is InstrumentPositionTask;

        foreach (InstrumentPositionTaskSlot slot in InstrumentPositionTaskLocManager.Instance.InstrumentSlots)
        {
            // hide all all the slots if not used for this task type, unhide if used
            slot.GetComponentInChildren<MeshRenderer>().enabled = isInstrumentPositionTask;
        }

        Player.Instance.FreezePlayer(true);
        GUIManager.Instance.GetMainCanvas().SetPauseIconOn(true);

        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(m_currentTask.instructions.ToArray(), () => {
            Player.Instance.FreezePlayer(false);

            Player.Instance.SetPickingEnabled(true);
            m_currentTask.taskStatus = Task.STATUS.STARTED;
            sessionResults.endTime = DateTime.Now;
            sessionResults.Log_Instruction(m_currentTask.instructions.ToArray());

            //Keep first instruction on screen
            GUIManager.Instance.GetMainCanvas().DogSpeak(m_currentTask.instructions[0]);
            GUIManager.Instance.GetMainCanvas().SetPauseIconOn(false);


        });
    }

    public enum SESSION_TYPE
    {
        SELECT_BY_NAME,
        SELECT_BY_PURPOSE,
        INSTRUMENT_POSITIONING
    }

}
