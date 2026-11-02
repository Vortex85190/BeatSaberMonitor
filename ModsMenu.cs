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
                new BSMenuButton("HardwareHud", "Choisir les infos affichées par le HUD de monitoring", Show));
        }

        static void Show()
        {
            if (_flow == null) _flow = BeatSaberUI.CreateFlowCoordinator<HudFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_flow);
        }
    }

    /// <summary>Écran qui s'ouvre depuis le bouton MODS, avec un bouton retour.</summary>
    internal class HudFlowCoordinator : FlowCoordinator
    {
        HudSettingsView _view;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("HardwareHud");
                showBackButton = true;
                _view = BeatSaberUI.CreateViewController<HudSettingsView>();
                ProvideInitialViewControllers(_view);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }

    /// <summary>Page de réglages (settings.bsml). Chaque changement est appliqué et sauvegardé tout de suite.</summary>
    [ViewDefinition("HardwareHud.settings.bsml")]
    internal class HudSettingsView : BSMLAutomaticViewController
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

        [UIValue("showVramTemp")]
        public bool ShowVramTemp { get { return Cfg.showVramTemp; } set { Cfg.showVramTemp = value; ApplyRows(); } }

        [UIValue("showInMenu")]
        public bool ShowInMenu { get { return Cfg.showInMenu; } set { Cfg.showInMenu = value; ApplyStyle(); } }

        [UIValue("showInSong")]
        public bool ShowInSong { get { return Cfg.showInSong; } set { Cfg.showInSong = value; ApplyStyle(); } }

        [UIValue("opacity")]
        public float Opacity { get { return Cfg.opacity; } set { Cfg.opacity = value; ApplyStyle(); } }
    }
}
