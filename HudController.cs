using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

namespace HardwareHud
{
    /// <summary>
    /// Panneau world-space avec une ligne (texte + graphique) par métrique.
    /// Mode édition : 2 sticks cliqués en VR (ou la touche clavier de la config).
    ///   VR      : grip = attraper/déplacer le panneau, stick haut/bas pendant le grip = taille
    ///   Clavier : flèches = X/Y, PageUp/PageDown = profondeur, Q/E = rotation, +/- = taille
    /// Re-valider le mode édition sauvegarde la position (profil menu ou profil en jeu).
    /// </summary>
    internal sealed class HudController : MonoBehaviour
    {
        const float BaseScale = 0.003f;   // 250 px de large ≈ 0,75 m
        const float PanelW = 250f;
        const float RowH = 50f;
        const float Pad = 8f;
        const int GraphW = 120;
        const int GraphH = 24;

        sealed class Row
        {
            public string Label;
            public Color Color;
            public bool IsTemp;
            public Func<SensorSnapshot, float> Value;
            public Func<SensorSnapshot, float> Max;
            public Func<SensorSnapshot, string> Text;
            public Sparkline Graph;
            public TextMeshProUGUI ValueText;
        }

        SensorReader _reader;
        HudConfig _cfg;
        KeyCode _editKey = KeyCode.F8;

        readonly List<Row> _rows = new List<Row>();
        GameObject _root;
        Image _bg;
        TextMeshProUGUI _editLabel;
        TMP_FontAsset _font;

        bool _built, _inSong, _edit, _prevCombo;
        float _nextRefresh;
        float _scale = 1f;

        XRNode? _grabNode;
        Vector3 _grabPos;
        Quaternion _grabRot;

        public void Init(SensorReader reader, HudConfig cfg)
        {
            _reader = reader;
            _cfg = cfg;
            KeyCode k;
            _editKey = Enum.TryParse(cfg.editKey, true, out k) ? k : KeyCode.F8;
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        void OnDestroy()
        {
            foreach (var r in _rows) r.Graph?.Dispose();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "GameCore") SetContext(true);
        }

        void OnSceneUnloaded(Scene scene)
        {
            if (scene.name == "GameCore") SetContext(false);
        }

        void SetContext(bool inSong)
        {
            if (_edit) SetEdit(false);
            _inSong = inSong;
            _scale = inSong ? _cfg.songScale : _cfg.menuScale;
            ApplyTransform();
        }

        // ---------------------------------------------------------------- construction

