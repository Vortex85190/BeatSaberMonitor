using UnityEngine;

namespace HardwareHud
{
    internal static class Lang
    {
        public static string Current
        {
            get
            {
                string language = Plugin.Config != null ? Plugin.Config.language : "auto";

                if (language == "fr" || language == "en" || language == "ja" || language == "ko")
                    return language;

                switch (Application.systemLanguage)
                {
                    case SystemLanguage.French:
                        return "fr";
                    case SystemLanguage.Japanese:
                        return "ja";
                    case SystemLanguage.Korean:
                        return "ko";
                    default:
                        return "en";
                }
            }
        }

        public static bool IsFrench { get { return Current == "fr"; } }
        public static bool IsJapanese { get { return Current == "ja"; } }
        public static bool IsKorean { get { return Current == "ko"; } }

        public static string MemUnit
        {
            get { return IsFrench ? "Go" : "GB"; }
        }

        public static string ButtonHint
        {
            get
            {
                switch (Current)
                {
                    case "fr":
                        return "Choisir les informations affichées par le HUD";
                    case "ja":
                        return "HUDに表示する情報を選択";
                    case "ko":
                        return "HUD에 표시할 정보를 선택";
                    default:
                        return "Choose which information the HUD shows";
                }
            }
        }

        public static string EditBanner(KeyCode key)
        {
            switch (Current)
            {
                case "fr":
                    return "EDITION - grip: déplacer | stick haut/bas: taille | " + key + ": valider";
                case "ja":
                    return "編集 - グリップ: 移動 | スティック上下: サイズ | " + key + ": 決定";
                case "ko":
                    return "편집 - 그립: 이동 | 스틱 위/아래: 크기 | " + key + ": 확인";
                default:
                    return "EDIT - grip: move | stick up/down: size | " + key + ": confirm";
            }
        }
    }
}
