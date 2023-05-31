// // ----------------------------------------------------------------------------
// // <copyright file="SaveSystem.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.UI
{
    public static class SaveSystem
    {
        private const string c_KinetixSelected = "c_KinetixSelected";
        private const string c_KinetixDidSave  = "c_KinetixDidSave";
        private const string c_KinetixEmoteChecked = "c_KinetixEmoteChecked";
        private const string c_KinetixEmoteContext = "c_KinetixEmoteContext";
        private const string c_KinetixEmoteContextSaved = "c_KinetixEmoteContextSaved";

        //*****************************************************************
        // FAVORITE EMOTE
        //*****************************************************************

        public static bool DidSave()
        {
            return PlayerPrefs.HasKey(c_KinetixDidSave);
        }
        
        public static void UpdateSave(Dictionary<int, AnimationIds> _FavoriteAnimationsIdByIndex)
        {
            string json = SerializeSave(_FavoriteAnimationsIdByIndex);
            PlayerPrefs.SetInt(c_KinetixDidSave, 1);
            PlayerPrefs.SetString(c_KinetixSelected, json);
        }

        private static string SerializeSave(Dictionary<int, AnimationIds> _FavoriteAnimationsIdByIndex)
        {
            return WrapperFavoriteDataCollection.Serialize(_FavoriteAnimationsIdByIndex);
        }

        public static Dictionary<int, AnimationIds> DeserializeSave()
        {
            if (!PlayerPrefs.HasKey(c_KinetixSelected))
            {
                Dictionary<int, AnimationIds> favoriteAnimationsIdByIndex = new Dictionary<int, AnimationIds>
                {
                    /*
                    { 1, new AnimationIds("-1", "local", "local") },
                    { 2, new AnimationIds("-2", "local", "local") },
                    { 3, new AnimationIds("-3", "local", "local") },
                    */
                };
                return favoriteAnimationsIdByIndex;
            }

            string json = PlayerPrefs.GetString(c_KinetixSelected, "");
            return WrapperFavoriteDataCollection.Deserialize(json);
        }

        //*****************************************************************
        // Notification Emote CHECK
        //*****************************************************************

        public static bool DidEmoteChecked(string _UUID)
        {
            return PlayerPrefs.HasKey(c_KinetixEmoteChecked+_UUID);
        }

        public static void SaveEmoteChecked(string _UUID)
        {
            PlayerPrefs.SetInt(c_KinetixEmoteChecked+_UUID, 1);
        }

        //*****************************************************************
        // CONTEXTUAL EMOTE
        //*****************************************************************

        public static bool DidContextSave()
        {
            return PlayerPrefs.HasKey(c_KinetixEmoteContextSaved);
        }
        
        public static void UpdateContextSave(Dictionary<string, ContextualEmote> _ContextualEmotes)
        {
            string json = SerializeSaveContext(_ContextualEmotes);
            PlayerPrefs.SetInt(c_KinetixEmoteContextSaved, 1);
            PlayerPrefs.SetString(c_KinetixEmoteContext, json);
        }

        private static string SerializeSaveContext(Dictionary<string, ContextualEmote> contextEmotes)
        {
            return WrapperContextDataCollection.Serialize(contextEmotes);
        }

        public static Dictionary<string, ContextualEmote> DeserializeContextSave( Dictionary<string, ContextualEmote> contextEmotes )
        {
            if (!PlayerPrefs.HasKey(c_KinetixEmoteContext))
            {
                return contextEmotes;
            }

            string json = PlayerPrefs.GetString(c_KinetixEmoteContext, "");
            return WrapperContextDataCollection.Deserialize(json, contextEmotes);
        }

    }
}
