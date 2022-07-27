using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private VideoLookup videoLookup;
    [SerializeField] private TMP_Dropdown videoDropdown;
    [SerializeField] private Button videoBrowser;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider rotationSlider;
    [SerializeField] private TMP_InputField urlInput;

    [HideInInspector] public UnityEvent<Video> OnVideoDropdownSelection = new();
    [HideInInspector] public UnityEvent<string> OnVideoUrlSelection = new();
    [HideInInspector] public UnityEvent<float> OnSpeedChange = new();
    [HideInInspector] public UnityEvent<float> OnRotationSpeedChange = new();

    private IEnumerable<string> videoDropdownOptions;

    private List<Selectable> Selectables => new() { videoDropdown, videoBrowser, speedSlider, rotationSlider, urlInput };

    private void Awake()
    {
        videoDropdownOptions = videoLookup.Videos.Select(x => x.Name).Prepend("Select a video...");
    }

    private void Start()
    {
        videoDropdown.SetOptions(videoDropdownOptions);
        videoDropdown.SetValueWithoutNotify(1);
        this.OnFrameEnd(BroadcastInitialSettings);
    }

    private void BroadcastInitialSettings()
    {
        OnVideoDropdownChange();
        OnSpeedSliderChange();
        OnRotationSliderChange();
    }

    public void OnVideoBrowserClick()
    {
        DisableSettings();

        VideoExplorer.SelectVideo(
            x => {
                OnVideoUrlSelection.Invoke(x);
                EnableSettings();
                ResetUrlInput();
                ResetDropdown();
            },
            EnableSettings
        );
    }

    public void OnVideoDropdownChange()
    {
        if (videoDropdown.value == 0) return;

        ResetUrlInput();

        var videoName = videoDropdown.GetCurrentOption();

        var video = videoLookup.Videos.Single(x => x.Name == videoName);

        OnVideoDropdownSelection.Invoke(video);
    }

    public void OnUrlSubmit()
    {
        ResetDropdown();
        OnVideoUrlSelection.Invoke(urlInput.text);
    }

    public void OnSpeedSliderChange()
    {
        var speed = speedSlider.value / speedSlider.maxValue;
        OnSpeedChange.Invoke(speed);
    }

    public void OnRotationSliderChange()
    {
        var rotationSpeed = rotationSlider.value / rotationSlider.maxValue;
        OnRotationSpeedChange.Invoke(rotationSpeed);
    }

    private void EnableSettings()
    {
        Selectables.ForEach(x => x.interactable = true);
    }

    private void DisableSettings()
    {
        Selectables.ForEach(x => x.interactable = false);
    }

    private void ResetDropdown()
    {
        videoDropdown.value = 0;
    }

    private void ResetUrlInput()
    {
        urlInput.text = "";
    }
}
