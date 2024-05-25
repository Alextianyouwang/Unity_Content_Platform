using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PSS_CameraCulling : MonoBehaviour
{
    Camera _cam;
    public Transform TestObjectHolder;

    private bool _actionOccupied;
    private void OnEnable()
    {
        _cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (!_actionOccupied)
            UpdateView();

        if (Input.GetKeyDown(KeyCode.Space))
            InitiateLineup();
    }

    void InitiateLineup() 
    {
        StartCoroutine(LineupAnimation());
    }

    IEnumerator LineupAnimation() 
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
                TestObjectHolder.GetChild(i).position = Vector3.Lerp(initialPos[i], Vector3.right * i, progresses[i]);
            }
            yield return null;  
        }
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
                obj.SetState(false);
            }
        Plane[] p = GeometryUtility.CalculateFrustumPlanes(_cam);
        foreach (Transform t in TestObjectHolder)
            if (GeometryUtility.TestPlanesAABB(p, t.GetComponent<MeshRenderer>().bounds))
                if (t != null)
                {
                    PSS_Object obj = t.GetComponent<PSS_Object>();
                    obj.SetState(true);
                }

    }


}
