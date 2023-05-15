using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLineBehaviour : MonoBehaviour {

    RhythmManager manager;

    NoteData start, end;

    LineRenderer line;

    void Awake() {
        manager = RhythmManager.instance;
    }

    // Use this for initialization
    void Start () {
        line = GetComponent<LineRenderer>();

        // Transform 지정
        transform.localPosition = new Vector3(0, manager.pivotYPos, manager.pivotZPos);
        transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    void SetNoteData(NoteData data) {
        start = data;
        end = manager.GetNoteByID(data.ID + 1);
    }
	
	// Update is called once per frame
	void Update () {
        if (start.Status == 2 || end.Status == 2) { Destroy(gameObject); return; }
        line.SetPosition(0, start.FieldObject.transform.position);
        line.SetPosition(1, end.FieldObject.transform.position);
        transform.localEulerAngles = new Vector3(start.GetAngle(), 0, 0);
    }
}
