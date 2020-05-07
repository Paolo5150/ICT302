using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentPositionTask : Task
{
    /// <summary>
    /// Gameobject representing position where instrument should be placed for this task.
    /// </summary>
    GameObject m_correctInstrumentPositionSlot;

    /// <summary>
    /// The correct instrument to go in that position
    /// </summary>
    Instrument.INSTRUMENT_TAG m_correctInstrument;

    public InstrumentPositionTask(string procedureName, GameObject correctInstrumentPositionSlot)
    {
        m_correctInstrumentPositionSlot = correctInstrumentPositionSlot;
        instructions.Add("New task: position the instruments for a <b><u>" + procedureName + "</u></b>");
    }

    public override STATUS Evaluate(Instrument.INSTRUMENT_TAG instrumentTag, Session session)
    {
        var instrumentInSlot = m_correctInstrumentPositionSlot.GetComponentInChildren<Instrument>();
        if(instrumentInSlot == null)
        {
            session.sessionResults.Log_FailedToPositionInstrument(m_correctInstrument, Instrument.INSTRUMENT_TAG.NONE);
            taskStatus = STATUS.COMPLETED_FAIL;
        }
        else
        {
            if (instrumentInSlot.instrumentTag != m_correctInstrument)
            {
                session.sessionResults.Log_FailedToPositionInstrument(m_correctInstrument, instrumentInSlot.instrumentTag);
                taskStatus = STATUS.COMPLETED_FAIL;
            }
            else
            {
                taskStatus = STATUS.COMPLETED_SUCCESS;
                session.sessionResults.Log_CorrectlyPositionedInstrument(m_correctInstrument);
            }
        }
        return taskStatus;
    }
}