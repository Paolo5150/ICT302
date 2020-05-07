using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Organises the instruments in the correct order when the scene starts.
/// Doesn't support cleanup/re-ordering instruments again.
/// </summary>
public class InstrumentLocManager : MonoBehaviour
{
    private static InstrumentLocManager m_instance;
    public static InstrumentLocManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject coreGameObject = GameObject.Find("Managers");
                m_instance = coreGameObject.AddComponent<InstrumentLocManager>();
            }

            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
            m_instance = GetComponent<InstrumentLocManager>();
        else
            DestroyImmediate(this);
    }
    /// <summary>
    /// Gameobjects representing locations where instruments can go.
    /// </summary>
    [SerializeField]
    List<GameObject> InstrumentLocationSlots;

    /// <summary>
    /// Instrument monobehaviours for instrument gameobjects in the scene that will be instantiated in the correct scene location.
    /// </summary>
    [SerializeField]
    List<Instrument> InstrumentGameObjects;


    public static List<Instrument.INSTRUMENT_TAG> CurrentInstrumentOrder { get; set; }

    /// <summary>
    /// Order the instruments in the order given by the string.
    /// </summary>
    /// <param name="instruments">String representing a list of instruments tags. Each one must be unique. 
    /// Number of instruments in the list must be <= than the number of InstrumentLocationSlots.</param>
    public void PlaceInstrumentsInOrder(string instrumentsString)
    {
        string[] instrumentsSplit = instrumentsString.Split(',');
        List<Instrument.INSTRUMENT_TAG> instrumentTags = new List<Instrument.INSTRUMENT_TAG>();
        foreach (var instrumentTagString in instrumentsSplit)
        {
            instrumentTags.Add(Instrument.GetInstrumentTagFromString(instrumentTagString));
        }
        PlaceInstrumentsInOrder(instrumentTags);
    }
    /// <summary>
    /// Order the instruments. (internal method)
    /// </summary>
    /// <param name="instruments">List of instruments. Each one must be unique. 
    /// Must not be larger in size than the number of InstrumentLocationSlots.</param>
    public void PlaceInstrumentsInOrder(List<Instrument.INSTRUMENT_TAG> instrumentsOrder)
    {
        CurrentInstrumentOrder = instrumentsOrder;
        int i = 0;
        foreach(Instrument.INSTRUMENT_TAG instrument in instrumentsOrder)
        {
            if(instrument != Instrument.INSTRUMENT_TAG.NONE)
            {
                // Get the instrument with each tag in the instrument gameobject list.
                // Also ensure there's no repeated instruments gameobjects.
                List<Instrument> taggedGameObjects = InstrumentGameObjects.FindAll(a => a.instrumentTag == instrument);
            	Assert.AreEqual(taggedGameObjects.Count, 1, "Duplicate INSTRUMENT_TAG in the InstrumentGameObjects");
                Instrument instrumentToPlace = taggedGameObjects[0];
                // Place the instrument gameobject in the desired location and unhide it
                Assert.IsTrue(InstrumentLocationSlots.Count > i);
                Instantiate<GameObject>(instrumentToPlace.gameObject,
                    InstrumentLocationSlots[i].transform.position,
                    instrumentToPlace.transform.rotation,
                    InstrumentLocationSlots[i].transform);
            }
            ++i;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
