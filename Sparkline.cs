using System;
using UnityEngine;

namespace HardwareHud
{
    /// <summary>Petit graphique historique (aire + courbe) dessiné dans une Texture2D.</summary>
    internal sealed class Sparkline : IDisposable
    {
        public readonly Texture2D Texture;
        readonly int _w, _h;
        readonly float[] _vals;
        readonly Color32[] _buf;
        int _count;

        public Sparkline(int width, int height)
        {
            _w = width;
            _h = height;
            _vals = new float[_w];
            _buf = new Color32[_w * _h];
            Texture = new Texture2D(_w, _h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            Draw(1f, Color.white);
        }

        public void Push(float value, float max, Color color)
        {
            Array.Copy(_vals, 1, _vals, 0, _w - 1);
            _vals[_w - 1] = value;
            if (_count < _w) _count++;
            Draw(max, color);
        }

        void Draw(float max, Color color)
        {
            if (max <= 0.0001f) max = 1f;

            var bg = new Color32(255, 255, 255, 18);
            var grid = new Color32(255, 255, 255, 40);
            Color32 fill = new Color(color.r, color.g, color.b, 0.35f);
            Color32 line = color;

            for (int i = 0; i < _buf.Length; i++) _buf[i] = bg;
            int mid = _h / 2;
            for (int x = 0; x < _w; x++) _buf[mid * _w + x] = grid;

            int prevY = -1;
            for (int x = _w - _count; x < _w; x++)
            {
                float n = Mathf.Clamp01(_vals[x] / max);
                int y = Mathf.Clamp(Mathf.RoundToInt(n * (_h - 1)), 0, _h - 1);

                for (int yy = 0; yy < y; yy++) _buf[yy * _w + x] = fill;

                int y0 = prevY < 0 ? y : prevY;
                int lo = Math.Min(y0, y), hi = Math.Max(y0, y);
                for (int yy = lo; yy <= hi; yy++) _buf[yy * _w + x] = line;
                prevY = y;
            }

            Texture.SetPixels32(_buf);
            Texture.Apply(false);
        }

        public void Dispose()
        {
            if (Texture != null) UnityEngine.Object.Destroy(Texture);
        }
    }
}
