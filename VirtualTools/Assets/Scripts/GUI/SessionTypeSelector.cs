using UnityEngine;

public class SessionTypeSelector : MonoBehaviour
{
	public void BeginSelectByNameSession()
	{
		SessionManager.Instance.CreateSelectByNameSession();
		GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}

	public void BeginSelectByPurposeSession()
	{
		SessionManager.Instance.CreateSelectByPurposeSession();
		GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}

	public void BeginSelectInstrumentPositionSession()
	{
		SessionManager.Instance.CreateInstrumentPositioningSession();
		GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}
}
