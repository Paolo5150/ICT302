using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInstrumentSelectable
{
    void OnPointing();
    void OnSelected();
    void OnReleasedPointing();
}
