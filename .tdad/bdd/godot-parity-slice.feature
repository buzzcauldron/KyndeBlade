# Godot parity slice — TDAD workflow: godot-parity-slice
# Steam / retail ship bar (paths, autosave, migration): TDAD workflow godot-steam-build + KyndeBlade_Godot/STEAM_BUILD.md
# Trace titles to demo-vertical-slice.feature (Unity oracle until M6 archive).

Feature: Godot Steam build mirrors Unity vertical slice behaviors

  @gdc-tower-intro @gdc-hub-slice-locations
  Scenario: Hub shows tour after New Game
    Given a new game was started in the Godot Steam build
    When the player confirms New Game from the main menu
    And the hub map scene loads
    Then current location id should be "tour"

  @gdc-hub-slice-locations
  Scenario: Hub offers travel to Fair Field
    Given the hub map
    When the player inspects destinations
    Then Fair Field (fayre_felde) is available

  @gdc-hub-counsel-gate
  Scenario: Fair Field travel requires counsel before combat
    Given the hub map at tour
    When the player opens travel to Fair Field
    Then a counsel choice is shown (Trewthe Mede or Name hunger)
    And proceeding to combat is disabled until a counsel is chosen

  @gdc-combat-misstep-feint
  Scenario: Ethical misstep inverts first defensive read in combat
    Given GameState ethical_misstep_count is odd before combat loads
    When the player opens the first dodge or parry window
    Then the first window is a feint not a real swing
    And headless suite scenario misstep_inverts_first_defensive_window passes

  @gdc-combat-encounter-load
  Scenario: Fair Field combat uses False encounter data
    Given EncounterDef for Fair Field
    When combat starts from that encounter
    Then the enemy identity matches the slice (False)

  @gdc-combat-scenarios
  Scenario: Headless combat scenario suite passes
    Given tests/combat_scenarios.gd with CombatManager.use_instant_resolution_for_tests
    When run_headless_tests.gd executes all scenarios
    Then victory by strikes dodge parry feint stamina and defeat cases assert expected hp and state

  Scenario: Save updates location and flags
    Given SaveService wrote a save with location "fayre_felde"
    When the save file is loaded
    Then location_id is "fayre_felde"

  Scenario: Save file roundtrips core fields
    Given a new save on disk
    When fields are written then read back
    Then version location_id and fair_field_cleared match

  @gdc-combat-pause
  Scenario: Pause exists in combat
    Given the combat scene is running
    When the player opens pause
    Then the pause overlay is visible and the window timer stops

  @manual @wireframe @gdc-combat-scenarios
  Scenario: Combat shows dynamic stage and parry dodge eye during defensive window
    Given the combat scene is running
    When the player triggers dodge or parry
    Then the side-view combat stage shows placeholder actors
    And the parry dodge eye and React countdown are visible until the window resolves

  Scenario: Master volume persists in settings file
    Given SaveService settings storage
    When master volume is saved
    Then loading settings returns the same linear value

  @gdc-main-menu
  Scenario: Continue is available when save exists
    Given a save file exists under user://
    When the main menu loads
    Then Continue is enabled

  @gdc-combat-victory-save
  Scenario: Victory returns to hub with updated progress
    Given the player won the Fair Field fight
    When they continue to hub
    Then save reflects cleared Fair Field and hub shows replay state

  @gdc-tower-intro @gdc-hub-slice-locations @gdc-combat-encounter-load
  Scenario: Unity export JSON is present for pipeline parity
    Given exported_from_unity.json in the Godot data folder
    When headless tests parse the file
    Then schema_version is at least 2 and tour lists fayre_felde and tower_vista beat text and FayreFeldeEncounter lists False

  Scenario: Meta flags roundtrip in save
    Given SaveService wrote ethical and hunger flags
    When the save file is loaded into GameState
    Then ethical_misstep_count and has_ever_had_hunger match

  Scenario: Audio buses exist after bootstrap
    Given the Godot project autoloads ran
    When the audio server lists buses
    Then Music and SFX buses exist

  Scenario: Autosave mirror file exists after save
    Given SaveService wrote a new game save
    When both primary and mirror cfg files are checked under user://
    Then kyndeblade_save.cfg and kyndeblade_autosave.cfg exist

  Scenario: Load falls back to autosave when primary save is missing
    Given a valid save was written to primary and autosave
    When the primary save file is removed from user://
    And SaveService loads save data
    Then location_id and fair_field_cleared still match the last save

  Scenario: Legacy demo save migrates to retail paths
    Given only kyndeblade_demo_save.cfg exists with known progress fields
    When SaveService load_save runs
    Then kyndeblade_save.cfg exists and the legacy demo save file is removed
    And loaded fields match the legacy content

  @manual @wireframe @gdc-combat-scenarios
  Scenario: Wireframe combat checklist audit in editor
    Given KyndeBlade_Godot/docs/WIREFRAME_COMBAT_CHECKLIST.md
    When the tester follows STEAM_BUILD.md wireframe combat pass after a combat or presentation change
    Then partial dodge parry riposte feint chip presentation and audio rows in the checklist are satisfied
