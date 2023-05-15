using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VLineBehaviour : MonoBehaviour, IPointerClickHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData EventData)
    {
        EditorManager manager = EditorManager.instance;
        Vector3 position = GetMousePosition();
        float time;
        int line;
        float lineHeight;

        time = transform.localPosition.x / manager.ContentWidth;
        lineHeight = manager.contentRect.sizeDelta.y / manager.IndexToLine(manager.LineDropdown.value);
        line = (int)Mathf.Round((position.y / lineHeight) + 0.5f) * -1 + 1;

        manager.CreateNote(time, line);
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
