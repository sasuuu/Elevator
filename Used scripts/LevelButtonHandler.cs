using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LevelButtonHandler : MonoBehaviour
{

    public string direction = "Up";
    public ElevatorController elevatorController;
    GameObject level;
    TextMeshPro text;
    
    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        GameObject[] objects = GetParentObjects();
        foreach (GameObject obj in objects)
        {
            if (obj.tag == "Level")
            {
                SetLevel(obj);
            }
        }
    }

    void SetLevel(GameObject level)
    {
        this.level = level;
    }

    private GameObject[] GetParentObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        Transform[] objects = GetComponentsInParent<Transform>();
        foreach (Transform obj in objects)
        {
            gameObjects.Add(obj.gameObject);
        }
        return gameObjects.ToArray();
    }

    public void RequestElevator()
    {
        elevatorController.RequestElevator(direction, level);
        text.color = Color.green;
    }
}
