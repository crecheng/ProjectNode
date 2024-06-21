using UnityEngine;
using UnityEngine.UI;

public class FPSTool: MonoBehaviour
{
    public float _UpdateInterval = 0.1f;
    public string format = "f2";

    private float _LastInterval;

    private int _Frames = 0;

    private float _FPS;

    public Text text;

    public void Awake()
    {
        _LastInterval = Time.realtimeSinceStartup;
        _Frames = 0;
    }
    public void Update()
    {
        _Frames++;
        if(Time.realtimeSinceStartup > _LastInterval + _UpdateInterval)
        {
            _FPS = _Frames / (Time.realtimeSinceStartup - _LastInterval);
            _Frames = 0;
            _LastInterval = Time.realtimeSinceStartup;
        }

        text.text = _FPS.ToString(format);
    }
}