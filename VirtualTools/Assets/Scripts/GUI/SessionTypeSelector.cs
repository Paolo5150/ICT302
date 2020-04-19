using UnityEngine;

public class SessionTypeSelector : MonoBehaviour
{
    public void CreateSelectByNameSession()
    {
        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Left click anywhere to start!" }, () =>
        {
            SessionManager.Instance.CreateSelectByNameSession();
            GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
            GUIManager.Instance.GetMainCanvas().DisplayEscapeMenu(false);
        });
    }

    public void CreateSelectByPurposeSession()
    {
        GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Left click anywhere to start!" }, () =>
        {
            SessionManager.Instance.CreateSelectByPurposeSession();
            GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
            GUIManager.Instance.GetMainCanvas().DisplayEscapeMenu(false);
        });
    }
}
