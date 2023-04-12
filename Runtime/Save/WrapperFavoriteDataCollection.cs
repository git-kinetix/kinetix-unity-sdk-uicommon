// // ----------------------------------------------------------------------------
// // <copyright file="WrapperFavoriteDataCollection.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Kinetix.UI.Common;
using UnityEngine;

namespace Kinetix
{
    [Serializable]
    internal struct WrapperFavoriteDataCollection
    {
        public List<FavoriteDataWrapper> favoriteDataWrappers;

        public static string Serialize(Dictionary<int, AnimationIds> _FavoritesAnimationIdByIndex)
        {
            WrapperFavoriteDataCollection dataCollection = new WrapperFavoriteDataCollection
            {
                favoriteDataWrappers = new List<FavoriteDataWrapper>()
            };

            foreach (KeyValuePair<int, AnimationIds> KVP in _FavoritesAnimationIdByIndex)
                dataCollection.favoriteDataWrappers.Add(
                    new FavoriteDataWrapper()
                    {
                        Index           = KVP.Key,
                        AnimationUUID   = KVP.Value.UUID
                    });

            return JsonUtility.ToJson(dataCollection);
        }

        public static Dictionary<int, AnimationIds> Deserialize(string _Json)
        {
            Dictionary<int, AnimationIds> favoriteAnimationIdByIndex = new Dictionary<int, AnimationIds>();
            WrapperFavoriteDataCollection dataCollection = JsonUtility.FromJson<WrapperFavoriteDataCollection>(_Json);

            foreach (FavoriteDataWrapper dataCollectionFavoriteDataWrapper in dataCollection.favoriteDataWrappers)
            {
                AnimationIds ids = new AnimationIds(dataCollectionFavoriteDataWrapper.AnimationUUID);
                favoriteAnimationIdByIndex.Add(dataCollectionFavoriteDataWrapper.Index, ids);
            }
                

            return favoriteAnimationIdByIndex;
        }
    }

    [Serializable]
    internal struct FavoriteDataWrapper
    {
        public int    Index;
        public string AnimationUUID;
    }
}
