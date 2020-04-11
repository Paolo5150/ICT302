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

    public bool IsStarted()
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
            StartCurrentTask();
        }
    }

    public void End()
    {
        if (m_isStarted)
        {          
            sessionResults.endTime = DateTime.Now;
            sessionResults.completed = true;
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

        });
    }

    public void Update()
    {
      
    }
  
}
