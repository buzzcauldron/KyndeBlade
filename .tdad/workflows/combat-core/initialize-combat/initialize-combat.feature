Feature: Initialize Combat

  Given a list of player characters and enemy characters, when InitializeCombat() is called, then PlayerCharacters and EnemyCharacters are set, TurnOrder is computed by descending Speed, and nulls are excluded.

  Scenario: Initialize Combat - Happy Path
    Given the preconditions are met
    When the user performs the action
    Then the expected outcome should occur

  # TODO: Add more scenarios based on requirements
