using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Slot : MonoBehaviour
{
    [SerializeField] private GameObject _slotContainer;
    [SerializeField] private GameObject _winSlot;
    [SerializeField] private GameObject _loseSlot;
    private const int WheelCapacity = 20;
    private const float dY = 2.55f;
    private const float minY = -(WheelCapacity - 1) * dY;
    [SerializeField] private Text _curPercentText;
    private bool rowStopped = true;
    
    private System.Random rnd;
    private int currentPercent;
    
    public static event Action StartGame = delegate {  };
    void Start()
    {
        rnd = new System.Random();
        StartGame += StartRotating;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (rowStopped)
            {
                currentPercent = rnd.Next(0, 100);
                _curPercentText.text = "Current Percent: " + currentPercent;
                _slotContainer.transform.position = Vector3.zero;
                ClearWheelVariants();
                InstantiateWheelVariants();
                StartRotating();
            }
        }

        if (rowStopped)
        {
            
        }
    }
    
    private void ClearWheelVariants()
    {
        foreach (Transform variant in _slotContainer.transform)
        {
            Destroy(variant.gameObject);
        }
    }

    private void InstantiateWheelVariants()
    {
        for (int i = 0; i < WheelCapacity - 1; i++)
        {
            var pseudoRandom = rnd.Next(0, 100);
            if (pseudoRandom < currentPercent)
            {
                CreateWheelVariant(_winSlot, i);
                continue;
            }
            CreateWheelVariant(_loseSlot, i);
        }
    }

    private void CreateWheelVariant(GameObject type, int curIndex)
    {
        var pref = Instantiate(type, new Vector3(0, curIndex * dY, 0), Quaternion.identity, _slotContainer.transform);
        pref.name = type.name + curIndex;
        if (curIndex == 0) 
        {
            //last variant == first variant on wheel for smooth transform reset
            pref = Instantiate(type, new Vector3(0, (WheelCapacity - 1) * dY, 0), Quaternion.identity, _slotContainer.transform);
            pref.name = type.name + (WheelCapacity - 1);
        }
    }

    private void StartRotating()
    {
        StartCoroutine("Rotate");
    }

    
    private IEnumerator Rotate()
    {
        rowStopped = false;
        var timeInterval = 0.025f;

        for (int i = 0; i < 100; i++)
        {
            if (_slotContainer.transform.position.y <= minY)
            {
                _slotContainer.transform.position = new Vector2(_slotContainer.transform.position.x, 0);
            }

            _slotContainer.transform.position = new Vector2(_slotContainer.transform.position.x,
                _slotContainer.transform.position.y - 0.5f);

            yield return new WaitForSeconds(timeInterval);
        }

        var localRnd = rnd.Next(70, 100);

        for (int i = 0; i < localRnd; i++)
        {
            if (_slotContainer.transform.position.y <= minY)
            {
                _slotContainer.transform.position = new Vector2(_slotContainer.transform.position.x, 0);
            }
            _slotContainer.transform.position = new Vector2(_slotContainer.transform.position.x,
                _slotContainer.transform.position.y - 0.5f);
            
            if (i > Mathf.RoundToInt(localRnd * 0.5f))
            {
                timeInterval = 0.1f;
            }
            if (i > Mathf.RoundToInt(localRnd * 0.65f))
            {
                timeInterval = 0.15f;
            }
            if (i > Mathf.RoundToInt(localRnd * 0.8f))
            {
                timeInterval = 0.2f;
            }
            yield return new WaitForSeconds(timeInterval);
        }

        rowStopped = true;
    }
    
    
}
