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

    abstract public STATUS Evaluate(params object[] args);


}
