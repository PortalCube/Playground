using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

public class Note {
    public JObject json;

    public int ID { get; set; }
    public float Time { get; set; }
    public int Type { get; set; }
    public float StartLine { get; set; }
    public int EndLine { get; set; }
    public int Direction { get; set; }
    public bool Sync { get; set; }
    public int GroupID { get; set; }
    public int NextNoteID { get; set; }
    public int PrevNoteID { get; set; }

    public Note(JObject j) {
        json = j;
        InitializeObject();
    }

    public Note(int id, float time, int line, int prevID, int nextID)
    {
        json = new JObject();
        json.Add("id", id);
        json.Add("timing", time);
        json.Add("startPos", line);
        json.Add("endPos", line);
        json.Add("nextNoteId", nextID);
        json.Add("prevNoteId", prevID);
        InitializeObject();
    }

    private void InitializeObject()
    {
        ID = (int)SetValue("id", 0, typeof(int));
        Time = (float)SetValue("timing", 0.0f, typeof(float));
        Type = (int)SetValue("type", 1, typeof(int));
        StartLine = (float)SetValue("startPos", 1f, typeof(float));
        EndLine = (int)SetValue("endPos", 1f, typeof(int));
        Direction = (int)SetValue("status", 0, typeof(int));
        Sync = (bool)SetValue("sync", false, typeof(bool));
        GroupID = (int)SetValue("groupId", 0, typeof(int));
        NextNoteID = (int)SetValue("nextNoteId", 0, typeof(int));
        PrevNoteID = (int)SetValue("prevNoteId", 0, typeof(int));
    }


    private object SetValue(string name, object alt, Type type)
    {
        JToken token;
        if (json.TryGetValue(name, out token))
        {
            return token.ToObject(type);
        }
        else
        {
            Debug.LogWarningFormat(
                "Warning: NoteData에 필요한 값이 JSON에 포함되어 있지 않습니다." +
                "{0}// 누락 데이터: {1}, 대체 값: {2}",
                Environment.NewLine, name, alt);
            return alt;
        }
    }

    public JObject GetJObject()
    {
        JObject j = new JObject();

        if (NextNoteID != 0 || PrevNoteID != 0) { Type = 2; }

        j.Add("id", ID);
        j.Add("timing", Time);
        j.Add("type", Type);
        j.Add("startPos", StartLine);
        j.Add("endPos", Convert.ToDouble(EndLine));
        j.Add("status", Direction);
        j.Add("sync", Sync ? 1 : 0);
        j.Add("groupId", GroupID);
        j.Add("nextNoteId", NextNoteID);
        j.Add("prevNoteId", PrevNoteID);
        return j;
    }
}
