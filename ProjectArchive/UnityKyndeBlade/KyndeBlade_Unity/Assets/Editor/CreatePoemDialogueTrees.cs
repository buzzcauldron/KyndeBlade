#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using KyndeBlade;

namespace KyndeBlade.Editor
{
    /// <summary>
    /// Generates DialogueTreeDefinition assets for every game location,
    /// drawing text from Piers Plowman, Sir Gawain, and Sir Orfeo.
    /// Each tree is a branching conversation with conditions and consequences.
    /// Run via KyndeBlade > Generate All Poem Dialogue Trees.
    /// </summary>
    public static class CreatePoemDialogueTrees
    {
        const string TreePath = "Assets/Resources/Data/DialogueTrees";

        [MenuItem("KyndeBlade/Generate All Poem Dialogue Trees")]
        public static void GenerateAll()
        {
            EnsureDirectory(TreePath);

            CreateFayreFeldeTree();
            CreateTourTree();
            CreateDongeounTree();
            CreatePiersFieldTree();
            CreateSevenSinsTree();
            CreateQuestDoWelTree();
            CreateDongeounDepthsTree();
            CreateYearsPassTree();
            CreateFieldOfGraceTree();
            CreateGreenChapelTree();
            CreateBoundaryTreeOrfeoTree();
            CreateOtherworldTree();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generated all poem dialogue trees in {TreePath}.");
        }

        // ─────────────────────────────────────────────────────────────
        //  VISION I
        // ─────────────────────────────────────────────────────────────

