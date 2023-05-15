using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


using Newtonsoft.Json.Linq;

public class PlayerData
{

    public string Name { get; set; }
    public string Nickname { get; set; }
    public string PhoneNumber { get; set; }
    public int Score { get; set; }

    public PlayerData(string name, string nick, string phone)
    {
        Name = name;
        Nickname = nick;
        PhoneNumber = phone;
    }

}

public class GameManeger : MonoBehaviour {

    public static GameManeger instance = null;

    public int Gamemode { get; set; } = 1;
    // 1: Standard
    // 2: Challange

    public string[] songExtension = { ".wav", ".ogg" };
    public string[] imageExtension = { ".png", ".jpg", ".jpeg" };

    public List<PlayerData> playerDatas = new List<PlayerData>();
    public string currentPlayer;

    public List<string> playlist;
    public Dictionary<string, FileInfo> beatmapFilesInfo;
    public Dictionary<string, FileInfo> musicFilesInfo;
    public Dictionary<string, FileInfo> coverFilesInfo;

    public TextAsset beatmapFile;
    public AudioClip musicFile;
    public Sprite coverFile;

    public bool autoMode = false;
    public string selectedFile;

    // 키 세팅
    public string[,] inputKey = {
                { "d", "", "", "", "", "" }, //1KEY
                { "d", "k", "", "", "", "" }, //2KEY
                { "d", "f", "j", "k", "", "" }, //4KEY
                { "f", "g", "h", "j", "k", "" }, //5KEY
                { "d", "f", "g", "h", "j", "k" } //6KEY
    };

    // 향상된 키 세팅
    private KeyCode[] _inputKey = { KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K };
    public KeyCode[] InputKey {
        get {
            return _inputKey;
        } set {
            _inputKey = value;
        }
    }

    // Use this for initialization
    void Awake() {
        if (instance == null) { instance = this; } else if (instance != this) { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        LoadData();
        LoadPlaylist();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SortData()
    {
        playerDatas.Sort(delegate (PlayerData A, PlayerData B) {
            if (A.Score < B.Score) { return 1; } else if (A.Score > B.Score) { return -1; }
            return 0;
        });
    }

    public void UpdateScore(string key, int score)
    {
        PlayerData data = playerDatas.Find(d => d.Name == key);
        if (data.Score < score) {
            data.Score = score;
            SortData();
        }
    }

    void LoadData()
    {
        if (System.IO.File.Exists(System.IO.Directory.GetParent(Application.dataPath).FullName + "/PlayerData.json"))
        {
            playerDatas = JArray.Parse(System.IO.File.ReadAllText(System.IO.Directory.GetParent(Application.dataPath).FullName + "/PlayerData.json")).ToObject<List<PlayerData>>();
            SortData();
        }
    }

    public bool isFirstRank()
    {
        if (playerDatas[0].Name == currentPlayer) { return true; }
        return false;
    }

    public void AddData(string name, string nick, string phone)
    {
        currentPlayer = name;
        PlayerData data = playerDatas.Find(d => d.Name == name);
        if (data != null) { return; }
        playerDatas.Add(new PlayerData(name, nick, phone));
        SaveData();
    }

    public void SaveData()
    {
        JArray j = JArray.FromObject(playerDatas);
        System.IO.File.WriteAllText(System.IO.Directory.GetParent(Application.dataPath).FullName + "/PlayerData.json", j.ToString());
    }

    public void LoadPlaylist()
    {
        FileInfo m_info = null;
        FileInfo[] m_files;

        DirectoryInfo directory = new DirectoryInfo(System.IO.Directory.GetParent(Application.dataPath).FullName + "/Songs");
        if (!directory.Exists) { directory.Create(); return; }

        DirectoryInfo[] directories = directory.GetDirectories();

        playlist = new List<string>();
        beatmapFilesInfo = new Dictionary<string, FileInfo>();
        musicFilesInfo = new Dictionary<string, FileInfo>();
        coverFilesInfo = new Dictionary<string, FileInfo>();

        foreach (DirectoryInfo dir in directories)
        {
            m_info = null;

            // Beatmap 찾기
            m_files = dir.GetFiles(dir.Name + ".json", SearchOption.TopDirectoryOnly);
            if (m_files.Length == 0) { continue; }
            playlist.Add(dir.Name);
            beatmapFilesInfo.Add(dir.Name, m_files[0]);
            Debug.Log("Add: " + dir.Name);
            m_info = null;

            
            foreach (string ext in songExtension)
            {
                m_files = dir.GetFiles(dir.Name + ext, SearchOption.TopDirectoryOnly);
                if (m_files.Length == 0) { continue; }
                m_info = m_files[0];
                break;
            }

            if (m_info != null) { musicFilesInfo.Add(dir.Name, m_info); Debug.Log("Music Add: " + dir.Name); }

            m_info = null;

            
            foreach (string ext in imageExtension)
            {
                m_files = dir.GetFiles(dir.Name + ext, SearchOption.TopDirectoryOnly);
                if (m_files.Length == 0) { continue; }
                m_info = m_files[0];
                break;
            }

            if (m_info != null) { coverFilesInfo.Add(dir.Name, m_info); Debug.Log("Image Add: " + dir.Name); }
        }
        foreach (string ext in songExtension)
        {
            foreach (FileInfo info in directory.GetFiles("*" + ext))
            { musicFilesInfo.Add(Path.GetFileNameWithoutExtension(info.Name), info); }
        }
    }

    public void LoadData(int song, int mode = 0)
    {
        LoadData(playlist[song], mode);
    }

    public void LoadData(string name, int mode = 0)
    {
        Debug.Log("Loading Data... " + name);

        if (mode == 0 || mode == 1) { beatmapFile = new TextAsset(beatmapFilesInfo[name].OpenText().ReadToEnd()); }
        if ((mode == 0 || mode == 2) && musicFilesInfo.ContainsKey(name))
        {
            musicFile = null;
            StartCoroutine(WWWLoad_Audio("file:///" + musicFilesInfo[name].FullName));

        }
        if ((mode == 0 || mode == 3) && coverFilesInfo.ContainsKey(name))
        {
            coverFile = null;
            StartCoroutine(WWWLoad_Audio("file:///" + coverFilesInfo[name].FullName));
        }
    }

    IEnumerator WWWLoad_Audio(string link)
    {
        WWW www = new WWW(link);
        yield return www;
        musicFile = www.GetAudioClip();
    }

    IEnumerator WWWLoad_Sprite(string link)
    {
        WWW www = new WWW(link);
        yield return www;
        coverFile = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
    }


}
