using UnityEngine;

public class SessionTypeSelector : MonoBehaviour
{
    public void BeginSelectByNameSession()
    {
        //GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Left click anywhere to start!" }, () =>
        //{
            SessionManager.Instance.CreateSelectByNameSession();
            GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
            GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
        //});
    }

    public void BeginSelectByPurposeSession()
    {
        //GUIManager.Instance.GetMainCanvas().DogInstructionSequence(new string[] { "Left click anywhere to start!" }, () =>
        //{
            SessionManager.Instance.CreateSelectByPurposeSession();
            GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
            GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
        //});
    }
}
