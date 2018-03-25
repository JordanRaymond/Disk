using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Song", menuName = "Song/Song", order = 1)]
public class Song : ScriptableObject
{
    [SerializeField] private string songDisplayName;
    [SerializeField] private float bmp;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float duration;
    [SerializeField] private float startTime = 0;

    public float Bpm {
        get {
            return bmp;
        }
    }

    public float SongDuration {
        get {
            return audioClip.length;
        }
    }

    public float StartTime {
        get {
            return startTime;
        }
    }

    public AudioClip AudioClip {
        get {
            return audioClip;
        }
    }

    private void OnValidate() {
        if (startTime < 0) {
            startTime = 0;
        }
        if (bmp <= 0) {
            bmp = 1;
        }
        if (!audioClip) {
            Debug.LogError("A song need an audioClip!");
        }
        else {
            duration = audioClip.length;
        }
    }
}
