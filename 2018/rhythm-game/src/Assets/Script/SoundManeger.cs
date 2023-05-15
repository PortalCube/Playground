using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManeger : MonoBehaviour {

    public AudioSource musicSource;
    public AudioSource hitSource;
    public AudioSource flipSource;
    public AudioSource holdSource;
    public AudioSource missSource;
    public AudioSource tapSource;

    public static SoundManeger instance = null;

    private GameManeger gameManeger;
    private RhythmManager noteManager;

    void Awake() {
        if (instance == null) { instance = this; } else if (instance != this) { Destroy(gameObject); }
    }

    private void Start() {
        noteManager = RhythmManager.instance;
        gameManeger = GameManeger.instance;

        hitSource.clip = noteManager.hitClip;
        flipSource.clip = noteManager.flipClip;
        missSource.clip = noteManager.missClip;
        tapSource.clip = noteManager.tapClip;
        holdSource.clip = noteManager.holdClip;
    }

    public void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public float GetPlayTime() {
        return musicSource.time;
    }

    public void PlayHit(int clip) {
        if (clip == 0) { hitSource.Play(); } else { flipSource.Play(); }
    }

    public void PlayHold() {
        if (holdSource.isPlaying == false) { holdSource.Play(); }
    }

    public void StopHold() {
        holdSource.Stop();
    }

    public void PlayMiss() {
        missSource.Play();
    }

    public void PlayTap() {
        tapSource.Play();
    }

    private void Update() {
    }
}
