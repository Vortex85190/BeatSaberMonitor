using IPA;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace HardwareHud
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static IPALogger Log { get; private set; }
        internal static HudConfig Config { get; private set; }

        SensorReader _reader;
        GameObject _host;

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Config = HudConfig.Load();
            if (!Config.enabled)
            {
                Log.Info("HardwareHud désactivé dans UserData/HardwareHud.json");
                return;
            }

            _reader = new SensorReader(Config.updateIntervalMs, Log);
            _reader.Start();

            _host = new GameObject("HardwareHudHost");
            Object.DontDestroyOnLoad(_host);
            var hud = _host.AddComponent<HudController>();
            hud.Init(_reader, Config);

            try
            {
                SettingsMenu.Register(Config, hud);
            }
            catch (System.Exception e)
            {
                Log.Warn("Menu de réglages indisponible (BSML manquant ?) : " + e.Message);
            }

            Log.Info("HardwareHud démarré.");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Config?.Save();
            _reader?.Dispose();
        }
    }
}
