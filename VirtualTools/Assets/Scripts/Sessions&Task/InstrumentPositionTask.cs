using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentPositionTask : Task
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="procedureName">name of the procedure this task is for</param>
    /// <param name="tag"></param>
    public InstrumentPositionTask(string procedureName)
    {
        instructions.Add("New task: position the instruments for a <b><u>" + procedureName + "</u></b>");
    }

    public override STATUS Evaluate(Instrument.INSTRUMENT_TAG instrumentTag, Session session)
    {
        taskStatus = STATUS.COMPLETED_SUCCESS;

        foreach (var instrumentSlot in InstrumentPositionTaskLocManager.Instance.InstrumentSlots)
        {
            if (instrumentSlot.CurrentInstrument == Instrument.INSTRUMENT_TAG.NONE)
            {
                session.sessionResults.Log_FailedToPositionInstrument(instrumentSlot.CorrectInstrument, Instrument.INSTRUMENT_TAG.NONE);
                taskStatus = STATUS.COMPLETED_FAIL;
            }
            else
            {
                if (instrumentSlot.CurrentInstrument != instrumentSlot.CorrectInstrument)
                {
                    session.sessionResults.Log_FailedToPositionInstrument(instrumentSlot.CorrectInstrument, instrumentSlot.CurrentInstrument);
                    taskStatus = STATUS.COMPLETED_FAIL;
                }
                else
                {
                    session.sessionResults.Log_CorrectlyPositionedInstrument(instrumentSlot.CorrectInstrument);
                }
            }
        }


        return taskStatus;
    }

}