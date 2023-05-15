using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    GameManeger manager;

    public GameObject uiGroup;
    public Dropdown songDropdown;
    public Toggle autoToggle;

    public GameObject loadingText;
    public GameObject blackPanel;

    // Use this for initialization
    void Start () {

        manager = GameManeger.instance;

        List<Dropdown.OptionData> datas;

        foreach (string name in GameManeger.instance.playlist)
        {
            if (manager.Gamemode == 2 && name != "소년 점프 HARD") { continue; }
            else if (manager.Gamemode == 1 && name == "소년 점프 HARD") { continue; }
            datas = new List<Dropdown.OptionData>();
            datas.Add(new Dropdown.OptionData(name));
            songDropdown.AddOptions(datas);
        }

        if (manager.Gamemode == 2) { autoToggle.gameObject.SetActive(false); }

        StartCoroutine(FadeIn());
    }

    public void ChangeScene()
    {
        manager.autoMode = autoToggle.isOn;
        manager.LoadData(songDropdown.captionText.text);

        uiGroup.SetActive(false);
        loadingText.gameObject.SetActive(true);

        StartCoroutine(FadeOut());
    }

    public void ReturnToMenu()
    {
        StartCoroutine(FadeOutMenu());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Image image = blackPanel.GetComponent<Image>();
        for (float i = 1; i > 0; i -= 0.0332f)
        {
            image.color = new Color(0, 0, 0, i);
            yield return new WaitForSecondsRealtime(0.0166f);
        }
        blackPanel.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(2);
        operation.allowSceneActivation = false;

        yield return new WaitForSecondsRealtime(1.5f);

        if (operation.progress >= 0.9f)
        {
            blackPanel.SetActive(true);
            Image image = blackPanel.GetComponent<Image>();
            for (float i = 0; i < 1; i += 0.0332f)
            {
                image.color = new Color(0, 0, 0, i);
                yield return new WaitForSecondsRealtime(0.0166f);
            }
            yield return new WaitForSecondsRealtime(1);
            operation.allowSceneActivation = true;
        }
    }

    IEnumerator FadeOutMenu()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
        operation.allowSceneActivation = false;

        yield return new WaitForSecondsRealtime(0.2f);

        blackPanel.SetActive(true);
        Image image = blackPanel.GetComponent<Image>();
        for (float i = 0; i < 1; i += 0.0332f)
        {
            image.color = new Color(0, 0, 0, i);
            yield return new WaitForSecondsRealtime(0.0166f);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        operation.allowSceneActivation = true;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
