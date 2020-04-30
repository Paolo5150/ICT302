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
    /// <summary>
    /// Gameobjects representing locations where instruments can go.
    /// </summary>
    [SerializeField]
    List<GameObject> InstrumentLocationSlots;

    /// <summary>
    /// Instrument monobehaviours for instrument gameobjects in the scene that will be moved to the correct scene location.
    /// </summary>
    [SerializeField]
    List<Instrument> InstrumentGameObjects;

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
    private void PlaceInstrumentsInOrder(List<Instrument.INSTRUMENT_TAG> instruments)
    {
        int i = 0;
        foreach(Instrument.INSTRUMENT_TAG instrument in instruments)
        {
            if(instrument != Instrument.INSTRUMENT_TAG.NONE)
            {
                // Get the instrument with each tag in the instrument gameobject list.
                // Also ensure there's no repeated instruments gameobjects.
                List<Instrument> taggedGameObjects = InstrumentGameObjects.FindAll(a => a.instrumentTag == instrument);
                Assert.AreEqual(taggedGameObjects.Count, 1);
                Instrument instrumentToMove = taggedGameObjects[0];
                // Ensure there's no repeats of the current item in the ordered list that was sent through.
                List<Instrument.INSTRUMENT_TAG> instrumentRepeatedTags = instruments.FindAll(b => b == instrument);
                Assert.AreEqual(instrumentRepeatedTags.Count, 1);
                // Place the instrument gameobject in the desired location and unhide it
                Assert.IsTrue(InstrumentLocationSlots.Count > i);
                instrumentToMove.gameObject.transform.SetParent(InstrumentLocationSlots[i].transform);
                instrumentToMove.gameObject.transform.localPosition = new Vector3();
                instrumentToMove.gameObject.transform.localRotation = new Quaternion();
                instrumentToMove.gameObject.SetActive(true);
            }
            ++i;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlaceInstrumentsInOrder("Addson-Brown Forceps,Empty,Scalpel,Towel Clamps");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
