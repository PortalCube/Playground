using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditNoteBehaviour : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler {

    public Note note;
    bool isMoved = false;
    float draggingTime = 0;

    EditorManager manager = EditorManager.instance;

    // Use this for initialization
    void Start () {
		
	}
	
    void SetNote(Note note)
    {
        this.note = note;
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData EventData)
    {
        if (draggingTime >= 0.04f || isMoved) { draggingTime = 0; isMoved = false; return; }
        Debug.Log("Removing... ID: " + note.ID);
        draggingTime = 0;
        manager.RemoveNote(note.ID);
        Destroy(gameObject);
    }

    public void OnDrag(PointerEventData EventData)
    {
        draggingTime += Time.unscaledDeltaTime;

        Vector3 original = transform.localPosition;

        Vector3 position = GetMousePosition();
        float time;
        int line;
        float lineHeight;

        time = manager.XPosToSnapTime(position.x);
        lineHeight = manager.contentRect.sizeDelta.y / manager.IndexToLine(manager.LineDropdown.value);
        line = (int)Mathf.Round((position.y / lineHeight) + 0.5f) * -1 + 1;
        Debug.Log(time);
        transform.localPosition = new Vector3(manager.TimeToXPos(time), manager.LineToYPos(line), 0);
        if (transform.localPosition != original) { isMoved = true; }
    }

    public void OnEndDrag(PointerEventData EventData)
    {
        Vector3 position = GetMousePosition();
        float time;
        int line;
        float lineHeight;

        time = manager.XPosToSnapTime(position.x);
        lineHeight = manager.contentRect.sizeDelta.y / manager.IndexToLine(manager.LineDropdown.value);
        line = (int)Mathf.Round((position.y / lineHeight) + 0.5f) * -1 + 1;
        manager.MoveNote(note.ID, time, line);
    }

    Vector3 GetMousePosition()
    {
        Vector3 mouse;
        Vector3 original = transform.position;
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse = transform.localPosition;
        transform.position = original;
        return mouse;
    }
}
