using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentPositionTask : Task
{

    private InstrumentPositionTaskSlot m_slot;

    public InstrumentPositionTaskSlot GetSlot()
    {
        return m_slot;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="procedureName">name of the procedure this task is for</param>
    /// <param name="tag"></param>
    public InstrumentPositionTask(string procedureName)
    {
        instructions.Add("New task: position the instruments for a <b><u>" + procedureName + "</u></b>");
    }

    public InstrumentPositionTask(InstrumentPositionTaskSlot slot)
    {
        instructions.Add("New task: position the instruments for a <b><u>Spay dog</u></b>");

        m_slot = slot;
    }

   /* public override STATUS Evaluate(Instrument.INSTRUMENT_TAG instrumentTag, Session session)
    {
        taskStatus = STATUS.COMPLETED_SUCCESS;

        if (instrumentTag != m_slot.CorrectInstrument)
        {
            session.sessionResults.Log_FailedToPositionInstrument(m_slot.CorrectInstrument, instrumentTag);
            taskStatus = STATUS.COMPLETED_FAIL;
        }
        else
        {
            session.sessionResults.Log_CorrectlyPositionedInstrument(instrumentTag);
        }


        return taskStatus;
    }*/

    public override STATUS Evaluate(params object[] list)
    {
        return STATUS.COMPLETED_SUCCESS;
    }

}