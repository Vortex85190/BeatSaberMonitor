using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;

namespace HardwareHud
{
    /// <summary>Dernière mesure. NaN = capteur indisponible.</summary>
    internal sealed class SensorSnapshot
    {
        public float CpuTemp = float.NaN;
        public float CpuLoad = float.NaN;
        public float GpuTemp = float.NaN;
        public float GpuLoad = float.NaN;
        public float VramUsedGb = float.NaN;
        public float VramTotalGb = float.NaN;
        public float VramTemp = float.NaN;
        public float GpuHotSpot = float.NaN;
        public float RamUsedGb = float.NaN;
        public float RamTotalGb = float.NaN;
    }

    /// <summary>
    /// Lit les capteurs dans un thread séparé (hardware.Update() est lent) via LibreHardwareMonitorLib.
    /// Note : la température CPU nécessite en général les droits administrateur (pilote du lib).
    /// </summary>
    internal sealed class SensorReader : IDisposable
    {
        volatile int _intervalMs;

        /// <summary>Intervalle de lecture (ms), modifiable à chaud depuis le menu.</summary>
        public int IntervalMs
        {
            get { return _intervalMs; }
            set { _intervalMs = Math.Max(250, Math.Min(10000, value)); }
        }
        readonly IPA.Logging.Logger _log;
        Computer _pc;
        Thread _thread;
        volatile bool _run;
        bool _warned;
        bool _logged;
        volatile SensorSnapshot _latest = new SensorSnapshot();

        public SensorSnapshot Latest => _latest;

        PerformanceCounter _cpuLoadCounter;
        bool _isAdministrator;

        public SensorReader(int intervalMs, IPA.Logging.Logger log)
        {
            _intervalMs = intervalMs;
            _log = log;
        }

        public void Start()
        {
            try
            {
                _isAdministrator = new System.Security.Principal.WindowsPrincipal(
                    System.Security.Principal.WindowsIdentity.GetCurrent())
                    .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

                _cpuLoadCounter = new PerformanceCounter(
                    "Processor",
                    "% Processor Time",
                    "_Total",
                    true);

                _pc = new Computer
                {
                    IsCpuEnabled = _isAdministrator,
                    IsGpuEnabled = true,
                    IsMemoryEnabled = true
                };

                _pc.Open();
            }
            catch (Exception e)
            {
                _log.Error("Initialisation des capteurs impossible : " + e);
                _pc = null;
                return;
            }

            _run = true;
            _thread = new Thread(Loop) { IsBackground = true, Name = "HardwareHud-Sensors" };
            _thread.Start();
        }

        void Loop()
        {
            while (_run)
            {
                try
                {
                    _latest = Poll();
                }
                catch (Exception e)
                {
                    if (!_warned) { _warned = true; _log.Warn("Erreur de lecture capteurs : " + e.Message); }
                }
                Thread.Sleep(_intervalMs);
            }
        }

        SensorSnapshot Poll()
        {
            foreach (var hw in _pc.Hardware) UpdateRec(hw);

            var s = new SensorSnapshot();

            var cpu = _pc.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            // GPU dédié en priorité, sinon iGPU Intel
            var gpu = _pc.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuNvidia || h.HardwareType == HardwareType.GpuAmd)
                      ?? _pc.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuIntel);

            if (!_logged)
            {
                _logged = true;
                LogSensors(cpu, gpu);
            }

            if (cpu != null)
            {
                s.CpuTemp = CpuTemperature(cpu);
            }

            try
            {
                s.CpuLoad = _cpuLoadCounter != null
                    ? _cpuLoadCounter.NextValue()
                    : float.NaN;
            }
            catch
            {
                s.CpuLoad = float.NaN;
            }

            if (gpu != null)
            {
                s.GpuTemp = FirstOf(gpu, SensorType.Temperature, "GPU Core", "GPU Hot Spot", "GPU Package");
                s.GpuLoad = Find(gpu, SensorType.Load, "GPU Core");
                s.VramUsedGb = Find(gpu, SensorType.SmallData, "GPU Memory Used") / 1024f;
                s.VramTotalGb = Find(gpu, SensorType.SmallData, "GPU Memory Total") / 1024f;
                s.VramTemp = VramTemperature(gpu);
                s.GpuHotSpot = FirstOfNamed(gpu, "GPU Hot Spot");
            }

