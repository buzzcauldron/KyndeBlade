#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace KyndeBlade
{
    /// <summary>
    /// Autogenerates DialogueChoiceBeat and StoryBeat assets from DialogueTreeDefinition.
    /// Baldur's Gate–style: branched trees, poem characters, game triggers.
    /// Run via KyndeBlade > Generate Dialogue Tree.
    /// </summary>
    public static class DialogueTreeGenerator
    {
        const string OutputPath = "Assets/Resources/Data/DialogueTrees";

        [MenuItem("KyndeBlade/Generate Dialogue Tree")]
        static void GenerateFromSelection()
        {
            var def = Selection.activeObject as DialogueTreeDefinition;
            if (def == null)
            {
                var guids = AssetDatabase.FindAssets("t:DialogueTreeDefinition");
                if (guids.Length > 0)
                    def = AssetDatabase.LoadAssetAtPath<DialogueTreeDefinition>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            if (def == null)
            {
                Debug.LogWarning("No DialogueTreeDefinition selected. Create one via Create > KyndeBlade > Dialogue Tree Definition.");
                return;
            }
            Generate(def);
        }

        [MenuItem("KyndeBlade/Generate Default Dialogue Tree (Poems + Triggers)")]
        static void GenerateDefault()
        {
            var def = CreateDefaultDefinition();
            if (def == null) return;
            Generate(def);
            Debug.Log("Generated default dialogue tree. Edit DialogueTreeDefinition to customize.");
        }

        static DialogueTreeDefinition CreateDefaultDefinition()
        {
            var path = "Assets/Resources/Data/DialogueTrees/DefaultDialogueTree.asset";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var existing = AssetDatabase.LoadAssetAtPath<DialogueTreeDefinition>(path);
            if (existing != null)
            {
                Debug.Log("Using existing DefaultDialogueTree. Select it and run Generate Dialogue Tree to regenerate.");
                return existing;
            }

            var def = ScriptableObject.CreateInstance<DialogueTreeDefinition>();
            def.RootNodeId = "green_chapel_intro";
            def.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "green_chapel_intro",
                    Text = "I am the Knight of the Grene Chapel. I offre the a game: smyte me ones, and in a yere and a day I schal quyte the. Wolt thou acorde?",
                    SpeakerName = "The Green Knight",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I acorde. I schal mete the at the Grene Chapel.",
                            IsCorrectChoice = true,
                            TransitionToLocationId = "green_chapel",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef { Type = DialogueTreeDefinition.ConsequenceType.SetGreenKnightRandom }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I refuse. I wyl not pleye thi game.",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef { Type = DialogueTreeDefinition.ConsequenceType.SetGreenKnightRandom, BoolParam = true }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I fle. This is no stede for me.",
                            IsCorrectChoice = false,
                            TransitionToLocationId = "",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef { Type = DialogueTreeDefinition.ConsequenceType.SetGreenKnightRandom, BoolParam = true }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "boundary_tree",
                    Text = "At the bounde bitwene worldes, a tre stondeth. Musyk drifteth from byonde—sorweful, unerthly. Wolt thou folwe?",
                    SpeakerName = "A Voice",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "The path is uncerteyn. I torne ayen.",
                            IsCorrectChoice = true,
                            TransitionToLocationId = ""
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I folwe the musyk into the twylyght.",
                            IsCorrectChoice = false,
                            TransitionToLocationId = "otherworld",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef { Type = DialogueTreeDefinition.ConsequenceType.SetOrfeoTriggered }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "piers_do_wel",
                    Text = "What is Do-Wel? Wille begins seeking Do-Wel, learning that it means doing well in one's work and life. But the quest is difficult when poor.",
                    SpeakerName = "Piers the Plowman",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I shall labour as I may.",
                            IsCorrectChoice = true,
                            TransitionToLocationId = "quest_do_wel"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "The world asketh otherwise.",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            TransitionToLocationId = "quest_do_wel"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "lady_mede_temptation",
                    Text = "Lady Mede extendeth hir hand. \"Come with me,\" she saith, \"and thou shalt have gold and ease.\"",
                    SpeakerName = "Lady Mede",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I refuse. Do-Wel is my path.",
                            IsCorrectChoice = true,
                            NextNodeId = "piers_approves"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I accept. Gold and ease I desire.",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Greed,
                            TransitionToLocationId = ""
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "piers_approves",
                    Text = "Piers noddeth. \"Thou hast chosen wel. The plow awaiteth.\"",
                    SpeakerName = "Piers the Plowman",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "I am ready.",
                            IsCorrectChoice = true,
                            TransitionToLocationId = "piers_field"
                        }
                    }
                }
            };
            AssetDatabase.CreateAsset(def, path);
            return def;
        }

        static void Generate(DialogueTreeDefinition def)
        {
            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            var nodesById = new Dictionary<string, DialogueTreeDefinition.DialogueNode>();
            foreach (var n in def.Nodes)
                if (!string.IsNullOrEmpty(n.NodeId))
                    nodesById[n.NodeId] = n;

            var generated = new Dictionary<string, DialogueChoiceBeat>();

            foreach (var node in def.Nodes)
            {
                if (string.IsNullOrEmpty(node.NodeId)) continue;

                var beat = ScriptableObject.CreateInstance<DialogueChoiceBeat>();
                beat.BeatId = node.NodeId;
                beat.Text = node.Text;
                beat.SpeakerName = node.SpeakerName ?? "Narrator";

                var choices = new List<DialogueChoiceBeat.Choice>();
                foreach (var c in node.Choices ?? new DialogueTreeDefinition.ChoiceDef[0])
                {
                    var choice = new DialogueChoiceBeat.Choice
                    {
                        Text = c.Text,
                        IsCorrectChoice = c.IsCorrectChoice,
                        TransitionToLocationId = c.TransitionToLocationId,
                        AssociatedSin = c.AssociatedSin
                    };
                    choices.Add(choice);
                }
                beat.Choices = choices.ToArray();

                var assetPath = $"{OutputPath}/Choice_{node.NodeId}.asset";
                AssetDatabase.CreateAsset(beat, assetPath);
                generated[node.NodeId] = beat;
            }

            foreach (var node in def.Nodes)
            {
                if (!generated.TryGetValue(node.NodeId, out var beat)) continue;
                for (int i = 0; i < node.Choices?.Length && i < beat.Choices.Length; i++)
                {
                    var nextId = node.Choices[i].NextNodeId;
                    if (!string.IsNullOrEmpty(nextId) && generated.TryGetValue(nextId, out var nextBeat))
                        beat.Choices[i].NextDialogueBeat = nextBeat;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generated {generated.Count} DialogueChoiceBeat assets in {OutputPath}.");
        }
    }
}
#endif
