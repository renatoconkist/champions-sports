using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Champions.Editor
{
    /// <summary>
    /// Champions → Localization → Create All String Tables
    ///
    /// Tables are grouped by domain, not by screen:
    ///   Auth    — login, register, forgot-password strings
    ///   System  — errors, warnings, feedback messages
    ///   Global  — shared actions (back, ok, cancel, loading…)
    ///   Dashboard — reserved for dashboard strings
    ///
    /// Re-running updates existing entries without touching others.
    /// </summary>
    public static class LocalizationSetup
    {
        private const string AssetPath = "Assets/Localization";

        // ── Table definitions ─────────────────────────────────────────────

        private static readonly Dictionary<string, Dictionary<string, string>> AuthEntries = new()
        {
            // Login screen
            ["login_btn"]            = T("LOGIN",               "LOGIN",                "INICIAR SESIÓN"),
            ["login_register_btn"]   = T("CRIAR CONTA",         "CREATE ACCOUNT",       "CREAR CUENTA"),
            ["login_forgot_btn"]     = T("Esqueci a Senha",     "Forgot Password",      "Olvidé mi contraseña"),
            ["login_support_btn"]    = T("SUPORTE",             "SUPPORT",              "SOPORTE"),
            ["login_email_ph"]       = T("E-mail",              "E-mail",               "Correo electrónico"),
            ["login_password_ph"]    = T("Senha",               "Password",             "Contraseña"),

            // Register screen
            ["register_title"]       = T("Criar Conta",                              "Create Account",                     "Crear Cuenta"),
            ["register_subtitle"]    = T("Preencha seus dados para começar.",         "Fill in your details to get started.","Completa tus datos para empezar."),
            ["register_first_ph"]    = T("Nome",                                     "First Name",                         "Nombre"),
            ["register_last_ph"]     = T("Sobrenome",                                "Last Name",                          "Apellido"),
            ["register_email_ph"]    = T("E-mail",                                   "E-mail",                             "Correo electrónico"),
            ["register_password_ph"] = T("Senha",                                    "Password",                           "Contraseña"),
            ["register_confirm_ph"]  = T("Confirmar senha",                          "Confirm password",                   "Confirmar contraseña"),
            ["register_btn"]         = T("CRIAR CONTA",                              "CREATE ACCOUNT",                     "CREAR CUENTA"),

            // Forgot-password screen
            ["forgot_title"]         = T("Esqueci a Senha",    "Forgot Password",      "Olvidé mi contraseña"),
            ["forgot_subtitle"]      = T(
                "Informe seu e-mail e enviaremos um link de recuperação.",
                "Enter your e-mail and we'll send you a recovery link.",
                "Ingresa tu correo y te enviaremos un enlace de recuperación."),
            ["forgot_email_ph"]      = T("E-mail",             "E-mail",               "Correo electrónico"),
            ["forgot_send_btn"]      = T("ENVIAR",             "SEND",                 "ENVIAR"),
        };

        private static readonly Dictionary<string, Dictionary<string, string>> SystemEntries = new()
        {
            ["error_wrong_credentials"] = T(
                "E-mail ou senha incorretos.",
                "Incorrect e-mail or password.",
                "Correo o contraseña incorrectos."),
            ["error_register_failed"]   = T(
                "Não foi possível criar a conta.",
                "Could not create account.",
                "No se pudo crear la cuenta."),
            ["error_password_mismatch"] = T(
                "As senhas não coincidem.",
                "Passwords do not match.",
                "Las contraseñas no coinciden."),
            ["error_send_failed"]       = T(
                "Não foi possível enviar o e-mail.",
                "Could not send the email.",
                "No se pudo enviar el correo."),
            ["success_recovery_sent"]   = T(
                "Link de recuperação enviado!",
                "Recovery link sent!",
                "¡Enlace de recuperación enviado!"),
        };

        private static readonly Dictionary<string, Dictionary<string, string>> GlobalEntries = new()
        {
            ["back"]    = T("Voltar",       "Back",     "Volver"),
            ["ok"]      = T("OK",           "OK",       "OK"),
            ["cancel"]  = T("Cancelar",     "Cancel",   "Cancelar"),
            ["loading"] = T("Carregando…",  "Loading…", "Cargando…"),
        };

        // Dashboard table starts empty — entries are added as the screen is built.
        private static readonly Dictionary<string, Dictionary<string, string>> DashboardEntries = new();

        // ── Helpers ───────────────────────────────────────────────────────

        private static Dictionary<string, string> T(string pt, string en, string es)
            => new() { ["pt"] = pt, ["en"] = en, ["es"] = es };

        // ── Menu items ────────────────────────────────────────────────────

        [MenuItem("Champions/Localization/Create All String Tables")]
        public static void CreateAllStringTables()
        {
            EnsureTable("Auth",      AuthEntries);
            EnsureTable("System",    SystemEntries);
            EnsureTable("Global",    GlobalEntries);
            EnsureTable("Dashboard", DashboardEntries);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[LocalizationSetup] Todas as String Tables criadas/atualizadas.");
        }

        // ── Internal ──────────────────────────────────────────────────────

        private static void EnsureTable(string tableName,
            Dictionary<string, Dictionary<string, string>> entries)
        {
            if (!System.IO.Directory.Exists(AssetPath))
                System.IO.Directory.CreateDirectory(AssetPath);

            var collection = LocalizationEditorSettings.GetStringTableCollection(tableName);
            if (collection == null)
            {
                collection = LocalizationEditorSettings.CreateStringTableCollection(tableName, AssetPath);
                Debug.Log($"[LocalizationSetup] Coleção '{tableName}' criada.");
            }

            // Map short locale prefix → StringTable
            var tableByLang = new Dictionary<string, StringTable>();
            foreach (StringTable table in collection.StringTables)
            {
                var code = table.LocaleIdentifier.Code;
                if      (code.StartsWith("pt")) tableByLang["pt"] = table;
                else if (code.StartsWith("en")) tableByLang["en"] = table;
                else if (code.StartsWith("es")) tableByLang["es"] = table;
            }

            foreach (var (key, translations) in entries)
            {
                collection.SharedData.AddKey(key);
                foreach (var (lang, text) in translations)
                {
                    if (!tableByLang.TryGetValue(lang, out var table)) continue;
                    var entry = table.GetEntry(key) ?? table.AddEntry(key, text);
                    entry.Value = text;
                    EditorUtility.SetDirty(table);
                }
            }

            EditorUtility.SetDirty(collection);
            Debug.Log($"[LocalizationSetup] '{tableName}' — {entries.Count} chaves.");
        }
    }
}
