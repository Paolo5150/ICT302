using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelectTask : Task
{
    private Instrument.INSTRUMENT_TAG m_instrumentToSelect;

    public InstrumentSelectTask(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        m_instrumentToSelect = instrumentTag;
        instructions.Add("New task: pick up a " + m_instrumentToSelect.ToString());
    }


}
