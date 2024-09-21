# Pathfinder Kingmaker - SpeechMod
By [Osmodium](https://github.com/Osmodium)

## This mod is made for Pathfinder Kingmaker and introduces TTS (TextToSpeech) in most places.
Version: 0.1.0

**Disclaimer: UNDER DEVELOPMENT**

- Windows: Should work with the features implemented.

- OSX: Not tested, let me know if it works.

**Works with all languages as long as you have a voice in that language installed.**

[How to unlock more voices in Windows 10/11](https://www.ghacks.net/2018/08/11/unlock-all-windows-10-tts-voices-system-wide-to-get-more-of-them/)

**How to use natural voices.**
Install this application: [NaturalVoiceSAPIAdapter](https://github.com/gexgd0419/NaturalVoiceSAPIAdapter)
Note: It seems to only support offline voices on Windows 11 and online voices seem to crash when they are being stopped during speech.
Disclaimer: I do **NOT** intend to support issues related to the NaturalVoicesSAPIAdapter application.

### How to install

 1. Download the PFKingmakerSpeechMod mod file and unzip.
 
 ...TODO
 
 x. Launch Pathfinder Kingmaker, you may need to hit **ctrl+F10** to see the mod manager window.

### Known issues / limitations

*If you find issues or would like to request features, please use the issues tracker in GitHub [here](https://github.com/Osmodium/KingmakerSpeechMod/issues)*

#### Limitations:
None, though a bunch of stuff still to do :)

#### Todo:
- Still some work to do, see https://github.com/Osmodium/KingmakerSpeechMod/blob/main/Todo.txt for more info.

### How to use

#### 1) Dialog
When in dialog you can press the play button next to the left image to listen to the current block of dialog. If autoplay is enabled, you don't have to push the playbutton.



#### 2) Dialog Answers
Also when in dialog, you can choose to have playbuttons for each of the dialog answers, and which color the hover effect should make the answer, so you know when you are selecting it or having it playback.



#### 3) Inspection Information
When inspecting items and links (through *right-click->Info* or just *right-click* for expanded tooltip) *hover* over the text and *left click*.

<image>

#### 4) Journal Quest and Rumors text
In the journal, each of the bigger text blocks and important stuff can be played through the play button adjacent to the text. The text blocks without a playbutton can be played by hovering and left-clicking.

<image>

#### 5) Book Event text
When encountering a book event, the text can be played by hovering the text part (it will apply the chosen hover effect) and left-clicking.
Historical text also supported.

<image>

#### 7) Messagebox text
The various pop-up boxes that eventually shows up throughout the game, can be played when hovered and left-clicked.

<image>

#### 8) Tutorial Windows text
Both big and small tutorial windows text is supported and can be played by hovering and left-clicking.

<image>

#### 9) Character biography
When inspecting a character, the story of that character is displayed both under *Summary* and under *Biography*, and are both supported by hovering and left-clicking.

<image>

#### 14) Settings texts
When hovering a setting, the right part shows a description of the setting, this can now be played back by hovering and left-clicking.

<image>

#### 15) Welcome message
In the main menu view, a welcome message is shown, this can be played back by hovering and left-clicking.

<image>

### Settings

The different settings (available through *ctrl+f10* if not overridden in the UMM) for SpeechMod
- **Narrator Voice**: The settings for the voice used for either all or non-gender specific text in dialogs when *Use gender specific voices* is turned on.
	- *Nationality*: Just shows the selected voices nationality.
	- **Speech rate**: The speed of the voice the higher number, the faster the speech.
		- Windows: from -10 to 10 (relative speed from 0).
		- macOS: from 150 to 300 (words per minute).
	- Windows Only:
		- **Speech volume**: The volume of the voice from 0 to 100.
		- **Speech pitch**: The pitch of the voice from -10 to 10.
	-**Preview Voice**: Used to preview the settings of the voice.
- **Use gender specific voices**: Specify voices for female and male dialog parts. Each of the voices can be adjusted with rate, volume and pitch where available.
- Windows Only:
	- **Interrupt speech on play**: 2 settings: *Interrupt and play* or *Add to queue*, hope this speaks for itself.
- **Auto stop playback on loading**: When enabled, currently playing TTS will stop whenever the game loads (through a loading screen).
- **Auto play dialog**: When enabled, dialogs will be played automatically when theres no voice acted dialog.
- **Auto play ignores voiced dialog lines**: Only available when using auto play dialog. This option makes the auto play ignore when there is voiced dialog, remember to turn dialog off in the settings.
- **Show playback button of dialog answers**: When enabled, a play button will be added next to all dialog choices. These buttons playbacks the dialog answer, while not selecting it.
- **Include dialog answer number in playback**: When enabled and pressing a play button next to a dialog answer, the playback will lead with the choice number.
- **Color answer on hover**: This enables highlighting the dialog choice that corresponds to the play button that is hovered.
- **Playback barks**: When clicking on points of interests in the world, the small description is called a "Bark". Enabling this feature reads the bark aloud in the narrator voice. This also applies to character "banter" which is in the same style.
- **Only playback barks if silence**: When enabled, barks are only played if there is no other TTS playback currently playing. Can be useful with the next option.
- **Playback vicinity and cutscene triggered barks**: When enabled, all barks that are automatically played by either triggers or cutscenes, will be played. This can cause a lot of overlapping of playbacks. Hence the prior setting to only play when silence. Might look at some sort of system where automatically played barks, are low priority, compared to manually started playbacks.
- **Show notification on playback stop**: When this is enabled, a notification will show on the screen that the playback was stopped when the keybind for stopping is pressed. This keybinding can be set in the game menu under "Sound".
- **Color on hover**: This is used only for the text boxes when inspecting items, and colors the text the selected color when hovering the text box.
- **Font style on hover**: As above this is only used for text boxes, but lets you set the style of the font.
- **Phonetic Dictionary Reload**: Reloads the PhoneticDictionary.json into the game, to facilitate modificaton while playing.

<image>

### Motivation
*Why did I create the mods?*
After having created the "same" mod for [Pathfinder: Wrath of the Righteous](https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/241) and [Pathfinder Kingmaker](https://www.nexusmods.com/warhammer40kroguetrader/mods/75), I got requests of doing it for this game too, and since I also wanted to play it, I graciousky obliged, even though it's been a while.

I have come to realize that I spend a lot of my energy through the day on various activities, so when I get to play a game I rarely have enough energy left over to focus on reading long passages of text. So I thought it nice if I could get a helping hand so I wouldn't miss out on the excellent stories and writing in text heavy games.

After I started creating the first mod, I have thought to myself that if I struggle with this issue, imagine what people with genuine disabilities must go through and possibly miss out on, which motivated me even more to get this mod working and release it. I really hope that it will help and encourage more people to get as much out of the game as possible.

### Contribute
If you find a name in the game which is pronounced funny by the voice, you can add it to the PhoneticDictionary.json in the mod folder (don't uninstall the mod as this will be deleted). I don't have a great way of submitting changes to this besides through GitHub pull requests, which is not super user friendly. But let's see if we can build a good pronunciation database for the voice together.
Also feel free to hit me up with ideas, issues and PRs on GitHub or NexusMods :)

### Acknowledgments
- [Chad Weisshaar](https://chadweisshaar.com/blog/author/wp_admin/) for his blog about [Windows TTS for Unity](https://chadweisshaar.com/blog/2015/07/02/microsoft-speech-for-unity/)
- [dope0ne](https://forums.nexusmods.com/index.php?/user/895998-dope0ne/) (zer0bits) for providing code to support macOS in the original mod, and various exploration work.
- Enhanced Controls mod for the keybinding and localization code.
- Owlcat Modding Discord channel members
- Join the [Discord](https://discord.gg/EFWq7rJFNN)