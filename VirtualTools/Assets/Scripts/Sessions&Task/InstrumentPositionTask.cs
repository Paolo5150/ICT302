using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentPositionTask : Task
{
    /// <summary>
    /// Gameobjects representing locations where instruments can be placed for the task.
    /// </summary>
    List<Instrument> InstrumentPositionSlots { get; set; }
    private string m_procedureName;
    private List<Instrument.INSTRUMENT_TAG> m_correctInstrumentPositions;
    public List<Instrument.INSTRUMENT_TAG> ActualInstrumentPositions
    {
        get
        {
            List<Instrument.INSTRUMENT_TAG> positions = new List<Instrument.INSTRUMENT_TAG>();
            foreach (var slot in InstrumentPositionSlots)
            {
                positions.Add(slot.instrumentTag);
            }
            return positions;
        }
    }

    public InstrumentPositionTask(string procedureName, List<Instrument.INSTRUMENT_TAG> correctInstrumentPositions)
    {
        m_procedureName = procedureName;
        m_correctInstrumentPositions = correctInstrumentPositions;

        instructions.Add("New task: position the instruments for a <b><u>" + "INSERT PROCEDURE NAME HERE" + "</u></b>");
    }

    public override STATUS Evaluate(Instrument.INSTRUMENT_TAG instrumentTag, Session session)
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
    }
}
