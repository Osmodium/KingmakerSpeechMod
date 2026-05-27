# SpeechMod – Todo

Items are grouped by UI area. Each item includes the relevant Unity path and/or class for quick reference.

---

## License / Terms of Use

- [x] **Look into license agreement**
  - Path: `/Canvas/TermOfUse/Body/MainContent/Scroll View/Viewport/Content/Licence`
  - Class: `Kingmaker.UI.TermsOfUseController`

---

## Dialog / Conversation

- [x] **Automatic playback with narrator, female and male voices**
  - Class: `Kingmaker.UI.Dialog.DialogCurrentPart`, `Kingmaker.UI.Dialog.DialogController`

- [x] **Add playback button in the dialog view**
  - Path: `/StaticCanvas/Dialogue/Body/View/Scroll View/Viewport/Content/CurrentPart/DialoguePhrase`
  - Path: `/StaticCanvas/ServiceWindow/Encyclopedia/PathView/Next`
  - Path: `/StaticCanvas/Dialogue/Body/View/Scroll View/ButtonEdge`
  - Note: Look at `globalmap_arrow` for a possible better arrow

- [x] **Load ButtonEdge prefab from scene (scene-independent)**
  - Path: `/StaticCanvas/Dialogue/Body/View/Scroll View/ButtonEdge`
  - Scene: `UI_Ingame_Scene` (build index 11)
  - Class: `ButtonFactory.cs`
  - Note: ButtonEdge is NOT a standalone asset; it's baked into the UI scene. Async-load scene by build index, extract the button, store as `DontDestroyOnLoad` prefab, then unload the scene.

- [x] **Refactor ButtonFactory (DRY/SOLID)**
  - Extracted methods: `Initialize`, `ExtractAndStorePrefab`, `FindButtonEdgeInRoots`, `InstantiateButton`, `CreatePlayButton`, `SetupButton`
  - `Initialize()` called from `Main.Load()`

- [ ] **Name of left dialog character**
  - Path: `StaticCanvas/Dialogue/AnswerBlock/AnswerName`
  - Class: `Kingmaker.UI.Dialog.DialogController`

- [ ] **Name of right dialog character (Speaker)**
  - Path: `StaticCanvas/Dialogue/SpeakerBlock/SpeakerName`
  - Class: `Kingmaker.UI.Dialog.DialogController`

- [x] **Stop playback keybinding**
  - Class: `PlaybackStop.cs`
  - Note: Look how to add a keybinding to the mod, and which key to use for stopping playback.

- [ ] **Toggle barks keybinding**
  - Class: `ToggleBarks.cs`
  - Note: Look how to add a keybinding to the mod, and which key to use for toggling barks.

---

## New Game / Campaign

- [x] **Campaign description**
  - Path: `/Canvas/NewGameWindow/Body/Content/Story/DescriptionBlock/Description/`

- [ ] **Character description block**
  - Path: `Canvas/NewGameWindow/Body/Content/Pregen/PregenSelector/Description/DescriptionView/Viewport/Content/Content/Description (1)`
  - Class: `Kingmaker.UI.Tooltip.DescriptionBricksBox`

- [ ] **Race description**
  - Path: `Canvas/CharacterBuild/Body/Content/Progression/CharacterClassDescription/0_Layer/DescriptionBody/Content (2)/Viewport/ViewPortContent/Content (1)/ParagraphText(Clone)/Text`
  - Class: `Kingmaker.UI.Tooltip.DescriptionBrick`

- [ ] **Start screen motivation text**
  - Path: `/!LIGHT_SETUP/SceneUICanvas/Motivation/Label/`
  - Class: `Kingmaker.UI.MainMenuUI.MotivationTextView`

---

## Tooltips / Descriptions

- [x] **Tooltip text**
  - Path: `Canvas/Tooltips/DescWinConstructor/Window/Content/Scroll View/Viewport/Content/ParagraphText(Clone)/Text`
  - Class: `Kingmaker.UI.Tooltip.DescriptionBrick`

- [ ] **Hint (small) description**
  - Path: `/StaticCanvas/Tooltips/TutorialHint/Content/Content/ParagraphText(Clone)/Text`

- [ ] **Hint H1 header**
  - Path: `/StaticCanvas/Tooltips/TutorialHint/Content/Header/Title-H1(Clone)/Title-H1`

- [ ] **Hint (big) text**
  - Path: `/StaticCanvas/Tooltips/TutorialConstructorBig/Window/Content/Content/ParagraphText(Clone)/Text`
  
