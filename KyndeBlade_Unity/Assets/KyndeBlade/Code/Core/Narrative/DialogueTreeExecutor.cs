using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Runs a DialogueTreeDefinition at runtime: evaluates conditions on choices,
    /// presents available options via DialogueSystem, and applies consequences.
    /// </summary>
    public class DialogueTreeExecutor : MonoBehaviour
    {
        DialogueSystem _dialogueSystem;
        SaveManager _saveManager;
        UnityEngine.Object _worldMapManager;
        UnityEngine.Object _gameManager;
        DialogueTreeDefinition _currentTree;
        Action _onComplete;

        Dictionary<string, DialogueTreeDefinition.DialogueNode> _nodeIndex;

        public void RunTree(DialogueTreeDefinition tree, Action onComplete)
        {
            if (tree == null || tree.Nodes == null || tree.Nodes.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }

            _currentTree = tree;
            _onComplete = onComplete;
            ResolveRefs();
            BuildNodeIndex();
            ShowNode(tree.RootNodeId);
        }

        void ResolveRefs()
        {
            if (_dialogueSystem == null)
                _dialogueSystem = GameRuntime.DialogueSystem ?? UnityEngine.Object.FindFirstObjectByType<DialogueSystem>();
            if (_saveManager == null)
                _saveManager = GameRuntime.SaveManager ?? UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (_worldMapManager == null)
                _worldMapManager = GameRuntime.WorldMapManager != null
                    ? GameRuntime.WorldMapManager
                    : FindRuntimeObject("KyndeBlade.WorldMapManager");
            if (_gameManager == null)
                _gameManager = GameRuntime.GameManager != null
                    ? GameRuntime.GameManager
                    : FindRuntimeObject("KyndeBlade.KyndeBladeGameManager");
        }

        void BuildNodeIndex()
        {
            _nodeIndex = new Dictionary<string, DialogueTreeDefinition.DialogueNode>();
            foreach (var node in _currentTree.Nodes)
                if (node != null && !string.IsNullOrEmpty(node.NodeId))
                    _nodeIndex[node.NodeId] = node;
        }

        void ShowNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId) || !_nodeIndex.TryGetValue(nodeId, out var node))
            {
                Complete();
                return;
            }

            var availableChoices = FilterChoices(node.Choices);

            if (availableChoices.Count == 0)
            {
                if (_dialogueSystem != null)
                    _dialogueSystem.ShowLine(node.Text, node.SpeakerName, 3f);
                Complete();
                return;
            }

            var beat = ScriptableObject.CreateInstance<DialogueChoiceBeat>();
            beat.BeatId = node.NodeId;
            beat.Text = node.Text;
            beat.SpeakerName = node.SpeakerName;
            beat.Choices = new DialogueChoiceBeat.Choice[availableChoices.Count];
            for (int i = 0; i < availableChoices.Count; i++)
            {
                var cd = availableChoices[i];
                beat.Choices[i] = new DialogueChoiceBeat.Choice
                {
                    Text = cd.Text,
                    IsCorrectChoice = cd.IsCorrectChoice,
                    TransitionToLocationId = cd.TransitionToLocationId,
                    AssociatedSin = cd.AssociatedSin
                };
            }

            if (_dialogueSystem != null)
            {
                _dialogueSystem.ShowChoiceBeat(beat, (idx, correct, transitionTo, sin) =>
                {
                    if (idx >= 0 && idx < availableChoices.Count)
                    {
                        var chosen = availableChoices[idx];
                        ApplyConsequence(chosen.Consequence);

                        if (!string.IsNullOrEmpty(chosen.NextNodeId))
                        {
                            ShowNode(chosen.NextNodeId);
                            return;
                        }
                    }
                    Destroy(beat);
                    Complete();
                });
            }
            else
            {
                Destroy(beat);
                Complete();
            }
        }

        List<DialogueTreeDefinition.ChoiceDef> FilterChoices(DialogueTreeDefinition.ChoiceDef[] choices)
        {
            var result = new List<DialogueTreeDefinition.ChoiceDef>();
            if (choices == null) return result;
            foreach (var c in choices)
            {
                if (c == null) continue;
                if (c.Condition != null && c.Condition.Type != DialogueTreeDefinition.ConditionType.None)
                {
                    if (!EvaluateCondition(c.Condition)) continue;
                }
                result.Add(c);
            }
            return result;
        }

        bool EvaluateCondition(DialogueTreeDefinition.ConditionDef cond)
        {
            if (cond == null || cond.Type == DialogueTreeDefinition.ConditionType.None) return true;
            var progress = _saveManager?.CurrentProgress;
            if (progress == null) return true;

            switch (cond.Type)
            {
                case DialogueTreeDefinition.ConditionType.HasVisited:
                    return _saveManager.HasVisited(cond.StringParam);
                case DialogueTreeDefinition.ConditionType.HasNotVisited:
                    return !_saveManager.HasVisited(cond.StringParam);
                case DialogueTreeDefinition.ConditionType.PovertyLevelAtLeast:
                    return progress.PovertyLevel >= cond.IntParam;
                case DialogueTreeDefinition.ConditionType.PovertyLevelBelow:
                    return progress.PovertyLevel < cond.IntParam;
                case DialogueTreeDefinition.ConditionType.EldeHitsAtLeast:
                    return progress.EldeHitsAccrued >= cond.IntParam;
                case DialogueTreeDefinition.ConditionType.GreenKnightWillAppear:
                    return progress.GreenKnightWillAppearRandomly == cond.BoolParam;
                case DialogueTreeDefinition.ConditionType.OrfeoTriggered:
                    return progress.OrfeoOtherworldTriggered == cond.BoolParam;
                case DialogueTreeDefinition.ConditionType.HasEverHadHunger:
                    return progress.HasEverHadHunger == cond.BoolParam;
                case DialogueTreeDefinition.ConditionType.GreenChapelBodiesAtLeast:
                    return progress.GreenChapelBodiesAccrued >= cond.IntParam;
                case DialogueTreeDefinition.ConditionType.OtherworldBodiesAtLeast:
                    return progress.OtherworldBodiesFromDeath >= cond.IntParam;
                default:
                    return true;
            }
        }

        void ApplyConsequence(DialogueTreeDefinition.ConsequenceDef cons)
        {
            if (cons == null || cons.Type == DialogueTreeDefinition.ConsequenceType.None) return;

            switch (cons.Type)
            {
                case DialogueTreeDefinition.ConsequenceType.SetGreenKnightRandom:
                    _saveManager?.SetGreenKnightWillAppearRandomly(cons.BoolParam);
                    break;
                case DialogueTreeDefinition.ConsequenceType.SetOrfeoTriggered:
                    _saveManager?.SetOrfeoOtherworldTriggered(cons.BoolParam);
                    break;
                case DialogueTreeDefinition.ConsequenceType.IncrementPoverty:
                    if (_saveManager?.CurrentProgress != null)
                        _saveManager.SetPovertyLevel(_saveManager.CurrentProgress.PovertyLevel + cons.IntParam);
                    break;
                case DialogueTreeDefinition.ConsequenceType.TransitionToLocation:
                    if (_worldMapManager != null && !string.IsNullOrEmpty(cons.StringParam))
                    {
                        var loc = InvokeInstanceMethod(_worldMapManager, "GetLocation", cons.StringParam);
                        if (loc != null) InvokeInstanceMethod(_worldMapManager, "TransitionTo", loc);
                    }
                    break;
                case DialogueTreeDefinition.ConsequenceType.StartSinMiniboss:
                    if (_gameManager != null)
                        InvokeInstanceMethod(_gameManager, "StartSinMinibossEncounter", cons.SinParam);
                    break;
                case DialogueTreeDefinition.ConsequenceType.SaveCheckpoint:
                    if (!string.IsNullOrEmpty(cons.StringParam))
                        _saveManager?.SaveCheckpoint(cons.StringParam);
                    break;
            }
        }

        void Complete()
        {
            _currentTree = null;
            _nodeIndex = null;
            var cb = _onComplete;
            _onComplete = null;
            cb?.Invoke();
        }

        static UnityEngine.Object FindRuntimeObject(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName)) return null;
            Type targetType = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                targetType = assemblies[i].GetType(fullTypeName, false);
                if (targetType != null) break;
            }
            if (targetType == null) return null;
            return UnityEngine.Object.FindFirstObjectByType(targetType);
        }

        static object InvokeInstanceMethod(object instance, string methodName, params object[] args)
        {
            if (instance == null || string.IsNullOrEmpty(methodName)) return null;
            var type = instance.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < methods.Length; i++)
            {
                var m = methods[i];
                if (m.Name != methodName) continue;
                var p = m.GetParameters();
                if (p.Length != (args?.Length ?? 0)) continue;
                return m.Invoke(instance, args);
            }
            return null;
        }
    }
}
