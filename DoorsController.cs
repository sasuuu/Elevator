using UnityEngine;

public class DoorsController : MonoBehaviour
{

    Animation anim;
    AnimationState animState;
    int objectsInTrigger = 0;
    bool isObjectInTrigger = false;
    float animProgress = 0f;
    bool isClosed = true;
    
    
    void Start()
    {
        anim = GetComponent<Animation>();
        animState = anim["Open"];
    }

    void OnTriggerEnter(Collider collider)
    {
        objectsInTrigger++;
    }

    void OnTriggerStay(Collider collider)
    {
        if (anim.isPlaying && animState.speed == -1)
        {
            animProgress = animState.time;
            anim.Stop();
            Invoke("ResumeAnimationReverse", 2f);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        objectsInTrigger--;
    }

    void Update()
    {
        if (objectsInTrigger > 0) isObjectInTrigger = true;
        else isObjectInTrigger = false;
    }

    public bool IsObjectInTrigger()
    {
        return isObjectInTrigger;
    }

    public bool IsClosed()
    {
        return isClosed;
    }

    void DoorsOpened()
    {
        isClosed = false;
    }

    void DoorsClosed()
    {
        isClosed = true;
    }

    public void OpenDoors()
    {
        animState.time = 0;
        animState.speed = 1;
        anim.Play(animState.name);
    }

    public void CloseDoors()
    {
        animState.time = animState.length;
        animState.speed = -1;
        anim.Play(animState.name);
    }

    void ResumeAnimationReverse()
    {
        animState.speed = -animState.speed;
        animState.time = animProgress;
        anim.Play();
        Invoke("CloseDoors", 3f);
    }
}
