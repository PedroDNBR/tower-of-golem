using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

namespace TW
{
    public class SettingsMenu : BaseMenu
    {
        [Header("UI Interactables")]
        [SerializeField] private Button backButton;
        [SerializeField] private Toggle bloomToggle;
        [SerializeField] private Toggle ambientOcclusionToggle;
        [SerializeField] private Toggle vignetteToggle;
        [SerializeField] private Toggle tonemappingToggle;

        [Header("Post Processings")]
        [SerializeField] VolumeProfile volumeProfile;
        [SerializeField] ScriptableRendererFeature[] ambientOcclusions;

        [Header("Window Mode")]
        [SerializeField] TMP_Dropdown windowDropdown;

        [Header("Quality")]
        [SerializeField] TMP_Dropdown qualityDropdown;
        [SerializeField] RenderPipelineAsset[] qualities;


        private void OnEnable()
        {
            SetToggles();
            SetDrowndownOptions();
            SetListeners();

        }

        private void SetToggles()
        {
            Bloom tempBloom;
            volumeProfile.TryGet(out tempBloom);
            SetBloomToggle(tempBloom.active);

            Vignette tempVig;
            volumeProfile.TryGet(out tempVig);
            SetVignetteToggle(tempVig.active);

            SetAmbientOcclusionToggle(ambientOcclusions[QualitySettings.GetQualityLevel()]);

            Vignette tempTone;
            volumeProfile.TryGet(out tempTone);
            SetTonemappingToggle(tempTone.active);
        }

        private void SetDrowndownOptions()
        {
            SetScreenModeDropdownOption(Screen.fullScreenMode);

            SetQualityDropdownOption(QualitySettings.GetQualityLevel());
        }

        private void SetListeners()
        {
            backButton.onClick.AddListener(BackToStartMenu);

            bloomToggle.onValueChanged.AddListener(SetBloom);
            vignetteToggle.onValueChanged.AddListener(SetVignette);
            ambientOcclusionToggle.onValueChanged.AddListener(SetAmbientOcclusion);
            tonemappingToggle.onValueChanged.AddListener(SetTonemapping);

            windowDropdown.onValueChanged.AddListener(SetScreenMode);
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }

        private void BackToStartMenu()
        {
            graphicsMenuTransform.gameObject.SetActive(false);
            startMenuTransform.gameObject.SetActive(true);
        }

        private void SetBloom(bool value)
        {
            Bloom tempBloom;
            if (volumeProfile.TryGet(out tempBloom))
            {
                tempBloom.active = value;
            }
        }

        private void SetBloomToggle(bool active) => bloomToggle.isOn = active;



        private void SetAmbientOcclusion(bool value)
        {
            foreach (var ambientOcclusion in ambientOcclusions)
                ambientOcclusion.SetActive(value);
        }

        private void SetAmbientOcclusionToggle(bool active) => ambientOcclusionToggle.isOn = active;


        private void SetVignette(bool value)
        {
            Vignette tempVignette;
            if (volumeProfile.TryGet(out tempVignette))
            {
                tempVignette.active = value;
            }
        }

        private void SetVignetteToggle(bool active) => vignetteToggle.isOn = active;

        private void SetTonemapping(bool value)
        {
            Tonemapping tempTonemapping;
            if (volumeProfile.TryGet(out tempTonemapping))
            {
                tempTonemapping.active = value;
            }
        }

        private void SetTonemappingToggle(bool active) => tonemappingToggle.isOn = active;

        private void SetScreenMode(int fullscreenMode)
        {
            switch (fullscreenMode)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    return;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    return;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    return;
            }
        }

        private void SetScreenModeDropdownOption(FullScreenMode fullscreenMode)
        {
            switch (fullscreenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    windowDropdown.SetValueWithoutNotify(0);
                    return;
                case FullScreenMode.FullScreenWindow:
                    windowDropdown.SetValueWithoutNotify(1);
                    return;
                case FullScreenMode.Windowed:
                    windowDropdown.SetValueWithoutNotify(2);
                    return;
            }
        }

        private void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            QualitySettings.renderPipeline = qualities[qualityIndex];
            SetAmbientOcclusionToggle(ambientOcclusions[qualityIndex]);
        }

        private void SetQualityDropdownOption(int qualityIndex) => qualityDropdown.SetValueWithoutNotify(qualityIndex);
    }
}
