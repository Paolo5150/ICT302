using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Organises the expected instruments in the instrument slots for the
/// instrument position task based on the active layout.
/// </summary>
public class InstrumentPositionTaskLocManager : MonoBehaviour
{
    private static InstrumentPositionTaskLocManager m_instance;
    public static InstrumentPositionTaskLocManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject coreGameObject = GameObject.Find("Managers");
                m_instance = coreGameObject.AddComponent<InstrumentPositionTaskLocManager>();
            }

            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
            m_instance = GetComponent<InstrumentPositionTaskLocManager>();
        else
            DestroyImmediate(this);
    }

    /// <summary>
    /// Gameobjects representing slots where instruments can be placed.
    /// </summary>
    public List<InstrumentPositionTaskSlot> InstrumentSlots;
    
    public void PlaceDesiredSlotsInOrder(string instrumentsString)
    {
        string[] instrumentsSplit = instrumentsString.Split(',');
        List<Instrument.INSTRUMENT_TAG> instrumentTags = new List<Instrument.INSTRUMENT_TAG>();
        foreach (var instrumentTagString in instrumentsSplit)
        {
            instrumentTags.Add(Instrument.GetInstrumentTagFromString(instrumentTagString));
        }
        PlaceDesiredSlotsInOrder(instrumentTags);
    }
    
    public void PlaceDesiredSlotsInOrder(List<Instrument.INSTRUMENT_TAG> instrumentsOrder)
    {
        int i = 0;
        foreach (Instrument.INSTRUMENT_TAG instrument in instrumentsOrder)
        {
            Assert.IsTrue(InstrumentSlots.Count > i);
            InstrumentSlots[i].CorrectInstrument = instrumentsOrder[i];
            ++i;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlaceDesiredSlotsInOrder("Scalpel,Scalpel,Scalpel,Scalpel");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
