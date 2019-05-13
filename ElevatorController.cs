using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElevatorController : MonoBehaviour
{

    public GameObject[] levels;
    public int startLevelIndex = 0;
    List<GameObject> requestedLevelsUp = new List<GameObject>();
    List<GameObject> requestedLevelsDown = new List<GameObject>();
    List<GameObject> choosedLevels = new List<GameObject>();
    DoorsController doorsController;
    DoorsController actualLevelDoorsController;
    BoxCollider elevatorCollider;
    AudioController audioController;
    const int ELEVATOR_UP = 1;
    const int ELEVATOR_DOWN = 2;
    int elevatorState = ELEVATOR_UP;
    int actualLevel;
    int previousLevel;
    int nextLevel;
    float elevatorSpeed = 2f;
    float timeToCloseDoors = 6f;
    bool waitingForClose = false;
    bool levelReached = false;
    bool isNextLevel = false;


    void Start()
    {
        audioController = GetComponent<AudioController>();
        elevatorCollider = GetComponent<BoxCollider>();
        doorsController = GetComponentInChildren<DoorsController>();
        if (levels.Length > 1)
        {
            System.Array.Sort(levels, delegate (GameObject level1, GameObject level2)
            {
                return level1.GetComponent<Transform>().position.y.CompareTo(level2.GetComponent<Transform>().position.y);
            });
        }
        if (levels.Length > 0)
        {
            GameObject obj = (GameObject)levels.GetValue(0);
            transform.SetPositionAndRotation(new Vector3(transform.position.x, obj.transform.position.y, transform.position.z), transform.localRotation);
            actualLevelDoorsController = obj.GetComponentInChildren<DoorsController>();
            actualLevel = startLevelIndex;
            nextLevel = actualLevel;
        }
    }

    
    void FixedUpdate()
    {
        FindNextLevel();
        FindActualLevel();
        CheckDoorsClose();
        Move();
    }

    void CheckDoorsClose()
    {
        if (!waitingForClose) return;
        if (doorsController.IsClosed() && actualLevelDoorsController.IsClosed())
        {
            audioController.Play("sound", true);
            levelReached = false;
            waitingForClose = false;
        }
    }

    void LevelVisited()
    {
        audioController.Play("open_close", false);
        doorsController.CloseDoors();
        actualLevelDoorsController.CloseDoors();
        waitingForClose = true;
        if (IsLevelInList(GetObjectByLevelIndex(actualLevel), requestedLevelsDown)) RemoveLevelFormList(GetObjectByLevelIndex(actualLevel), requestedLevelsDown);
        if (IsLevelInList(GetObjectByLevelIndex(actualLevel), requestedLevelsUp)) RemoveLevelFormList(GetObjectByLevelIndex(actualLevel), requestedLevelsUp);
        if (IsLevelInList(GetObjectByLevelIndex(actualLevel), choosedLevels)) RemoveLevelFormList(GetObjectByLevelIndex(actualLevel), choosedLevels);
    }

    void RemoveLevelFormList(GameObject level, List<GameObject> list)
    {
        int index = 0;
        foreach(GameObject obj in list)
        {
            if (level.GetInstanceID() == obj.GetInstanceID()) break;
            index++;
        }
        list.RemoveAt(index);
    }

    void Move()
    {
        if (!isNextLevel) return;
        Vector3 move = new Vector3(0,0,0);
        if (elevatorState == ELEVATOR_UP && transform.position.y < GetObjectByLevelIndex(nextLevel).transform.position.y) move = new Vector3(0, elevatorSpeed, 0);
        else if (elevatorState == ELEVATOR_DOWN && transform.position.y > GetObjectByLevelIndex(nextLevel).transform.position.y) move = new Vector3(0, -elevatorSpeed, 0);
        if (!waitingForClose && !levelReached)
        {
            transform.Translate(move * Time.deltaTime);
            Collider[] hitColliders = Physics.OverlapBox(elevatorCollider.transform.position, elevatorCollider.transform.localScale / 2, Quaternion.identity);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].tag == "Player") hitColliders[i].transform.Translate(move * Time.deltaTime);
                i++;
            }
        }
        if (elevatorState == ELEVATOR_UP && transform.position.y > GetObjectByLevelIndex(nextLevel).transform.position.y)
        {
            LevelReached();
        }
        else if (elevatorState == ELEVATOR_DOWN && transform.position.y < GetObjectByLevelIndex(nextLevel).transform.position.y)
        {
            LevelReached();
        }
        else if(isNextLevel && !levelReached && transform.position.y == GetObjectByLevelIndex(nextLevel).transform.position.y)
        {
            LevelReached();
        }
    }

    void LevelReached()
    {
        levelReached = true;
        transform.SetPositionAndRotation(new Vector3(transform.position.x, GetObjectByLevelIndex(nextLevel).transform.position.y, transform.position.z), transform.localRotation);
        audioController.Play("open_close", false);
        doorsController.OpenDoors();
        actualLevelDoorsController.OpenDoors();
        TextMeshPro[] objects;
        objects = GetObjectByLevelIndex(nextLevel).GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro obj in objects) obj.color = Color.white;
        objects = GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro obj in objects)
            if (nextLevel.ToString() == obj.text)
                obj.color = Color.white;
        Invoke("LevelVisited", timeToCloseDoors);
    }

    GameObject GetObjectByLevelIndex(int index)
    {
        if (index < startLevelIndex || index > levels.Length + startLevelIndex) return null;
        return (GameObject)levels.GetValue(index-startLevelIndex);
    }

    void FindNextLevel()
    {
        if (requestedLevelsDown.Count == 0 && requestedLevelsUp.Count == 0 && choosedLevels.Count == 0)
        {
            isNextLevel = false;
            return;
        }
        isNextLevel = true;
        if (elevatorState == ELEVATOR_UP) SearchNextLevelUp();
        if (elevatorState == ELEVATOR_DOWN) SearchNextLevelDown();
    }

    bool SearchNextLevelDown()
    {
        for(int i = levels.Length - 1; i >= 0; i--)
        {
            if (levels[i].transform.position.y <= transform.position.y && (IsLevelInList(levels[i], requestedLevelsDown) || IsLevelInList(levels[i], choosedLevels)))
            {
                SetNextLevel(levels[i]);
                return true;
            }
        }
        if(requestedLevelsUp.Count > 0)
        {
            SetNextLevel((GameObject)requestedLevelsUp.ToArray().GetValue(0));
            return true;
        }
        elevatorState = ELEVATOR_UP;
        return false;
    }

    bool SearchNextLevelUp()
    {
        foreach(GameObject obj in levels)
        {
            if (obj.transform.position.y >= transform.position.y && (IsLevelInList(obj, requestedLevelsUp) || IsLevelInList(obj, choosedLevels)))
            {
                SetNextLevel(obj);
                return true;
            }
        }
        if (requestedLevelsDown.Count > 0)
        {
            SetNextLevel((GameObject)requestedLevelsDown.ToArray().GetValue(requestedLevelsDown.Count-1));
            return true;
        }
        elevatorState = ELEVATOR_DOWN;
        return false;
    }

    bool IsLevelInList(GameObject level, List<GameObject> list)
    {
        foreach(GameObject obj in list)
        {
            if (level.GetInstanceID() == obj.GetInstanceID()) return true;
        }
        return false;
    }

    void SetNextLevel(GameObject level)
    {
        int index = 0;
        foreach(GameObject obj in levels)
        {
            if(obj.GetInstanceID() == level.GetInstanceID())
            {
                nextLevel = index + startLevelIndex;
                return;
            }
            index++;
        }
    }
    
    void FindActualLevel()
    {
        float actualY = transform.position.y;
        int index = 0;
        foreach(GameObject obj in levels)
        {
            float objY = obj.transform.position.y;
            float tolerance = 0.1f;
            if(actualY >= objY && actualY < objY + tolerance)
            {
                actualLevel = index + startLevelIndex;
                break;
            }else if(actualY < objY && index > 0)
            {
                actualLevel = index - 1 + startLevelIndex;
                break;
            }
            index++;
        }
        GameObject actualLevelObj = (GameObject)levels.GetValue(index);
        actualLevelDoorsController = actualLevelObj.GetComponentInChildren<DoorsController>();

    }

    public void ChooseLevel(int level)
    {
        if (level < startLevelIndex || ((level - startLevelIndex) > levels.Length)) return;
        else AddChoosedLevel(level);
    }

    void AddChoosedLevel(int level)
    {
        GameObject levelToAdd = (GameObject)levels.GetValue(level - startLevelIndex);
        if (IsLevelInList(levelToAdd, choosedLevels)) return;
        choosedLevels.Add(levelToAdd);
        choosedLevels.Sort(delegate (GameObject level1, GameObject level2)
        {
            return level1.GetComponent<Transform>().position.y.CompareTo(level2.GetComponent<Transform>().position.y);
        });
    }

    public void RequestElevator(string direction, GameObject level)
    {
        if (direction == "Up") AddRequestUp(level);
        else if(direction == "Down") AddRequestDown(level);
    }

    void AddRequestUp(GameObject level)
    {
        if (IsLevelInList(level, requestedLevelsUp)) return;
        requestedLevelsUp.Add(level);
        requestedLevelsUp.Sort(delegate (GameObject level1, GameObject level2)
        {
            return level1.GetComponent<Transform>().position.y.CompareTo(level2.GetComponent<Transform>().position.y);
        });
    }

    void AddRequestDown(GameObject level)
    {
        if (IsLevelInList(level, requestedLevelsDown)) return;
        requestedLevelsDown.Add(level);
        requestedLevelsDown.Sort(delegate (GameObject level1, GameObject level2)
        {
            return level1.GetComponent<Transform>().position.y.CompareTo(level2.GetComponent<Transform>().position.y);
        });
    }
}
