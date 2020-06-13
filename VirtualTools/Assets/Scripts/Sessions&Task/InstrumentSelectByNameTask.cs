using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelectByNameTask : Task
{
    private Instrument.INSTRUMENT_TAG m_instrumentToSelect;

    public InstrumentSelectByNameTask(Instrument.INSTRUMENT_TAG instrumentTag)
    {
        m_instrumentToSelect = instrumentTag;

        instructions.Add("New task: pick up a <b><u>" + Instrument.GetName(m_instrumentToSelect) + "</u></b>");
    }

    /* public override STATUS Evaluate(Instrument.INSTRUMENT_TAG instrumentTag, Session session)
     {
         if (m_instrumentToSelect == instrumentTag)
         {
             taskStatus = STATUS.COMPLETED_SUCCESS;
             session.sessionResults.Log_CorrectlySelectedInstrumentByName(m_instrumentToSelect);
         }
         else
         {
             session.sessionResults.Log_FailedToSelectByName(m_instrumentToSelect, instrumentTag);

             taskStatus = STATUS.COMPLETED_FAIL;
         }

         return taskStatus;
     }*/
    /// <summary>
    /// Evaluate task
    /// </summary>
    /// <param name="list">Index 0 must be the instrument tag, index 1 must be the session</param>
    /// <returns>The completion status of the task</returns>
    public override STATUS Evaluate(params object[] list)
    {
        Instrument.INSTRUMENT_TAG instrumentTag = (Instrument.INSTRUMENT_TAG)list[0];
        Session session = (Session)list[1];
        if (m_instrumentToSelect == instrumentTag)
        {
            taskStatus = STATUS.COMPLETED_SUCCESS;
            session.sessionResults.Log_CorrectlySelectedInstrumentByName(m_instrumentToSelect);
        }
        else
        {
            session.sessionResults.Log_FailedToSelectByName(m_instrumentToSelect, instrumentTag);

            taskStatus = STATUS.COMPLETED_FAIL;
        }

        return taskStatus;
    }
}
