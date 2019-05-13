using UnityEngine;
using TMPro;

public class ElevatorButtonHandler : MonoBehaviour
{
    
    public int targetLevel = 0;
    ElevatorController elevatorController;
    TextMeshPro text;

    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        elevatorController = GetComponentInParent<ElevatorController>();
        text.text = targetLevel.ToString();
    }

    public void ChooseLevel()
    {
        elevatorController.ChooseLevel(targetLevel);
    }
}
