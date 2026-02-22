# CombatAction ScriptableObject — How to Set Up Attacks

Kynde Blade uses **CombatAction** as a ScriptableObject so you can create new attacks in the Inspector without writing code.

## Quick setup

1. In the Project window: **Right‑click → Create → KyndeBlade → Combat Action**.
2. Name it (e.g. `MeleeStrike`, `FrostBite`).
3. In the Inspector, set:
   - **Action Type** — Strike, RangedStrike, Heal, Rest, Special, etc.
   - **Action Name** — Shown in the combat UI.
   - **Damage** — Base damage (or heal amount for Heal).
   - **Break Damage** — Amount applied to the break gauge.
   - **Stamina Cost** — Cost to use the action.
   - **Kynde Cost** — Optional Kynde cost for skills.
   - **Kynde Generated** — Kynde gained when using (e.g. from melee).
   - **Cast Time / Execution Time** — For turn timing.

4. Assign the asset to a character’s **Available Actions** list (or via a moveset like Expedition33Moveset).

## Data card idea

Think of each CombatAction asset as a **data card**: one card per attack. You can make a “Slime” set and a “Boss” set by creating different assets and assigning them to different prefabs or encounter configs—no new C# script per enemy.

## Custom behaviour

For actions that need custom logic (e.g. “hit all enemies”), create a new script that **inherits from CombatAction** and overrides `ExecuteAction`. Then create a ScriptableObject from that class (e.g. **Create → KyndeBlade → My Custom Action**). The base **CombatAction** class is in the Combat assembly (`KyndeBlade.Combat`).

## IKyndeUser

Any character that uses Kynde (gain/spend) implements **IKyndeUser** (e.g. `MedievalCharacter`). This keeps the Kynde system decoupled so bosses or other entities can use it without duplicating logic.
