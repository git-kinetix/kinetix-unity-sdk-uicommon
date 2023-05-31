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
    internal struct WrapperContextDataCollection
    {
        public List<ContextDataWrapper> contextDataWrappers;

        public static string Serialize(Dictionary<string, ContextualEmote> _ContextByEventName)
        {
            WrapperContextDataCollection dataCollection = new WrapperContextDataCollection
            {
                contextDataWrappers = new List<ContextDataWrapper>()
            };

            foreach (KeyValuePair<string, ContextualEmote> KVP in _ContextByEventName)
                dataCollection.contextDataWrappers.Add(
                    new ContextDataWrapper()
                    {
                        EventName 		= KVP.Key,
                        AnimationUUID	= KVP.Value.EmoteUuid
                    });

            return JsonUtility.ToJson(dataCollection);
        }

        public static Dictionary<string, ContextualEmote> Deserialize(string _Json, Dictionary<string, ContextualEmote> _ContextByEventName)
        {
            _ContextByEventName ??= new Dictionary<string, ContextualEmote>();
            WrapperContextDataCollection dataCollection = JsonUtility.FromJson<WrapperContextDataCollection>(_Json);

            foreach (ContextDataWrapper dataCollectionContextDataWrapper in dataCollection.contextDataWrappers)
            {
				if( _ContextByEventName.ContainsKey(dataCollectionContextDataWrapper.EventName))
				{
					_ContextByEventName[dataCollectionContextDataWrapper.EventName].EmoteUuid = dataCollectionContextDataWrapper.AnimationUUID;
				}
            }

            return _ContextByEventName;
        }
    }

    [Serializable]
    internal struct ContextDataWrapper
    {
        public string EventName;
        public string AnimationUUID;
    }
}
