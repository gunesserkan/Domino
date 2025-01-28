using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public GameObject settingsPanel;

    public static string themeKeeper="light";
    public void ChangeScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void exit()
    {
        Application.Quit();
    }
    public void ChangeTheme(string theme)
    {
        Debug.Log(theme);
        themeKeeper= theme;
    }
    public float moveDuration = 0.5f;
    public float targetXPosition = 0.0f;

    public void MoveSettingsPanel(float targetXPosition)
    {
        this.targetXPosition = targetXPosition;
        StartCoroutine(MovePanelCoroutine());
    }

    private IEnumerator MovePanelCoroutine()
    {
        Vector3 startPosition = settingsPanel.transform.position;
        Vector3 targetPosition = new Vector3(targetXPosition, startPosition.y, startPosition.z);

        float startTime = Time.time;
        while (Time.time < startTime + moveDuration)
        {
            float t = (Time.time - startTime) / moveDuration;
            settingsPanel.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        settingsPanel.transform.position = targetPosition;
    }
}
