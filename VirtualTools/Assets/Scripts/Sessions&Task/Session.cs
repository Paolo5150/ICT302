using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session
{
    public List<Task> tasks;

    private Task m_currentTask;
    private bool m_isStarted;
    private DateTime m_startTime;
    private DateTime m_endTime;
    private int m_attempts;
    private int m_id;

    public Session(int id)
    {
        tasks = new List<Task>();
        m_isStarted = false;
        m_id = id;
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
            StartCurrentTask();
        }
    }

    public void End()
    {
        if (m_isStarted)
        {
            m_isStarted = false;            
            m_endTime = DateTime.Now;

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
        m_attempts++;
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
            m_startTime = DateTime.Now;

        });
    }

    public void Update()
    {
      
    }
  
}
