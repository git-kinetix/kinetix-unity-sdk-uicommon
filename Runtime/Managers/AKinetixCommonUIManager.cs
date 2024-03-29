// // ----------------------------------------------------------------------------
// // <copyright file="AKinetixCommonUIManager.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kinetix.Utils;
using UnityEngine;

namespace Kinetix.UI.Common
{
    public abstract class AKinetixCommonUIManager : MonoBehaviour
    {
        [SerializeField] protected MainView        mainView;

        private readonly int c_BaseCountEmotesOnWheel = 5;
        private readonly int c_CountSlotOnWheel = 9;
        private readonly int c_DaysOfNotification = 1;

        // CACHE
        protected Dictionary<int, AnimationIds> FavoritesAnimationIdByIndex;
        protected Dictionary<string, ContextualEmote> ContextEmotesByEventName;

        protected KinetixCommonUIConfiguration kinetixCommonUIConfiguration;

        protected bool bShowNotificationNewEmotes = false;

        const string LOCK_FAVORITES_ID = "Wheel Favorites";

        protected void Initialize(KinetixCommonUIConfiguration KinetixCommonConfig)
        {
            DontDestroyOnLoad(gameObject);
            mainView.Show();

            if(KinetixCommonConfig == null)
                KinetixCommonConfig = new KinetixCommonUIConfiguration();
            kinetixCommonUIConfiguration = KinetixCommonConfig;

            InitInputManager(kinetixCommonUIConfiguration);
            Translation.KinetixTranslationManager.Initialize(KinetixCommonConfig?.baseLanguage ?? SystemLanguage.English);

            if (KinetixCore.IsInitialized())
                Setup();
            else
                KinetixCore.OnInitialized += OnKinetixPluginInitialized;
        }

        protected virtual void OnDestroy()
        {
            KinetixCore.OnInitialized -= OnKinetixPluginInitialized;
        }

        private void OnKinetixPluginInitialized()
        {
            Setup();
        }

        protected virtual void Setup()
        {
            KinetixCore.Account.OnUpdatedAccount += OnUpdatedAccount;
            KinetixCore.Account.OnConnectedAccount += OnConnectedAccount;

            FavoritesAnimationIdByIndex ??= new Dictionary<int, AnimationIds>();
            FavoritesAnimationIdByIndex = SaveSystem.DeserializeSave();

            ContextEmotesByEventName ??= new Dictionary<string, ContextualEmote>();
            ContextEmotesByEventName = SaveSystem.DeserializeContextSave( KinetixCore.Context.GetContextEmotes() );

            

            KinetixCore.Animation.OnRegisteredLocalPlayer += LoadData;
            LoadData();
        }

        private void OnUpdatedAccount()
        {
            LoadData();
        }

        private void LoadData()
        {
            KinetixCore.Metadata.GetUserAnimationMetadatas((userMetadatas) =>
            {
                List<AnimationMetadata> metadatas = userMetadatas.ToList();

                //if has new emotes, show notification indication visual on menu tab Bag
                KinetixUI.OnUpdateNotificationNewEmote?.Invoke( HasNewEmotes(metadatas) );

                LoadFavorites(metadatas);
                LoadContexts(metadatas);
                
                OnLoadData();
            });
        }

        private void LoadFavorites(List<AnimationMetadata> _AnimationMetadatas)
        {
            List<AnimationIds> ids = new List<AnimationIds>();
            
            //for add emote in the wheel if there is no in the wheel playerprefs
            if (!SaveSystem.DidSave() && FavoritesAnimationIdByIndex.Keys.Count < c_BaseCountEmotesOnWheel)
            {
                int maxAnimationsCount = Mathf.Min(c_BaseCountEmotesOnWheel, _AnimationMetadatas.Count);
                int counter            = FavoritesAnimationIdByIndex.Keys.Count;

                for (int i = counter; i < maxAnimationsCount; i++)
                {
                    int index = (int)Mathf.Ceil(i / 2.0f);
                    index *= (i % 2 == 0 ? -1 : 1);
                    index =  (int)(Mathf.Floor(Mathf.Repeat((float)index, 9.0f)));
                    if (!FavoritesAnimationIdByIndex.ContainsKey(index))
                        FavoritesAnimationIdByIndex.Add(index, _AnimationMetadatas[i].Ids);
                }
            }

            //add in the ids to preload the Emotes that the user has in favorites
            foreach (KeyValuePair<int, AnimationIds> kvp in FavoritesAnimationIdByIndex)
            {
                if (_AnimationMetadatas.Exists(data => data.Ids.UUID == kvp.Value.UUID))
                    ids.Add(kvp.Value);
            }
            
            if (ids.Count > 0)
                KinetixCore.Animation.LoadLocalPlayerAnimations(ids.Select(id => id.UUID).ToArray(), LOCK_FAVORITES_ID);
        }