- [ ] **Global Map Messagebox**
  - Class: `Kingmaker.UI.GlobalMap.GlobalMapMessageBox`

---

## Encyclopedia

- [x] **Add Encyclopedia playback buttons**
  - Class: `Kingmaker.UI.ServiceWindow.Encyclopedia.Blocks.EncyclopediaBlockRootPage`
  - Class: `Kingmaker.UI.ServiceWindow.Encyclopedia.Blocks.EncyclopediaBlockText`

---

## Journal

- [x] **Journal Quest Title**
  - Path: `/StaticCanvas/ServiceWindow/Journal/Content/QuestContentElement/Header`
  - Class: `Kingmaker.UI.Journal.JournalQuestElement`, `Kingmaker.UI.Journal.JournalQuestLog`

- [x] **Journal Quest Description**
  - Path: `/StaticCanvas/ServiceWindow/Journal/Content/QuestContentElement/Description`
  - Class: `Kingmaker.UI.Journal.JournalQuestElement`, `Kingmaker.UI.Journal.JournalQuestLog`

- [x] **Journal Quest part header**
  - Path: `/StaticCanvas/ServiceWindow/Journal/Content/QuestContentElement/Body/Scroll View/Viewport/Content/QuestObjective(Clone)/Header/Header`
  - Class: `Kingmaker.UI.Journal.JournalQuestObjective`

- [x] **Journal Quest part description**
  - Path: `/StaticCanvas/ServiceWindow/Journal/Content/QuestContentElement/Body/Scroll View/Viewport/Content/QuestObjective(Clone)/Body/Description`
  - Class: `Kingmaker.UI.Journal.JournalQuestObjective`

---

## Character Screen / Story

- [x] **Story text**
  - Path: `/StaticCanvas/ServiceWindow/CharacterScreen/Story/Stories/StoryEntry/StoryContent/TextBox/Text`
  - Path (header): `/StaticCanvas/ServiceWindow/CharacterScreen/Story/Stories/StoryEntry/StoryContent/TextBox/Label (1)/Label`
  - Class: `Kingmaker.UI.ServiceWindow.CharacterScreen.CharSComponentStory`

---

## Book Events

- [ ] **Book event interchapter**
  - Path: `/StaticCanvas/BookEventInterchapter/Window/Content/TitleTape`
  - Path: `/StaticCanvas/BookEventInterchapter/Window/Content/Cue`
  - Path: `/StaticCanvas/BookEventInterchapter/Window/Content/Answer`
  - Class: `Kingmaker.UI.BookEvent.BookEventInterchapterController`

- [ ] **Fix duplicate button in BookEventInterchapterController**
  - Both `UiDialogController_Patch` and `BookEventInterchapterController_Patch` create buttons when a book event interchapter opens, resulting in two play buttons visible.

- [ ] **Book event**
  - Path: `/StaticCanvas/BookEvent/Window/Content/Cues/Viewport/Content/`
  - Class: `Kingmaker.UI.BookEvent.BookEventController`

---

## Loading Screen

- [ ] **Look into loading screen**
  - [x] Path: `/Canvas/LoadingScreen/Window/HintBlock/Background/Paper/Background/HintLabel`
  - [x] Path: `/Canvas/LoadingScreen/Window/MapContainer/Papers/Texts/Description`
  - Class: `Kingmaker.UI.LoadingScreen.LoadingScreen`

---

## Barks / Overtip

- [ ] **Look into Barks**
  - Class: `Kingmaker.UI.Overtip.OvertipController`

##  Kingdom

- [ ] **The opportunity Window and possibly others**
  - [x] Class: `Kingmaker.UI.Kingdom.KingdomUIEventWindow`
  - [x] Class: `Kingmaker.UI.Kingdom.KingdomUILeaderController`
  - [ ] Class: `Kingmaker.UI.Kingdom.KingdomUILeaderCharacterController`
  - [x] Class: `Kingmaker.UI.Kingdom.KingdomUIEventWindowFooter`
  - [x] Class: `Kingmaker.UI.Kingdom.KingdomNameDialogue`
  - [ ] Class: `Kingmaker.UI.Kingdom.KingdomToolbar`
  - [ ] Class: `Kingmaker.UI.Kingdom.KingdomStatController`
  - [ ] Class: `Kingmaker.UI.Kingdom.KingdomPurchaseBuildPoint`
  