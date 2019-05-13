using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActions : MonoBehaviour
{

    public Text text;
    public Canvas info;

    new GameObject camera;
    float playerRange = 1f;

    void Start()
    {
        camera = GetComponentInChildren<Camera>().gameObject;
    }

    void Update()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hitInfo;
        int layerMask = 1 << gameObject.layer;
        layerMask = ~layerMask;
        if (Physics.Raycast(ray, out hitInfo, playerRange, layerMask))
        {
            collideHandler(hitInfo);
        }
        else
        {
            info.enabled = false;
        }
    }

    void collideHandler(RaycastHit hitInfo)
    {
        if (hitInfo.collider.tag == "LevelElevatorButton")
        {
            text.text = "Click left mouse button";
            info.enabled = true;
            if (Input.GetMouseButtonDown(0))
            {
                hitInfo.collider.GetComponent<LevelButtonHandler>().RequestElevator();
            }
        }
        else if(hitInfo.collider.tag == "ElevatorButton")
        {
            ElevatorButtonHandler elevatorButton = hitInfo.collider.GetComponent<ElevatorButtonHandler>();
            TextMeshPro elevatorButtonText = hitInfo.collider.GetComponentInChildren<TextMeshPro>();
            text.text = "Click left mouse button(level " + elevatorButtonText.text + ")";
            info.enabled = true;
            if (Input.GetMouseButtonDown(0))
            {
                elevatorButtonText.color = Color.green;
                elevatorButton.ChooseLevel();
            }
        }
        else
        {
            info.enabled = false;
        }
    }
}
