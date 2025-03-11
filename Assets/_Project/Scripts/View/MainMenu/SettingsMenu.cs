using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Reflection;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

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
        [SerializeField] ScriptableRendererFeature ambientOcclusion;

        [Header("Window Mode")]
        [SerializeField] TMP_Dropdown windowDropdown;

        [Header("Quality")]
        [SerializeField] UniversalRenderPipelineAsset urpAsset;
        [SerializeField] TMP_Dropdown shadowDropdown;


        private void OnEnable()
        {
            backButton.onClick.AddListener(BackToStartMenu);

            bloomToggle.onValueChanged.AddListener(SetBloom);
            vignetteToggle.onValueChanged.AddListener(SetVignette);
            ambientOcclusionToggle.onValueChanged.AddListener(SetAmbientOcclusion);
            tonemappingToggle.onValueChanged.AddListener(SetTonemapping);

            SetScreenModeDropdownOption(Screen.fullScreenMode);

            SetShadowDropdownOption(
                urpAsset.mainLightRenderingMode,
                (ShadowResolution) urpAsset.mainLightShadowmapResolution
            );

            windowDropdown.onValueChanged.AddListener(SetScreenMode);
            shadowDropdown.onValueChanged.AddListener(SetShadow);
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

        private void SetAmbientOcclusion(bool value)
        {
            ambientOcclusion.SetActive(value);
        }

        private void SetVignette(bool value)
        {
            Vignette tempVignette;
            if (volumeProfile.TryGet(out tempVignette))
            {
                tempVignette.active = value;
            }
        }

        private void SetTonemapping(bool value)
        {
            Tonemapping tempTonemapping;
            if (volumeProfile.TryGet(out tempTonemapping))
            {
                tempTonemapping.active = value;
            }
        }

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

        private void SetShadow(int shadowQuality)
        {
            urpAsset.GetType().GetField("m_MainLightRenderingMode", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(urpAsset, LightRenderingMode.PerPixel);
            switch (shadowQuality)
            {
                case 0:
                    urpAsset.GetType().GetField("m_MainLightRenderingMode", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(urpAsset, LightRenderingMode.Disabled);
                    return;
                case 1:
                    urpAsset.GetType().GetField("m_MainLightShadowmapResolution", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(urpAsset, ShadowResolution._256);
                    return;
                case 2:
                    urpAsset.GetType().GetField("m_MainLightShadowmapResolution", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(urpAsset, ShadowResolution._512);
                    return;
                case 3:
                    urpAsset.GetType().GetField("m_MainLightShadowmapResolution", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(urpAsset, ShadowResolution._1024);
                    return;
                case 4:
                    urpAsset.GetType().GetField("m_MainLightShadowmapResolution", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(urpAsset, ShadowResolution._2048);
                    return;
                case 5:
                    urpAsset.GetType().GetField("m_MainLightShadowmapResolution", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(urpAsset, ShadowResolution._4096);
                    return;

            }
        }

        private void SetShadowDropdownOption(LightRenderingMode shadowEnabled, ShadowResolution shadowResolution)
        {
            switch(shadowEnabled)
            {
                case LightRenderingMode.Disabled:
                    shadowDropdown.SetValueWithoutNotify(0);
                    return;
            }

            switch (shadowResolution)
            {
                case ShadowResolution._256:
                    shadowDropdown.SetValueWithoutNotify(1);
                    return;
                case ShadowResolution._512:
                    shadowDropdown.SetValueWithoutNotify(2);
                    return;
                case ShadowResolution._1024:
                    shadowDropdown.SetValueWithoutNotify(3);
                    return;
                case ShadowResolution._2048:
                    shadowDropdown.SetValueWithoutNotify(4);
                    return;
                case ShadowResolution._4096:
                    shadowDropdown.SetValueWithoutNotify(5);
                    return;
            }
        }
    }
}
