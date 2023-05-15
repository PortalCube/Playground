using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class Beatmap {

    // 노래 채보가 저장될 클래스
    // 하는 일: 채보의 정보를 저장하고, 노트 데이터를 저장


    #region Property

    private string _name = "제목 없는 노래";
    public string Name {
        get {
            return _name;
        } set {
            _name = value;
        }
    }

    private string _filename = "default";
    public string FileName {
        get {
            return _filename;
        }
        set {
            _filename = value;
        }
    }

    private string _artist = "Various Artist";
    public string Artist {
        get {
            return _artist;
        }
        set {
            _artist = value;
        }
    }

    private int _id = 100000;
    public int ID {
        get {
            return _id;
        }
        set {
            _id = value;
        }
    }

    private int _difficultyType = 0; // 0: Easy 1: Normal 2: Pro 3: Master
    public int Difficulty {
        get {
            return _difficultyType;
        }
        set {
            _difficultyType = value;
        }
    }

    private int _difficultyNumber = 1;
    public int DifficultyValue {
        get {
            return _difficultyNumber;
        }
        set {
            _difficultyNumber = value;
        }
    }

    private int _inputLine = 1;
    public int Line {
        get {
            return _inputLine;
        }
        set {
            _inputLine = value;
        }
    }

    private string _genre = "노래";
    public string Genre {
        get {
            return _genre;
        }
        set {
            _genre = value;
        }
    }

    private int _bpm = 0;
    public int BPM {
        get {
            return _bpm;
        }
        set {
            _bpm = value;
        }
    }

    #endregion

    public JArray notes;

    public Beatmap(string json) {

        JObject j;
        JObject metadata;
        JToken value; // dummy var

        if (json != "")
        {
            j = JObject.Parse(json);
            metadata = (JObject)j["metadata"];
            notes = (JArray)j["notes"];
        } else
        {
            metadata = new JObject();
            notes = new JArray();
        }

        if (metadata.TryGetValue("songName", out value)) { Name = (string)metadata.GetValue("songName"); }
        if (metadata.TryGetValue("songFile", out value)) { FileName = (string)metadata.GetValue("songFile"); }
        if (metadata.TryGetValue("artist", out value)) { Artist = (string)metadata.GetValue("artist"); }
        if (metadata.TryGetValue("id", out value)) { ID = (int)metadata.GetValue("id"); }
        if (metadata.TryGetValue("difficultyType", out value)) { Difficulty = (int)metadata.GetValue("difficultyType"); }
        if (metadata.TryGetValue("difficultyNumber", out value)) { DifficultyValue = (int)metadata.GetValue("difficultyNumber"); }
        if (metadata.TryGetValue("inputLine", out value)) { Line = (int)metadata.GetValue("inputLine"); }
        if (metadata.TryGetValue("genre", out value)) { Genre = (string)metadata.GetValue("genre"); }
        if (metadata.TryGetValue("bpm", out value)) { BPM = (int)metadata.GetValue("bpm"); }
    }
}