using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;

namespace HardwareHud
{
    /// <summary>Page "HardwareHud" dans Paramètres > Mods (BSML). Chaque changement est appliqué et sauvegardé tout de suite.</summary>
    internal sealed class SettingsMenu
    {
        readonly HudConfig _cfg;
        readonly HudController _hud;

        SettingsMenu(HudConfig cfg, HudController hud)
        {
            _cfg = cfg;
            _hud = hud;
        }

        // NoInlining : si BSML est absent, l'erreur se produit ici et est attrapée par l'appelant (Plugin.cs)
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Register(HudConfig cfg, HudController hud)
        {
            BSMLSettings.Instance.AddSettingsMenu("HardwareHud", "HardwareHud.settings.bsml", new SettingsMenu(cfg, hud));
        }

        // Changement de lignes affichées : on sauvegarde puis on reconstruit le panneau
        void ApplyRows()
        {
            _cfg.Save();
            _hud.Rebuild();
        }

        // Changement d'apparence / visibilité : pas besoin de reconstruire
        void ApplyStyle()
        {
            _cfg.Save();
            _hud.RefreshStyle();
        }

        [UIValue("showCpuTemp")]
        public bool ShowCpuTemp { get { return _cfg.showCpuTemp; } set { _cfg.showCpuTemp = value; ApplyRows(); } }

        [UIValue("showCpuLoad")]
        public bool ShowCpuLoad { get { return _cfg.showCpuLoad; } set { _cfg.showCpuLoad = value; ApplyRows(); } }

        [UIValue("showGpuTemp")]
        public bool ShowGpuTemp { get { return _cfg.showGpuTemp; } set { _cfg.showGpuTemp = value; ApplyRows(); } }

        [UIValue("showGpuLoad")]
        public bool ShowGpuLoad { get { return _cfg.showGpuLoad; } set { _cfg.showGpuLoad = value; ApplyRows(); } }

        [UIValue("showVram")]
        public bool ShowVram { get { return _cfg.showVram; } set { _cfg.showVram = value; ApplyRows(); } }

        [UIValue("showVramTemp")]
        public bool ShowVramTemp { get { return _cfg.showVramTemp; } set { _cfg.showVramTemp = value; ApplyRows(); } }

        [UIValue("showInMenu")]
        public bool ShowInMenu { get { return _cfg.showInMenu; } set { _cfg.showInMenu = value; ApplyStyle(); } }

        [UIValue("showInSong")]
        public bool ShowInSong { get { return _cfg.showInSong; } set { _cfg.showInSong = value; ApplyStyle(); } }

        [UIValue("opacity")]
        public float Opacity { get { return _cfg.opacity; } set { _cfg.opacity = value; ApplyStyle(); } }
    }
}
