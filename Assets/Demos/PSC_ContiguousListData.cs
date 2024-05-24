using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "PSC Demo/ContiguousListData")]
public class PSC_ContiguousListData : ScriptableObject
{
    public int CandidateNumberEXP2 = 4;
    private int _totalElements;
    public PSS_VoteCandidate[] _voteCandidates;
    private void OnValidate()
    { 

        if (_voteCandidates == null)
            Initialize();
    }
    public void Initialize() 
    {
        _totalElements = (int)Mathf.Pow(2, CandidateNumberEXP2);
        _voteCandidates = new PSS_VoteCandidate[_totalElements];
        for (int i = 0; i < _totalElements; i++)
        {

        }
        EditorUtility. SetDirty(this);
    }
}

