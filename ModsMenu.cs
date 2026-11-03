using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using BSMenuButton = BeatSaberMarkupLanguage.MenuButtons.MenuButton;
using BSMenuButtons = BeatSaberMarkupLanguage.MenuButtons.MenuButtons;

namespace HardwareHud
{
    /// <summary>Ajoute le bouton "HardwareHud" dans le menu MODS du jeu.</summary>
    internal static class ModsMenu
    {
        static HudFlowCoordinator _flow;

        // NoInlining : si BSML est absent, l'erreur se produit ici et est attrapée par l'appelant (Plugin.cs)
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Register()
        {
            BSMenuButtons.Instance.RegisterButton(
                new BSMenuButton("HardwareHud", Lang.ButtonHint, Show));
        }

        static void Show()
        {
            // Nouveau coordinateur à chaque ouverture : la page est recréée dans la langue choisie
            if (_flow != null) UnityEngine.Object.Destroy(_flow);
            _flow = BeatSaberUI.CreateFlowCoordinator<HudFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_flow);
        }
    }

    /// <summary>Écran qui s'ouvre depuis le bouton MODS, avec un bouton retour.</summary>
    internal class HudFlowCoordinator : FlowCoordinator
    {
        ViewController _view;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("HardwareHud");
                showBackButton = true;
                if (Lang.IsFrench) _view = BeatSaberUI.CreateViewController<HudSettingsViewFr>();
                else _view = BeatSaberUI.CreateViewController<HudSettingsViewEn>();
                ProvideInitialViewControllers(_view);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }

    [ViewDefinition("HardwareHud.settings_fr.bsml")]
    internal sealed class HudSettingsViewFr : HudSettingsViewBase { }

    [ViewDefinition("HardwareHud.settings_en.bsml")]
    internal sealed class HudSettingsViewEn : HudSettingsViewBase { }

    /// <summary>Page de réglages (settings_fr/en.bsml). Chaque changement est appliqué et sauvegardé tout de suite.</summary>
    internal abstract class HudSettingsViewBase : BSMLAutomaticViewController
    {
        static HudConfig Cfg { get { return Plugin.Config; } }
        static HudController Hud { get { return Plugin.Hud; } }

        // Changement de lignes affichées : on sauvegarde puis on reconstruit le panneau
        static void ApplyRows()
        {
            Cfg.Save();
            if (Hud != null) Hud.Rebuild();
        }

        // Changement d'apparence / visibilité : pas besoin de reconstruire
        static void ApplyStyle()
        {
            Cfg.Save();
            if (Hud != null) Hud.RefreshStyle();
        }

        [UIValue("showCpuTemp")]
        public bool ShowCpuTemp { get { return Cfg.showCpuTemp; } set { Cfg.showCpuTemp = value; ApplyRows(); } }

        [UIValue("showCpuLoad")]
        public bool ShowCpuLoad { get { return Cfg.showCpuLoad; } set { Cfg.showCpuLoad = value; ApplyRows(); } }

        [UIValue("showGpuTemp")]
        public bool ShowGpuTemp { get { return Cfg.showGpuTemp; } set { Cfg.showGpuTemp = value; ApplyRows(); } }

        [UIValue("showGpuLoad")]
        public bool ShowGpuLoad { get { return Cfg.showGpuLoad; } set { Cfg.showGpuLoad = value; ApplyRows(); } }

        [UIValue("showVram")]
        public bool ShowVram { get { return Cfg.showVram; } set { Cfg.showVram = value; ApplyRows(); } }

        [UIValue("showRam")]
        public bool ShowRam { get { return Cfg.showRam; } set { Cfg.showRam = value; ApplyRows(); } }

        [UIValue("showGpuHotSpot")]
        public bool ShowGpuHotSpot { get { return Cfg.showGpuHotSpot; } set { Cfg.showGpuHotSpot = value; ApplyRows(); } }

        [UIValue("showVramTemp")]
        public bool ShowVramTemp { get { return Cfg.showVramTemp; } set { Cfg.showVramTemp = value; ApplyRows(); } }

        [UIValue("showInMenu")]
        public bool ShowInMenu { get { return Cfg.showInMenu; } set { Cfg.showInMenu = value; ApplyStyle(); } }

        [UIValue("showInSong")]
        public bool ShowInSong { get { return Cfg.showInSong; } set { Cfg.showInSong = value; ApplyStyle(); } }

        [UIValue("opacity")]
        public float Opacity { get { return Cfg.opacity; } set { Cfg.opacity = value; ApplyStyle(); } }

        [UIValue("languages")]
        public List<object> Languages = new List<object> { "auto", "en", "fr" };

        [UIValue("language")]
        public string Language
        {
            get { return Cfg.language; }
            set { Cfg.language = value; ApplyRows(); }   // appliqué au prochain affichage du menu
        }

        [UIValue("interval")]
        public float Interval
        {
            get { return Cfg.updateIntervalMs; }
            set
            {
                int ms = (int)System.Math.Round(value);
                Cfg.updateIntervalMs = ms;
                if (Plugin.Reader != null) Plugin.Reader.IntervalMs = ms;
                Cfg.Save();
            }
        }

        // Seuils de température : le menu ne change que la couleur, pas besoin de reconstruire le panneau
        [UIValue("cpuWarnTemp")]
        public float CpuWarnTemp { get { return Cfg.cpuWarnTemp; } set { Cfg.cpuWarnTemp = value; Cfg.Save(); } }

        [UIValue("cpuHotTemp")]
        public float CpuHotTemp { get { return Cfg.cpuHotTemp; } set { Cfg.cpuHotTemp = value; Cfg.Save(); } }

        [UIValue("gpuWarnTemp")]
        public float GpuWarnTemp { get { return Cfg.gpuWarnTemp; } set { Cfg.gpuWarnTemp = value; Cfg.Save(); } }

        [UIValue("gpuHotTemp")]
        public float GpuHotTemp { get { return Cfg.gpuHotTemp; } set { Cfg.gpuHotTemp = value; Cfg.Save(); } }

        [UIValue("vramWarnTemp")]
        public float VramWarnTemp { get { return Cfg.vramWarnTemp; } set { Cfg.vramWarnTemp = value; Cfg.Save(); } }

        [UIValue("vramHotTemp")]
        public float VramHotTemp { get { return Cfg.vramHotTemp; } set { Cfg.vramHotTemp = value; Cfg.Save(); } }

        [UIValue("gpuHotSpotWarnTemp")]
        public float GpuHotSpotWarnTemp { get { return Cfg.gpuHotSpotWarnTemp; } set { Cfg.gpuHotSpotWarnTemp = value; Cfg.Save(); } }

        [UIValue("gpuHotSpotHotTemp")]
        public float GpuHotSpotHotTemp { get { return Cfg.gpuHotSpotHotTemp; } set { Cfg.gpuHotSpotHotTemp = value; Cfg.Save(); } }

        [UIValue("colorLoad")]
        public bool ColorLoad { get { return Cfg.colorLoad; } set { Cfg.colorLoad = value; Cfg.Save(); } }

        [UIValue("loadWarn")]
        public float LoadWarn { get { return Cfg.loadWarn; } set { Cfg.loadWarn = value; Cfg.Save(); } }

        [UIValue("loadHot")]
        public float LoadHot { get { return Cfg.loadHot; } set { Cfg.loadHot = value; Cfg.Save(); } }
    }
}
