using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Champions.Editor
{
    /// <summary>
    /// Captura screenshots da Game View durante o Play Mode para auxiliar na
    /// iteração de design de UI Toolkit.
    ///
    /// Menu: Champions → Tools → UI Screenshot Tool
    ///
    /// Uso via CLI (Play Mode já iniciado):
    ///   unity command script eval
    ///     "Champions.Editor.UIScreenshotTool.CaptureNow();"
    ///     --project-path /caminho/do/projeto
    ///
    /// A imagem é salva em: Assets/Screenshots/UI_yyyy-MM-dd_HH-mm-ss.png
    /// </summary>
    public class UIScreenshotTool : EditorWindow
    {
        // ── Constants ─────────────────────────────────────────────────────

        private const string OutputDir = "Assets/Screenshots";

        // ── Menu entry ────────────────────────────────────────────────────

        [MenuItem("Champions/Tools/UI Screenshot Tool")]
        public static void OpenWindow()
        {
            var win = GetWindow<UIScreenshotTool>("UI Screenshot");
            win.minSize = new Vector2(340, 160);
            win.Show();
        }

        // ── GUI ───────────────────────────────────────────────────────────

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("UI Screenshot Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            EditorGUILayout.HelpBox(
                "Capture a Game View durante o Play Mode para análise de design.\n" +
                "As imagens são salvas em Assets/Screenshots/.",
                MessageType.Info);

            EditorGUILayout.Space(8);

            bool playing = EditorApplication.isPlaying;
            using (new EditorGUI.DisabledScope(!playing))
            {
                if (GUILayout.Button("📸  Capturar agora", GUILayout.Height(36)))
                    CaptureNow();
            }

            if (!playing)
                EditorGUILayout.HelpBox("Entre no Play Mode para capturar.", MessageType.Warning);

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Abrir pasta de screenshots"))
                EditorUtility.RevealInFinder(Path.GetFullPath(OutputDir));
        }

        // ── Static API (chamável via CLI eval) ────────────────────────────

        /// <summary>
        /// Captura a Game View e salva em Assets/Screenshots/.
        /// Pode ser chamado via: unity command script eval "Champions.Editor.UIScreenshotTool.CaptureNow();"
        /// </summary>
        [MenuItem("Champions/Tools/Capturar UI Agora %&s")]  // Cmd+Alt+S
        public static string CaptureNow()
        {
            EnsureOutputDir();

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var filename  = $"UI_{timestamp}.png";
            var fullPath  = Path.GetFullPath(Path.Combine(OutputDir, filename));

            // ScreenCapture.CaptureScreenshot funciona em Play Mode e no Editor.
            ScreenCapture.CaptureScreenshot(fullPath, superSize: 1);

            Debug.Log($"[UIScreenshotTool] Screenshot salvo: {fullPath}");
            return fullPath;
        }

        /// <summary>
        /// Captura com um supersampling multiplicado (útil para exportar para Figma).
        /// Exemplo: CaptureNow(superSize: 2) → imagem 2160×3840 para um display 1080p.
        /// </summary>
        public static string CaptureNow(int superSize)
        {
            EnsureOutputDir();

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var filename  = $"UI_{timestamp}_x{superSize}.png";
            var fullPath  = Path.GetFullPath(Path.Combine(OutputDir, filename));

            ScreenCapture.CaptureScreenshot(fullPath, superSize: superSize);

            Debug.Log($"[UIScreenshotTool] Screenshot @{superSize}x salvo: {fullPath}");
            return fullPath;
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static void EnsureOutputDir()
        {
            var fullDir = Path.GetFullPath(OutputDir);
            if (!Directory.Exists(fullDir))
            {
                Directory.CreateDirectory(fullDir);
                // Cria o meta para que o Unity não ignore a pasta ao importar.
                File.WriteAllText(fullDir + ".meta",
                    "fileFormatVersion: 2\nguid: " + GUID.Generate() + "\nfolderAsset: yes\n");
            }
        }
    }
}
