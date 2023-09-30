using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;
using Michsky.MUIP;

public class GameManager : MonoBehaviour
{
    #region variables
    [Serializable]
    /// <summary>
    /// Function definition for a click event.
    /// </summary>
    public class MyEvent : UnityEvent { }
    // Event delegates triggered on Pointer Down.
    [Space(5), Header("event run when gameObject awake"), Space(5)]

    [FormerlySerializedAs("EventCustom")]
    [SerializeField]
    private MyEvent m_event = new MyEvent();
    public MyEvent Event
    {
        get => m_event;
        set { m_event = value; }
    }
    [Space(5), Header("Properties"), Space(5)]
    [SerializeField] private RawImage _targetImageInput = null;
    [SerializeField] private RawImage _targetImageOutput = null;
    [SerializeField] private Slider _slider;
    [SerializeField] private Slider _threshold;
    [SerializeField]
    private WebGLUpDownloadManager webGLUpDownloadManager = null;
    [SerializeField] private LogTransformation logTransformation;
    [SerializeField] private NegativeTranformation negativeTranformation;
    [SerializeField] private ThresholdTransformation thresholdTransformation;
    private float current_slider, current_threshold;
    #endregion

    private void Awake()
    {
        Application.targetFrameRate = 60;
        this.EventActivate();
        this.current_slider = 50;
        this.current_threshold = 0.5f;
    }

    public void FixButton()
    {
#if UNITY_WEBGL
        if (Application.isMobilePlatform)
        {
            this.webGLUpDownloadManager.SelectLocalImage();
            Debug.Log("ok"); // Thiết bị là điện thoại, in ra "ok"
        }
        else
        {
            Debug.Log("web run!"); // Thiết bị không phải là điện thoại, in ra "no"
        }
#endif
    }


    private void EventActivate()
    {
        if (!this.gameObject.activeSelf)
            return;

        UISystemProfilerApi.AddMarker("MyEvent.EventCustom", this);
        m_event.Invoke();
    }
    public void SetNegativeTransformation()
    {
        this._targetImageOutput.texture = negativeTranformation.GetNegativeTransformation(this._targetImageInput.texture as Texture2D);
    }

    public void SetLogTransformation()
    {
        try
        {
            this._targetImageOutput.texture = logTransformation.GetLogTransformation(this._targetImageInput.texture as Texture2D, this._slider.value * 0.1f);
        }
        catch { };
    }

    public void SetThresholdingTransformation()
    {
        try
        {
            this._targetImageOutput.texture = thresholdTransformation.GetThresholdTransformation(this._targetImageInput.texture as Texture2D, this._threshold.value);
        }
        catch { };
    }

    public void RecoverImage()
    {
        try
        {
            this._targetImageOutput.texture = this._targetImageInput.texture;
        }
        catch { };
    }
}
