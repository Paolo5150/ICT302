using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelectTask : Task
{
    private Instrument.INSTRUMENT_TAG m_instrumentToSelect;

    public InstrumentSelectTask(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        m_instrumentToSelect = instrumentTag;
        instructions.Add("New task: pick up a <b>" + Instrument.GetName(m_instrumentToSelect) + "</b>");
    }

    public override STATUS Evaluate(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        if (m_instrumentToSelect == instrumentTag)
            taskStatus = STATUS.COMPLETED_SUCCESS;
        else
            taskStatus = STATUS.COMPLETED_FAIL;

        return taskStatus;
    }
}
