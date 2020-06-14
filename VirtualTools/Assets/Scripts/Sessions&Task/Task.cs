using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task
{
    public enum STATUS
    {
        PENDING,
        INSTRUCTING,
        STARTED,
        COMPLETED_FAIL,
        COMPLETED_SUCCESS
    }

    public STATUS taskStatus;

    public List<string> instructions;

    public Task()
    {
        instructions = new List<string>();
        Reset();
    }

    /// <summary>
    /// Restart the task from the beginning
    /// </summary>
    public void Restart()
    {
        taskStatus = STATUS.PENDING;
        AttemptNumber += 1;
    }

    /// <summary>
    /// Reset the task and the number of attempts.
    /// </summary>
    public void Reset()
    {
        taskStatus = STATUS.PENDING;
        AttemptNumber = 1;
    }

    public int AttemptNumber { get; set; } = 1;

    /// <summary>
    /// Evaluate the task
    /// </summary>
    /// <param name="args">Arguments are of generic type as different tasks are evaluated differently (and need different parameters)</param>
    /// <returns>The competion status of the task/returns>
    abstract public STATUS Evaluate(params object[] args);


}
