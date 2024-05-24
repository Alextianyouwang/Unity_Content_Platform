
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
[ExecuteInEditMode]
public class PSS_VisualManager : MonoBehaviour
{
    public GameObject Element;
    public int CandidateNumberWithEXP2 = 4;

    private int _totalElements;


    private List<PSS_Array> _allPSS = new List<PSS_Array>();


    private void OnEnable()
    {
        _totalElements = (int)Mathf.Pow(2, CandidateNumberWithEXP2);
        AddNewList(0);
        AddNewList(2);

        CalculatPSSAnimation();

    }
    public void Sort() 
    {
           
    }
    private void Update()
    {
      
    }
    private void OnDisable()
    {
        _allPSS.ForEach(e => e.Dispose());
        _allPSS.Clear();

    }

    public void AddNewList(float yOffset) 
    {
        PSS_Array initialArray = new PSS_Array(Element, _totalElements, yOffset) ;
        initialArray.InitializeList();
        _allPSS.Add(initialArray);
    }

    async void CalculatPSSAnimation() 
    {
        for (int i = 0; i < (int)Mathf.Log(_totalElements,2) * 2 + 1; i++) 
        {
            AddNewList( (i + 2 ) *2);
        }
        int offset = 1;
        int step = 1;
        for (int d = _totalElements / 2; d > 0; d >>= 1)
        {
            PSS_Array current = _allPSS[step];
            PSS_Array next = _allPSS[step + 1];
            await Task.Delay(1200);


            for (int i = 0; i < _totalElements; i++)
            {
                next.Objects[i].Number = current.Objects[i].Number;
                current.Objects[i].DuplicateMoveTo(next.Objects[i], true, next.Objects[i].UpdateNumberText);
            }
            for (int k = 0; k < _totalElements; k++)
            {
                if (k < d)
                {
                    int ai = offset * (2 * k + 1) - 1;
                    int bi = offset * (2 * k + 2) - 1;
                    current.Objects[ai].MoveTo(next.Objects[bi], true, next.Objects[ai].UpdateNumberText);
                    current.Objects[bi].MoveTo(next.Objects[bi], true, next.Objects[bi].UpdateNumberText);
                    next.Objects[bi].Number = current.Objects[ai].Number + current.Objects[bi].Number;
                }
  
            }
           
            step += 1;
            offset *= 2;
        }
        await Task.Delay(1200);

        for (int i = 0; i < _totalElements; i++)
        {
            _allPSS[step].Objects[i].MoveTo(_allPSS[step + 1].Objects[i], true, _allPSS[step + 1].Objects[i].UpdateNumberText);
            _allPSS[step + 1].Objects[i].Number = _allPSS[step].Objects[i].Number;
        }
        _allPSS[step + 1].Objects[_totalElements - 1].Number = 0;

        step += 1;

        for (int d = 1; d <= _totalElements / 2; d *= 2)
        {
            await Task.Delay(1200);
            PSS_Array current = _allPSS[step];
            PSS_Array next = _allPSS[step +1];

            offset /= 2;
            for (int i = 0; i < _totalElements; i++)
            {
                next.Objects[i].Number = current.Objects[i].Number;
                next.Objects[i].UpdateNumberText();
            }
            for (int k = 0; k < _totalElements; k++)
            {
                if (k < d)
                {
                    int ai = offset * (2 * k + 1) - 1;
                    int bi = offset * (2 * k + 2) - 1;
                    current.Objects[ai].MoveTo(next.Objects[bi], true, next.Objects[bi].UpdateNumberText);
                    current.Objects[bi].MoveTo(next.Objects[ai], true, next.Objects[ai].UpdateNumberText);
                    int t = current.Objects[bi].Number;
                    next.Objects[bi].Number = current.Objects[ai].Number + current.Objects[bi].Number;
                    next.Objects[ai].Number = t;


                }
            }
            step += 1;

        }
    }


}

public class PSS_Array
{
    private int _totalElements;
    private PSS_VoteCandidate[] _spawnBufferObjects;
    public PSS_VoteCandidate[] Objects { get { return _spawnBufferObjects; } }
    private GameObject _element;

    private float _yOffset;
    public PSS_Array(GameObject element, int totalElements, float yOffset)
    {
        _element = element;
        _totalElements = totalElements;
        _yOffset = yOffset;
    }

    public void InitializeList()
    {
        if (_spawnBufferObjects != null)
            return;
        _spawnBufferObjects = new PSS_VoteCandidate[_totalElements];
        for (int i = 0; i < _totalElements; i++)
        {
            _spawnBufferObjects[i] = new PSS_VoteCandidate(_element, Vector3.right * i + Vector3.down * _yOffset);
            _spawnBufferObjects[i].SetNumber(Mathf.PerlinNoise1D(i * 1298.98f) < 0.5 ? 0 : 1);
        }

    }
    public void Dispose()
    {
        if (_spawnBufferObjects == null)
            return;
        foreach (PSS_VoteCandidate g in _spawnBufferObjects)
            GameObject.DestroyImmediate(g?.Element);
        _spawnBufferObjects = null;

    }


}
public class PSS_VoteCandidate
{
    private GameObject _elementInstance;
    private TextMeshProUGUI _text;
    public GameObject Element;
    public int Number;

    public PSS_VoteCandidate( GameObject elementInstance, Vector3 worldPos)
    {
        _elementInstance = elementInstance;
        Element = GameObject.Instantiate(_elementInstance);
        Element.transform.position = worldPos;
        _text = Element.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetNumber(int number) 
    {
        Number = number;
        UpdateNumberText();
    }
    public void UpdateNumberText() => _text.text = Number.ToString();

    public void SetColor(Color _color) 
    {
    
    }

    public async void MoveTo(PSS_VoteCandidate targetObject, bool turnOff , Action ToDo) 
    {
        float percent = 0;
        Vector3 initialPos = Element.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime;
            Element.transform.position = Vector3.Lerp(initialPos,targetObject.Element.transform.position, percent);
            await Task.Yield();
        }
        Element.SetActive(!turnOff);
        ToDo?.Invoke();
    }

    public async void DuplicateMoveTo(PSS_VoteCandidate targetObject, bool turnOff, Action ToDo)
    {
        float percent = 0;
        GameObject newObj = GameObject.Instantiate(Element);
        newObj.transform.position = newObj.transform.position;
        Vector3 initialPos = newObj.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime;
            newObj.transform.position = Vector3.Lerp(initialPos, targetObject.Element.transform.position, percent);
            await Task.Yield();
        }
        if (turnOff)
            GameObject.DestroyImmediate(newObj);
        ToDo?.Invoke();
    }
}