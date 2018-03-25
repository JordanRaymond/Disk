using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPunch : MonoBehaviour {

    public Vector3 direction = Vector3.right;
    public float startPosition;
    public float targetPosition;
    public float mouveOutSpeed = 3;
    public float moveInSpeed = 1;

    public WallState wallState;


    public AudioClip sound;
    public bool soundWassPlayed = false;

    private AudioSource audioSource;
    private Rigidbody rigid;

    void Start () {
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody>();

        wallState = WallState.moveOut;
        startPosition = transform.position.x;
	}
	
	void Update () {

        switch (wallState)
        {
            case WallState.moveIn:
                if (soundWassPlayed) soundWassPlayed = false;

                if (transform.position.x > startPosition)
                {
                    transform.Translate(-direction * Time.deltaTime * moveInSpeed);
                }
                else
                {
                    wallState = WallState.moveOut;
                }
                break;
            case WallState.moveOut:
                if (! soundWassPlayed)
                {
                    soundWassPlayed = true;
                    audioSource.PlayOneShot(sound);
                }
                //if (transform.position.x < targetPosition)
                //{
                //    transform.Translate(Vector3.right * Time.deltaTime * mouveOutSpeed);
                //}
                //else
                //{
                //    wallState = WallState.moveIn;
                //}
                break;
            case WallState.stop:
                break;
            default:
                break;
        }
        
	}

    private void FixedUpdate()
    {
        switch (wallState)
        {
            case WallState.moveIn:
            
                break;
            case WallState.moveOut:

                if (transform.position.x < targetPosition)
                {
                    rigid.AddForce(direction * mouveOutSpeed, ForceMode.Impulse);
                }
                else
                {
                    rigid.velocity = Vector3.zero;
                    wallState = WallState.moveIn;
                }
                break;
            case WallState.stop:
                break;
            default:
                break;
        }
    }


    public enum WallState
    {
        moveIn, moveOut, stop
    }
}
