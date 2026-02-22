# UX and Game Design Basics
## Kynde Blade Design Framework

This document implements foundational principles from **The Gamer's Brain** (Celia Hodent) and **Beginner's Guide to Game Development** (Punky Publishing), adapted for Kynde Blade's medieval RPG context.

---

## From The Gamer's Brain (Celia Hodent)

### Core Pillars: Understanding the Player's Brain

**Perception**
- How players perceive game information (UI, feedback, cues)
- **Apply**: Clear visual hierarchy in combat UI; readable sprite silhouettes; distinct feedback for dodge/parry success vs failure
- Avoid: Information overload; ambiguous indicators

**Memory**
- How games leverage and work with player memory
- **Apply**: Consistent control schemes; recurring visual/audio cues for similar mechanics; tutorial that builds on prior knowledge
- Short-term: Turn order, current action; Long-term: Character abilities, enemy patterns

**Attention**
- Managing cognitive load and player focus
- **Apply**: Focus attention on active turn; minimize clutter during real-time windows; highlight actionable elements
- One primary task at a time during critical moments (dodge/parry)

**Motivation**
- What drives player engagement
- **Apply**: Clear goals (defeat enemy, survive); meaningful rewards (XP, progression); mastery through timing (dodge/parry skill)
- Intrinsic: Mastery, challenge; Extrinsic: Loot, levels

**Emotion**
- How games elicit emotional responses
- **Apply**: Melancholic tone; tension during real-time windows; relief on successful parry; weight of consequences
- Align with Piers Plowman themes: work, poverty, spiritual seeking

**Learning Principles**
- How players learn game mechanics
- **Apply**: Introduce one mechanic at a time; scaffold difficulty; immediate feedback; allow failure as learning (trial-and-error)
- Tutorial: Basic attack → Dodge → Parry → Counter

### UX Framework: Usability + Engageability

**Usability**
- Practical ease of use and navigation
- **Apply**: Intuitive menus; clear button prompts; accessible timing adjustments; readable fonts and icons
- Players should never be confused about *what* they can do

**Engageability**
- What makes games compelling and engaging
- **Apply**: Satisfying combat feel; meaningful choices; sense of progression; atmospheric world
- Balance challenge and skill; avoid frustration without challenge

---

## From Beginner's Guide to Game Development

### Core Design Fundamentals

**Mechanics**
- The actions players can perform
- **Apply**: Attack, Dodge, Parry, Counter, Wait, Guard; class-specific skills
- Each action should have clear cause and effect

**Core Gameplay Loop**
- What players repeatedly do to stay engaged
- **Apply**: Select action → Execute → React (real-time window) → Resolve → Next turn
- Loop should feel rewarding and varied

**Systems Design**
- How mechanics interact to create cause-and-effect
- **Apply**: Stamina limits actions; Speed determines turn order; Parry enables Counter; Break system rewards coordination
- Systems should create emergent strategy

**Goals and Rewards**
- Giving players purpose and motivation
- **Apply**: Win combat → XP, loot; Master timing → Survival, counter damage; Complete quest → Story progression
- Goals should be clear; rewards should feel earned

**Testing and Feedback**
- Essential for iterating on design
- **Apply**: Playtest timing windows; balance stamina costs; tune difficulty curves
- Iterate based on player behavior and feedback

**Difficulty Balancing**
- Maintaining appropriate challenge levels
- **Apply**: Easy/Normal/Hard/Expert modes; timing window sliders; optional auto-actions
- Challenge should match player skill; avoid unfair spikes

---

## Implementation Checklist

### Perception
- [x] Combat UI has clear visual hierarchy (`CombatUI.cs` — turn order, goals, state)
- [x] Dodge/parry success/failure has distinct feedback (`CombatFeedback.cs` — flash, audio hooks)
- [x] Turn order is always visible (turn slots with HP)
- [x] Action buttons are readable and distinct (Strike, Rest, Escapade, Ward)

### Memory
- [x] Controls are consistent (action buttons, same layout)
- [x] Tutorial introduces mechanics in logical order (`TutorialManager.cs` — Basic → Dodge → Parry → Counter)
- [x] Similar mechanics use similar cues (shared feedback system)

### Attention
- [x] Real-time window focuses attention (StateText shows "Dodge/Parry! X.Xs")
- [x] Active character/target is highlighted (yellow in turn order)
- [x] One primary task during critical moments (minimal UI during real-time window)

### Motivation
- [x] Goals are clear (`GameStateManager` — "Defeat all enemies", Victory/Defeat)
- [x] Rewards feel meaningful (XP on victory, `CalculateVictoryXP`)
- [x] Mastery is achievable (timing windows, Kynde from perfect dodge/parry)

### Emotion
- [x] Tone supports melancholic themes (design docs)
- [x] Tension in real-time windows (countdown, feedback)
- [x] Consequences feel weighty (damage, defeat)

### Learning
- [x] Tutorial: Basic → Dodge → Parry → Counter (`TutorialManager`)
- [x] Failure teaches (clear defeat, feedback on failed dodge/parry)
- [x] Difficulty scales gradually (`GameSettings` — Easy/Normal/Hard/Expert, timing multiplier)

### Usability
- [x] Menus are navigable (auto-created CombatUI)
- [x] Timing adjustments available (`GameSettings.TimingWindowMultiplier`, `GetAdjustedWindow`)
- [x] No ambiguous prompts (GoalText, StateText)

### Engageability
- [x] Combat feels satisfying (feedback, rewards)
- [x] Choices matter (action selection, timing)
- [x] Progression is visible (XP, victory panel)

---

## Core Loop (Beginner's Guide)

**Actions → Feedback → Rewards → Progression**

| Layer | Kynde Blade Implementation |
|-------|----------------------------|
| **Minute-to-minute** | Turn-based action selection, real-time dodge/parry |
| **Most repeated** | Strike, Rest, Escapade, Ward — clear cause and effect |
| **Progression engine** | XP on victory, TutorialManager phases, GameStateManager |

**Feedback**: CombatFeedback (visual flash, audio), CombatUI (state text, turn order).

---

## References

- **The Gamer's Brain: How Neuroscience and UX Can Impact Video Game Design** (Celia Hodent) — Perception, Memory, Attention, Motivation, Emotion, Learning; Usability and Engageability
- **Beginner's Guide to Game Development** (Punky Publishing) — Mechanics, core loop, systems, goals/rewards, testing, difficulty balancing
