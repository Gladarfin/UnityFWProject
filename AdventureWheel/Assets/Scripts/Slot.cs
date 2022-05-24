using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private GameObject slotContainer;
    [SerializeField] private GameObject winSlot;
    [SerializeField] private GameObject loseSlot;
    [SerializeField] private Text curPercentText;
    
    private const int WheelCapacity = 20;
    private const float dY = 2.55f;
    private const float minY = -(WheelCapacity - 1) * dY;
    private bool _rowStopped = true;
    
    private System.Random _rnd;
    private int _currentPercent;
    
    void Start()
    {
        _rnd = new System.Random();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (!_rowStopped) return;
            
            _currentPercent = _rnd.Next(0, 100);
            curPercentText.text = "Current Percent: " + _currentPercent;
            slotContainer.transform.position = Vector3.zero;
            ClearWheelVariants();
            InstantiateWheelVariants();
            StartRotating();
        }
    }
    
    private void ClearWheelVariants()
    {
        foreach (Transform variant in slotContainer.transform)
        {
            Destroy(variant.gameObject);
        }
    }

    private void InstantiateWheelVariants()
    {
        for (int i = 0; i < WheelCapacity - 1; i++)
        {
            var pseudoRandom = _rnd.Next(0, 100);
            if (pseudoRandom < _currentPercent)
            {
                CreateWheelVariant(winSlot, i);
                continue;
            }
            CreateWheelVariant(loseSlot, i);
        }
    }

    private void CreateWheelVariant(GameObject type, int curIndex)
    {
        var pref = Instantiate(type, new Vector3(0, curIndex * dY, 0), Quaternion.identity, slotContainer.transform);
        pref.name = type.name + curIndex;
        if (curIndex == 0) 
        {
            //last variant == first variant on wheel for smooth transform reset
            pref = Instantiate(type, new Vector3(0, (WheelCapacity - 1) * dY, 0), Quaternion.identity, slotContainer.transform);
            pref.name = type.name + (WheelCapacity - 1);
        }
    }

    private void StartRotating()
    {
        StartCoroutine(nameof(Rotate));
    }

    
    private IEnumerator Rotate()
    {
        _rowStopped = false;
        var timeInterval = 0.025f;

        for (int i = 0; i < 100; i++)
        {
            if (slotContainer.transform.position.y <= minY)
            {
                slotContainer.transform.position = new Vector2(slotContainer.transform.position.x, 0);
            }

            slotContainer.transform.position = new Vector2(slotContainer.transform.position.x,
                slotContainer.transform.position.y - 0.5f);

            yield return new WaitForSeconds(timeInterval);
        }

        var localRnd = _rnd.Next(70, 100);

        for (int i = 0; i < localRnd; i++)
        {
            if (slotContainer.transform.position.y <= minY)
            {
                slotContainer.transform.position = new Vector2(slotContainer.transform.position.x, 0);
            }
            slotContainer.transform.position = new Vector2(slotContainer.transform.position.x,
                slotContainer.transform.position.y - 0.5f);
            
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

        _rowStopped = true;
    }
    
    
}
