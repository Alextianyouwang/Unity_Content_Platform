
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

    public int[] InitialArray;

    private List<PSS_Array> _allPSS = new List<PSS_Array>();


    private void OnEnable()
    {
        SetInitialArray(InitialArray);

        AddNewList(0, true);
        CalculatPSSAnimation();

    }
    public int CeilToNearestPowerOf2(int value)
    {
        int target = 2;
        while (target < value)
            target <<= 1;
        return target;
    }
    public void SetInitialArray(int[]value) 
    {
        int[] newArry= new int[CeilToNearestPowerOf2(value.Length)] ;
        Array.Copy(value, newArry, value.Length);
        InitialArray = newArry;
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


    public void AddNewList(float yOffset, bool setVisible) 
    {
        PSS_Array initialArray = new PSS_Array(Element, InitialArray, yOffset) ;
        initialArray.InitializeList();
        initialArray.ToggleListVisibility(setVisible);
        _allPSS.Add(initialArray);
    }

    async void CalculatPSSAnimation() 
    {
        for (int i = 0; i < (int)Mathf.Log(InitialArray.Length,2) * 2 + 1; i++) 
        {
            AddNewList( (i + 1 ) *2, false);
            
        }
        int offset = 1;
        int step = 0;
        for (int d = InitialArray.Length / 2; d > 0; d >>= 1)
        {
            PSS_Array current = _allPSS[step];
            PSS_Array next = _allPSS[step + 1];
            await Task.Delay(1200);

            next.ToggleListVisibility(true);
            for (int i = 0; i < InitialArray.Length; i++)
            {
                next.Objects[i].Number = current.Objects[i].Number;
                next.Objects[i].UpdateNumberText();
                //current.Objects[i].DuplicateMoveTo(next.Objects[i], true, next.Objects[i].UpdateNumberText);
            }
            for (int k = 0; k < InitialArray.Length; k++)
            {
                if (k < d)
                {
                    int ai = offset * (2 * k + 1) - 1;
                    int bi = offset * (2 * k + 2) - 1;
                    current.Objects[ai].DuplicateMoveTo(next.Objects[bi], true, next.Objects[ai].UpdateNumberText);
                    current.Objects[bi].DuplicateMoveTo(next.Objects[bi], true, next.Objects[bi].UpdateNumberText);
                    next.Objects[bi].Number = current.Objects[ai].Number + current.Objects[bi].Number;
                }
  
            }
  

            step += 1;
            offset *= 2;
        }
        await Task.Delay(1200);

        for (int i = 0; i < InitialArray.Length; i++)
        {
            _allPSS[step].Objects[i].DuplicateMoveTo(_allPSS[step + 1].Objects[i], true, _allPSS[step + 1].Objects[i].UpdateNumberText);
            _allPSS[step + 1].Objects[i].Number = _allPSS[step].Objects[i].Number;
        }
        _allPSS[step + 1].Objects[InitialArray.Length - 1].Number = 0;
        _allPSS[step + 1].ToggleListVisibility(true);
        step += 1;

        for (int d = 1; d <= InitialArray.Length / 2; d *= 2)
        {
            await Task.Delay(1200);
            PSS_Array current = _allPSS[step];
            PSS_Array next = _allPSS[step +1];

            offset /= 2;
            next.ToggleListVisibility(true);
            for (int i = 0; i < InitialArray.Length; i++)
            {
                next.Objects[i].Number = current.Objects[i].Number;
                next.Objects[i].UpdateNumberText();
            }
            for (int k = 0; k < InitialArray.Length; k++)
            {
                if (k < d)
                {
                    int ai = offset * (2 * k + 1) - 1;
                    int bi = offset * (2 * k + 2) - 1;
                    current.Objects[ai].DuplicateMoveTo(next.Objects[bi], true, next.Objects[bi].UpdateNumberText);
                    current.Objects[bi].DuplicateMoveTo(next.Objects[ai], true, next.Objects[ai].UpdateNumberText);
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
    private int[] _value;
    private PSS_VoteCandidate[] _spawnBufferObjects;
    public PSS_VoteCandidate[] Objects { get { return _spawnBufferObjects; } }
    private GameObject _element;

    private float _yOffset;
    public PSS_Array(GameObject element, int[] value, float yOffset)
    {
        _element = element;
        _value = value; 
        _yOffset = yOffset;
    }

    public void InitializeList()
    {
        if (_spawnBufferObjects != null)
            return;
        _spawnBufferObjects = new PSS_VoteCandidate[_value.Length];
        for (int i = 0; i < _value.Length; i++)
        {
            _spawnBufferObjects[i] = new PSS_VoteCandidate(_element, Vector3.right * i + Vector3.down * _yOffset);
            _spawnBufferObjects[i].SetNumber(_value[i]);
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

    public void ToggleListVisibility(bool value) => _spawnBufferObjects.ToList().ForEach(p => p.ToggleVisibility(value));

}
public class PSS_VoteCandidate
{
    private GameObject _elementInstance;
    private TextMeshProUGUI _text;
    public GameObject Element;
    public int Number;
    public bool OccupiedByAction;
    private MaterialPropertyBlock _mpb;

    public PSS_VoteCandidate( GameObject elementInstance, Vector3 worldPos)
    {
        _elementInstance = elementInstance;
        Element = GameObject.Instantiate(_elementInstance);
        Element.transform.position = worldPos;
        _text = Element.GetComponentInChildren<TextMeshProUGUI>();
        _mpb = new MaterialPropertyBlock();
    }

    public void SetNumber(int number) 
    {
        Number = number;
        UpdateNumberText();
    }
    public void UpdateNumberText() => _text.text = Number.ToString();

    public void SetColor(Color _color) 
    {
        _mpb.SetColor("_BaseColor", _color);
        Element.GetComponent<MeshRenderer>().SetPropertyBlock(_mpb);
    }
    public void ToggleVisibility(bool value) 
    {
        Element.SetActive(value);
    }
  
    public async void MoveTo(PSS_VoteCandidate targetObject, bool turnOff , Action ToDo) 
    {
        float percent = 0;
        Vector3 initialPos = Element.transform.position;
        OccupiedByAction = true;
        while (percent < 1f)
        {
            percent += Time.deltaTime;
            Element.transform.position = Vector3.Lerp(initialPos,targetObject.Element.transform.position, percent);
            await Task.Yield();
        }
        OccupiedByAction = false;
        Element.SetActive(!turnOff);
        ToDo?.Invoke();
    }

    public async void DuplicateMoveTo(PSS_VoteCandidate targetObject, bool deleteDupWhenFinish, Action ToDo)
    {
        float percent = 0;
        GameObject newObj = GameObject.Instantiate(Element);
        newObj.transform.position = newObj.transform.position;
        Vector3 initialPos = newObj.transform.position;
        OccupiedByAction = true;
        while (percent < 1f)
        {
            percent += Time.deltaTime;
            newObj.transform.position = Vector3.Lerp(initialPos, targetObject.Element.transform.position, percent);
            await Task.Yield();
        }
        OccupiedByAction = false;

        if (deleteDupWhenFinish)
            GameObject.DestroyImmediate(newObj);
        ToDo?.Invoke();
    }
}