using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Seven Deadly Sins. Used for dialogue choices—wrong (sin-aligned) choice triggers sin miniboss fight; defeat = Orfeo Otherworld.</summary>
    public enum SinType
    {
        None,
        Pride,
        Envy,
        Wrath,
        Sloth,
        Greed,
        Gluttony,
        Lust
    }

    /// <summary>Dialogue with choices. Used for Green Chapel etc. Wrong choice = Green Knight appears randomly. Sin-aligned choice = fight that sin miniboss; defeat = Orfeo.</summary>
    [CreateAssetMenu(fileName = "DialogueChoice", menuName = "KyndeBlade/Dialogue Choice Beat")]
    public class DialogueChoiceBeat : ScriptableObject
    {
        [Serializable]
        public class Choice
        {
            [TextArea(1, 3)]
            public string Text;
            [Tooltip("If true, this is the 'correct' choice (e.g. accepts challenge). Prevents Green Knight random appearance.")]
            public bool IsCorrectChoice;
            [Tooltip("If set, picking this choice transitions to this location (e.g. otherworld for Orfeo alternate ending).")]
            public string TransitionToLocationId;
            [Tooltip("If not None and not IsCorrectChoice: picking this choice triggers a miniboss fight with this sin. Defeat = Orfeo Otherworld.")]
            public SinType AssociatedSin = SinType.None;
            [Tooltip("Baldur's Gate–style chaining: show this dialogue next instead of transitioning. Overrides TransitionToLocationId if set.")]
            public DialogueChoiceBeat NextDialogueBeat;
        }

        [Header("Content")]
        public string BeatId;
        [TextArea(3, 6)]
        public string Text;
        public string SpeakerName = "The Green Knight";
        public Sprite Portrait;

        [Header("Choices")]
        public Choice[] Choices = new Choice[0];

        [Header("Campaign")]
        public int VisionIndex;
    }
}
