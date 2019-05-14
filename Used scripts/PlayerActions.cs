using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActions : MonoBehaviour
{

    public Text text;
    public Canvas info;
    new GameObject camera;
    float playerRange = 1f;
    int LEFT_MOUSE_BUTTON = 0;

    void Start()
    {
        camera = GetComponentInChildren<Camera>().gameObject;
    }

    void Update()
    {
        RaycastCheck();
    }

    void RaycastCheck(){
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hitInfo;
        int layerMask = 1 << gameObject.layer;
        layerMask = ~layerMask;
        if (Physics.Raycast(ray, out hitInfo, playerRange, layerMask))
        {
            CollideHandler(hitInfo);
        }
        else
        {
            HideInfo();
        }
    }

    void HideInfo()
    {
        info.enabled = false;
    }

    void CollideHandler(RaycastHit hitInfo)
    {
        if (hitInfo.collider.tag == "LevelElevatorButton")
        {
            LevelElevatorButtonHandler(hitInfo);
        }
        else if(hitInfo.collider.tag == "ElevatorButton")
        {
            ElevatorButtonHandler(hitInfo);
        }
        else
        {
            HideInfo();
        }
    }

    void ShowInfo()
    {
        info.enabled = true;
    }

    void ElevatorButtonHandler(RaycastHit hitInfo)
    {
        ElevatorButtonHandler elevatorButton = hitInfo.collider.GetComponent<ElevatorButtonHandler>();
        TextMeshPro elevatorButtonText = hitInfo.collider.GetComponentInChildren<TextMeshPro>();
        text.text = "Click left mouse button(level " + elevatorButtonText.text + ")";
        ShowInfo();
        if (Input.GetMouseButtonDown(0))
        {
            ChooseLevel(elevatorButton, elevatorButtonText);
        }
    }

    void ChooseLevel(ElevatorButtonHandler elevatorButton, TextMeshPro elevatorButtonText)
    {
        elevatorButtonText.color = Color.green;
        elevatorButton.ChooseLevel();
    }

    void LevelElevatorButtonHandler(RaycastHit hitInfo)
    {
        text.text = "Click left mouse button";
        ShowInfo();
        if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
        {
            RequestLevel(hitInfo);
        }
    }

    void RequestLevel(RaycastHit hitInfo)
    {
        hitInfo.collider.GetComponent<LevelButtonHandler>().RequestElevator();
    }
}