        void TryBuild()
        {
            var fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            _font = fonts.FirstOrDefault(f => f.name == "Teko-Medium SDF") ?? fonts.FirstOrDefault();
            if (_font == null) return; // le jeu n'a pas encore chargé ses polices, on réessaie

            if (_cfg.showCpuTemp) _rows.Add(new Row
            {
                Label = "CPU Temp", Color = new Color(1f, 0.6f, 0.2f), IsTemp = true,
                Value = s => s.CpuTemp, Max = s => 100f,
                Text = s => s.CpuTemp.ToString("0") + " °C"
            });
            if (_cfg.showCpuLoad) _rows.Add(new Row
            {
                Label = "CPU Usage", Color = new Color(0.35f, 0.8f, 1f),
                Value = s => s.CpuLoad, Max = s => 100f,
                Text = s => s.CpuLoad.ToString("0") + " %"
            });
            if (_cfg.showGpuTemp) _rows.Add(new Row
            {
                Label = "GPU Temp", Color = new Color(1f, 0.6f, 0.2f), IsTemp = true,
                Value = s => s.GpuTemp, Max = s => 100f,
                Text = s => s.GpuTemp.ToString("0") + " °C"
            });
            if (_cfg.showGpuLoad) _rows.Add(new Row
            {
                Label = "GPU Usage", Color = new Color(0.35f, 1f, 0.6f),
                Value = s => s.GpuLoad, Max = s => 100f,
                Text = s => s.GpuLoad.ToString("0") + " %"
            });
            if (_cfg.showVram) _rows.Add(new Row
            {
                Label = "VRAM", Color = new Color(0.75f, 0.5f, 1f),
                Value = s => s.VramUsedGb,
                Max = s => float.IsNaN(s.VramTotalGb) ? 8f : s.VramTotalGb,
                Text = s => s.VramUsedGb.ToString("0.0") + " / " + s.VramTotalGb.ToString("0.0") + " Go"
            });
            if (_cfg.showVramTemp) _rows.Add(new Row
            {
                Label = "VRAM Temp", Color = new Color(1f, 0.6f, 0.2f), IsTemp = true,
                Value = s => s.VramTemp, Max = s => 100f,
                Text = s => s.VramTemp.ToString("0") + " °C"
            });

            float height = _rows.Count * RowH + Pad * 2f;

            _root = new GameObject("HardwareHudPanel", typeof(RectTransform));
            _root.transform.SetParent(transform, false);
            var canvas = _root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            ((RectTransform)_root.transform).sizeDelta = new Vector2(PanelW, height);

            // Fond
            var bgGo = new GameObject("Background", typeof(RectTransform));
            bgGo.transform.SetParent(_root.transform, false);
            _bg = bgGo.AddComponent<Image>();
            _bg.raycastTarget = false;
            var bgRt = _bg.rectTransform;
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = bgRt.offsetMax = Vector2.zero;
            SetBackground(false);

            // Lignes
            for (int i = 0; i < _rows.Count; i++)
            {
                var row = _rows[i];
                float y = Pad + i * RowH;

                MakeText(_root.transform, "Label" + i, Pad, y, 120f, 20f, TextAlignmentOptions.Left, 15f,
                    new Color(0.8f, 0.8f, 0.8f)).text = row.Label;
                row.ValueText = MakeText(_root.transform, "Value" + i, PanelW - Pad - 130f, y, 130f, 20f,
                    TextAlignmentOptions.Right, 16f, Color.white);

                row.Graph = new Sparkline(GraphW, GraphH);
                var imgGo = new GameObject("Graph" + i, typeof(RectTransform));
                imgGo.transform.SetParent(_root.transform, false);
                var raw = imgGo.AddComponent<RawImage>();
                raw.texture = row.Graph.Texture;
                raw.raycastTarget = false;
                var rt = raw.rectTransform;
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(Pad, -(y + 22f));
                rt.sizeDelta = new Vector2(PanelW - Pad * 2f, RowH - 26f);
            }

            // Bandeau du mode édition (au-dessus du panneau)
            _editLabel = MakeText(_root.transform, "EditLabel", 0f, -24f, PanelW, 20f, TextAlignmentOptions.Left, 11f,
                new Color(1f, 0.9f, 0.3f));
            _editLabel.text = "EDITION - grip: deplacer | stick haut/bas: taille | " + _editKey + ": valider";
            _editLabel.gameObject.SetActive(false);

            _built = true;
            _scale = _inSong ? _cfg.songScale : _cfg.menuScale;
            ApplyTransform();
            Refresh();
        }

        TextMeshProUGUI MakeText(Transform parent, string name, float x, float y, float w, float h,
            TextAlignmentOptions align, float size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(x, -y);
            rt.sizeDelta = new Vector2(w, h);

            var t = go.AddComponent<TextMeshProUGUI>();
            t.font = _font;
            t.fontSize = size;
            t.alignment = align;
            t.color = color;
            t.enableWordWrapping = false;
            t.overflowMode = TextOverflowModes.Overflow;
            t.raycastTarget = false;
            return t;
        }

        void SetBackground(bool edit)
        {
            _bg.color = edit
                ? new Color(0.25f, 0.2f, 0f, Mathf.Max(_cfg.opacity, 0.8f))
                : new Color(0f, 0f, 0f, _cfg.opacity);
        }

        // ---------------------------------------------------------------- position

        void ApplyTransform()
        {
            if (!_built) return;
            var pos = _inSong ? _cfg.songPosition : _cfg.menuPosition;
            var euler = _inSong ? _cfg.songEuler : _cfg.menuEuler;
            _root.transform.SetPositionAndRotation(pos, Quaternion.Euler(euler));
            ApplyScale();
            _root.SetActive(_inSong ? _cfg.showInSong : _cfg.showInMenu);
        }

        void ApplyScale()
        {
            _root.transform.localScale = Vector3.one * (BaseScale * _scale);
        }

        void SaveTransform()
        {
            var t = _root.transform;
            if (_inSong)
            {
                _cfg.songPosition = t.position;
                _cfg.songEuler = t.rotation.eulerAngles;
                _cfg.songScale = _scale;
            }
            else
            {
                _cfg.menuPosition = t.position;
                _cfg.menuEuler = t.rotation.eulerAngles;
                _cfg.menuScale = _scale;
            }
            _cfg.Save();
        }

        void SetEdit(bool on)
        {
            if (!_built || _edit == on) return;
            _edit = on;
            _grabNode = null;
            _editLabel.gameObject.SetActive(on);
            SetBackground(on);
            if (on) _root.SetActive(true);   // visible même si masqué dans ce contexte
            else
            {
                SaveTransform();
                _root.SetActive(_inSong ? _cfg.showInSong : _cfg.showInMenu);
            }
        }

        // ---------------------------------------------------------------- boucle

