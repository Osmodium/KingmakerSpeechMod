﻿//using HarmonyLib;
//using Kingmaker.Code.UI.MVVM.View.Dialog.BookEvent;
//using SpeechMod.Unity.Extensions;
//#if DEBUG
//using Kingmaker;
//using UnityEngine;
//#endif

//namespace SpeechMod.Patches;

//[HarmonyPatch(typeof(BookEventCueView), nameof(BookEventCueView.SetText))]
//public static class BookEventView_Patch
//{
//    public static void Postfix(BookEventCueView __instance)
//    {
//        if (!Main.Enabled)
//            return;

//#if DEBUG
//        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;
//        Debug.Log($"{nameof(BookEventCueView)}_SetText_Postfix @ {sceneName}");
//#endif
//        __instance.Text.HookupTextToSpeech();
//    }
//}