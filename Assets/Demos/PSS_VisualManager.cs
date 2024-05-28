
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PSS_VisualManager : MonoBehaviour
{
    public GameObject Element;

    public int[] InitialArray;

    private List<PSS_Array> _allPSS = new List<PSS_Array>();

    private LineRenderer[] _allLRs;
    private LineRenderer[] _allLRsBot;
    public static bool[] _BotLRStatus;

    public Material LineMaterial;

    private void OnEnable()
    {


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

    public void InitialLR() 
    {
        _allLRs = new LineRenderer[InitialArray.Length];
        for (int i = 0; i < InitialArray.Length; i++)
        {
            GameObject g = new GameObject();
            g.transform.parent = transform;
            _allLRs[i] = g.AddComponent<LineRenderer>();
            _allLRs[i].startWidth = 0.1f;
            _allLRs[i].endWidth = 0.1f;
            _allLRs[i].material = LineMaterial;
        }
        _allLRsBot = new LineRenderer[InitialArray.Length];
        _BotLRStatus = new bool[InitialArray.Length];
        for (int i = 0; i < InitialArray.Length; i++)
        {
            GameObject g = new GameObject();
            g.transform.parent = transform;
            _allLRsBot[i] = g.AddComponent<LineRenderer>();
            _allLRsBot[i].startWidth = 0.1f;
            _allLRsBot[i].endWidth = 0.1f;
            _allLRsBot[i].material = LineMaterial;
            _BotLRStatus[i] = true;
        }
    }

    public void SetLRPositions() 
    {
        for (int i = 0; i < InitialArray.Length; i++)
        {
            _allLRs[i].SetPosition(1, _allPSS[0].GetPositions()[i]);
            _allLRs[i].SetPosition(0, _allPSS[0].GetPositions()[i]);
            _allLRsBot[i].SetPosition(0, _allPSS[0].GetPositions()[i]);
            _allLRsBot[i].SetPosition(1, _allPSS[0].GetPositions()[i]);
        }
    }
    public void StartAnimation() 
    {
        SetInitialArray(InitialArray);
        InitialLR();
        AddNewList(0, true);
        for (int i = 0; i < (int)Mathf.Log(InitialArray.Length, 2) * 2 + 1; i++)
        {
            AddNewList((i + 1) * 2, false);

        }
        SetLRPositions();
        CalculatPSSAnimation();
    }
    public void StartAnimation(int[] arrayValue)
    {
        SetInitialArray(arrayValue);
        InitialLR();

        AddNewList(0, true);
        for (int i = 0; i < (int)Mathf.Log(InitialArray.Length, 2) * 2 + 1; i++)
        {
            AddNewList((i + 1) * 2, false);

        }
        SetLRPositions();
        CalculatPSSAnimation();
    }

    private void OnDisable()
    {
        _allPSS.ForEach(e => e.Dispose());
        _allPSS.Clear();
        _allLRs = null;
        _allLRsBot = null;
        _BotLRStatus = null;
    }


    public void AddNewList(float yOffset, bool setVisible) 
    {
        PSS_Array initialArray = new PSS_Array(Element, InitialArray, yOffset, _allLRs,_allLRsBot,transform) ;
        initialArray.InitializeList();
        initialArray.ToggleListVisibility(setVisible);
        _allPSS.Add(initialArray);
    }

    async void CalculatPSSAnimation() 
    {
      
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
                    current.Objects[ai].DuplicateMoveTo(next.Objects[bi], true, Color.blue,current.Objects[ai].Number, next.Objects[ai].UpdateNumberText, next.Objects[ai].SetColor, Color.cyan, true,false, true,1f);
                    current.Objects[bi].DuplicateMoveTo(next.Objects[bi], true, Color.blue, current.Objects[bi].Number, next.Objects[bi].UpdateNumberText, next.Objects[bi].SetColor, Color.cyan, true, false, true, 1f);

                    next.Objects[bi].Number = current.Objects[ai].Number + current.Objects[bi].Number;
                }
  
            }
  

            step += 1;
            offset *= 2;
        }
        await Task.Delay(1200);

        for (int i = 0; i < InitialArray.Length; i++)
        {
            if (i == InitialArray.Length - 1)
            {
                _allPSS[step + 1].Objects[i].Number = 0;
                _allPSS[step + 1].Objects[i].SetColor(Color.red);
                _allPSS[step].Objects[i].DuplicateMoveTo(_allPSS[step + 1].Objects[i], true, Color.red, _allPSS[step + 1].Objects[i].Number, _allPSS[step + 1].Objects[i].UpdateNumberText, _allPSS[step + 1].Objects[i].SetColor, Color.red,false,false,false,1f);
            }
            else 
            {
                _allPSS[step + 1].Objects[i].Number = _allPSS[step].Objects[i].Number;
                _allPSS[step].Objects[i].DuplicateMoveTo(_allPSS[step + 1].Objects[i], true,Color.gray, _allPSS[step + 1].Objects[i].Number, _allPSS[step + 1].Objects[i].UpdateNumberText,null,Color.clear, false, false, false, 1f);
            }
            //_allPSS[step].Objects[i].UpdateLRStartPos(_allPSS[step].Objects[i].Element.transform.position);
        }
        
        _allPSS[step + 1].ToggleListVisibility(true);
        step += 1;

        for (int d = 1; d <= InitialArray.Length / 2; d *= 2)
        {
            await Task.Delay(1500);
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
                    current.Objects[ai].DuplicateMoveTo(next.Objects[bi], true, Color.blue, current.Objects[ai].Number, next.Objects[bi].UpdateNumberText, next.Objects[bi].SetColor, Color.cyan, false, true, true, 1.3f);
                    current.Objects[bi].DuplicateMoveTo(next.Objects[ai], true, Color.yellow, current.Objects[bi].Number, next.Objects[ai].UpdateNumberText, next.Objects[ai].SetColor, Color.yellow, false,false, false, 0.7f);
                    current.Objects[bi].DuplicateMoveTo(next.Objects[bi], true, Color.blue, current.Objects[bi].Number, null ,null,Color.clear, false,true,true, 1f);

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
    private LineRenderer[] _lrs;
    private LineRenderer[] _botLrs;
    private Transform _parent;


    private float _yOffset;
    public PSS_Array(GameObject element, int[] value, float yOffset , LineRenderer[] lr, LineRenderer[] botLrs, Transform parent)
    {
        _element = element;
        _value = value;
        _yOffset = yOffset;
        _lrs = lr;
        _botLrs = botLrs;
        _parent = parent;
    }

    public void InitializeList()
    {
        if (_spawnBufferObjects != null)
            return;
        _spawnBufferObjects = new PSS_VoteCandidate[_value.Length];
        Matrix4x4 localToWorld = _parent.localToWorldMatrix;
        for (int i = 0; i < _value.Length; i++)
        {
            Vector3 pos =  Vector3.right * i + Vector3.down * _yOffset;
            pos = localToWorld * new Vector4(pos.x, pos.y, pos.z, 1);
            Vector3 scale = _parent.localScale;
            Vector3 euler = _parent.localEulerAngles;
            _spawnBufferObjects[i] = new PSS_VoteCandidate(_element,pos,euler, scale, _lrs[i], _botLrs[i],i);
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
    public Vector3[] GetPositions() => _spawnBufferObjects.ToList().Select(x => x.Element.transform.position).ToArray();

}
public class PSS_VoteCandidate
{
    private GameObject _elementInstance;
    private TextMeshProUGUI _text;
    public GameObject Element;
    public int Number;
    public bool OccupiedByAction;
    private MaterialPropertyBlock _mpb;
    private LineRenderer _lr;
    private LineRenderer _botLr;
    public Action<Vector3> OnMove;
    private int _index;

    public PSS_VoteCandidate( GameObject elementInstance, Vector3 worldPos, Vector3 worldEuler, Vector3 worldScale, LineRenderer lr, LineRenderer botLr, int index)
    {
        _elementInstance = elementInstance;
        Element = GameObject.Instantiate(_elementInstance);
        Element.transform.position = worldPos;
        Element.transform.eulerAngles = worldEuler;
        Element.transform.localScale = worldScale;
        _text = Element.GetComponentInChildren<TextMeshProUGUI>();
        _mpb = new MaterialPropertyBlock();
        _lr = lr;
        _botLr = botLr;
        _index = index;
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
    public void UpdateLRStartPos(Vector3 pos)
    {
        _lr.SetPosition(0,pos);
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



    public async void DuplicateMoveTo(PSS_VoteCandidate targetObject, bool deleteDupWhenFinish, Color dupColor, int dupNum,  Action ToDo, Action<Color> ToSetColor, Color colorToSet ,bool useLR, bool useLRBot , bool resetLRBotPos, float speed)
    {
        float percent = 0;
        PSS_VoteCandidate newObj = new PSS_VoteCandidate(_elementInstance, Element.transform.position, Element.transform.eulerAngles, Element.transform.localScale,_lr,_botLr,_index);
        Vector3 initialPos = newObj.Element.transform.position;
        newObj.Element.transform.localScale *= 0.8f;
       OccupiedByAction = true;
      
        newObj.SetColor(dupColor);
        newObj.SetNumber(dupNum);
        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            newObj.Element.transform.position = Vector3.Lerp(initialPos, targetObject.Element.transform.position, percent);

            OnMove?.Invoke(newObj.Element.transform.position);
            if (useLR) 
            {
                _lr.positionCount++;
                _lr.SetPosition(_lr.positionCount - 1, newObj.Element.transform.position);
            }
            if (useLRBot) 
            {
                if (resetLRBotPos && PSS_VisualManager._BotLRStatus[_index] ) 
                {
                    PSS_VisualManager._BotLRStatus[_index] = false;
                    _botLr.SetPosition(1, initialPos);
                    _botLr.SetPosition(0, initialPos);
                }
                _botLr.positionCount++;
                _botLr.SetPosition(_botLr.positionCount - 1, newObj.Element.transform.position);
            }

          await Task.Yield();
        }
        OccupiedByAction = false;

        if (deleteDupWhenFinish)
            GameObject.DestroyImmediate(newObj.Element);
        ToDo?.Invoke();
        ToSetColor?.Invoke(colorToSet);
    }
}