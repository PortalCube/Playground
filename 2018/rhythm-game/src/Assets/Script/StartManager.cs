using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartManager : MonoBehaviour {

    public GameObject blackPanel;

    public TMP_InputField nameField;
    public TMP_InputField phoneField;
    public TMP_InputField nickField;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Shutdown()
    {
        Application.Quit();
    }

    public void AddPlayerData()
    {
        GameManeger.instance.AddData(nameField.text, nickField.text, phoneField.text);
    }

    public void StartGame(bool isChallange)
    {
        if (isChallange) { GameManeger.instance.Gamemode = 2; AddPlayerData(); }
        else { GameManeger.instance.Gamemode = 1; }
        StartCoroutine(FadeOut(1));
    }

    public void OpenEditor()
    {
        StartCoroutine(FadeOut(3));
    }

    IEnumerator FadeOut(int scene)
    {
        blackPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;

        yield return new WaitForSecondsRealtime(0.2f);

        Image image = blackPanel.GetComponent<Image>();
        for (float i = 0; i < 1; i += 0.0332f)
        {
            image.color = new Color(0, 0, 0, i);
            yield return new WaitForSecondsRealtime(0.0166f);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        operation.allowSceneActivation = true;
    }
}
