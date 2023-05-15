using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour {
    #region 변수 목록

    #region 정적 개체 변수
    public static RhythmManager instance = null;
    private GameManeger manager;
    #endregion

    #region 에디터에서 값을 받아오는 변수
    public float noteScale = 0.9f;
    public float noteStartScale = 0.6f;
    public int noteStartAngle = 90;
    public int notePlayAngle = 90;
    public float noteFieldTime = 2.5f;
    public float pivotZPos = -3f;
    public float pivotYPos = -3f;
    public float pivotGap = 4f;

    public float[] inputGap = {8, 4, 2.6f, 2, 1.6f};

    // 판정 범위 - 0: Perfect, 1: Great, 2: Good, 3: Fast/Slow
    public float[] accuracyRange = {0.070f, 0.100f, 0.150f, 0.200f, 0.5f};
    public int[] accuracyScore = { 1000, 850, 550, 150, 150, 0 };
    public float startChainBonus = 1.2f;

    public bool noteFixedStartPos = false;
    public bool autoPlayMode = false;

    public bool startChain = true;

    public int comboCount = 0;
    public int score = 0;
    public int life = 10;

    public GameObject menuPanel;
    public GameObject blackPanel;
    public GameObject pauseButton;

    public Text[] playerNickText;
    public Text[] playerScoreText;

    public Text timeText;
    public Text scoreText;
    public Text comboText;
    public GameObject scorePanel;
    public Slider timeSlider;
    public Slider lifeSlider;

    public Image fullCombo;

    public Text fpsText;

    public bool isGameEnded = false;
    bool fullComboEffect = false;
    int noteCount;

    private int currentNote = 0;
    public int pressedNoteCount = 0;
    #endregion

    #region Line에 관한 변수
    public enum KeyMode : short {
        KEY1 = 1,
        KEY2 = 2,
        KEY4 = 4,
        KEY5 = 5,
        KEY6 = 6
    };

    public KeyMode mode = KeyMode.KEY1;
    //{ -2f, 2f, 0, 0, 0, 0 }, // 총 길이: 4, 개별 간격: 4
    //{ -3.9f, -1.3f, 1.3f, 3.9f, 0, 0 }, // 총 길이: 7.8, 개별 간격: 2.6
    //{ -4f, -2f, 0f, 2f, 4f, 0 }, // 총 길이: 8, 개별 간격: 2
    //{ -4f, -2.4f, -0.8f, 0.8f, 2.4f, 4f } // 총 길이: 8, 개별 간격: 1.6
    public float moveValue;
    #endregion

    #region Prefab 변수
    public GameObject input;
    public GameObject note;
    public GameObject notifier;
    public GameObject noteLine;
    public GameObject multipleLine;

    private GameObject _notifi;
    #endregion

    #region 리스트, 딕셔너리
    public List<NoteData> noteQuery = new List<NoteData>(); // 노트 목록
    public List<GameObject> inputList = new List<GameObject>();
    #endregion

    #region 유니티 에셋
    public AudioClip tapClip;
    public AudioClip hitClip;
    public AudioClip holdClip;
    public AudioClip missClip;
    public AudioClip flipClip;

    public Sprite perfectSprite;
    public Sprite greatSprite;
    public Sprite goodSprite;
    public Sprite fastSprite;
    public Sprite slowSprite;
    public Sprite missSprite;

    public Sprite tapNote;
    public Sprite flickLeftNote;
    public Sprite flickRightNote;
    public Sprite flickUpNote;
    public Sprite longNote;
    public Sprite slideNote;

    public Sprite gameOver;
    public Sprite newRecord;
    #endregion
    #endregion

    void Awake() {
        instance = this;
        manager = GameManeger.instance;
        StartCoroutine(FadeIn());
    }

    void Start() {
        _notifi = Instantiate(notifier, new Vector3(0, pivotYPos + 1, pivotZPos - (pivotGap - 2)), Quaternion.identity);
        _notifi.name = "NotifierObj";

        autoPlayMode = manager.autoMode;

        if (manager.Gamemode == 1) { scoreText.gameObject.SetActive(false); lifeSlider.gameObject.SetActive(false); scorePanel.gameObject.SetActive(false); }
        else { UpdateScoreboard(); }

        LoadBeatmap();
        InitializeMoveValue();
        CreateInput();
    }

    float deltaTime;

    void Update() {

        UpdateText();

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = string.Format("FPS: {0:00.00}", fps);

        if (isGameEnded) { return; }

        if (SoundManeger.instance.musicSource.time >= SoundManeger.instance.musicSource.clip.length)
        {
            pauseButton.SetActive(false);
            isGameEnded = true;
            StartCoroutine(ReturnToMenu(2));
        }

        if (pressedNoteCount >= noteCount)
        {
            if (manager.isFirstRank()) { fullCombo.GetComponent<Image>().sprite = newRecord; }
            if ((manager.isFirstRank() || comboCount == noteCount) && !fullComboEffect) { fullCombo.gameObject.SetActive(true); }
            if (manager.Gamemode == 2 && comboCount == noteCount) { score = (int)Mathf.Round(score * 1.25f); UpdateScore(); }
            return;
        }

        if (noteQuery.Count - 1 < currentNote) { return; }

        if (noteQuery[currentNote].Time - noteFieldTime <= SoundManeger.instance.GetPlayTime()) {
            noteQuery[currentNote].CreateFieldObject();
            currentNote++;
            //if (currentNote != 0 && noteQuery[currentNote - 1].Time == noteQuery[currentNote].Time) {
            //    GameObject line = Instantiate(multipleLine);
            //    line.SendMessage("SetNoteData", noteQuery[currentNote - 1], SendMessageOptions.RequireReceiver);
            //}
            Update();
            return;
        }
    }

    /// <summary>
    /// 현재 객체의 mode 값을 Index 값으로 변환하여 반환합니다.
    /// </summary>
    /// <returns>mode의 Index 값</returns>
    public int GetKeyIndex() {
        switch (mode) {
            case KeyMode.KEY1:
                return 0;
            case KeyMode.KEY2:
                return 1;
            case KeyMode.KEY4:
                return 2;
            case KeyMode.KEY5:
                return 3;
            case KeyMode.KEY6:
                return 4;
            default:
                return 0;
        }
    }

    /// <summary>
    /// inputID의 X 좌표값을 반환합니다.
    /// </summary>
    /// <param name="startLine">inputID 값</param>
    /// <returns>id의 X 좌표값</returns>
    public float GetInputPos(float startLine) {
        return startLine * inputGap[GetKeyIndex()] + moveValue;
    }

    /// <summary>
    /// id 값으로 채보의 노트 리스트에서 찾고자하는 노트의 채보 데이터를 반환합니다.
    /// </summary>
    /// <param name="id">찾고자하는 노트의 id</param>
    /// <returns>노트의 채보 데이터(Notemap)</returns>
    public NoteData GetNoteByID(int id) {
        foreach (NoteData note in noteQuery) {
            if (note.ID == id) { return note; }
        }
        return null;
    }

    /// <summary>
    /// 채보를 불러옵니다.
    /// </summary>
    void LoadBeatmap() {
        Beatmap data = new Beatmap(manager.beatmapFile.ToString());

        mode = (KeyMode)data.Line;

        foreach (JObject obj in data.notes)
        {
            noteQuery.Add(new NoteData(obj));
        }

        noteQuery.Sort(delegate (NoteData A, NoteData B) {
            if (A.Time > B.Time) { return 1; } else if (A.Time < B.Time) { return -1; }
            return 0;
        });

        noteCount = noteQuery.Count;
        if (noteCount == 0) { fullComboEffect = true; }

        SoundManeger.instance.PlayMusic(manager.musicFile);
    }

    /// <summary>
    /// moveValue 값을 계산합니다.
    /// </summary>
    void InitializeMoveValue() {
        int line = (int)mode + 2;
        if (line % 2 != 0) { moveValue = 0 - (int)(line / 2 + 0.5f) * inputGap[GetKeyIndex()]; } else { moveValue = 0 - (line / 2 * inputGap[GetKeyIndex()] + (line / 2 - 1) * inputGap[GetKeyIndex()]) / 2; }
    }

    /// <summary>
    /// 입력 라인 (판정선)을 생성합니다.
    /// </summary>
    void CreateInput() {
        GameObject parent = new GameObject("Input_Pivot");
        GameObject obj;

        parent.transform.position = new Vector3(0, pivotYPos, pivotZPos);
        parent.transform.localEulerAngles = new Vector3((notePlayAngle - noteStartAngle) * -1, 0, 0);

        for (int i = 1; i < (int)mode + 1; i++) {
            obj = Instantiate(input);
            obj.name = "Input[" + i + "]";

            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = new Vector3(GetInputPos(i), 0, pivotGap * -1);
            obj.transform.localEulerAngles = new Vector3(notePlayAngle - noteStartAngle, 0, 0);

            obj.SendMessage("SetID", i, SendMessageOptions.RequireReceiver);
            obj.SendMessage("SetKey", manager.inputKey[GetKeyIndex(), i - 1], SendMessageOptions.RequireReceiver);
            inputList.Add(obj);
        }
    }


    public void UpdateScore(int addScore = 0)
    {
        score += addScore;
        manager.UpdateScore(manager.currentPlayer, score);
        UpdateScoreboard();

    }

    public void UpdateScoreboard()
    {
        int loop = manager.playerDatas.Count > 10 ? 10 : manager.playerDatas.Count;

        for (int i = 0; i < loop; i++)
        {
            playerNickText[i].text = (i + 1) + ". " + manager.playerDatas[i].Nickname;
            playerScoreText[i].text = manager.playerDatas[i].Score.ToString();
        }
    }

    public void UpdateLife(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            lifeSlider.value = 0;
            SoundManeger.instance.musicSource.Pause();
            pauseButton.SetActive(false);
            fullCombo.gameObject.SetActive(true);
            fullCombo.GetComponent<Image>().sprite = gameOver;
            isGameEnded = true;
            StartCoroutine(ReturnToMenu(2));
            foreach (NoteData obj in noteQuery)
            {
                if (obj.FieldObject != null) { Destroy(obj.FieldObject); }
                if (obj.Trail[0] != null) { Destroy(obj.Trail[0]); }
                if (obj.Trail[1] != null) { Destroy(obj.Trail[1]); }
            }
            noteQuery.Clear();
        } else { lifeSlider.value = life; }
    }

    /// <summary>
    /// 판정 표시기에게 level의 판정을 표시하도록 지시합니다.
    /// </summary>
    /// <param name="level">판정 레벨</param>
    public void UpdateNotification(int level) {
        Sprite[] spr = {
            perfectSprite,
            greatSprite,
            goodSprite,
            fastSprite,
            slowSprite,
            missSprite
        };

        NotifiBehaviour behaviour = _notifi.GetComponent<NotifiBehaviour>();
        behaviour.Notification(spr[level]);
    }

    /// <summary>
    /// 각종 텍스트 및 UI 요소를 업데이트 합니다.
    /// </summary>
    void UpdateText()
    {
        // Time Text 업데이트
        int min = (int)Math.Floor(SoundManeger.instance.GetPlayTime() / 60);
        float sec = SoundManeger.instance.GetPlayTime() % 60;
        timeText.text = string.Format("{0:00}:{1:00.00}", min, sec);

        // Combo Text 업데이트
        comboText.text = comboCount.ToString();

        // Score Text 업데이트
        scoreText.text = score.ToString();

        // Time Slider 업데이트
        timeSlider.value = SoundManeger.instance.GetPlayTime() / SoundManeger.instance.musicSource.clip.length;
    }

    /// <summary>
    /// 검은 화면을 걷어냅니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeIn()
    {
        Image image = blackPanel.GetComponent<Image>();
        for (float i = 1; i > 0; i -= 0.0333f)
        {
            image.color = new Color(0, 0, 0, i);
            yield return new WaitForSecondsRealtime(0.01666f);
        }
        blackPanel.SetActive(false);
    }

    /// <summary>
    /// 검은 화면을 다시 만듭니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeOut(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;
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

    /// <summary>
    /// 게임을 중지합니다.
    /// </summary>
    /// 
    public void PauseGame()
    {
        SoundManeger.instance.musicSource.Pause();
        Time.timeScale = 0f;
        GameObject[] inputs = inputList.ToArray();
        foreach (GameObject obj in inputs)
        {
            obj.GetComponent<BoxCollider2D>().enabled = false;
        }
        menuPanel.SetActive(true);
    }

    /// <summary>
    /// 게임을 재개합니다.
    /// </summary>
    public void UnPauseGame()
    {
        SoundManeger.instance.musicSource.UnPause();
        Time.timeScale = 1f;
        GameObject[] inputs = inputList.ToArray();
        foreach (GameObject obj in inputs)
        {
            obj.GetComponent<BoxCollider2D>().enabled = true;
        }
        menuPanel.SetActive(false);
    }

    public void ReturnToMenu()
    {
        StartCoroutine(ReturnToMenu(0.5f));
    }

    IEnumerator ReturnToMenu(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (manager.Gamemode == 2 && Time.timeScale != 0)
        {
            var query = from data in manager.playerDatas
                        where data.Name == manager.currentPlayer
                        select data;
            foreach (var t in query)
            {
                t.Score = score;
            }
            manager.SaveData();
        }

        StartCoroutine(FadeOut(1));
    }

    /// <summary>
    /// 게임을 종료합니다.
    /// </summary>
    public void ExitGame() {
        SceneManager.LoadScene(1);
    }
}
