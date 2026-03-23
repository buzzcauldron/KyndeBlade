# Tier A — demo vertical slice (TDAD workflow: demo-vertical-slice)
# Linked tests: DemoVerticalSlicePlayModeTests, PlayableSliceEditModeTests, MapSaveTest, UiShellEditModeTests

Feature: Demo vertical slice bootstrap and data

  Scenario: Main scene loads map at tour when menu is skipped for automation
    Given the Main scene is loaded with SkipMainMenuForAutomatedTests
    When WorldMapManager has finished lazy initialization
    Then CurrentLocation.LocationId should be "tour"

  Scenario: Tour offers Fair Field as next location
    Given the Loc_tour ScriptableObject
    When NextLocationIds is read
    Then it contains "fayre_felde"

  Scenario: Fair Field has encounter wired for slice
    Given the Loc_fayre_felde ScriptableObject
    When the encounter configuration is inspected
    Then combat can start with the False enemy for the minimal slice

  Scenario: Save checkpoint moves current location to Fair Field
    Given SaveManager after NewGame
    When SaveCheckpoint("fayre_felde") is called
    Then CurrentProgress.CurrentLocationId is "fayre_felde"
    And the location is visited and unlocked

  Scenario: Pause overlay exists and starts hidden
    Given the Main scene with GameFlowController UI built
    Then a child named PauseRoot exists under the menu canvas
    And PauseRoot is inactive until the player opens pause

  Scenario: Settings master volume persists
    Given KyndeBladeSettingsStore
    When MasterVolume is set to a test value
    Then PlayerPrefs stores the same value

  Scenario: Continue is available when save JSON exists in PlayerPrefs
    Given valid KyndeBlade save JSON in PlayerPrefs
    When SaveManager is present
    Then HasSavedGame is true
