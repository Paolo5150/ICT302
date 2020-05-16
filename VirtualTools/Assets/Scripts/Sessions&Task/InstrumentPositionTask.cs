using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentPositionTask : Task
{

    private InstrumentPositionTaskSlot m_slot;
    public Instrument.INSTRUMENT_TAG m_instrument;

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

    public InstrumentPositionTask(InstrumentPositionTaskSlot slot, Instrument.INSTRUMENT_TAG instrument)
    {
        instructions.Add("New task: position the instruments for a <b><u>Spay dog</u></b>");

        m_slot = slot;
        m_instrument = instrument;
    }


    public override STATUS Evaluate(params object[] list)
    {
        Instrument.INSTRUMENT_TAG instrumentSelected = (Instrument.INSTRUMENT_TAG)list[0];
        InstrumentPositionTaskSlot slotPlacedTo = (InstrumentPositionTaskSlot)list[1];
        Session session = (Session)list[2];

        if(slotPlacedTo != m_slot)
        {
            session.sessionResults.Log_FailedToPositionInstrument(slotPlacedTo.CorrectInstrument,instrumentSelected );
            return STATUS.COMPLETED_FAIL;
        }
        else
        {
            session.sessionResults.Log_CorrectlyPositionedInstrument(instrumentSelected);
            return STATUS.COMPLETED_SUCCESS;

        }
    }

}