# A-Grade Refactors Applied

Summary of performance and decoupling changes applied to reach the "A" grade.

---

## 1. AgingManager — No FindObjectOfType in Update()

**Problem:** `Update()` called `FindObjectOfType<WorldMapManager>()` and `FindObjectOfType<GameStateManager>()` every frame.

**Fix:**
- **WorldMapManager** is now a cached reference: assign in Inspector or found once in `Start()`.
- **GameStateManager** is not referenced from AgingManager (Core cannot reference Combat). Instead, AgingManager fires **OnDeathOfOldAgeRequested** when at Field of Grace and age tier hits the death threshold. **GameStateManager** (Combat) subscribes in `Start()` and calls `TriggerDeathOfOldAge()` when the event fires.

**Takeaway:** Cache manager references in `Start()`/`Awake()`; use events to cross assembly boundaries instead of FindObjectOfType every frame.

---

## 2. TurnManager — Coroutine Instead of Invoke; Damage Logic Moved Out

**Problem:** `Invoke(nameof(NextTurn), 0.01f)` used a magic-number delay and could cause race conditions.

**Fix:** Replaced with **NextTurnAfterFrame()** coroutine: `yield return null;` then `NextTurn()`. Ensures one frame passes before the next turn so action/anim state can finish.

**Problem:** `GetAttackerDamage(attacker)` lived in TurnManager and searched the character’s actions for Strike/RangedStrike. The manager shouldn’t own damage calculation.

**Fix:** **MedievalCharacter.GetPrimaryStrikeDamage()** now holds that logic. TurnManager calls `attacker.GetPrimaryStrikeDamage()` in `ResolveDefenseWindow()` and no longer has a `GetAttackerDamage` method.

---

## 3. AdaptiveEnemyAI — TurnManager Already Cached

**Status:** TurnManager is only resolved in `Start()` when null (`FindObjectOfType<TurnManager>()` once). No per-frame or per-decision search.

**Change:** Tooltip added: *"Assign in Inspector to avoid FindObjectOfType; otherwise cached once in Start()."* Prefer assigning in Inspector when many enemies exist.

---

## 4. ParryDodgeInputHandler — Events Instead of Direct Character Calls

**Problem:** The handler called `defender.StartDodge(remaining)` / `defender.StartParry(remaining)` directly, tying input to character commands and making it harder to add states like "Stunned" (ignore input).

**Fix:**
- **ParryDodgeInputHandler** now fires **OnDodgePressed** and **OnParryPressed** with the window time remaining. It no longer references the defender.
- **CombatUI** subscribes to these events in `EnsureParryDodgeIndicator()` and, when they fire, gets `TurnManager.DefenderDuringWindow` and calls `StartDodge`/`StartParry`. Unsubscribes in `OnDestroy()`.

**Takeaway:** Input layer fires events; game logic (CombatUI + TurnManager state) decides who applies them. Easier to add "input ignored when stunned" later by not subscribing or by checking state before calling StartDodge/StartParry.

---

## Habits to Keep

- **No FindObjectOfType in Update()** — Cache in Start/Awake.
- **Manager doesn’t own damage math** — Keep it on character or a dedicated CombatCalculator if you add one.
- **State transitions via coroutines** — Prefer `yield return null` or `WaitForSeconds` over `Invoke` with magic numbers.
- **Input as events** — Handler raises events; another system applies them so you can gate or ignore input by state.
