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
        taskStatus = STATUS.PENDING;
        instructions = new List<string>();
    }

    /// <summary>
    /// Evaluate the task
    /// </summary>
    /// <param name="args">Arguments are of generic type as different tasks are evaluated differently (and need different parameters)</param>
    /// <returns>The competion status of the task/returns>
    abstract public STATUS Evaluate(params object[] args);


}