        void Update()
        {
            if (!_built)
            {
                TryBuild();
                if (!_built) return;
            }

            bool bothSticks = StickClick(XRNode.LeftHand) && StickClick(XRNode.RightHand);
            bool combo = bothSticks && !_prevCombo;
            _prevCombo = bothSticks;
            if (Input.GetKeyDown(_editKey) || combo) SetEdit(!_edit);

            if (_edit)
            {
                HandleVrGrab();
                HandleKeyboard();
            }

            if (Time.unscaledTime >= _nextRefresh)
            {
                _nextRefresh = Time.unscaledTime + _cfg.updateIntervalMs / 1000f;
                Refresh();
            }
        }

        /// <summary>Reconstruit le panneau (après changement des lignes affichées). L'historique des graphiques repart de zéro.</summary>
        public void Rebuild()
        {
            if (_edit) SetEdit(false);
            foreach (var r in _rows) r.Graph?.Dispose();
            _rows.Clear();
            if (_root != null) Destroy(_root);
            _root = null;
            _grabNode = null;
            _built = false; // Update() reconstruit au frame suivant
        }

        /// <summary>Applique opacité et visibilité menu/partie sans reconstruire.</summary>
        public void RefreshStyle()
        {
            if (!_built) return;
            SetBackground(_edit);
            if (!_edit) _root.SetActive(_inSong ? _cfg.showInSong : _cfg.showInMenu);
        }

        void Refresh()
        {
            var s = _reader.Latest;
            foreach (var row in _rows)
            {
                float v = row.Value(s);
                bool ok = !float.IsNaN(v);
                row.ValueText.text = ok ? row.Text(s) : "N/A";

                Color c = row.IsTemp ? TempColor(v) : row.Color;
                row.ValueText.color = ok ? (row.IsTemp ? c : Color.white) : new Color(0.6f, 0.6f, 0.6f);
                row.Graph.Push(ok ? v : 0f, row.Max(s), c);
            }
        }

        Color TempColor(float v)
        {
            if (float.IsNaN(v)) return new Color(0.6f, 0.6f, 0.6f);
            if (v >= _cfg.hotTemp) return new Color(1f, 0.3f, 0.3f);
            if (v >= _cfg.warnTemp) return new Color(1f, 0.65f, 0.2f);
            return new Color(0.35f, 0.9f, 0.45f);
        }

        // ---------------------------------------------------------------- entrées

        static bool StickClick(XRNode node)
        {
            var dev = InputDevices.GetDeviceAtXRNode(node);
            bool b;
            return dev.isValid && dev.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out b) && b;
        }

        void HandleVrGrab()
        {
            foreach (var node in new[] { XRNode.LeftHand, XRNode.RightHand })
            {
                var dev = InputDevices.GetDeviceAtXRNode(node);
                if (!dev.isValid) continue;

                bool grip;
                if (!dev.TryGetFeatureValue(CommonUsages.gripButton, out grip)) grip = false;

                Vector3 p;
                Quaternion q;
                if (!dev.TryGetFeatureValue(CommonUsages.devicePosition, out p)) continue;
                if (!dev.TryGetFeatureValue(CommonUsages.deviceRotation, out q)) continue;

                var t = _root.transform;

                if (grip && _grabNode == null)
                {
                    _grabNode = node;
                    var inv = Quaternion.Inverse(q);
                    _grabPos = inv * (t.position - p);
                    _grabRot = inv * t.rotation;
                }

                if (_grabNode == node)
                {
                    if (!grip) { _grabNode = null; continue; }

                    t.SetPositionAndRotation(p + q * _grabPos, q * _grabRot);

                    Vector2 axis;
                    if (dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis) && Mathf.Abs(axis.y) > 0.2f)
                    {
                        _scale = Mathf.Clamp(_scale * (1f + axis.y * Time.unscaledDeltaTime), 0.3f, 3f);
                        ApplyScale();
                    }
                }
            }
        }

        void HandleKeyboard()
        {
            float dt = Time.unscaledDeltaTime;
            var t = _root.transform;

            float x = (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
            float y = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
            float z = (Input.GetKey(KeyCode.PageUp) ? 1f : 0f) - (Input.GetKey(KeyCode.PageDown) ? 1f : 0f);
            if (x != 0f || y != 0f || z != 0f)
                t.position += new Vector3(x, y, z) * (1f * dt);

            // 1 = rotation vers la gauche, 2 = rotation vers la droite (vu du joueur)
            float yaw = ((Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1)) ? 1f : 0f)
                        - ((Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2)) ? 1f : 0f);
            if (yaw != 0f) t.Rotate(0f, yaw * 60f * dt, 0f, Space.World);

            float sc = (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus) ? 1f : 0f)
                       - (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus) ? 1f : 0f);
            if (sc != 0f)
            {
                _scale = Mathf.Clamp(_scale * (1f + sc * dt), 0.3f, 3f);
                ApplyScale();
            }
        }
    }
}
