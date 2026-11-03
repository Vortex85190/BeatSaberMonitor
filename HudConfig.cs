using System;
using System.IO;
using IPA.Utilities;
using UnityEngine;

namespace HardwareHud
{
    /// <summary>Config sauvegardée dans UserData/HardwareHud.json (modifiable à la main).</summary>
    [Serializable]
    internal class HudConfig
    {
        public bool enabled = true;

        // Langue de l'interface : "auto" (langue du système), "en" ou "fr"
        public string language = "auto";
        public bool showInMenu = true;
        public bool showInSong = true;

        // Rafraîchissement des capteurs (ms). Le graphique couvre 120 échantillons.
        public int updateIntervalMs = 1000;
        public float opacity = 0.6f;

        // Quelles lignes afficher
        public bool showCpuTemp = true;
        public bool showCpuLoad = true;
        public bool showGpuTemp = true;
        public bool showGpuLoad = true;
        public bool showVram = true;
        public bool showVramTemp = true;
        public bool showGpuHotSpot = true;
        public bool showRam = true;

        // Seuils de couleur des températures (°C) : vert -> orange (warn) -> rouge (hot)
        public float cpuWarnTemp = 75f;
        public float cpuHotTemp = 85f;
        public float gpuWarnTemp = 75f;
        public float gpuHotTemp = 85f;
        public float vramWarnTemp = 85f;
        public float vramHotTemp = 95f;
        public float gpuHotSpotWarnTemp = 90f;
        public float gpuHotSpotHotTemp = 105f;

        // Seuils de couleur des utilisations (%), pour les lignes Usage et VRAM utilisée
        public bool colorLoad = false;
        public float loadWarn = 80f;
        public float loadHot = 95f;

        // Touche clavier pour le mode édition (en plus des 2 sticks cliqués en VR)
        public string editKey = "F8";

        // Position / rotation / taille — un profil pour le menu, un pour en jeu
        public Vector3 menuPosition = new Vector3(-1.9f, 1.3f, 2.0f);
        public Vector3 menuEuler = new Vector3(0f, -30f, 0f);
        public float menuScale = 1f;

        public Vector3 songPosition = new Vector3(-1.5f, 1.35f, 1.8f);
        public Vector3 songEuler = new Vector3(0f, -25f, 0f);
        public float songScale = 1f;

        static string FilePath => Path.Combine(UnityGame.UserDataPath, "HardwareHud.json");

        public static HudConfig Load()
        {
            HudConfig cfg = null;
            try
            {
                if (File.Exists(FilePath))
                    cfg = JsonUtility.FromJson<HudConfig>(File.ReadAllText(FilePath));
            }
            catch (Exception e)
            {
                Plugin.Log?.Warn("Config illisible, valeurs par défaut : " + e.Message);
            }
            cfg = cfg ?? new HudConfig();
            cfg.updateIntervalMs = Mathf.Clamp(cfg.updateIntervalMs, 250, 10000);
            cfg.Save(); // réécrit le fichier pour y ajouter les champs manquants
            return cfg;
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, JsonUtility.ToJson(this, true));
            }
            catch (Exception e)
            {
                Plugin.Log?.Warn("Impossible de sauvegarder la config : " + e.Message);
            }
        }
    }
}
