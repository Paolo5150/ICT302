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

    public void Update()
    {
        if(m_isStarted)
        {
            if(m_currentTask.taskStatus == Task.STATUS.PENDING)
            {
                m_currentTask.taskStatus = Task.STATUS.INSTRUCTING;
                GameManager.Instance.SetGameMode(GameManager.GAME_MODE.INSTRUCTION);
                GUIManager.Instance.GetMainCanvas().DogInstructionSequence(m_currentTask.instructions.ToArray(), ()=> {
                    GameManager.Instance.SetGameMode(GameManager.GAME_MODE.PLAYING);
                    m_currentTask.taskStatus = Task.STATUS.STARTED;
                });
            }
        }

        Debug.Log("Task status " + m_currentTask.taskStatus.ToString());
    }
  
}
