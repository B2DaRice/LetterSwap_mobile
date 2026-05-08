# LetterSwap

LetterSwap is a mobile-first Unity proof of concept for a word-based tile swapping puzzle game.

The goal of this project is not just to build a game, but to make the game development process understandable from a software engineering background. The code should stay simple, readable, and easy to reason about while still following useful Unity patterns.

## Current Target

- Android first
- Portrait orientation
- Playable in the Unity Editor first
- Fixed `8x8` board for the first proof of concept
- 2D board with placeholder art
- Tap-based controls
- No multiplayer
- No backend
- No in-app purchases
- Ads stubbed behind clean interfaces later, not part of the first slice

## Core Gameplay

The player sees a board of square letter tiles. They tap one tile, then tap an adjacent tile to attempt a swap.

If the swap creates at least one valid word horizontally or vertically:

- The swap is accepted.
- The created word is scored.
- The word tiles are removed.
- Tiles above empty spaces fall down.
- New random letters fill the board from the top.

If the swap does not create a valid word:

- The tiles briefly swap.
- The invalid pair is shown with feedback, such as a red outline or flash.
- The tiles bounce back to their original positions.

The level is complete when the player reaches a target score.

## MVP Feature List

The first playable version should include:

- A portrait game scene.
- A fixed `8x8` board.
- Randomly generated letter tiles.
- Tap one tile to select it.
- Tap an adjacent tile to attempt a swap.
- Valid words are at least 3 letters long.
- Word validation uses a simple local word list.
- Accepted swaps remove valid words.
- Removed tiles award score.
- Columns collapse after removal.
- New letters spawn into empty spaces.
- A target score triggers a simple level complete screen.
- A restart button.

Deferred until after the first vertical slice:

- Different board sizes and board shapes.
- Level progression.
- Better letter distribution tuning.
- Bonus tiles.
- Sounds and animations polish.
- Save data.
- Ads.
- Real Android build pipeline.

## Scoring Direction

The first version can use Scrabble-like letter values:

| Letter | Value |
| --- | ---: |
| A, E, I, O, U, L, N, S, T, R | 1 |
| D, G | 2 |
| B, C, M, P | 3 |
| F, H, V, W, Y | 4 |
| K | 5 |
| J, X | 8 |
| Q, Z | 10 |

Longer words should score more than shorter words. A simple starting formula could be:

```text
word score = sum(letter values) * length multiplier
```

Example length multipliers:

| Word Length | Multiplier |
| ---: | ---: |
| 3 | 1.0 |
| 4 | 1.25 |
| 5 | 1.5 |
| 6 | 2.0 |
| 7+ | 2.5 |

This can be tuned after the first vertical slice is playable.

## Key Unity Terms

### Scene

A Unity scene is like an application route, page, or screen plus the runtime objects that exist on it.

For this MVP, the main scene will be:

- `GameScene`

It will contain the board, UI, game controller objects, and level complete panel.

### GameObject

A GameObject is a thing that exists in a scene. It is mostly a container for components.

Examples:

- Board root object
- Tile object
- Score UI object
- Game controller object

### Component

A component adds behavior or data to a GameObject.

In Unity C#, most behavior scripts inherit from `MonoBehaviour` and are attached to GameObjects as components.

### MonoBehaviour

A `MonoBehaviour` is a Unity script component. It is similar to a class that participates in Unity's runtime lifecycle.

Common lifecycle methods:

- `Awake`: initialization before the object is used.
- `Start`: initialization after objects are active.
- `Update`: called every frame.

For this project, we should avoid putting all gameplay logic directly in `Update`. Most game behavior should happen in response to taps, swaps, and board resolution steps.

### Prefab

A prefab is a reusable GameObject template.

It is similar to a reusable UI component with serialized default settings.

For this MVP, the main prefab will be:

- `TilePrefab`

### ScriptableObject

A `ScriptableObject` is a Unity asset used to store shared data or configuration.

It is similar to a typed config file or data module.

Possible future config assets:

- `LevelConfig`
- `LetterDistributionConfig`
- `ScoringConfig`

For the first slice, plain serialized fields on scene objects may be enough. We can introduce ScriptableObjects when configuration starts growing.

### Canvas

A Canvas is Unity's UI root. Text, buttons, panels, and other UI elements live inside a Canvas.

For this game, the score display and level complete screen will be Canvas UI.

### RectTransform

UI elements use `RectTransform` instead of normal `Transform`. It controls anchoring, size, and position inside the Canvas.

This matters for mobile portrait layouts because the UI needs to adapt to different screen sizes.

### Coroutine

A coroutine is a Unity-friendly way to run logic over time.

Examples:

- Animate a swap.
- Flash invalid tiles.
- Wait until tiles finish falling before checking the next board state.

For a web analogy, it is a little like an async sequence that yields control back to the runtime between steps.

### Board Model vs Board View

The board model is the actual game state.

The board view is the visual representation of that state.