        /// <summary>Passus I – Holy Church descends and teaches about Truth, Love, and the Fair Field.</summary>
        static void CreateFayreFeldeTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("FayreFeldeDialogue");
            tree.RootNodeId = "ff_holy_church_appears";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ff_holy_church_appears",
                    SpeakerName = "Holy Church",
                    Text = "A comely lady in white cometh down from the Tower. \"I am Holy Chirche,\" she saith. \"I taughte thee firste, Wille, er thou knewe what was right or wrong. What this montaigne bymeneth and the merke dale and the feld ful of folk, I shal yow faire shewe.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"What betokeneth the Tour upon the toft?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ff_truth_tower"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"What lieth in that derke dale bynethe?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ff_dungeon_below"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I nede no lernyng. I knowe the worlde well enough.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            NextNodeId = "ff_pride_rebuke"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ff_truth_tower",
                    SpeakerName = "Holy Church",
                    Text = "\"The Tour on the toft — Treuthe is therinne, and He wolde that ye wroughte as His word techeth. For He is fader of feith. Treuthe is the beste tresour that I can teche.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"How may I knowe Treuthe? Where doth he dwelle?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ff_love_teaching"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"What of tresour of this worlde — gold and silver?\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Greed,
                            NextNodeId = "ff_mede_warning"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ff_dungeon_below",
                    SpeakerName = "Holy Church",
                    Text = "\"The depe dale bynethe — the Dongeoun — therein dwelleth Wrong, fader of falseness. He that trusteth in his tresours, bitrayeth soonest. All that worketh with Wrong shal wende to the Dongeoun after dethe.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal kepe me from Wrong. Teche me of Love.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ff_love_teaching"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Wrong hath power in this world. Perhaps wisdom lieth there.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            NextNodeId = "ff_mede_warning"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ff_pride_rebuke",
                    SpeakerName = "Holy Church",
                    Text = "She looketh upon thee with sorrow. \"Thou art a dotede daffe — a fool — that defieth kynde knowing. All the wyes of the worlde that have good wit knowe the nede of lernyng. And yet thou speakest as a drunken fool.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Forgive me, lady. I spoke in haste. Teche me Treuthe.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ff_love_teaching"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"A fool I may be, but mine own fool.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ff_love_teaching",
                    SpeakerName = "Holy Church",
                    Text = "\"Love is leche of lif, and nexte Oure Lord selve, and also the graithe gate that goth into hevene. For-thi I seye — lerne to love, as the lore of the holy telleth. For no cause sholde overcome us, kynde and unkynde alike. Love alone is worth more than all the gold in the world.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal remember. Love above alle.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "fayre_felde"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"But what of Mede? I herde speak of a faire lady...\"",
                            IsCorrectChoice = false,
                            NextNodeId = "ff_mede_warning"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ff_mede_warning",
                    SpeakerName = "Holy Church",
                    Text = "\"Beware of Mede, that mayden faire. She is lemman of Fals, betrothed to falsehood. Her father is Favel — flattery — and her kindred are liars all. Many a wise man hath she betrayed. Take hede, Wille.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal beware. Thanke thee, Holy Chirche.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "fayre_felde"
                            }
                        }
                    }
                }
            };

            Save(tree, "FayreFeldeDialogue");
        }

        /// <summary>Passus II – The Tower of Truth. Holy Church's deeper teaching.</summary>
        static void CreateTourTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("TourDialogue");
            tree.RootNodeId = "tour_approach";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "tour_approach",
                    SpeakerName = "Narrator",
                    Text = "The Tour on the Toft riseth above thee, trieliche ymaked. A light breaketh from its crown like the sonne through stained glass. The folk of the feld labour below, unknowing of what dwelleth above.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Climb toward the Tower of Truth.",
                            IsCorrectChoice = true,
                            NextNodeId = "tour_kynde_wit"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Descend to the valley where shadows gather.",
                            IsCorrectChoice = false,
                            NextNodeId = "tour_valley"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "tour_kynde_wit",
                    SpeakerName = "Kynde Wit",
                    Text = "At the foot of the tower, a figure waiteth — Kynde Wit, natural understanding. \"Wille,\" he saith, \"thou canst not enter the Tour by climbing alone. The gate is opened by Treuthe, and the key is love. But first thou must learn to see with thine inner sight.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Teche me to see, Kynde Wit.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "tour_inner_sight"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I have seyen enough. Show me the Dongeoun instead.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "tour_valley"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "tour_inner_sight",
                    SpeakerName = "Kynde Wit",
                    Text = "\"Mark well: Mesure is medicyne. Not too muche, not too litel. The man who eateth and drinketh in mesure hath more joye than the riche glutton. The man who speaketh in mesure hath more wisdom than the shouting prestes. Mesure in all thynges — that is the firste lesoun.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Mesure. I shal kepe it in mynde.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "tour"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Mesure is for the weak. I wolde have alle or nothing.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Gluttony,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "tour_valley",
                    SpeakerName = "Narrator",
                    Text = "The valley is cold. Shadows move between the stones. A voice drifteth from the Dongeoun: \"Come, Wille. The truthe is not in towers but in the deep places where men hide what they truly are.\" The voice belongeth to Wrong.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Turn back toward the light. Wrong speaketh only lies.",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "tour"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Thou mayest be right. What hast thou to show me?\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                }
            };

            Save(tree, "TourDialogue");
        }

        /// <summary>Passus II-III – Lady Mede's trial before the King. Conscience argues against her.</summary>
        static void CreateDongeounTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("DongeounDialogue");
            tree.RootNodeId = "dong_mede_court";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dong_mede_court",
                    SpeakerName = "Narrator",
                    Text = "In the King's Court, Lady Mede standeth accused. She is clad in scarlet, ringed with gold, crowned with a coronet richer than the King's. Favel and False stand at her sides. The King demandeth: shall Mede wed Conscience?",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Stand with Conscience. \"Mede corrupteth all she toucheth.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dong_conscience_speaks"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Speak for Mede. \"She maketh the worlde turn. What harm in reward?\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Greed,
                            NextNodeId = "dong_mede_seduces"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Stay silent. This is not thy quarrel.",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "dong_silence"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dong_conscience_speaks",
                    SpeakerName = "Conscience",
                    Text = "\"Nay!\" quod Conscience to the Kyng. \"Crist it me forbede! Er I wedde that wife, wo me bitide! She is frele of hir feith, fikel of hir speche. She maketh men mysdo many score tymes. In Mede is the moste mischief — she bribeth judges, shieldeth murderers, and selleth the lawe for silver.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Conscience speaketh true. Cast Mede from the court.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dong_reason_called"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Yet workers deserve their mede — their just reward?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dong_just_mede"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dong_mede_seduces",
                    SpeakerName = "Lady Mede",
                    Text = "Mede smileth. She draweth thee aside. \"Wille, thou art poor and ragged. I can clothe thee in silk, fill thy purse with nobles. Conscience is a bare-footed friar who knoweth nothing of the real world. Come — take what I offer.\" She holdeth out a ring of gold.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Take the ring. Gold is gold.",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Greed,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 2
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Cast the ring away. \"Thy gold is tainted, Mede.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dong_reason_called"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dong_silence",
                    SpeakerName = "Narrator",
                    Text = "Thy silence speaketh loud. The courtiers mark thee — one who will not stand for right when the cost is nothing. Mede's servants approach; they mistake thy silence for sympathy.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Break thy silence at last. \"I stand with Conscience.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dong_reason_called"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Continue to watch. This is entertainment enough.",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dong_just_mede",
                    SpeakerName = "Conscience",
                    Text = "Conscience pauseth. \"Thou speakest of mercedem — just reward for honest labour. That is not Mede. Mede is the bribe, the false gift. Mercedem cometh from God for good works. Mark the difference well, Wille: one buildeth, the other corrupteth.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I understand the difference now. Let Reason judge.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dong_reason_called"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dong_reason_called",
                    SpeakerName = "Reason",
                    Text = "Reason entereth the court. He speaketh plainly: \"Lat no man commune with Mede til I come to the court, for I wol rule this reaume and rede the best. With Conscience for my counsellour, I shal governe wel.\" The King agreeth. False and Favel flee. Mede is cast down.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Justice is served. Reason and Conscience shal guide us.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "dongeoun"
                            }
                        }
                    }
                }
            };

            Save(tree, "DongeounDialogue");
        }

        /// <summary>Passus IV-V – Piers the Plowman offers to guide Wille to Truth.</summary>
        static void CreatePiersFieldTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("PiersFieldDialogue");
            tree.RootNodeId = "pf_piers_appears";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "pf_piers_appears",
                    SpeakerName = "Piers the Plowman",
                    Text = "A plowman putteth forth his hed: \"Peter!\" quod he. \"I knowe hym as kyndely as clerc doth his bokes. Conscience and Kynde Wit kende me to his place, and diden me suren hym sithen to serve hym for evere. I have ben his folwere al this fifty wynter. I sowed his seed and herded his beestes.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Lede us to Treuthe, good Piers. We shal pay thee for thy peyne.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "pf_half_acre"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"How can a plowman knowe what clerkes cannot teche?\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            NextNodeId = "pf_piers_rebuke"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "pf_piers_rebuke",
                    SpeakerName = "Piers the Plowman",
                    Text = "\"The clerkes have their bokes,\" Piers saith, unmoved. \"I have my plough. Both serve Treuthe. The clerk readeth of love; I live it in the furrow. Tell me, dreamer — hast thou ever turned the earth and felt God in the soil?\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Forgive me. I speke as one who hath not laboured.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "pf_half_acre"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I am a dreamer, not a plowman. My work is in the mind.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "pf_half_acre"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "pf_half_acre",
                    SpeakerName = "Piers the Plowman",
                    Text = "\"First,\" saith Piers, \"we muste plowe this half acre. Before I lede anyone to Treuthe, this feld must be tilled, this seed sown. There are wasters and shirkers among this crowd — those who wolde eat but will not werke. Wille, wolt thou helpe me?\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal plowe beside thee, Piers. Set me to werk.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "pf_hunger_comes"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I am a poet and a dreamer. Plowing is beneath me.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "pf_wasters"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "pf_wasters",
                    SpeakerName = "Piers the Plowman",
                    Text = "The wasters laugh and drink ale while others labour. Piers' face hardeneth. \"If thou wolt not plowe, thou shalt not eat. Hunger shal be thy teacher where words faileth.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Very well. I take up the plough.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "pf_hunger_comes"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Let Hunger come. I fear him not.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Gluttony,
                            NextNodeId = "pf_hunger_comes",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "pf_hunger_comes",
                    SpeakerName = "Narrator",
                    Text = "Hunger cometh like a shadow. The wasters cower. Those who refused to werk now beg for bread. Piers calleth Hunger off at last: \"Enough! They have lerned.\" But some have lerned nothing. The feld is half-plowed. The question remaineth: what is Do-Wel?",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Do-Wel is honest labour — the plough, the soil, the seed.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "pf_pardon"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Do-Wel? I knowe not yet. Perhaps more than plowing.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "pf_pardon"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "pf_pardon",
                    SpeakerName = "Narrator",
                    Text = "A priest bringeth a pardoun from Treuthe. It saith two lines only: Et qui bona egerunt ibunt in vitam eternam — qui vero mala in ignem eternum. Those who do good go to life eternal; those who do evil to eternal fire. Piers readeth it. His face darkeneth. In pure tene he teareth the pardoun asunder.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Why dost thou tear it, Piers? It cometh from Treuthe!\"",
                            IsCorrectChoice = true,
                            NextNodeId = "pf_piers_answer"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Good. Pardons from priests are worth nothing.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Wrath,
                            NextNodeId = "pf_piers_answer"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "pf_piers_answer",
                    SpeakerName = "Piers the Plowman",
                    Text = "\"Because no pardoun can save a man who will not save hymself. Do-Wel — do good — is the only pardoun that mattereth. Not parchment nor wax nor the pope's seal. Deeds. Kyndeness. Love. That is the pardoun of Treuthe.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Then I shal seke Do-Wel with alle my herte.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "piers_field"
                            }
                        }
                    }
                }
            };

            Save(tree, "PiersFieldDialogue");
        }

        /// <summary>Passus V-VII – The Seven Deadly Sins confess. Each sin speaks.</summary>
        static void CreateSevenSinsTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("SevenSinsDialogue");
            tree.RootNodeId = "ss_gathering";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_gathering",
                    SpeakerName = "Reason",
                    Text = "Reason preacheth before the King and all the commons. His words strike like a flail. One by one the Sins come forward, weeping, driven by Repentaunce to confess. \"Now cometh forth,\" Reason crieth. \"Whiche of you will speke first?\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Listen to Pride's confession.",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_pride"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Listen to Wrath's confession.",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_wrath"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Listen to Envy's confession.",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_envy"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_pride",
                    SpeakerName = "Pride",
                    Text = "Peronelle Proudherte casteth hirself to the erthe. \"Lord, I have synned! I bosted and bragged before alle folk. I looked down on the poore and wore finer clothes than I sholde. I sat in the highest seat in chirche. Mercy, Lord! I shal be meke hereafter.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Meekness becometh thee. Arise and sin no more.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_avarice"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Thy pride is the moste dangerous sin. I trust thee not.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Wrath,
                            NextNodeId = "ss_avarice"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_wrath",
                    SpeakerName = "Wrath",
                    Text = "Wrath cometh with red yën, a running nose, and biting lippes. \"I have been cook in the convent and the prioress's kitchen, brewing bitter sauces of strife. I set monk against monk, friar against friar, with tales and treacheries til both sides tore their clothes in fury.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Repent truly and let go thy anger.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_avarice"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Thou art a serpent. No repentance can cure thee.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Wrath,
                            NextNodeId = "ss_avarice",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.StartSinMiniboss,
                                SinParam = SinType.Wrath
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_envy",
                    SpeakerName = "Envy",
                    Text = "Envy with hevy herte asketh shrifte. \"I have been envious thirty wynter. Whanne my neighbour prospereth, I wepe. If his corn groweth, I wish it blighten. If he hath a faire wife, I gnaw my lippes. I can not eat for envye til mischief falle him.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Turn thy gaze inward. What hast thou that deserveth gratitude?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_avarice"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Thou art pitiful. I can not even be angry at thee.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            NextNodeId = "ss_avarice"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_avarice",
                    SpeakerName = "Avarice",
                    Text = "Avarice cometh, thin as a leek, with chapped lippes. \"I have been a mercer and a moneylender. I lerned to lie when young — the firste lesoun of my craft. I wove false cloth, stretched it wet to make it seem longer. My wife brewed thin ale and mixed it with good to deceive the poor.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Restore what thou hast stolen. Restitution is the path.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_sloth"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Thou knowest not how to repent, dost thou?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_avarice_ignorance"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_avarice_ignorance",
                    SpeakerName = "Avarice",
                    Text = "\"Restitution?\" Avarice looketh blankly. \"I wende restitucioun were a thing of robbing! I knowe no French — reddite was to me as strange as any tongue. I swear I thought good dealing was just clever dealing.\" He weepeth, not from guilt but from confusion.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Give back what thou hast taken. That is restitution.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_sloth"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_sloth",
                    SpeakerName = "Sloth",
                    Text = "Sloth falleth forward, barely awake. \"I have be prest and person passing thretti wynter. I can not sing ne read saints' lives. I know rymes of Robyn Hood better than my Paternoster. If I pray, my tongue runneth on trifles. I have made vowes forty tymes and forgeten hem on the morwe.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Wake, Sloth. The pilgrimage awaiteth. There is still time.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_glutton"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Thou art hopeless. Sleep on, if thou wilt.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "ss_glutton"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_glutton",
                    SpeakerName = "Narrator",
                    Text = "Glotoun is headed to chirche to confess — but Beton the brewstere calleth from the alehouse door: \"Good ale, Glutton! Come in and taste!\" He goeth in. He drinketh til his eyen glaze. He pisseth four pintels and bloweth his round ruwet at Clementis nose. He falleth down like a gleeman's bitch, and Clement and his wife carry him home.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Poor wretch. Even confession cannot hold against habit.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_repentance"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I too have tasted too deeply. Perhaps I understand him.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ss_repentance"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ss_repentance",
                    SpeakerName = "Repentaunce",
                    Text = "Repentaunce weepeth and prayeth for alle sinners. \"Now God, of thy goodnesse, graunte us thy grace. Have mercy on these wrecches that repenten now! A thousand fold of angels clustereth about thee: had they not thy bidding, they might not come hither.\" The folk cry out: \"Where is Treuthe? Who shal lede us there?\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal seke Treuthe with alle these penitents.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "seven_sins"
                            }
                        }
                    }
                }
            };

            Save(tree, "SevenSinsDialogue");
        }

        // ─────────────────────────────────────────────────────────────
        //  VISION II
        // ─────────────────────────────────────────────────────────────

        /// <summary>Passus VIII-IX – The Quest for Do-Wel. Thought, Wit, and Study teach Wille.</summary>
        static void CreateQuestDoWelTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("QuestDoWelDialogue");
            tree.RootNodeId = "dw_thought_appears";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dw_thought_appears",
                    SpeakerName = "Thought",
                    Text = "\"Thus Y-robed in russet, I romede aboute al a somer seson for to seke Dowel.\" A tall man overcometh thee — Thought is his name. \"I have folwed thee this seven yere. What sekest thou, Wille?\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"What is Do-Wel? And Do-Bet? And Do-Best?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dw_thought_teaches"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I seke a living. The dreaming hath gone on long enough.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "dw_thought_teaches"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dw_thought_teaches",
                    SpeakerName = "Thought",
                    Text = "\"Do-Wel is the man who laboureth truly, liveth by the lawe, and taketh but his own. Do-Bet doeth all this and more — he techeth, he helpeth, he giveth from his own good. Do-Best beareth a bishop's crozier — he ruleth what Do-Wel and Do-Bet have wrought.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"That is too simple. Where is Wit? I wolde here more.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dw_wit_teaches"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Enough. Do-Wel is labour. I understand.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "quest_do_wel"
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dw_wit_teaches",
                    SpeakerName = "Wit",
                    Text = "Wit sitteth in a castle — the castle of Kynde, which is the human body. \"Inwit dwelleth within, and Anima is the lady of the house. Do-Wel is Inwit — conscience, understanding. He that liveth by Inwit and doth what conscience techeth — that is Do-Wel. But there is a wicked keeper at the gate: Kynde Thankyng — the flesh, the senses. Beware him.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"What of Study? Hath she aught to teche?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dw_study_rebuke"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal guard my Inwit well. That is enough for me.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "quest_do_wel"
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dw_study_rebuke",
                    SpeakerName = "Study",
                    Text = "Dame Study scoldeth Wit: \"Thou fonnede fool, why teachest thou this man? Wisdom and wit are wasted on one who will not labour! Has he plowed? Has he woven? Nay — he dreameth and wandereth.\" She turneth to thee. \"Dreamer, if thou wolt have my cunning, thou must be humble. Swear to love loyally and I shal direct thee to Clergie.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I swear it. I shal be humble and lerning.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "quest_do_wel"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I nede no schoolmaster. The road itself techeth.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                }
            };

            Save(tree, "QuestDoWelDialogue");
        }

        /// <summary>Passus X-XI – Deeper into the quest. Scripture, Imaginatif, and Fortune.</summary>
        static void CreateDongeounDepthsTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("DongeounDepthsDialogue");
            tree.RootNodeId = "dd_scripture";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dd_scripture",
                    SpeakerName = "Scripture",
                    Text = "\"Multi to a mangerie and to the mete were sompned, and whan the peple was plener comen the porter unpynned the gate. He plukked in pauci priveliche and leet the remenaunt go roume.\" Many are called, few are chosen. The words sting — for who among the dreamers is truly chosen?",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Am I among the chosen? How can I knowe?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dd_fortune_tempts"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"If few are chosen, why strive at all?\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "dd_fortune_tempts"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dd_fortune_tempts",
                    SpeakerName = "Narrator",
                    Text = "Fortune fetcheth thee into the land of Couvetise. A mirror showeth thee the whole world. Two damoseles attend: Concupiscentia-carnis and Couvetise-of-eyes. \"Looke,\" Fortune whispereth. \"All this may be thine. Youthe and pleasure and the long bright afternoon of life.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Turn from the mirror. \"Fortune's gifts are borrowed, not given.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dd_imaginatif"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Gaze deeper into the mirror. \"Show me more.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Lust,
                            NextNodeId = "dd_elde_approaches",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dd_elde_approaches",
                    SpeakerName = "Elde",
                    Text = "While thou gazest, Elde creepeth close. His hand toucheth thy shoulder and thy hair whiteneth. \"Fortune showeth thee Youthe,\" Elde croaketh, \"but I am the truth of the mirror. The face thou seest is not thine — it is what was. I am what is, and what shall be.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Shake off Elde's grip. \"I have much to do ere thou claimest me.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "dd_imaginatif"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"If age is inevitable, what use is striving?\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            NextNodeId = "dd_imaginatif",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.IncrementPoverty,
                                IntParam = 1
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "dd_imaginatif",
                    SpeakerName = "Imaginatif",
                    Text = "Imaginatif cometh at last — the power of memory and reflection. \"Wille, thou hast wandered long and lerned muche but understood litel. Clergie is not wisdom; doing is not enough. The heathen who liveth justly is nearer Treuthe than the baptised man who betrayeth his othe. God saveth whom He will — and He seeth the herte, not the name.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Then kynde knowing — experience and conscience — is the truest guide.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "dongeoun_depths"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"That giveth me hope. Even a dreamer may finde Treuthe.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "dongeoun_depths"
                            }
                        }
                    }
                }
            };

            Save(tree, "DongeounDepthsDialogue");
        }

        /// <summary>Passus XII-XIII – Patience and Conscience feast. Time's passage and moral growth.</summary>
        static void CreateYearsPassTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("YearsPassDialogue");
            tree.RootNodeId = "yp_patience_feast";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "yp_patience_feast",
                    SpeakerName = "Narrator",
                    Text = "Years have passed in the dreaming. Conscience inviteth Wille to a feast with a Doctor of Divinity. Patience sitteth beside thee, clad in poor clothes, eating crusts. The Doctor eateth rich food and drinketh deep wine while preaching poverty to others.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"This Doctor preacheth what he doeth not. His words are empty.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "yp_patience_speaks"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"The Doctor hath lerning. Perhaps his indulgence is earned.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Gluttony,
                            NextNodeId = "yp_patience_speaks"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "yp_patience_speaks",
                    SpeakerName = "Patience",
                    Text = "Patience smileth at his crusts. \"Contra!\" he saith to the Doctor. \"I shal prove that Do-Wel and Do-Bet and Do-Best are not as this Doctor techeth.\" He openeth his hand: in it lieth nothing but a phrase — Disce, Doce, Dilige Deum. Learn, teach, love God. Three words. A life's work.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Disce, Doce, Dilige Deum. Learn, teach, love. I shal remember.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "yp_conscience_leaves"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Three words? After all these years of seeking? That is all?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "yp_patience_riddle"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "yp_patience_riddle",
                    SpeakerName = "Patience",
                    Text = "\"A riddle, then. I bere a thing — no tonge can telle what it is. Clerkes cannot finde it though they searched forever. He that suffreth all that cometh, hath this thing already. It is the patience of the poor, the acceptance of Kynde's will, the love that asketh nothing. Dost thou understand?\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"It is love. Patient love.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "yp_conscience_leaves"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I do not understand. But I shal carry thy words.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "yp_conscience_leaves"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "yp_conscience_leaves",
                    SpeakerName = "Conscience",
                    Text = "Conscience riseth from the feast. \"I shal no longer abide with this Doctor. I wolde go with Patience, and be parfite in alle weathers and alle sufferings. Wille — come. We shal seke Piers the Plowman once more, for he can set all things right.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal go with thee and Patience. The journey continueth.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "years_pass"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I am weary. But I shal not stop now.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "years_pass"
                            }
                        }
                    }
                }
            };

            Save(tree, "YearsPassDialogue");
        }

        /// <summary>Passus XIV+ – The Field of Grace. The ending. Grace does not come.</summary>
        static void CreateFieldOfGraceTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("FieldOfGraceDialogue");
            tree.RootNodeId = "fog_arrival";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "fog_arrival",
                    SpeakerName = "Narrator",
                    Text = "The field is empty. The wind moveth through the grass. Conscience standeth beside thee. In the distance, the Tower of Treuthe still gloweth — but the path is lost. Antichrist's forces have overrun the barn of Unity. Conscience crieth out.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"What dost thou cry for, Conscience?\"",
                            IsCorrectChoice = true,
                            NextNodeId = "fog_conscience_cries"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "fog_conscience_cries",
                    SpeakerName = "Conscience",
                    Text = "\"By Crist and by the crosse! I wol become a pilgrym and walken as wide as the worlde lasteth to seke Piers the Plowman — that he may destroye Pryde. Kynde, avenge me! Send me hap and hele til I have Piers the Plowman!\" And Conscience walketh forth, crying after Grace.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal walk with thee, Conscience. Til Piers cometh.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "fog_waiting",
                            Condition = new DialogueTreeDefinition.ConditionDef
                            {
                                Type = DialogueTreeDefinition.ConditionType.HasVisited,
                                StringParam = "piers_field"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"We have come so far. We shal not turn back now.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "fog_waiting"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "fog_waiting",
                    SpeakerName = "Narrator",
                    Text = "And I awakede therewith, and wroot what I had dremed. The poem endeth as the dream endeth — not with triumph, not with failure, but with the cry of Conscience into the wind, seeking Grace. She does not come. She does not come.\n\nBut the dreamer keepeth dreaming. And that, perhaps, is enough.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"And so the dream continueth...\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "field_of_grace"
                            }
                        }
                    }
                }
            };

            Save(tree, "FieldOfGraceDialogue");
        }

        // ─────────────────────────────────────────────────────────────
        //  CROSSOVER: SIR GAWAIN AND THE GREEN KNIGHT
        // ─────────────────────────────────────────────────────────────

        /// <summary>Green Chapel – expanded from the existing choice beat into a full tree.</summary>
        static void CreateGreenChapelTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("GreenChapelDialogue");
            tree.RootNodeId = "gc_approach";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "gc_approach",
                    SpeakerName = "Narrator",
                    Text = "The Chapel is no chirche but a barrow — an olde cave at the bank of a stream, overgrown with grass. The ax-marks on the stones are old. A sound cometh from within: the whetting of a blade on a grindstone. WHIRRRRE. WHIRRRRE.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Call out: \"Who is there? Show thyself!\"",
                            IsCorrectChoice = true,
                            NextNodeId = "gc_green_knight_appears"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Creep forward in silence.",
                            IsCorrectChoice = true,
                            NextNodeId = "gc_green_knight_appears"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Turn back. This place is cursed.",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Sloth,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SetGreenKnightRandom,
                                BoolParam = true
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "gc_green_knight_appears",
                    SpeakerName = "The Green Knight",
                    Text = "He vaulteth over the stream on his ax like a pole, striding huge across the snow. His skin is green as summer grass, his beard red as holly berries. \"Now, sir sweet, thou seest that I kepe my covenant. Art thou Gawain that sittz before me? Thou shalt fange the tap that I thee owe.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I am. I schal stonde thy stroke and start no more.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "gc_three_blows"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I am not Gawain. Thou hast the wrong dreamer.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Pride,
                            NextNodeId = "gc_green_knight_laughs"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "gc_green_knight_laughs",
                    SpeakerName = "The Green Knight",
                    Text = "The Green Knight laugheth. \"Not Gawain? Then thou art less than Gawain — for even he, who kept the girdle in cowardice, stood to receive the blow. Thou art the dreamer Wille, and thou art in my chapel now. The covenant standeth, whatever name thou bearest.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Then I shal stand as Gawain stood.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "gc_three_blows"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "gc_three_blows",
                    SpeakerName = "The Green Knight",
                    Text = "He lifteth the ax. The first blow — he stoppeth short. \"Thou flinched,\" he saith. The second — again he feints. \"Art thou afraid?\" The third blow falleth true — a nick upon thy neck, no more. Blood staineth the snow. The Green Knight resteth the ax upon the ground.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I have taken thy blow. The covenant is fulfilled.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "gc_revelation"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Draw thy weapon. \"Now it is my turn.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Wrath,
                            NextNodeId = "gc_revelation"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "gc_revelation",
                    SpeakerName = "The Green Knight",
                    Text = "\"Thou art confessed so clean, beknowen of thy misdeeds, that I hold thee polished as a pearl. The first two blows were for thy honesty. The nick — that was for the green girdle, the small flaw: thou lovest thy life too well. But that is a small sin. Go, dreamer, and bear the scar as reminder: Treuthe is never perfect in mortal flesh.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal wear the scar gladly, and remember.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "green_chapel"
                            }
                        }
                    }
                }
            };

            Save(tree, "GreenChapelDialogue");
        }

        // ─────────────────────────────────────────────────────────────
        //  CROSSOVER: SIR ORFEO
        // ─────────────────────────────────────────────────────────────

        /// <summary>Boundary Tree – expanded Orfeo encounter with the fairy music.</summary>
        static void CreateBoundaryTreeOrfeoTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("BoundaryTreeDialogue");
            tree.RootNodeId = "bt_tree";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "bt_tree",
                    SpeakerName = "Narrator",
                    Text = "At the boundary of the dreaming, an ympe-tree standeth — a grafted tree, olde and strange. Beneath its branches, the air shimmereth. Music drifteth from beyond: a harpe of such sweetness that the heart acheth to follow. In Sir Orfeo's tale, his queen Heurodis lay beneath such a tree when the Fairy King stole her away.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I shal not sleep beneath this tree. I know what happened to Heurodis.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "bt_resist"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Sit beneath the tree and listen to the music.",
                            IsCorrectChoice = false,
                            NextNodeId = "bt_listen"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "bt_resist",
                    SpeakerName = "Narrator",
                    Text = "Thou pullest away from the music. It followeth — threading through the branches, sweet and insistent. But thou art stronger than the song. The ympe-tree fadeth. The boundary holdeth. Yet thou knowest now: there is another world beside this one, and its music never truly stops.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Return to the path.",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "boundary_tree"
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "bt_listen",
                    SpeakerName = "Narrator",
                    Text = "The music deepeneth. Through the shimmering air thou seest riders — sixty ladies on snow-white steeds, falcons on their wrists. No man among them. They do not see thee — or perhaps they see only what they wish to take. The Fairy King's court moveth through the boundary like mist.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Rise and flee before they notice thee.",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "boundary_tree"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "Follow the riders into the Otherworld.",
                            IsCorrectChoice = false,
                            TransitionToLocationId = "otherworld",
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SetOrfeoTriggered
                            }
                        }
                    }
                }
            };

            Save(tree, "BoundaryTreeDialogue");
        }

        /// <summary>Otherworld – Orfeo's fairy realm. No return.</summary>
        static void CreateOtherworldTree()
        {
            var tree = CreateOrLoad<DialogueTreeDefinition>("OtherworldDialogue");
            tree.RootNodeId = "ow_arrival";
            tree.Nodes = new DialogueTreeDefinition.DialogueNode[]
            {
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ow_arrival",
                    SpeakerName = "Narrator",
                    Text = "The boundary dissolveth. Thou standest in a country faire as any sunne could make: castles of crystal, pillars of gold, every stone a gemstone. But look closer — in the cloister of the castle, folk lye that were thither fetched: some headless, some armless, some strangled while eating. They are not dead. They are not alive. They are taken.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I have entered the realm of the Fairy King. There is no going back.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ow_fairy_king"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ow_fairy_king",
                    SpeakerName = "The Fairy King",
                    Text = "He sitteth upon a throne of green jade, his queen pale beside him. \"Another dreamer cometh to my court. Wille — I have watched thee long. In the waking world and in the dream. Thou seekest Do-Wel. Here there is no doing — only being. Thou shalt remain.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Orfeo won back his queen with music. I have no harpe, but I have my words.\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ow_orfeo_gambit"
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Then I shal fight my way free.\"",
                            IsCorrectChoice = false,
                            AssociatedSin = SinType.Wrath,
                            NextNodeId = "ow_no_escape"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ow_orfeo_gambit",
                    SpeakerName = "The Fairy King",
                    Text = "\"Words?\" The King's eyes glitter. \"Orfeo played his harpe and I wept. Words — poetry — that is another kind of music. Very well, dreamer: speak thy poem, and if it moveth me, I shal grant thee... not freedom. But a contest. Win, and the boundary openeth. Lose, and thou art mine forever.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"In a somer seson, whan softe was the sonne...\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ow_contest",
                            Condition = new DialogueTreeDefinition.ConditionDef
                            {
                                Type = DialogueTreeDefinition.ConditionType.HasVisited,
                                StringParam = "malvern"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"Love is leche of lif, and nexte Oure Lord selve...\"",
                            IsCorrectChoice = true,
                            NextNodeId = "ow_contest",
                            Condition = new DialogueTreeDefinition.ConditionDef
                            {
                                Type = DialogueTreeDefinition.ConditionType.HasVisited,
                                StringParam = "fayre_felde"
                            }
                        },
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I... I have no words.\"",
                            IsCorrectChoice = false,
                            NextNodeId = "ow_no_escape"
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ow_contest",
                    SpeakerName = "The Fairy King",
                    Text = "A tear runneth down the King's face. He doth not wipe it. \"Thou hast the gift of the dreamer — to make the unseen visible. Very well. My champion shall face thee. Defeat him and the boundary openeth.\" He gestureth, and from the crystal cloister a figure riseth: Pride incarnate, armoured in stolen glory.",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"I am ready.\"",
                            IsCorrectChoice = true,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "otherworld"
                            }
                        }
                    }
                },
                new DialogueTreeDefinition.DialogueNode
                {
                    NodeId = "ow_no_escape",
                    SpeakerName = "The Fairy King",
                    Text = "The King smileth. It is not a kind smile. \"No music, no words, no bargain. Thou art mine, dreamer. Thou shalt stand in my cloister with the taken. But fret not — time doth not pass here. Thou shalt forget, in time. They all forget.\"",
                    Choices = new DialogueTreeDefinition.ChoiceDef[]
                    {
                        new DialogueTreeDefinition.ChoiceDef
                        {
                            Text = "\"...\"",
                            IsCorrectChoice = false,
                            Consequence = new DialogueTreeDefinition.ConsequenceDef
                            {
                                Type = DialogueTreeDefinition.ConsequenceType.SaveCheckpoint,
                                StringParam = "otherworld"
                            }
                        }
                    }
                }
            };

            Save(tree, "OtherworldDialogue");
        }

        // ─────────────────────────────────────────────────────────────
        //  UTILITIES
        // ─────────────────────────────────────────────────────────────

        static T CreateOrLoad<T>(string name) where T : ScriptableObject
        {
            string path = $"{TreePath}/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            return ScriptableObject.CreateInstance<T>();
        }

        static void Save(DialogueTreeDefinition tree, string name)
        {
            string path = $"{TreePath}/{name}.asset";
            if (!AssetDatabase.Contains(tree))
                AssetDatabase.CreateAsset(tree, path);
            else
                EditorUtility.SetDirty(tree);
        }

        static void EnsureDirectory(string path)
        {
            if (Directory.Exists(path)) return;

            var parts = path.Replace("\\", "/").Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