            var mem = _pc.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
            if (mem != null)
            {
                float used = Find(mem, SensorType.Data, "Memory Used");
                float avail = Find(mem, SensorType.Data, "Memory Available");
                if (!float.IsNaN(used))
                {
                    s.RamUsedGb = used;
                    if (!float.IsNaN(avail)) s.RamTotalGb = used + avail;
                }
            }

            return s;
        }

        // Diagnostic écrit une seule fois dans le log BSIPA (Logs\...) : droits admin + capteurs de temp CPU vus
        void LogSensors(IHardware cpu, IHardware gpu)
        {
            try
            {
                bool admin = new System.Security.Principal.WindowsPrincipal(
                    System.Security.Principal.WindowsIdentity.GetCurrent())
                    .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                _log.Info("HardwareHud diag - admin: " + admin);
                foreach (var hw in new[] { cpu, gpu })
                {
                    if (hw == null) continue;
                    _log.Info("HardwareHud diag - " + hw.HardwareType + ": " + hw.Name);
                    foreach (var x in AllSensors(hw).Where(x => x.SensorType == SensorType.Temperature))
                        _log.Info("HardwareHud diag -   temp '" + x.Name + "' = " + (x.Value.HasValue ? x.Value.Value.ToString("0.0") : "null"));
                }
            }
            catch (Exception e)
            {
                _log.Warn("HardwareHud diag impossible : " + e.Message);
            }
        }

        static void UpdateRec(IHardware hw)
        {
            hw.Update();
            foreach (var sub in hw.SubHardware) UpdateRec(sub);
        }

        static IEnumerable<ISensor> AllSensors(IHardware hw)
        {
            return hw.Sensors.Concat(hw.SubHardware.SelectMany(AllSensors));
        }

        static float Find(IHardware hw, SensorType type, string name)
        {
            var sensor = AllSensors(hw).FirstOrDefault(x => x.SensorType == type && x.Name == name);
            return sensor?.Value ?? float.NaN;
        }

        static float FirstOf(IHardware hw, SensorType type, params string[] names)
        {
            foreach (var n in names)
            {
                var v = Find(hw, type, n);
                if (!float.IsNaN(v) && v > 0.5f) return v;
            }
            var any = AllSensors(hw).FirstOrDefault(x => x.SensorType == type && x.Value.HasValue && x.Value.Value > 0.5f);
            return any?.Value ?? float.NaN;
        }

        static float VramTemperature(IHardware gpu)
        {
            var v = FirstOfNamed(gpu, "GPU Memory Junction", "GPU Memory", "GPU VRAM");
            if (!float.IsNaN(v)) return v;

            var m = AllSensors(gpu).FirstOrDefault(x =>
                x.SensorType == SensorType.Temperature && x.Value.HasValue && x.Value.Value > 0.5f &&
                (x.Name.IndexOf("Memory", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 x.Name.IndexOf("VRAM", StringComparison.OrdinalIgnoreCase) >= 0));
            return m?.Value ?? float.NaN;
        }

        static float CpuTemperature(IHardware cpu)
        {
            // Intel : "CPU Package" ; AMD : "Core (Tctl/Tdie)" ou "Core (Tdie)"
            var v = FirstOfNamed(cpu, "CPU Package", "Core (Tctl/Tdie)", "Core (Tdie)", "Core Max");
            if (!float.IsNaN(v) && v > 0.5f) return v;

            // Sinon : moyenne des "Core #n"
            var cores = AllSensors(cpu)
                .Where(x => x.SensorType == SensorType.Temperature && x.Name.StartsWith("Core #") && x.Value.HasValue && x.Value.Value > 0.5f)
                .Select(x => x.Value.Value).ToList();
            if (cores.Count > 0) return cores.Average();

            var any = AllSensors(cpu).FirstOrDefault(x => x.SensorType == SensorType.Temperature && x.Value.HasValue && x.Value.Value > 0.5f);
            return any?.Value ?? float.NaN;
        }

        static float FirstOfNamed(IHardware hw, params string[] names)
        {
            foreach (var n in names)
            {
                var v = Find(hw, SensorType.Temperature, n);
                if (!float.IsNaN(v) && v > 0.5f) return v;
            }
            return float.NaN;
        }

        public void Dispose()
        {
            _run = false;
            try { _thread?.Join(1500); } catch { }
            try { _pc?.Close(); } catch { }
            try
            {
                _cpuLoadCounter?.Dispose();
            }
            catch
            {
            }
        }
    }
}