Keeping them separate makes the project easier to understand:

- The model knows what letters are in which cells.
- The view knows where tile objects should appear on screen.

This is similar to separating business state from UI rendering in a web app.

## Proposed Architecture

### Scene

#### `GameScene`

Responsibilities:

- Hosts the playable board.
- Hosts the score UI.
- Hosts the level complete UI.
- Contains controller objects that coordinate the game.

### Scripts

#### `GameController`

Owns the overall game flow.

Responsibilities:

- Start a new game.
- Track whether the game is accepting input.
- Coordinate swap attempts.
- Ask the validator whether a move is valid.
- Trigger board resolution.
- Update score.
- Detect the win condition.
- Show level complete UI.

This is similar to an application-level orchestrator or page controller.

#### `BoardModel`

Pure C# representation of the board.

Responsibilities:

- Store board width and height.
- Store letters by grid coordinate.
- Read and write individual cells.
- Swap letters in two cells.
- Clear cells.
- Apply gravity/fill operations, or expose enough state for another service to do it.

This should not depend on Unity visuals.

#### `BoardView`

Unity visual layer for the board.

Responsibilities:

- Create tile views from the board model.
- Convert grid positions to screen/local positions.
- Move tile visuals during swaps and falls.
- Highlight selected tiles.
- Show invalid move feedback.
- Keep visuals synced with the model.

This is similar to a view component that renders state.

#### `TileView`

Visual behavior for a single tile.

Responsibilities:

- Display the tile letter.
- Store its current grid coordinate.
- Show selected state.
- Show invalid state.
- Notify input code when tapped.

#### `InputController`

Handles tap selection logic.

Responsibilities:

- Track the first selected tile.
- Track the second selected tile.
- Check whether two selected tiles are adjacent.
- Send valid swap attempts to `GameController`.
- Clear selection when needed.

For the MVP, tap input is preferred over drag input because it is easier to implement and debug.

#### `WordDictionary`

Loads and stores valid words.

Responsibilities:

- Load a simple local text word list.
- Normalize words to a consistent case.
- Provide `IsValidWord(string word)`.

#### `WordValidator`

Checks whether a swap creates valid words.

Responsibilities:

- Inspect horizontal and vertical lines through swapped cells.
- Find contiguous letter sequences.
- Accept words of length 3 or greater.
- Check candidates against `WordDictionary`.
- Return the valid words and their board positions.

#### `BoardResolver`

Handles what happens after a valid word is made.

Responsibilities:

- Remove matched word tiles.
- Apply gravity to each affected column.
- Spawn new letters to refill the board.
- Return enough information for `BoardView` to animate the changes.

#### `ScoreController`

Tracks and calculates score.

Responsibilities:

- Store current score.
- Calculate word score using letter values.
- Apply word length multipliers.
- Notify UI when score changes.

#### `GameUI`

Connects gameplay state to Unity UI.

Responsibilities:

- Display current score.
- Display target score.
- Show/hide level complete panel.
- Handle restart button clicks.

### Prefabs

#### `TilePrefab`

Visual template for one board tile.

Expected contents:

- Square background.
- Letter text.
- Selection highlight.
- Invalid move highlight.
- `TileView` script.
- Tap input component, such as a `Button` or pointer click handler.

### Data Files

#### Local Word List

A simple text file with one word per line.

Example:

```text
CAT
DOG
TREE
STONE
```

For the first version, the dictionary does not need to be exhaustive. It only needs enough words to prove the game loop.

## First Vertical Slice

The first vertical slice is one thin but complete playable path through the actual game.

For LetterSwap, that means:

1. Open `GameScene`.
2. Generate an `8x8` board of letter tiles.
3. Tap one tile to select it.
4. Tap an adjacent tile to attempt a swap.
5. Briefly animate the attempted swap.
6. If the swap creates a valid word:
   - accept the swap,
   - remove the word,
   - add score,
   - collapse affected columns,
   - spawn replacement letters.
7. If the swap does not create a valid word:
   - flash invalid feedback,
   - bounce the tiles back.
8. If the target score is reached:
   - show level complete UI.

## Suggested First Implementation Order

1. Create the Unity project structure and `GameScene`.
2. Add a simple portrait Canvas and board area.
3. Create `TilePrefab`.
4. Implement `BoardModel`.
5. Implement board generation and `BoardView`.
6. Implement tap selection.
7. Implement adjacent tile swap animation.
8. Implement local dictionary loading.
9. Implement word validation.
10. Implement invalid swap bounce-back.
11. Implement word removal, gravity, and refill.
12. Implement scoring.
13. Implement level complete UI.

## Later Level Design Direction

The first version uses a fixed `8x8` board, but the architecture should allow future levels to vary challenge with:

- Different board sizes.
- Blocked cells.
- Irregular board shapes.
- Different target scores.
- Different letter distributions.
- Limited moves.
- Special tiles.

These should be introduced only after the basic loop feels understandable and fun.
