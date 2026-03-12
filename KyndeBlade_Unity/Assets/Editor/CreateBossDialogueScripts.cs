#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using KyndeBlade;

namespace KyndeBlade.Editor
{
    /// <summary>
    /// Generates BossDialogueScript assets for every boss in the game.
    /// Lines are drawn from / inspired by Piers Plowman, Sir Gawain, and medieval tradition.
    /// Run via KyndeBlade > Generate Boss Dialogue Scripts.
    /// </summary>
    public static class CreateBossDialogueScripts
    {
        const string OutputPath = "Assets/Resources/Data/BossDialogue";

        [MenuItem("KyndeBlade/Generate Boss Dialogue Scripts")]
        public static void GenerateAll()
        {
            EnsureDirectory(OutputPath);

            CreateWrathScript();
            CreateFalseScript();
            CreateLadyMedeScript();
            CreateGreenKnightScript();
            CreateEldeScript();
            CreatePrideScript();
            CreateHungerScript();
            CreateEnvyScript();
            CreateSlothScript();
            CreateLustScript();
            CreateGluttonyScript();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generated all boss dialogue scripts in {OutputPath}.");
        }

        static void CreateWrathScript()
        {
            var script = CreateOrLoad("WrathDialogue");
            script.EntryLine =
                "I am Wrath, that was a frere, / and convent gardyner of grete houses. " +
                "I sette strife bitwene brethren / til they tore their clothes for ire!";
            script.MidPointLine =
                "My veins run hotter than a forge! / Thou thinkest patience is a virtue? " +
                "I shal show thee what burneth beneath forbearance!";
            script.DefeatLine =
                "The anger... fadeth. / Like embers in rain. / Was it mine own fury, " +
                "or did I carry another's fire?";
            script.BossDefeatedLine =
                "Wrath lieth still. / The red mist lifteth from the air. / " +
                "Somewhere, a bell ringeth for vespers.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Wrathful Blow", Dialogue = "Feel my burning tene! / No man escapeth my stroke unscathed!" },
                new BossDialogueScript.ActionLine { ActionName = "Fury Rush", Dialogue = "I am as wrooth as the wynde! / Stand aside or be broken!" },
                new BossDialogueScript.ActionLine { ActionName = "Berserker Rage", Dialogue = "MY BLOOD BOILETH! / The convent walls crumble before my fist!" },
                new BossDialogueScript.ActionLine { ActionName = "Rage of the Friar", Dialogue = "I set brethren to blowes / and nonne agayn nonne!" }
            };
            Save(script, "WrathDialogue");
        }

        static void CreateFalseScript()
        {
            var script = CreateOrLoad("FalseDialogue");
            script.EntryLine =
                "I am False — fader of flatterie, / wedded to Favel, " +
                "kin to every liar that liveth. / Thinkest thou thy sword cutteth a lie?";
            script.MidPointLine =
                "Thou strikest at shadows, dreamer! / I am in every word unspoken, " +
                "every promise unkept. / Cut me and I multiply.";
            script.DefeatLine =
                "I am undone... but falseness / never truly dieth. / " +
                "It only changeth its name.";
            script.BossDefeatedLine =
                "False crumbleth — not into dust, / but into whispers. " +
                "The air itself seemeth less honest.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Deceiver's Strike", Dialogue = "Was that my blade — or thine own shadow?" },
                new BossDialogueScript.ActionLine { ActionName = "Mirror Image", Dialogue = "Which False is real? / Neither. Both. It mattereth not." },
                new BossDialogueScript.ActionLine { ActionName = "Venom Word", Dialogue = "A word whispered / is sharper than any edge." },
                new BossDialogueScript.ActionLine { ActionName = "Favel's Flattery", Dialogue = "Thou art a fine fighter, dreamer — / nay, the finest! Dost thou believe me?" }
            };
            Save(script, "FalseDialogue");
        }

        static void CreateLadyMedeScript()
        {
            var script = CreateOrLoad("LadyMedeDialogue");
            script.EntryLine =
                "I am Mede the mayde — / Lady of reward and ruin. " +
                "All men love me. / Even thou, dreamer. Especially thou.";
            script.MidPointLine =
                "Gold buyeth all things! / Judges, priests, even kings " +
                "bow to my purse. / What maketh thee different, Wille?";
            script.DefeatLine =
                "My gold... scattereth. / But someone always gathereth it up. " +
                "And when they do... / I return.";
            script.BossDefeatedLine =
                "Lady Mede falleth, / her crown rolling across the court floor. " +
                "Conscience standeth over her, unmoved.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Mede's Demand", Dialogue = "Every man hath his price. / What is thine, dreamer?" },
                new BossDialogueScript.ActionLine { ActionName = "Golden Lash", Dialogue = "My chains are gold — / and men wear them willingly!" },
                new BossDialogueScript.ActionLine { ActionName = "Bribe", Dialogue = "Take this. / Forget thy purpose. / It is easier." },
                new BossDialogueScript.ActionLine { ActionName = "Court Intrigue", Dialogue = "I have bought judges and sheriffs — / what is one poor dreamer?" }
            };
            Save(script, "LadyMedeDialogue");
        }

        static void CreateGreenKnightScript()
        {
            var script = CreateOrLoad("GreenKnightDialogue");
            script.EntryLine =
                "Now, sir sweet, / thou seest I kepe covenauntz. " +
                "Thou shalt fange the tap / that I thee owe!";
            script.MidPointLine =
                "Thou art a good knight, Wille! / Better than many who have stood here. " +
                "But the ax remembreth / what the arm forgetteth.";
            script.DefeatLine =
                "The hunte... lasteth ever. / Thou art confessed clean, " +
                "polished as a pearl. / Gawain himself could not do better.";
            script.BossDefeatedLine =
                "The Green Knight boweth — / not in defeat, but in respect. " +
                "He vanisheth / like mist on the Wirral.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Beheading Blow", Dialogue = "Bare thy neck, dreamer! / The covenant demandeth it!" },
                new BossDialogueScript.ActionLine { ActionName = "The Green Girdle", Dialogue = "Didst thou keep the girdle? / That small cowardice costeth dear." },
                new BossDialogueScript.ActionLine { ActionName = "Axe Sweep", Dialogue = "This ax hath felled stronger knights / than thee, in grener chapels!" },
                new BossDialogueScript.ActionLine { ActionName = "Chapel's Fury", Dialogue = "The Wirral trembleth! / Feel the Chapel's wrath!" }
            };
            Save(script, "GreenKnightDialogue");
        }

        static void CreateEldeScript()
        {
            var script = CreateOrLoad("EldeDialogue");
            script.EntryLine =
                "I am Elde — Old Age. / I come for alle men, dreamer. " +
                "Not today, perhaps. / But soon. Always soon.";
            script.MidPointLine =
                "Feel the joint stiffen? / The breath shorten? / " +
                "That is my touch. / Every blow of mine taketh a year.";
            script.DefeatLine =
                "Thou canst not defeat me, dreamer. / Only delay me. " +
                "I shal wait. / I am the most patient of enemies.";
            script.BossDefeatedLine =
                "Elde withdraweth — / not vanquished, but patient. " +
                "He will return. / He always returneth.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Years Touch", Dialogue = "Feel the years pass! / Thy youth frayeth like old cloth." },
                new BossDialogueScript.ActionLine { ActionName = "Decaying Grasp", Dialogue = "Nothing lasteth. / Not strength, not beauty, not dreaming." },
                new BossDialogueScript.ActionLine { ActionName = "Wither", Dialogue = "Thy bones creak / as mine have creaked these hundred years." },
                new BossDialogueScript.ActionLine { ActionName = "Time's Weight", Dialogue = "The burden of years / is heavier than any armour." }
            };
            Save(script, "EldeDialogue");
        }

        static void CreatePrideScript()
        {
            var script = CreateOrLoad("PrideDialogue");
            script.EntryLine =
                "I am Pride — Peronelle Proudherte! / I sat in the highest seat in chirche, " +
                "and looked down upon all folk. / Now I shal look down upon thee.";
            script.MidPointLine =
                "Dost thou think thyself worthy / to face me? / " +
                "I who wore finer robes than the Queen? / Thou art dirt beneath my slipper!";
            script.DefeatLine =
                "Brought low... / I who was highest. / " +
                "Is this what the meek inherit? / This cold, hard ground?";
            script.BossDefeatedLine =
                "Pride falleth — / the oldest lesson, the hardest learnt. " +
                "Her silks are suddenly threadbare.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Pride's Fall", Dialogue = "I shal cast thee down / as I have cast down alle who stood against me!" },
                new BossDialogueScript.ActionLine { ActionName = "Vainglory", Dialogue = "Behold my glory! / No dreamer can stand before such magnificence!" },
                new BossDialogueScript.ActionLine { ActionName = "Contempt", Dialogue = "Thou art nothing. / A ragged dreamer playing at knighthood." },
                new BossDialogueScript.ActionLine { ActionName = "Peacock's Display", Dialogue = "See how I shine! / Even in battle I am more beautiful than thee." }
            };
            Save(script, "PrideDialogue");
        }

        static void CreateHungerScript()
        {
            var script = CreateOrLoad("HungerDialogue");
            script.EntryLine =
                "Piers calleth me forth. / I am Hunger — " +
                "the teacher of wasters. / The belly knoweth no pride.";
            script.MidPointLine =
                "Thy strength faileth! / An empty belly " +
                "fighteth poorly. / Now thou knowest what the poor endure.";
            script.DefeatLine =
                "Piers... calleth me off. / I obey. / " +
                "But I am never far. / I wait in every empty cupboard.";
            script.BossDefeatedLine =
                "Hunger retreateth — / not vanquished, but restrained. " +
                "Piers hath power over him / for now.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Gnawing Famine", Dialogue = "Thy belly growleth! / I am the pain thou canst not ignore!" },
                new BossDialogueScript.ActionLine { ActionName = "Empty Belly", Dialogue = "The wasters begged / when I gripped their guts!" },
                new BossDialogueScript.ActionLine { ActionName = "Desperate Swipe", Dialogue = "A hungry man / fighteth like a cornered beast!" },
                new BossDialogueScript.ActionLine { ActionName = "Withering Grip", Dialogue = "I squeeze the life / from those who will not labour." }
            };
            Save(script, "HungerDialogue");
        }

        static void CreateEnvyScript()
        {
            var script = CreateOrLoad("EnvyDialogue");
            script.EntryLine =
                "I am Envye — / with hevy herte I came to shrifte. " +
                "Thirty wynter I have been envious. / Thy joy is my torment!";
            script.MidPointLine =
                "If thy neighbour prospereth, I wepe! / If his corn groweth, " +
                "I wish it blight! / Now I wish blight upon thee, dreamer!";
            script.DefeatLine =
                "I can not wepe... / for envye of thy victory. " +
                "Even in defeat / I envy those who lose with grace.";
            script.BossDefeatedLine =
                "Envy crawleth away, / gnawing his own lippes. " +
                "His eyes never leave thee — / green with wanting.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Envious Glare", Dialogue = "I gnaw my lippes at thy strength! / Why shouldst thou have what I lack?" },
                new BossDialogueScript.ActionLine { ActionName = "Bitter Spite", Dialogue = "If I can not have joy, / neither shalt thou!" },
                new BossDialogueScript.ActionLine { ActionName = "Covetous Strike", Dialogue = "That sword — those skills — / they should be mine!" },
                new BossDialogueScript.ActionLine { ActionName = "Poisoned Heart", Dialogue = "My venom is not in my blade / but in my gaze." }
            };
            Save(script, "EnvyDialogue");
        }

        static void CreateSlothScript()
        {
            var script = CreateOrLoad("SlothDialogue");
            script.EntryLine =
                "I am Sloth... / *yawn* ... I have be prest and person " +
                "passing thretti wynter / and can neyther read ne singe saints' lives...";
            script.MidPointLine =
                "*yawn* ... Must we continue? / I knowe rymes of Robyn Hood " +
                "better than my Paternoster. / Fighting is so... tiresome.";
            script.DefeatLine =
                "Defeated? / I supposed it would come to this. / " +
                "I made vowes forty tymes / and forgat hem on the morwe...";
            script.BossDefeatedLine =
                "Sloth collapseth — / not from wounds but from sheer lethargy. " +
                "Even defeat / is too much effort.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Drowsy Swing", Dialogue = "*yawn* ... Oh, is it my turn? / Very well then..." },
                new BossDialogueScript.ActionLine { ActionName = "Torpor Wave", Dialogue = "Sleep, dreamer. / Sleep is the only honest occupation." },
                new BossDialogueScript.ActionLine { ActionName = "Procrastination", Dialogue = "I shal fight thee properly / ... tomorrow. Definitely tomorrow." },
                new BossDialogueScript.ActionLine { ActionName = "Forgotten Prayer", Dialogue = "Paternoster... Pater... / what was the rest again?" }
            };
            Save(script, "SlothDialogue");
        }

        static void CreateLustScript()
        {
            var script = CreateOrLoad("LustDialogue");
            script.EntryLine =
                "I am Lecherye — / I lay by the loveliest " +
                "and by the laste also. / Every glaunce is a snare, dreamer.";
            script.MidPointLine =
                "Thou resistest? / Few do. / Even holy men " +
                "have fallen to a fair face / and a whispered word.";
            script.DefeatLine =
                "The fire... cooled. / Perhaps there is warmth " +
                "that burneth not. / I had forgotten.";
            script.BossDefeatedLine =
                "Lust shrinketh back, / ashamed at last. " +
                "What seemed alluring / now seemeth only sad.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Tempting Gaze", Dialogue = "Looke upon me, dreamer. / Am I not faire?" },
                new BossDialogueScript.ActionLine { ActionName = "Ensnaring Touch", Dialogue = "One touch — / and thy resolve unraveleth." },
                new BossDialogueScript.ActionLine { ActionName = "Burning Desire", Dialogue = "The flesh is weaker / than the spirit knoweth." },
                new BossDialogueScript.ActionLine { ActionName = "Silk Trap", Dialogue = "My chains are silk, / and men embrace them." }
            };
            Save(script, "LustDialogue");
        }

        static void CreateGluttonyScript()
        {
            var script = CreateOrLoad("GluttonyDialogue");
            script.EntryLine =
                "I am Glotoun! / Beton the brewstere bade me in " +
                "and I drank til my eyen glazed. / Now I shal make THEE reel!";
            script.MidPointLine =
                "More! MORE! / Is that all thou hast? / I have supped " +
                "on stronger brews / and swallowed fiercer men!";
            script.DefeatLine =
                "Urrrp... / I think I have had... too muche. " +
                "The alehouse... spinneth...";
            script.BossDefeatedLine =
                "Gluttony falleth, / a full belly and an empty soul. " +
                "Clement and his wife / drag him home once more.";
            script.ActionBarks = new List<BossDialogueScript.ActionLine>
            {
                new BossDialogueScript.ActionLine { ActionName = "Belch Blast", Dialogue = "He blew his rouwet — / foul as a furnace!" },
                new BossDialogueScript.ActionLine { ActionName = "Drunken Swipe", Dialogue = "I can fiiight... *hic* / stood still, the lot of ye!" },
                new BossDialogueScript.ActionLine { ActionName = "Ale Flood", Dialogue = "Beton's best ale! / Drink deep or drown!" },
                new BossDialogueScript.ActionLine { ActionName = "Gluttonous Charge", Dialogue = "I pissed four pintels / and still had fight in me!" }
            };
            Save(script, "GluttonyDialogue");
        }

        // ─────────────────────────────────────────────────────────────

        static BossDialogueScript CreateOrLoad(string name)
        {
            string path = $"{OutputPath}/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<BossDialogueScript>(path);
            if (existing != null) return existing;
            return ScriptableObject.CreateInstance<BossDialogueScript>();
        }

        static void Save(BossDialogueScript script, string name)
        {
            string path = $"{OutputPath}/{name}.asset";
            if (!AssetDatabase.Contains(script))
                AssetDatabase.CreateAsset(script, path);
            else
                EditorUtility.SetDirty(script);
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
