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
            GUIManager.Instance.GetMainCanvas().DogPopUp(3.0f, "Well done!");
            //Next task
        }
        else
        {
            GameManager.Instance.SetGameMode(GameManager.GAME_MODE.INSTRUCTION);
            GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Oh no, wrong item!" } , ()=> {
                GameManager.Instance.m_player.ResetPlayerMode();
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
    }
  
}
