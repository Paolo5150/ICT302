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
    /// Generate task
    /// </summary>
    /// <param name="slot">The slot the instrument argument is supposed to go in</param>
    /// <param name="instrument">The instrument for this task</param>
    public InstrumentPositionTask(InstrumentPositionTaskSlot slot, Instrument.INSTRUMENT_TAG instrument)
    {
        // Hacky: all tasks of this type have the same instructioni, which is currently hardcoded
        // (this feature was requested later in the project and the 'Task' class wasn't very suitable for this particular type)
        instructions.Add("New task: position the instruments for a <b><u>Spay dog</u></b>");

        m_slot = slot;
        m_instrument = instrument;
    }

    /// <summary>
    /// Check if the instrument was placed in the right slot
    /// </summary>
    /// <param name="list">List of generic arguments. Index 0 must be the instrument selected tag, index 1 must be the slot it was placed in</param>
    /// <returns>Whether the task was completed successfully</returns>
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