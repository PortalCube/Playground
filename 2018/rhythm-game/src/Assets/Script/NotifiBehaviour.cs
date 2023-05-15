using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifiBehaviour : MonoBehaviour {

    public float popupTime = 0.2f; // popupTime초 동안 판정기가 S1에서 S2으로 크기가 변화
    public float waitTime = 0.8f; // waitTime초 동안 크기, 투명도 변화 없이 그대로 띄우기
    public float fadeTime = 0.5f; // fadeTime초 동안 판정기가 1.0에서 0.0으로 투명도가 변화
    public float _popupTime;
    public float _waitTime;
    public float _fadeTime;

    public Vector3 startScale;
    private Vector3 endScale;

    private SpriteRenderer sprRender;

    // Use this for initialization
    void Start () {
        sprRender = gameObject.GetComponent<SpriteRenderer>();
        endScale = transform.localScale;
    }

    public void Notification(Sprite spr) {
        // 노트 판정 표시기를 업데이트하는 함수
        sprRender.sprite = spr;
        sprRender.color = new Color(1, 1, 1, 1);
        transform.localScale = startScale;
        _popupTime = popupTime;
        _waitTime = waitTime;
        _fadeTime = fadeTime;
    }

	void Update () {
        //x초 동안 판정기가 S1에서 S2으로 크기가 변화
        //y초 동안 유지
        //z초 동안 판정기가 1.0에서 0.0으로 투명도가 변화

        if (_popupTime > 0f)
        {
            transform.localScale = Vector3.Slerp(startScale, endScale, (popupTime - _popupTime) / popupTime);
            _popupTime -= Time.deltaTime;
        } else if (_popupTime != 0)
        {
            _popupTime = 0;
            transform.localScale = endScale;
        } else if (_waitTime > 0f)
        {
            _waitTime -= Time.deltaTime;
        } else if (_fadeTime > 0f)
        {
            sprRender.color = new Color(1, 1, 1, _fadeTime / fadeTime);
            _fadeTime -= Time.deltaTime;
        } else if (_fadeTime != 0)
        {
            _fadeTime = 0;
            sprRender.color = new Color(1, 1, 1, 0);
        }
    }
}
