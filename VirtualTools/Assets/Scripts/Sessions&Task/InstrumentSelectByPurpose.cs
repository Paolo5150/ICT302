using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelectByPurpose : Task
{
    private Instrument.INSTRUMENT_TAG m_instrumentToSelect;

    public InstrumentSelectByPurpose(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        m_instrumentToSelect = instrumentTag;

        instructions.Add("New task: pick up the best instrument for <b><u>" + Instrument.GetPurposeDescription(m_instrumentToSelect) + "</u></b>");
    }

    /*public override STATUS Evaluate(Instrument.INSTRUMENT_TAG instrumentTag, Session session)
    {
        if (m_instrumentToSelect == instrumentTag)
        {
            taskStatus = STATUS.COMPLETED_SUCCESS;
            session.sessionResults.Log_CorrectlySelectedInstrumentByPurpose(m_instrumentToSelect);
        }
        else
        {
            session.sessionResults.Log_FailedToSelectByPurpose(m_instrumentToSelect, instrumentTag);

            taskStatus = STATUS.COMPLETED_FAIL;
        }

        return taskStatus;
    }*/

    public override STATUS Evaluate(params object[] list)
    {
        Instrument.INSTRUMENT_TAG instrumentTag = (Instrument.INSTRUMENT_TAG)list[0];
        Session session = (Session)list[1];
        if (m_instrumentToSelect == instrumentTag)
        {
            taskStatus = STATUS.COMPLETED_SUCCESS;
            session.sessionResults.Log_CorrectlySelectedInstrumentByPurpose(m_instrumentToSelect);
        }
        else
        {
            session.sessionResults.Log_FailedToSelectByPurpose(m_instrumentToSelect, instrumentTag);

            taskStatus = STATUS.COMPLETED_FAIL;
        }

        return taskStatus;
    }
}
