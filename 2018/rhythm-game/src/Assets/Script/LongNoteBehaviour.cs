using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteBehaviour : MonoBehaviour {

    RhythmManager manager;

    LineRenderer line;
    NoteData startBottomNote;
    NoteData endUpNote;

    public float elapsedTime;

    public float startX;
    public float endX;

    public float[] angle;
    // 0: Start
    // 1: End

    public float[] time;
    // 0: A Section: StartNote O / EndNote X
    // 1: B Section: StartNote O or X / EndNote O or X
    // 2: C Section: StartNote X / EndNote O

    public float timeDiff;

    private void Awake() {
        manager = RhythmManager.instance;

        // Transform 지정
        transform.localPosition = new Vector3(0, manager.pivotYPos, manager.pivotZPos);
        transform.localEulerAngles = new Vector3(90, 0, 0);

        // LineRenderer 설정
        line = gameObject.GetComponent<LineRenderer>();

        time = new float[2] { 0, 0 };
    }

    float GetTiming() {
        return endUpNote.Time - startBottomNote.Time;
    }

    void SetNoteData(NoteData data) {
        // 시작, 끝 노트를 불러옴
        startBottomNote = data;
        endUpNote = manager.GetNoteByID(data.NextNoteID);
        endUpNote.SetTrail(gameObject); // 끝 노트에 현재 트레일 등록
    }

    void Update() {
        List<Vector3> linePoints;
        startX = manager.GetInputPos(startBottomNote.EndLine);
        endX = manager.GetInputPos(endUpNote.EndLine);
        
        angle = new float[] { startBottomNote.GetAngle(), endUpNote.GetAngle() };

        timeDiff = endUpNote.Time - startBottomNote.Time;
        elapsedTime += Time.deltaTime;

        if (!endUpNote.IsNoteSpawned() && time[1] < timeDiff) {
            if (endUpNote.EndLine != endUpNote.StartLine) { endX = endUpNote.GetXPos(); }
            else { endX = Mathf.Lerp(startBottomNote.GetXPos(), endUpNote.GetXPos(), time[1] / timeDiff); }
            time[1] += Time.deltaTime;
        } else {
            endX = endUpNote.GetXPos();
        }

        if (!startBottomNote.IsNoteSpawned() && time[0] < timeDiff) {
            if (startBottomNote.EndLine != startBottomNote.StartLine) { startX = startBottomNote.GetXPos(); }
            else { startX = Mathf.Lerp(startBottomNote.GetXPos(), endUpNote.GetXPos(), time[0] / timeDiff); }
            time[0] += Time.deltaTime;
        } else {
            startX = startBottomNote.GetXPos();
        }

        linePoints = CreateCircleLine(angle[0], angle[1], startX, endX, manager.pivotGap);
        line.positionCount = linePoints.Count;
        line.SetPositions(linePoints.ToArray());

        if (startX != endX) { line.alignment = LineAlignment.View; }
    }

    List<Vector3> CreateCircleLine(float startAngle, float endAngle, float startX, float endX, float radius) {
        List<Vector3> linePoints = new List<Vector3>();
        float x, y, z;

        int _startAngle = Mathf.RoundToInt(startAngle);
        int _endAngle = Mathf.RoundToInt(endAngle);

        float segment = _endAngle - _startAngle;
        int count = 0;

        for (int i = _startAngle; i < _endAngle; i++) {
            count++;
            x = Mathf.Lerp(startX, endX, count / segment);
            y = Mathf.Cos(Mathf.Deg2Rad * i) * radius * -1;
            z = Mathf.Sin(Mathf.Deg2Rad * i) * radius * -1;
            linePoints.Add(new Vector3(x, y, z));
        }
        return linePoints;
    }

}
