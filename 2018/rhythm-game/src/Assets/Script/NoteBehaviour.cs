using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NoteBehaviour : MonoBehaviour {

    private RhythmManager m;
    public NoteData Note { set; get; } = null;

    private Vector3[] vectorAngle = new Vector3[4];
    private Vector3[] vectorScale = new Vector3[2];
    private Vector3[] vectorPosition = new Vector3[2];

    void SetNoteData(NoteData data) {
        Note = data;
    }

    private void Start() {
        // 기본 변수들 불러오기
        m = RhythmManager.instance;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        // 스프라이트 설정
        Sprite[] spr = {
            m.tapNote,
            m.flickLeftNote,
            m.flickRightNote,
            m.flickUpNote};

        if (Note.Type == 2) { renderer.sprite = m.longNote; }
        renderer.sprite = spr[Note.Direction];

        // Vector3 초기 값 - 종료 값 설정
        vectorPosition[0] = new Vector3(m.GetInputPos(Note.StartLine), 0, m.pivotGap * -1);
        vectorPosition[1] = new Vector3(m.GetInputPos(Note.EndLine), 0, m.pivotGap * -1);

        vectorScale[0] = new Vector3(m.noteStartScale, m.noteStartScale, 1);
        vectorScale[1] = new Vector3(m.noteScale, m.noteScale, vectorScale[0].z);

        vectorAngle[0] = new Vector3(m.noteStartAngle * -1, 0, 0);
        vectorAngle[1] = new Vector3((m.noteStartAngle - m.notePlayAngle) * -1, 0, 0);
        vectorAngle[2] = new Vector3(m.noteStartAngle, 0, 0);
        vectorAngle[3] = new Vector3(m.noteStartAngle - m.notePlayAngle, 0, 0);
    }

    private void Update() {
        Note.UpdateTime(Time.deltaTime);

        // 노트 하강
        transform.localEulerAngles = Vector3.LerpUnclamped(vectorAngle[0], vectorAngle[1], Note.Percent);
        Note.ParentObject.transform.localEulerAngles = Vector3.LerpUnclamped(vectorAngle[2], vectorAngle[3], Note.Percent);

        // 노트 X축 이동
        transform.localPosition = Vector3.LerpUnclamped(vectorPosition[0], vectorPosition[1], Note.Percent);

        // 노트 스케일 조정
        transform.localScale = Vector3.Lerp(vectorScale[0], vectorScale[1], Note.Percent);
    }
}
