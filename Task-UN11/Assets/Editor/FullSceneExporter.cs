using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FullProjectExporter : EditorWindow
{
    [MenuItem("Tools/Export FULL Project Audit")]
    public static void ExportEverything()
    {
        var sb = new StringBuilder();
        string projectPath = Application.dataPath;
        string outputPath = Path.Combine(Path.GetDirectoryName(projectPath), "FullProjectAudit.txt");

        WriteHeader(sb, "PROJECT META");
        sb.AppendLine($"Unity Version  : {Application.unityVersion}");
        sb.AppendLine($"Platform       : {EditorUserBuildSettings.activeBuildTarget}");
        sb.AppendLine($"Color Space    : {PlayerSettings.colorSpace}");
        sb.AppendLine($"Render Pipeline: {(UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline != null ? UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.GetType().Name : "Built-in")}");
        sb.AppendLine($"Scripting Backend: {PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone)}");
        sb.AppendLine($"API Compatibility: {PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone)}");
        sb.AppendLine($"Active Input Handling: {GetActiveInputHandling()}");
        sb.AppendLine($"Active Scene   : {SceneManager.GetActiveScene().name} ({SceneManager.GetActiveScene().path})");
        sb.AppendLine($"Export Time    : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        WriteHeader(sb, "BUILD SCENES");
        foreach (var s in EditorBuildSettings.scenes)
            sb.AppendLine($"  [{(s.enabled ? "X" : " ")}] {s.path}");

        WriteHeader(sb, "TAGS");
        foreach (var t in UnityEditorInternal.InternalEditorUtility.tags) sb.AppendLine($"  {t}");

        WriteHeader(sb, "LAYERS");
        for (int i = 0; i < 32; i++)
        {
            string n = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(n)) sb.AppendLine($"  {i}: {n}");
        }

        WriteHeader(sb, "ALL C# SCRIPTS (FULL SOURCE)");
        var csFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        Array.Sort(csFiles);
        foreach (var f in csFiles)
        {
            string rel = "Assets" + f.Substring(Application.dataPath.Length).Replace("\\", "/");
            // Skip third-party huge scripts to keep file manageable
            if (rel.Contains("/StarterAssets/") || rel.Contains("/Cinemachine/") ||
                rel.Contains("/_ThirdParty/") || rel.Contains("/TextMesh Pro/") ||
                rel.Contains("/PostProcessing/"))
            {
                sb.AppendLine($"--- (SKIPPED THIRD-PARTY) {rel} ---");
                continue;
            }
            sb.AppendLine();
            sb.AppendLine($"================== FILE: {rel} ==================");
            try { sb.AppendLine(File.ReadAllText(f)); }
            catch (Exception ex) { sb.AppendLine($"[Read error: {ex.Message}]"); }
        }

        WriteHeader(sb, "FULL SCENE HIERARCHY (with all components)");
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var go in roots) DumpGameObject(sb, go, 0);

        WriteHeader(sb, "INPUT ACTION ASSETS");
        string[] inputAssetGuids = AssetDatabase.FindAssets("t:InputActionAsset");
        foreach (var guid in inputAssetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            sb.AppendLine($"  {path}");
        }

        WriteHeader(sb, "MATERIALS IN SCENE");
        var materialSet = new HashSet<Material>();
        foreach (var rend in GameObject.FindObjectsOfType<Renderer>(true))
            foreach (var m in rend.sharedMaterials)
                if (m != null) materialSet.Add(m);
        foreach (var m in materialSet)
        {
            sb.AppendLine($"  • {m.name} (Shader: {(m.shader != null ? m.shader.name : "NULL")})");
        }

        WriteHeader(sb, "PROJECT FOLDER TREE");
        DumpFolderTree(sb, Application.dataPath, "");

        File.WriteAllText(outputPath, sb.ToString());
        Debug.Log($"[FullExporter] Exported {sb.Length:N0} chars to: {outputPath}");
        EditorUtility.RevealInFinder(outputPath);
    }

    static void WriteHeader(StringBuilder sb, string title)
    {
        sb.AppendLine();
        sb.AppendLine("================================================================================");
        sb.AppendLine("  " + title);
        sb.AppendLine("================================================================================");
    }

    static string GetActiveInputHandling()
    {
        try
        {
            var settingsAsset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/ProjectSettings.asset");
            var so = new SerializedObject(settingsAsset);
            var prop = so.FindProperty("activeInputHandler");
            if (prop != null)
            {
                int v = prop.intValue;
                return v == 0 ? "Old (Input Manager)" : v == 1 ? "New (Input System Package)" : "Both";
            }
        }
        catch { }
        return "Unknown";
    }

    static void DumpGameObject(StringBuilder sb, GameObject go, int depth)
    {
        string indent = new string(' ', depth * 2);
        string status = go.activeSelf ? "[+]" : "[-]";
        sb.AppendLine($"{indent}{status} {go.name}  tag={go.tag}  layer={LayerMask.LayerToName(go.layer)}  static={go.isStatic}");
        sb.AppendLine($"{indent}    Pos: {go.transform.localPosition}  Rot: {go.transform.localEulerAngles}  Scale: {go.transform.localScale}");

        var comps = go.GetComponents<Component>();
        foreach (var c in comps)
        {
            if (c == null)
            {
                sb.AppendLine($"{indent}  • <MISSING SCRIPT>");
                continue;
            }
            if (c is Transform) continue;
            sb.AppendLine($"{indent}  • {c.GetType().Name}");
            DumpComponentFields(sb, c, indent + "      ");
        }

        foreach (Transform child in go.transform) DumpGameObject(sb, child.gameObject, depth + 1);
    }

    static void DumpComponentFields(StringBuilder sb, Component c, string indent)
    {
        try
        {
            var so = new SerializedObject(c);
            var prop = so.GetIterator();
            bool enterChildren = true;
            int count = 0;
            while (prop.NextVisible(enterChildren) && count < 100)
            {
                enterChildren = false;
                count++;
                string val = SafePropValue(prop);
                sb.AppendLine($"{indent}{prop.displayName}: {val}");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"{indent}[Field dump error: {ex.Message}]");
        }
    }

    static string SafePropValue(SerializedProperty p)
    {
        try
        {
            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer: return p.intValue.ToString();
                case SerializedPropertyType.Boolean: return p.boolValue.ToString();
                case SerializedPropertyType.Float: return p.floatValue.ToString("F4");
                case SerializedPropertyType.String: return $"\"{p.stringValue}\"";
                case SerializedPropertyType.Color: return p.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return p.objectReferenceValue != null ? $"→ {p.objectReferenceValue.name} ({p.objectReferenceValue.GetType().Name})" : "null";
                case SerializedPropertyType.LayerMask: return $"mask:{p.intValue}";
                case SerializedPropertyType.Enum: return p.enumNames.Length > p.enumValueIndex ? p.enumNames[p.enumValueIndex] : p.intValue.ToString();
                case SerializedPropertyType.Vector2: return p.vector2Value.ToString();
                case SerializedPropertyType.Vector3: return p.vector3Value.ToString();
                case SerializedPropertyType.Vector4: return p.vector4Value.ToString();
                case SerializedPropertyType.Quaternion: return p.quaternionValue.eulerAngles + " (euler)";
                case SerializedPropertyType.ArraySize: return p.intValue.ToString();
                case SerializedPropertyType.Rect: return p.rectValue.ToString();
                case SerializedPropertyType.Generic: return p.isArray ? $"<array len={p.arraySize}>" : "<generic>";
                default: return $"<{p.propertyType}>";
            }
        }
        catch { return "<error>"; }
    }

    static void DumpFolderTree(StringBuilder sb, string root, string indent, int depth = 0)
    {
        if (depth > 8) return;
        try
        {
            string name = Path.GetFileName(root);
            sb.AppendLine($"{indent}{name}/");
            foreach (var d in Directory.GetDirectories(root))
            {
                string folderName = Path.GetFileName(d);
                if (folderName.StartsWith(".") || folderName == "Library" || folderName == "Temp" || folderName == "Logs") continue;
                DumpFolderTree(sb, d, indent + "  ", depth + 1);
            }
        }
        catch { }
    }
}