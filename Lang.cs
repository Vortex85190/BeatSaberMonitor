using UnityEngine;

namespace HardwareHud
{
    /// <summary>Langue de l'interface : "auto" (langue du système), "en" ou "fr" (champ language de la config).</summary>
    internal static class Lang
    {
        public static bool IsFrench
        {
            get
            {
                string l = Plugin.Config != null ? Plugin.Config.language : "auto";
                if (l == "fr") return true;
                if (l == "en") return false;
                return Application.systemLanguage == SystemLanguage.French;
            }
        }

        public static string MemUnit { get { return IsFrench ? "Go" : "GB"; } }

        public static string ButtonHint
        {
            get
            {
                return IsFrench
                    ? "Choisir les infos affichées par le HUD de monitoring"
                    : "Choose which info the monitoring HUD shows";
            }
        }

        public static string EditBanner(KeyCode key)
        {
            return IsFrench
                ? "EDITION - grip: deplacer | stick haut/bas: taille | " + key + ": valider"
                : "EDIT - grip: move | stick up/down: size | " + key + ": confirm";
        }
    }
}
