using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Baldur's Gate–style dialogue tree definition for autogeneration.
    /// Maps poem characters, game triggers, and consequences to DialogueChoiceBeat/StoryBeat assets.
    /// See docs/DIALOGUE_SOURCE_TEXT_PLAN.md and docs/DIALOGUE_TREE_GENERATOR.md.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueTree", menuName = "KyndeBlade/Dialogue Tree Definition")]
    public class DialogueTreeDefinition : ScriptableObject
    {
        [Serializable]
        public class DialogueNode
        {
            public string NodeId;
            [TextArea(2, 5)]
            public string Text;
            public string SpeakerName = "Narrator";
            public ChoiceDef[] Choices = new ChoiceDef[0];
        }

        [Serializable]
        public class ChoiceDef
        {
            [TextArea(1, 3)]
            public string Text;
            public bool IsCorrectChoice;
            public string TransitionToLocationId;
            public SinType AssociatedSin = SinType.None;
            [Tooltip("If set, show this dialogue node next (chained conversation).")]
            public string NextNodeId;
            public ConditionDef Condition;
            public ConsequenceDef Consequence;
        }

        /// <summary>Baldur's Gate–style conditions. Checked before choice is available.</summary>
        [Serializable]
        public class ConditionDef
        {
            public ConditionType Type = ConditionType.None;
            public string StringParam;   // e.g. location id, "piers"
            public int IntParam;        // e.g. poverty level, elde hits
            public bool BoolParam;      // e.g. flag for conditional branch
        }

        public enum ConditionType
        {
            None,
            HasVisited,
            HasNotVisited,
            PovertyLevelAtLeast,
            PovertyLevelBelow,
            EldeHitsAtLeast,
            GreenKnightWillAppear,
            OrfeoTriggered,
            HasEverHadHunger,
            GreenChapelBodiesAtLeast,
            OtherworldBodiesAtLeast
        }

        /// <summary>Consequences when choice is selected. Triggers for actions in game.</summary>
        [Serializable]
        public class ConsequenceDef
        {
            public ConsequenceType Type = ConsequenceType.None;
            public string StringParam;
            public int IntParam;
            public bool BoolParam;
            public SinType SinParam = SinType.None;
        }

        public enum ConsequenceType
        {
            None,
            SetGreenKnightRandom,
            SetOrfeoTriggered,
            IncrementPoverty,
            TransitionToLocation,
            StartSinMiniboss,
            SaveCheckpoint
        }

        [Header("Tree")]
        public string RootNodeId = "green_chapel_intro";
        public DialogueNode[] Nodes = new DialogueNode[0];
    }
}
