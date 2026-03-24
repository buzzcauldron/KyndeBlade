using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace KyndeBlade.Editor
{
    /// <summary>
    /// One-time / repeatable export: LocationNode + EncounterConfig → JSON for Godot (data pipeline spike).
    /// Menu: KyndeBlade → Export Slice Data for Godot
    /// </summary>
    public static class ExportGodotSliceData
    {
        const string TourPath = "Assets/Resources/Data/Vision1/Loc_tour.asset";
        const string FayrePath = "Assets/Resources/Data/Vision1/Loc_fayre_felde.asset";

        [MenuItem("KyndeBlade/Export Slice Data for Godot")]
        public static void Export()
        {
            var tour = AssetDatabase.LoadAssetAtPath<LocationNode>(TourPath);
            var fayre = AssetDatabase.LoadAssetAtPath<LocationNode>(FayrePath);
            if (tour == null || fayre == null)
            {
                Debug.LogError("ExportGodotSliceData: missing Loc_tour or Loc_fayre_felde under Resources/Data/Vision1.");
                return;
            }

            EncounterConfig enc = fayre.Encounter != null
                ? fayre.Encounter
                : AssetDatabase.LoadAssetAtPath<EncounterConfig>("Assets/Resources/Data/Vision1/FayreFeldeEncounter.asset");

            var locs = new List<LocationNode> { tour, fayre };
            var encs = new List<EncounterConfig>();
            if (enc != null)
                encs.Add(enc);

            string json = BuildJson(locs, encs);
            string repoRoot = Directory.GetParent(Application.dataPath)!.FullName;
            string outDir = Path.Combine(repoRoot, "KyndeBlade_Godot", "data");
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "exported_from_unity.json");
            File.WriteAllText(outPath, json);
            Debug.Log($"[KyndeBlade] Exported Godot slice JSON to {outPath}");
        }

        static string BuildJson(List<LocationNode> locations, List<EncounterConfig> encounters)
        {
            var sb = new StringBuilder(2048);
            sb.Append("{\n");
            sb.Append("  \"schema_version\": 2,\n");
            sb.Append("  \"exported_at\": \"").Append(EscapeIso(DateTime.UtcNow.ToString("o"))).Append("\",\n");
            sb.Append("  \"unity_note\": \"KyndeBlade/Export Slice Data for Godot (LocationNode + EncounterConfig + StoryBeatOnArrival)\",\n");
            sb.Append("  \"locations\": [\n");
            for (int i = 0; i < locations.Count; i++)
            {
                if (i > 0) sb.Append(",\n");
                AppendLocation(sb, locations[i]);
            }
            sb.Append("\n  ],\n");
            sb.Append("  \"encounters\": [\n");
            for (int i = 0; i < encounters.Count; i++)
            {
                if (i > 0) sb.Append(",\n");
                AppendEncounter(sb, encounters[i]);
            }
            sb.Append("\n  ]\n}\n");
            return sb.ToString();
        }

        static void AppendLocation(StringBuilder sb, LocationNode loc)
        {
            sb.Append("    {\n");
            sb.Append("      \"location_id\": \"").Append(Escape(loc.LocationId)).Append("\",\n");
            sb.Append("      \"display_name\": \"").Append(Escape(loc.DisplayName)).Append("\",\n");
            sb.Append("      \"description\": \"").Append(Escape(loc.Description)).Append("\",\n");
            sb.Append("      \"vision_index\": ").Append(loc.VisionIndex).Append(",\n");
            sb.Append("      \"passus_index\": ").Append(loc.PassusIndex).Append(",\n");
            sb.Append("      \"passus_title\": \"").Append(Escape(loc.PassusTitle)).Append("\",\n");
            sb.Append("      \"next_location_ids\": ");
            AppendStringArray(sb, loc.NextLocationIds ?? new List<string>());
            sb.Append(",\n");
            var beat = loc.StoryBeatOnArrival;
            sb.Append("      \"arrival_beat_id\": \"").Append(Escape(beat != null ? beat.BeatId : "")).Append("\",\n");
            sb.Append("      \"arrival_beat_speaker\": \"").Append(Escape(beat != null ? beat.SpeakerName : "")).Append("\",\n");
            sb.Append("      \"arrival_beat_text\": \"").Append(Escape(beat != null ? beat.Text : "")).Append("\",\n");
            string encName = loc.Encounter != null ? loc.Encounter.name : "";
            sb.Append("      \"encounter_asset_name\": \"").Append(Escape(encName)).Append("\"\n");
            sb.Append("    }");
        }

        static void AppendEncounter(StringBuilder sb, EncounterConfig enc)
        {
            sb.Append("    {\n");
            sb.Append("      \"asset_name\": \"").Append(Escape(enc.name)).Append("\",\n");
            sb.Append("      \"enemy_spacing\": ").Append(enc.EnemySpacing.ToString("R", System.Globalization.CultureInfo.InvariantCulture)).Append(",\n");
            sb.Append("      \"enemy_character_types\": ");
            var types = new List<string>();
            if (enc.Enemies != null)
            {
                foreach (var e in enc.Enemies)
                {
                    if (e != null && !string.IsNullOrEmpty(e.CharacterTypeName))
                        types.Add(e.CharacterTypeName);
                }
            }
            AppendStringArray(sb, types);
            sb.Append("\n    }");
        }

        static void AppendStringArray(StringBuilder sb, IList<string> items)
        {
            sb.Append("[");
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append("\"").Append(Escape(items[i])).Append("\"");
                }
            }
            sb.Append("]");
        }

        static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        }

        static string EscapeIso(string s) => Escape(s);
    }
}
