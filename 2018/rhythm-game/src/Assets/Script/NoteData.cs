using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

public class NoteData {
    private RhythmManager manager;

    private JObject jsonData;

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

    public float Percent { get; set; }
    public byte Status { get; set; } = 0;

    private float elapsedTime = 0f;

    public GameObject FieldObject { get; set; } = null;
    public GameObject ParentObject { get; set; } = null;

    public GameObject[] Trail { get; set; } = { null, null };

    public NoteData(JObject obj) {
        manager = RhythmManager.instance;
        jsonData = obj;
        InitializeObject();
    }

    private void InitializeObject() {
        ID = (int)SetValue("id", 0, typeof(int));
        Time = (float)SetValue("timing", 0.0f, typeof(float));
        Type = (int)SetValue("type", 0, typeof(int));
        StartLine = (float)SetValue("startPos", 1f, typeof(float));
        EndLine = (int)SetValue("endPos", 1f, typeof(int));
        Direction = (int)SetValue("status", 0, typeof(int));
        Sync = (bool)SetValue("sync", false, typeof(bool));
        GroupID = (int)SetValue("groupId", 0, typeof(int));
        NextNoteID = (int)SetValue("nextNoteId", 0, typeof(int));
        PrevNoteID = (int)SetValue("prevNoteId", 0, typeof(int));
        if (manager.noteFixedStartPos) { StartLine = EndLine; }
    }

    private object SetValue(string name, object alt, Type type) {
        JToken token;
        if (jsonData.TryGetValue(name, out token)) {
            return token.ToObject(type);
        } else {
            Debug.LogWarningFormat(
                "Warning: NoteData에 필요한 값이 JSON에 포함되어 있지 않습니다." +
                "{0}// 누락 데이터: {1}, 대체 값: {2}",
                Environment.NewLine, name, alt);
            return alt;
        }
    }

    public void CreateFieldObject() {

        Status = 1;

        // 부모 오브젝트 생성
        ParentObject = new GameObject(String.Format("Note[{0}]", ID));
        ParentObject.transform.localPosition = new Vector3(0, manager.pivotYPos, manager.pivotZPos);
        ParentObject.transform.localEulerAngles = new Vector3(manager.noteStartAngle, 0, 0);

        // 메인 오브젝트 생성
        FieldObject = UnityEngine.Object.Instantiate(manager.note, ParentObject.transform);
        FieldObject.name = String.Format("NoteObject[{0}]", ID);

        // 메인 오브젝트 Transform
        FieldObject.transform.localPosition = new Vector3(manager.GetInputPos(StartLine), 0, manager.pivotGap * -1);
        FieldObject.transform.localScale = new Vector3(manager.noteStartScale, manager.noteStartScale, 1);
        FieldObject.transform.localEulerAngles = new Vector3(manager.noteStartAngle * -1, 0, 0);

        // 메인 오브젝트 behaviour에 데이터 전달
        FieldObject.SendMessage("SetNoteData", this, SendMessageOptions.RequireReceiver);

        manager.inputList[EndLine - 1].SendMessage("AddNoteID", ID, SendMessageOptions.RequireReceiver);
        //Debug.LogFormat("노트 생성 // ID:{0}, Time:{1}", ID, SoundManeger.instance.GetPlayTime());
        if (Type == 2 && NextNoteID != 0) { CreateTrail(); }
    }

    private void CreateTrail() {
        Trail[0] = UnityEngine.Object.Instantiate(manager.noteLine);
        Trail[0].SendMessage("SetNoteData", this, SendMessageOptions.RequireReceiver);
    }

    public void SetTrail(GameObject obj) {
        Trail[1] = obj;
    }

    public void Pressed(int accuracy) {
        //Debug.LogFormat("노트 삭제 // ID:{0}, Time:{1}", ID, SoundManeger.instance.GetPlayTime());
        Status = 2;

        if (accuracy < 3) {
            manager.comboCount += 1;
            SoundManeger.instance.PlayHit(Direction);
        } else { manager.comboCount = 0; SoundManeger.instance.PlayMiss(); }

        UnityEngine.Object.Destroy(ParentObject, 0f);
        UnityEngine.Object.Destroy(FieldObject, 0f);

        if (Trail[1] != null) { UnityEngine.Object.Destroy(Trail[1], 0f); }
    }

    public void UpdateTime(float elapsed) {
        elapsedTime += elapsed;
        Percent = elapsedTime / manager.noteFieldTime;
    }

    public float GetXPos() {
        if (Status == 0) {
            return manager.GetInputPos(StartLine);
        } else if (Status == 1) {
            return FieldObject.transform.localPosition.x;
        } else if (Status == 2) {
            return manager.GetInputPos(EndLine);
        }
        Debug.LogErrorFormat("Error: 유효하지 않은 Status 값 입니다. Status: {0}", Status);
        return -1;
    }

    public float GetAngle() {
        if (Status == 0) {
            return manager.noteStartAngle;
        } else if (Status == 1) {
            return ParentObject.transform.localEulerAngles.x;
        } else if (Status == 2) {
            return manager.noteStartAngle - manager.notePlayAngle;
        }
        Debug.LogErrorFormat("Error: 유효하지 않은 Status 값 입니다. Status: {0}", Status);
        return -1;
    }

    public bool IsNoteSpawned() {
        if (Status == 1) { return true; } else { return false; }
    }
}
