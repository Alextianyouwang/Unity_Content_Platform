
using TMPro;
using UnityEngine;
public class PSS_Object : MonoBehaviour
{
    private MaterialPropertyBlock _mpb;

    private bool _inView;
    private void OnEnable()
    {
        
    }
    public void SetState(bool value) 
    {
        _inView = value;
        if (_inView)
        {

            SetColor(Color.green);
            SetNumber(1);
        }
        else
        {
            SetColor(Color.red);
            SetNumber(0);
        }
    }
    public void SetColor(Color _color)
    {
        _mpb = new MaterialPropertyBlock();
        _mpb.SetColor("_BaseColor", _color);
        GetComponent<MeshRenderer>().SetPropertyBlock(_mpb);
    }

    public void SetNumber(int _number) 
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = _number.ToString();
    }
}
