using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class InputBehaviour : MonoBehaviour {

    public enum ClickMode : short
    {
        Down = 1,
        Up = 2,
        Flick = 3,
        Auto = -1,
    };

    public string inputKey;
    public float[] accuracyRange;
    public int ID { get; set; } = 0;
    private int nextNote = -1;
    private RhythmManager manager;

    public List<int> FieldNoteList = new List<int>();

    public void SetID(int id) {
        ID = id;
    }

    public void SetKey(string key)
    {
        inputKey = key;
    }

    public void AddNoteID(int id) {
        FieldNoteList.Add(id);
    }

    private void Start() {
        manager = RhythmManager.instance;
        accuracyRange = manager.accuracyRange;
        GetComponent<BoxCollider2D>().size = new Vector2(manager.inputGap[manager.GetKeyIndex()], 6);
    }

    private void Update() {
        if (manager.isGameEnded) { return; }
        ClickMode keyResult = CheckKeyInput();
        if (keyResult != ClickMode.Auto) { NoteCheck(keyResult); }
        float timing = GetTime();
        if (timing == -1) { return; } else if (timing >= 0 && manager.autoPlayMode) { NotePressed(0); } //AutoPlay 스크립트
                                      else if (timing >= 0.2) { NotePressed(5); }
    }

    private void OnMouseDown() {
        if (manager.autoPlayMode) { return; }
        NoteCheck(ClickMode.Down);
    }

    private void OnMouseUp()
    {
        if (manager.autoPlayMode) { return; }
        NoteCheck(ClickMode.Up);
    }

    private ClickMode CheckKeyInput()
    {
        if (manager.autoPlayMode) { return ClickMode.Auto; }
        else if (Input.GetKeyDown(inputKey)) { return ClickMode.Down; }
        else if (Input.GetKeyUp(inputKey)) { return ClickMode.Up; }
        return ClickMode.Auto;
    }

    private void NoteCheck(ClickMode mode) {
        float timing = GetTime();
        if (timing == -1)
        {
            if (mode == ClickMode.Up)
            {
                if (nextNote != -1) { AddNoteID(nextNote); timing = manager.GetNoteByID(nextNote).Time; }
                else { return; }
            }
            else { TapEffect(); return; }
        }
        if (nextNote == -1 && mode == ClickMode.Up) { return; }

        // 0: Perfect
        // 1: Great
        // 2: Good
        // 3: Fast
        // 4: Slow
        // 5: Miss

        for (int i = 0; i < accuracyRange.Length; i++) {
            if (Math.Abs(timing) <= accuracyRange[i]) {
                if (i == 3) {
                    if (timing >= accuracyRange[i] * -1 && timing < accuracyRange[i - 1] * -1) { NotePressed(3); } // Fast
                    else if (timing <= accuracyRange[i] && timing > accuracyRange[i - 1]) { NotePressed(4); } // Slow
                } else if (i == 4) {
                    NotePressed(5);
                } else { NotePressed(i); }
                return;
            }
        }
        TapEffect();
    }

    private void TapEffect() {
        SoundManeger.instance.PlayTap();
    }

    void NotePressed(int accuracy) {
        NoteData note = manager.GetNoteByID(FieldNoteList[0]);

        if (note.NextNoteID == 0) { nextNote = -1; }
        else { nextNote = note.NextNoteID; }

        // Score 기능
        if (GameManeger.instance.Gamemode == 2)
        {
            float score = manager.accuracyScore[accuracy];

            if (accuracy >= 3) { manager.startChain = false; }
            if (manager.startChain) { score *= manager.startChainBonus; }

            manager.UpdateScore((int)Mathf.Round(score));
        }

        manager.pressedNoteCount++;

        manager.UpdateNotification(accuracy);

        FieldNoteList.RemoveAt(0);

        note.Pressed(accuracy);

        // Life 기능
        if (GameManeger.instance.Gamemode == 2)
        {
            int damage = 0;
            if (accuracy == 3 || accuracy == 4) { damage = 6; }
            if (accuracy == 5) { damage = 10; }
            manager.UpdateLife(damage);
        }
    }

    private float GetTime() {
        if (FieldNoteList.Count == 0) { return -1; }

        return SoundManeger.instance.GetPlayTime() - manager.GetNoteByID(FieldNoteList[0]).Time;
    }
}
