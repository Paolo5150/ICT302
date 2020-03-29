using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session
{
    public List<Task> tasks;

    private Task m_currentTask;
    private bool m_isStarted;
    public Session()
    {
        tasks = new List<Task>();
        m_isStarted = false;
        Player.instrumentSelectedEvent += OnInstrumentSelected;
    }

    private void OnInstrumentSelected(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        Task.STATUS status = m_currentTask.Evaluate(instrumentTag);
        if(status == Task.STATUS.COMPLETED_SUCCESS)
        {
            Player.Instance.FreezePlayer(true);

            GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Nicely done!" }, () => {
                Player.Instance.ResetItemAndPlayerToFree();
                Player.Instance.FreezePlayer(false);
                Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start
                
                // Next task
                if (!NextTask())
                {
                    //If this is reached there are no tasks left (write to report here?)
                    Player.Instance.SetPickingEnabled(false);
                    GUIManager.Instance.GetMainCanvas().DogPopUp(5.0f, "SESSION COMPLETE!");

                }

            });
        }
        else
        {
            Player.Instance.FreezePlayer(true);
            GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Oh no, wrong item!" } , ()=> {
                Player.Instance.ResetItemAndPlayerToFree();
                Player.Instance.FreezePlayer(false);
                Player.Instance.SetPickingEnabled(false); // Will be set to true when the task start

                // Restart session
                Restart();

            });

        }
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

    }

    private bool NextTask()
    {
        int indexOfCurrent = tasks.IndexOf(m_currentTask);
        if (indexOfCurrent == tasks.Count - 1)
            return false;

        m_currentTask = tasks[indexOfCurrent + 1];
        return true;
    }

    public void Update()
    {
        if(m_isStarted)
        {
            if(m_currentTask.taskStatus == Task.STATUS.PENDING)
            {
                m_currentTask.taskStatus = Task.STATUS.INSTRUCTING;
                Player.Instance.FreezePlayer(true);
                GUIManager.Instance.GetMainCanvas().DogInstructionSequence(m_currentTask.instructions.ToArray(), ()=> {
                    Player.Instance.FreezePlayer(false);
                    Player.Instance.SetPickingEnabled(true);
                    m_currentTask.taskStatus = Task.STATUS.STARTED;
                });
            }
        }
    }
  
}
