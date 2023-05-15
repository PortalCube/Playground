using Crosstales.FB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour {

    public static EditorManager instance = null;
    GameManeger m = GameManeger.instance;

    AudioSource audioSource;
    AudioSource tapAudioSource;

    public int rectWidth = 200;
    public float ContentWidth { get { return rectWidth * (viewSlider.value / 10); } }

    public float[] pressTime = new float[6];
    bool isLoading = false;

    public float NoteInteval { get { return 60f / BPM / Mathf.Pow(2, snapSizeSlider.value - 1); } }
    public int BPM {
        get {
            int bpm = -1;
            int.TryParse(bpmField.text, out bpm);

            if (bpm <= 0) { return 60; }
            else { return bpm; }
        }
    }

    public AudioClip tapSound;

    public GameObject blackPanel;
    public GameObject scrollContent;
    public Scrollbar scroll;
    
    public GameObject editNote;
    public GameObject vLine;
    public GameObject longLine;
    public GameObject whiteLeftLine;
    public GameObject whiteCenterLine;

    GameObject playLine;

    public RectTransform contentRect;

    public List<Note> noteList;
    public List<GameObject> noteObjectList = new List<GameObject>();

    List<GameObject> lineList = new List<GameObject>();
    List<GameObject> snapList = new List<GameObject>();
    public List<float> timeList = new List<float>();

    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicSpeedSlider;

    public Text musicSpeedText;

    #region 좌측 패널
    public Dropdown beatmapDropdown;
    public Dropdown songDropdown;
    public TMP_InputField songNameField;
    public Text selectedSongText;
    public TMP_InputField songFileNameField;
    public TMP_InputField artistField;
    public TMP_InputField idField;
    public Dropdown difficultyDropdown;
    public Slider difficultyNumberSlider;
    public Text difficultyNumberText;
    public Dropdown LineDropdown;
    public TMP_InputField genreField;
    public TMP_InputField bpmField;
    #endregion

    #region 하단 패널

    public Text timeText;
    public Toggle snapToggle;
    public Slider snapSizeSlider;
    public Slider viewSlider;
    public Slider playSlider;
    public Text snapText;
    public Text viewText;

    #endregion

    private void Awake()
    {
        instance = this;
        audioSource = GetComponents<AudioSource>()[0];
        tapAudioSource = GetComponents<AudioSource>()[1];
        contentRect = scrollContent.GetComponent<RectTransform>();
        tapAudioSource.clip = tapSound;
        audioSource.clip = tapSound;
    }

    // Use this for initialization
    void Start () {
        List<Dropdown.OptionData> datas;

        datas = new List<Dropdown.OptionData>();
        datas.Add(new Dropdown.OptionData("새로운 채보"));
        beatmapDropdown.AddOptions(datas);

        foreach (string song in m.playlist)
        {
            datas = new List<Dropdown.OptionData>();
            datas.Add(new Dropdown.OptionData(song));
            beatmapDropdown.AddOptions(datas);
        }

        foreach (KeyValuePair<string, FileInfo> pair in m.musicFilesInfo)
        {
            datas = new List<Dropdown.OptionData>();
            datas.Add(new Dropdown.OptionData(pair.Key));
            songDropdown.AddOptions(datas);
        }

        LoadBeatmap(false);
        CreatePlayLine();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeText();
        CheckInput();
        CheckNoteTime();
        if (audioSource.isPlaying) { UpdateScroll(); }
    }

    #region 텍스트 설정
    public void UpdateTimeText()
    {
        float t = GetComponent<AudioSource>().time;
        int min = (int)Math.Floor(t / 60);
        float sec = t % 60;
        timeText.text = string.Format("{0:00}:{1:00.0}", min, sec);
        playSlider.value = audioSource.time / audioSource.clip.length;
    }

    public void UpdateSliderText()
    {
        difficultyNumberText.text = difficultyNumberSlider.value.ToString();
        snapText.text = Mathf.Pow(2, snapSizeSlider.value - 1).ToString() + "X";
        viewText.text = (viewSlider.value * 10).ToString() + "%";
    }


    #endregion

    void CreatePlayLine()
    {
        // PlayLine 생성
        playLine = Instantiate(whiteLeftLine, contentRect.transform);
        playLine.GetComponent<Image>().color = Color.yellow;
        playLine.GetComponent<RectTransform>().localPosition = new Vector3(0, -100, playLine.GetComponent<RectTransform>().localPosition.z);
    }

    public void UpdateAudioClip(bool isNew)
    {
        if (isLoading) { }
        string song = isNew ? beatmapDropdown.captionText.text : songDropdown.captionText.text;
        if (m.musicFilesInfo.ContainsKey(song))
        {
            m.LoadData(song, 2);
            selectedSongText.text = song;
        }
        else { m.LoadData("00test", 2); selectedSongText.text = "00test"; }
        StartCoroutine(InsertAudioClip());
    }

    IEnumerator InsertAudioClip()
    {
        while (m.musicFile == null)
        {
            yield return null;
        }
        AudioClip clip = m.musicFile;
        audioSource.clip = clip;
        UpdateContent();
        yield return null;
    }

    public void PlayAudio()
    {
        if (audioSource.isPlaying) { audioSource.Pause(); }
        else {
            if (audioSource.time == 0) { audioSource.Play(); }
            else { audioSource.UnPause(); }
            UpdateTimeList();
        }
    }

    public void UpdateVolume()
    {
        audioSource.volume = musicVolumeSlider.value;
        tapAudioSource.volume = sfxVolumeSlider.value;
        audioSource.pitch = musicSpeedSlider.value / 4;

        musicSpeedText.text = audioSource.pitch + "X";
    }

    public void UpdateTimeList()
    {
        timeList = new List<float>();

        Array.ForEach(noteList.ToArray(), element => timeList.Add(element.Time));

        timeList.Sort(delegate (float A, float B) {
            if (A > B) { return 1; } else if (A < B) { return -1; }
            return 0;
        });

        bool remove = false;

        for (int i = 0; i < timeList.Count; i++)
        {
            if (timeList[i] >= audioSource.time) { timeList.RemoveRange(0, i); remove = true; break; }
        }

        if (!remove) { timeList.Clear(); }
    }

    public void AudioMovePosition()
    {
        audioSource.time = audioSource.clip.length * playSlider.value;
        UpdateScroll();
    }

    public void SnapUpdate()
    {
        float value = 60f / BPM / Mathf.Pow(2, snapSizeSlider.value - 1);
        Array.ForEach(snapList.ToArray(), element => Destroy(element));
        snapList = new List<GameObject>();

        for (int i = 1; i < (int)Math.Floor(audioSource.clip.length / value); i++)
        {
            GameObject obj = Instantiate(vLine, scrollContent.transform);
            obj.name = "VLine";
            snapList.Add(obj);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(value * ContentWidth * i, -100, obj.GetComponent<RectTransform>().localPosition.z);
        }
    }

    void CheckNoteTime()
    {
        if (!audioSource.isPlaying || timeList.Count == 0) { return; }
        if (timeList[0] <= audioSource.time)
        {
            tapAudioSource.Play();
            timeList.RemoveAt(0);
        }
    }

    void UpdateScroll()
    {
        RectTransform rect = playLine.GetComponent<RectTransform>();
        float rectWidth = ContentWidth;
        if (audioSource.time < 200 / rectWidth) { scroll.value = 0; }
        else if (audioSource.time > audioSource.clip.length - (700 / rectWidth)) { scroll.value = 1; }
        else { scroll.value = (audioSource.time - (200 / rectWidth)) / (audioSource.clip.length - (900 / rectWidth)); }

        rect.localPosition = new Vector3(contentRect.sizeDelta.x * audioSource.time / audioSource.clip.length, rect.localPosition.y, rect.localPosition.z);
    }

    public Note CreateNote(float time, int line, int id = -1, int prevID = 0, int nextID = 0)
    {
        // Note Check -- time과 EndLine가 동일한 노트가 있으면 노트 추가 취소
        foreach (Note n in noteList)
        {
            if (n.Time == time && n.EndLine == line) { return null; }
        }

        if (id != -1) { }
        else if (noteList.Count == 0) { id = 1; }
        else { id = noteList[noteList.Count - 1].ID + 1; }

        Note note = new Note(id, time, line, prevID, nextID);
        noteList.Add(note);

        Debug.LogFormat("Create Note - Time: {0}, Line: {1}, ID: {2}", note.Time, note.EndLine, note.ID);

        if (audioSource.isPlaying) { UpdateTimeList(); }

        CreateNoteObject(note);
        return note;
    }

    void CreateNoteObject(Note note)
    {
        RectTransform rect;
        GameObject obj = Instantiate(editNote, scrollContent.transform);
        rect = obj.GetComponent<RectTransform>();
        obj.name = "Note_" + note.ID;
        noteObjectList.Add(obj);
        obj.SendMessage("SetNote", note);
        rect.localPosition = new Vector3(note.Time * ContentWidth, LineToYPos(note.EndLine), rect.localPosition.z);
    }

    void CreateNoteLine(Note note)
    {
        RectTransform rect;

        // Long 노트 라인 생성 코드
        if (note.PrevNoteID != 0)
        {
            Note prev = FindNote(note.PrevNoteID);
            if (prev == null ) { return; }
            if (prev.NextNoteID != note.ID) { note.PrevNoteID = 0; return; }

            GameObject line = Instantiate(longLine, scrollContent.transform);
            rect = line.GetComponent<RectTransform>();
            line.name = "LongLine_" + note.ID;
            lineList.Add(line);
            float width = (note.Time - prev.Time) * ContentWidth;
            rect.localPosition = new Vector3(prev.Time * ContentWidth + width / 2, LineToYPos(prev.EndLine), rect.localPosition.z);
            rect.localEulerAngles = new Vector3(0, 0, GetAngle(FindNoteObject(note.ID).transform.localPosition, FindNoteObject(prev.ID).transform.localPosition));
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
            line.transform.SetParent(FindNoteObject(note.PrevNoteID).transform);
        }

    }

    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;

        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    GameObject FindNoteObject(int id)
    {
        GameObject obj = null;
        foreach (GameObject n in noteObjectList)
        {
            if (n == null) { continue; }
            if (n.GetComponent<EditNoteBehaviour>().note.ID == id) { obj = n; break; }
        }
        return obj;
    }

    Note FindNote(int id)
    {
        Note obj = null;
        foreach (Note n in noteList)
        {
            if (n.ID == id) { obj = n; break; }
        }
        return obj;
    }

    public void MoveNote(int id, float time, int line)
    {

        foreach (Note n in noteList)
        {
            if (n.Time == time && n.EndLine == line)
            {
                RemoveNote(id);
                UpdateContent();
                return;
            }
        }

        for (int i = 0; i < noteList.Count; i++)
        {
            if (noteList[i].ID == id)
            {
                noteList[i].Time = time;
                noteList[i].StartLine = line;
                noteList[i].EndLine = line;
            }
        }

        UpdateContent();
    }

    public void RemoveNote(int id, int streakID = 0)
    {
        bool reload = false;
        Note note;
        for (int i = 0; i < noteList.Count; i++)
        {
            if (noteList[i].ID == id)
            {
                note = noteList[i];
                Debug.LogFormat("Debug Info: id: {0}, streak: {1}", id, streakID);
                Debug.LogFormat("prev: {0}, next: {1}", note.PrevNoteID, note.NextNoteID);
                if (note.PrevNoteID != 0 && note.PrevNoteID != streakID) { RemoveNote(note.PrevNoteID, id); reload = true; }
                if (note.NextNoteID != 0 && note.NextNoteID != streakID) { RemoveNote(note.NextNoteID, id); reload = true; }
                noteList.Remove(note);
            }
        }
        if (reload && streakID == 0) { UpdateContent(); }
        Debug.LogFormat("Remove Note - Time: {0}", id);
    }

    public void UpdateContent()
    {
        if (isLoading) { return; }
        Debug.Log("Updating!");

        // ContentRect의 사이즈를 Audio에 맞게 수정
        contentRect.sizeDelta = new Vector2(audioSource.clip.length * ContentWidth, contentRect.sizeDelta.y);

        // 기존의 HLine을 지우고 새로운 HLine 투입
        Array.ForEach(lineList.ToArray(), element => Destroy(element));
        lineList = new List<GameObject>();
        for (int i = 1; i < IndexToLine(LineDropdown.value) + 1; i++)
        {
            GameObject obj = Instantiate(whiteCenterLine, scrollContent.transform);
            obj.name = "HLine";
            lineList.Add(obj);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(contentRect.sizeDelta.x / 2, LineToYPos(i), obj.GetComponent<RectTransform>().localPosition.z);
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(contentRect.sizeDelta.x, obj.GetComponent<RectTransform>().sizeDelta.y);
        }

        // 기존의 VLine을 지우고 새로운 HLine 투입
        SnapUpdate();

        // 기존의 노트를 지우고 새로운 노트 투입
        Array.ForEach(noteObjectList.ToArray(), element => Destroy(element));
        noteObjectList = new List<GameObject>();
        foreach (Note note in noteList)
        {
            CreateNoteObject(note);
        }
        foreach (Note note in noteList)
        {
            CreateNoteLine(note);
        }
    }

    #region Beatmap 저장 및 불러오기

    public void LoadBeatmap(bool external)
    {
        Beatmap beatmap;
        if (external)
        {
            string path = FileBrowser.OpenSingleFile("채보를 선택하세요.", "", "json");
            beatmap = new Beatmap(File.ReadAllText(path));
        } else { beatmap = null; }
        LoadBeatmap(beatmap);
    }

    public void LoadBeatmap(Beatmap bMap)
    {
        isLoading = true;

        // Beatmap 로드
        Beatmap beatmap;

        noteList = new List<Note>();

        if (bMap == null) {
            if (beatmapDropdown.value == 0) { beatmap = new Beatmap(""); }
            else
            {
                m.LoadData(beatmapDropdown.value - 1);
                beatmap = new Beatmap(m.beatmapFile.ToString());
            }
        } else {
            beatmap = bMap;
        }

        foreach (JObject obj in beatmap.notes) {
            noteList.Add(new Note(obj));
        }

        // Beatmap/Metadata -> 필드 값
        songNameField.text = beatmap.Name;
        songFileNameField.text = beatmap.FileName;
        artistField.text = beatmap.Artist;
        idField.text = beatmap.ID.ToString();
        difficultyDropdown.value = beatmap.Difficulty;
        difficultyNumberSlider.value = beatmap.DifficultyValue;
        LineDropdown.value = LineToIndex(beatmap.Line);
        genreField.text = beatmap.Genre;
        bpmField.text = beatmap.BPM.ToString();

        isLoading = false;

        // Content 업데이트 & Audio Clip 업데이트
        UpdateAudioClip(true);
    }

    public void SaveFile()
    {

        string path = FileBrowser.SaveFile("Beatmap을 저장할 위치 선택", "", songNameField.text, "json");

        if (path == null) { return; }

        JObject json = new JObject();
        JObject metadata = new JObject();
        JArray notes = new JArray();
        List<Note> notelist = new List<Note>();

        foreach (Note note in noteList)
        {
            notelist.Add(note);
        }

        for (int i = 1; i < notelist.Count + 1; i++)
        {
            notelist[i - 1].ID = i;
        }

        foreach (Note note in notelist)
        {
            notes.Add(note.GetJObject());
        }


        metadata.Add("songName", songNameField.text);
        metadata.Add("songFile", songFileNameField.text);
        metadata.Add("artist", artistField.text);
        metadata.Add("id", int.Parse(idField.text));
        metadata.Add("difficultyType", difficultyDropdown.value);
        metadata.Add("difficultyNumber", difficultyNumberSlider.value);
        metadata.Add("inputLine", IndexToLine(LineDropdown.value));
        metadata.Add("genre", genreField.text);
        metadata.Add("bpm", int.Parse(bpmField.text));

        json.Add("metadata", metadata);
        json.Add("notes", notes);

        File.WriteAllText(path, json.ToString());
        
    }
    #endregion

    void CheckInput()
    {
        if (!audioSource.isPlaying) { return; }
        int mode = LineDropdown.value;
        for (int i = 0; i < IndexToLine(LineDropdown.value); i++)
        {
            if (Input.GetKeyDown(m.inputKey[mode, i]))
            {
                pressTime[i] = audioSource.time;
            }
            else if (Input.GetKeyUp(m.inputKey[mode, i]))
            {
                float pressingTime = audioSource.time - pressTime[i];
                //if (pressingTime >= NoteInteval) { LongInput(pressTime[i], audioSource.time, i + 1); }
                //else { ShortInput(pressTime[i], i + 1); }
                ShortInput(pressTime[i], i + 1);
            }
        }
    }

    void ShortInput(float start, int line)
    {
        float time;
        if (snapToggle.isOn)
        {
            int space = 1;
            if (Input.GetKey(KeyCode.Space) && snapSizeSlider.value != 4) { space = 2; }
            float value = NoteInteval / space;
            time = (int)Math.Round(start / value) * value;
        }
        else { time = start; }
        if (CreateNote(time, line) != null && time <= audioSource.time) { audioSource.PlayOneShot(tapSound); }
    }

    void LongInput(float start, float end, int line)
    {
        float time;
        int id;
        Note sNote, eNote;

        // Value 구하기
        int space = 1;
        if (Input.GetKey(KeyCode.Space) && snapSizeSlider.value != 4) { space = 2; }
        float value = NoteInteval / space;

        // ID 구하기
        if (noteList.Count == 0) { id = 1; }
        else { id = noteList[noteList.Count - 1].ID + 1; }

        // startNote
        if (snapToggle.isOn) { time = (int)Math.Round(start / value) * value; }
        else { time = start; }
        sNote = CreateNote(time, line, id, 0, id + 1);

        if (sNote == null) { return; }

        // endNote
        if (snapToggle.isOn) { time = (int)Math.Round(end / value) * value; }
        else { time = end; }
        eNote = CreateNote(time, line, id + 1, id, 0);

        if (eNote == null) { RemoveNote(sNote.ID); Destroy(FindNoteObject(sNote.ID)); return; }

        if (eNote.Time <= audioSource.time) { audioSource.PlayOneShot(tapSound); }
    }

    public float LineToYPos(int line)
    {
        float contentY = contentRect.sizeDelta.y;
        return (contentY / (IndexToLine(LineDropdown.value) + 1)) * line * -1;
    }

    public float TimeToSnapXPos(float time)
    {
        float value = NoteInteval;
        return Mathf.Round(time / value) * value * ContentWidth;
    }

    public float XPosToSnapTime(float xpos)
    {
        float value = NoteInteval * ContentWidth;
        return Mathf.Round(xpos / value) * NoteInteval;
    }

    public float XPosToTime(float xpos)
    {
        return xpos / ContentWidth;
    }

    public float TimeToXPos(float time)
    {
        return time * ContentWidth;
    }

    //public float LineToYPos(int line)
    //{
    //    return time * ContentWidth;
    //}

    //public float YPosToLine(float ypos)
    //{
    //    return time * ContentWidth;
    //}

    int LineToIndex(int line)
    {
        switch (line)
        {
            case 1:
                return 0;
            case 2:
                return 1;
            case 4:
                return 2;
            case 5:
                return 3;
            case 6:
                return 4;
        }
        return -1;
    }

    public int IndexToLine(int index)
    {
        int[] arr = { 1, 2, 4, 5, 6 };
        return arr[index];
    }


    public void ReturnToMenu()
    {
        blackPanel.SetActive(true);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
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
}