        private void LoadContexts(List<AnimationMetadata> _AnimationMetadatas)
        {
            foreach (KeyValuePair<string, ContextualEmote> emoteByContext in ContextEmotesByEventName)
            {
                if (_AnimationMetadatas.Exists(data => data.Ids.UUID == emoteByContext.Value.EmoteID))
                    KinetixCore.Context.RegisterEmoteForContext(emoteByContext.Key, emoteByContext.Value.EmoteID);
            }
        }

        protected bool HasNewEmotes(List<AnimationMetadata> listMetadata)
        {
            bool bNoNewEmotes = false;
            //is there new emote creates during the last day ? (c_DaysOfNotification) 
            for (int i = 0; i < listMetadata.Count; i++)
            {
                if( System.DateTime.Compare(listMetadata[i].CreatedAt, System.DateTime.Now.AddDays(-c_DaysOfNotification)) > 0 )
                {
                    //but has not be checked already
                    if( !SaveSystem.DidEmoteChecked(listMetadata[i].Ids.UUID) )
                    {
                        bNoNewEmotes = true;
                        break;
                    }
                }
            }
            return bNoNewEmotes;
        }

        private void InitInputManager (KinetixCommonUIConfiguration kinetixCommonConfiguration)
        {
            if( kinetixCommonConfiguration == null)
                kinetixCommonConfiguration = new KinetixCommonUIConfiguration();

            if( kinetixCommonConfiguration.kinetixInputActionMap == null )
                kinetixCommonConfiguration.kinetixInputActionMap = Resources.Load<KinetixInputMapSO>("InputActionMap/DefaultKinetixInputActionMapSO");
            
            KinetixInputManager.Initialize(kinetixCommonConfiguration.kinetixInputActionMap.kinetixActionMap);
        }

        protected void OnAddFavoriteAnimation(int _Index, AnimationIds ids)
        {
            KinetixCore.Animation.LoadLocalPlayerAnimation(ids.UUID, LOCK_FAVORITES_ID);

            if (FavoritesAnimationIdByIndex.ContainsKey(_Index))
                OnRemoveFavoriteAnimation(_Index);

            FavoritesAnimationIdByIndex.Add(_Index, ids);
            
            SaveSystem.UpdateSave(FavoritesAnimationIdByIndex);
            OnLoadData();

            int tile = _Index % c_CountSlotOnWheel + 1;
            KinetixAnalytics.SendEvent("Add_To_Wheel", ids.UUID, KinetixAnalytics.Page.EmoteWheel, KinetixAnalytics.Event_type.DragDrop, tile);
        }

        protected void OnRemoveFavoriteAnimation(int _Index)
        {
            if (FavoritesAnimationIdByIndex == null)
                return;

            AnimationIds idsToRemove = FavoritesAnimationIdByIndex[_Index];
            FavoritesAnimationIdByIndex.Remove(_Index);
            SaveSystem.UpdateSave(FavoritesAnimationIdByIndex);
            
            if (!FavoritesAnimationIdByIndex.Values.ToList().Exists(id => id.Equals(idsToRemove)))
                KinetixCore.Animation.UnloadLocalPlayerAnimation(idsToRemove.UUID, LOCK_FAVORITES_ID);

            OnLoadData();

            int tile = _Index % c_CountSlotOnWheel + 1;
            KinetixAnalytics.SendEvent("Remove_From_Wheel", idsToRemove.UUID, KinetixAnalytics.Page.EmoteWheel, KinetixAnalytics.Event_type.DragDrop, tile);
        }

        protected void OnSelectAnimation(AnimationIds _IDs)
        {
            KinetixUI.HideAll();
            KinetixUI.OnPlayedAnimationWithEmoteSelector?.Invoke(_IDs);
            KinetixCore.Animation.PlayAnimationOnLocalPlayer(_IDs.UUID);
        }

        protected void OnAddContext(string eventName, string UUID)
        {
            if (ContextEmotesByEventName == null)
                return;
            if (ContextEmotesByEventName.ContainsKey(eventName))
                ContextEmotesByEventName[eventName].EmoteID = UUID;

            SaveSystem.UpdateContextSave(ContextEmotesByEventName);
            KinetixCore.Context.RegisterEmoteForContext(eventName, UUID);
        }

        protected void OnRemoveContext(string eventName)
        {
            if (ContextEmotesByEventName == null)
                return;
            if (ContextEmotesByEventName.ContainsKey(eventName))
                ContextEmotesByEventName[eventName].EmoteID = "";

            SaveSystem.UpdateContextSave(ContextEmotesByEventName);
            KinetixCore.Context.UnregisterEmoteForContext(eventName);
        }

        // On Should Reload Views
        protected abstract void OnLoadData();
        // On New Account Connected
        protected abstract void OnConnectedAccount();
    }
}
