using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class PSS_CameraCulling : MonoBehaviour
{
    Camera _cam;
    public Transform TestObjectHolder;

    private bool _actionOccupied, _disableInsteadOfChangeColor;
    public PSS_VisualManager _visualManager;
    private int[] _objectArray;
    private Color[] _colorArray;
    
    private void OnEnable()
    {
        _cam = GetComponent<Camera>();
        PSS_VisualManager.OnFinishSorting += FinishSorting;
    }

    private void OnDisable()
    {
        PSS_VisualManager.OnFinishSorting -= FinishSorting;

    }

    void Update()
    {
        if (!_actionOccupied)
            UpdateView();

        if (Input.GetKeyDown(KeyCode.Space))
            InitiateLineup();
    }

    private void FinishSorting(int[] numVisible) 
    {
        for (int i = 0; i < TestObjectHolder.childCount; i++)
        {
                TestObjectHolder.GetChild(i).gameObject.SetActive( numVisible.ToList().Contains(i));

            TestObjectHolder.GetChild(i).GetComponent<PSS_Object>().SetColor(Color.green);
            TestObjectHolder.GetChild(i).GetComponent<PSS_Object>().SetNumber (1);
        }
        GetBackToOriginalPos();

    }
    void InitiateLineup() 
    {
        StartCoroutine(LineupAnimation(false,StartAnimation ));
    }
    void GetBackToOriginalPos() 
    {
        StartCoroutine(LineupAnimation(true,SetBools));
    }

    void SetBools() 
    {
         _disableInsteadOfChangeColor = true;
         _actionOccupied = false;
    }
    IEnumerator LineupAnimation(bool reverse, Action todo) 
    {
        float percent = 0f;
        Vector3[] initialPos = new Vector3[TestObjectHolder.childCount];
        for (int i = 0; i < TestObjectHolder.childCount; i++) 
        {
            
            initialPos[i] = TestObjectHolder.GetChild(i).position;
        }
        float[] progresses = new float[TestObjectHolder.childCount];
        _actionOccupied = true;
        while (!AllProgressesFinished (progresses)) 
        {
            percent += Time.deltaTime;
            for (int i = 0; i < TestObjectHolder.childCount; i++)
            {
                progresses[i] += Time.deltaTime * (1 + i * 0.1f);
                Vector3 targetPos = reverse ? TestObjectHolder.GetChild(i).GetComponent<PSS_Object>().InitialPosition : Vector3.right * i;
                TestObjectHolder.GetChild(i).position = Vector3.Lerp(initialPos[i], targetPos, progresses[i]);
            }
            yield return null;  
        }

        todo?.Invoke();
    }

    private void StartAnimation() 
    {
        _objectArray = new int[TestObjectHolder.childCount];
        _colorArray = new Color[TestObjectHolder.childCount];
        for (int i = 0; i < TestObjectHolder.childCount; i++)
        {
            _objectArray[i] = TestObjectHolder.GetChild(i).GetComponent<PSS_Object>().InView ? 1 : 0;
            _colorArray[i] = TestObjectHolder.GetChild(i).GetComponent<PSS_Object>().ObjColor;
            TestObjectHolder.GetChild(i).gameObject.SetActive(false);

        }
        _visualManager.StartAnimation(_objectArray, _colorArray);
    }
    private bool AllProgressesFinished(float[] input) 
    {
        for (int i = 0; i < input.Length; i++) 
        {
            if (input[i] < 1)
                return false;
        }
        return true;
    }

    private void UpdateView() 
    {
        foreach (Transform t in TestObjectHolder)
            if (t != null)
            {
                PSS_Object obj = t.GetComponent<PSS_Object>();
                if (_disableInsteadOfChangeColor)
                    obj.gameObject.SetActive(false);
                else
                    obj.SetState(false);
            }
        Plane[] p = GeometryUtility.CalculateFrustumPlanes(_cam);
        foreach (Transform t in TestObjectHolder)
            if (GeometryUtility.TestPlanesAABB(p, t.GetComponent<MeshRenderer>().bounds))
                if (t != null)
                {
                    PSS_Object obj = t.GetComponent<PSS_Object>();
                    if (_disableInsteadOfChangeColor)
                        obj.gameObject.SetActive(true);
                    else
                        obj.SetState(true);
                }

    }


}
