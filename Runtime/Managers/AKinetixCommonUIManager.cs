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

        // CACHE
        protected Dictionary<int, AnimationIds> FavoritesAnimationIdByIndex;

        protected KinetixCommonUIConfiguration kinetixCommonUIConfiguration;

        protected void Initialize(KinetixCommonUIConfiguration KinetixCommonConfig)
        {
            DontDestroyOnLoad(gameObject);
            mainView.Show();

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

            FavoritesAnimationIdByIndex ??= new Dictionary<int, AnimationIds>();
            FavoritesAnimationIdByIndex =   SaveSystem.DeserializeSave();

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
                List<AnimationIds>      ids       = new List<AnimationIds>();
                List<AnimationMetadata> metadatas = userMetadatas.ToList();

                if (!SaveSystem.DidSave() && FavoritesAnimationIdByIndex.Keys.Count < c_BaseCountEmotesOnWheel)
                {
                    int maxAnimationsCount = Mathf.Min(c_BaseCountEmotesOnWheel, metadatas.Count);
                    int counter            = FavoritesAnimationIdByIndex.Keys.Count;
                
                    for (int i = counter; i < maxAnimationsCount; i++)
                    {
                        int index = (int)Mathf.Ceil(i / 2.0f);
                        index *= (i % 2 == 0 ? -1 : 1);
                        index =  (int)(Mathf.Floor(Mathf.Repeat((float)index, 9.0f)));
                        if (!FavoritesAnimationIdByIndex.ContainsKey(index))
                            FavoritesAnimationIdByIndex.Add(index, metadatas[i].Ids);
                    }
                }

                foreach (KeyValuePair<int, AnimationIds> kvp in FavoritesAnimationIdByIndex)
                {
                    if (metadatas.Exists(data => data.Ids.UUID == kvp.Value.UUID))
                        ids.Add(kvp.Value);
                }
            
                OnLoadData();
                
                if (ids.Count > 0)
                    KinetixCore.Animation.LoadLocalPlayerAnimations(ids.ToArray());
            });
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
            KinetixCore.Animation.LoadLocalPlayerAnimation(ids);
            if (FavoritesAnimationIdByIndex.ContainsKey(_Index))
                OnRemoveFavoriteAnimation(_Index);
            FavoritesAnimationIdByIndex.Add(_Index, ids);
            SaveSystem.UpdateSave(FavoritesAnimationIdByIndex);
            OnLoadData();

            int page = _Index / c_CountSlotOnWheel + 1;
            int tile = _Index % c_CountSlotOnWheel + 1;

            KinetixAnalytics.SendEvent("Add_To_Wheel", ids.UUID, KinetixAnalytics.Page.EmoteWheel, KinetixAnalytics.Event_type.DragDrop, tile, page);
        }

        protected void OnRemoveFavoriteAnimation(int _Index)
        {
            if (FavoritesAnimationIdByIndex == null)
                return;

            AnimationIds idsToRemove = FavoritesAnimationIdByIndex[_Index];
            FavoritesAnimationIdByIndex.Remove(_Index);
            SaveSystem.UpdateSave(FavoritesAnimationIdByIndex);
            if (!FavoritesAnimationIdByIndex.Values.ToList().Exists(id => id.Equals(idsToRemove)))
                KinetixCore.Animation.UnloadLocalPlayerAnimation(idsToRemove);
            OnLoadData();

            int page = _Index / c_CountSlotOnWheel + 1;
            int tile = _Index % c_CountSlotOnWheel + 1;
            KinetixAnalytics.SendEvent("Remove_From_Wheel", idsToRemove.UUID, KinetixAnalytics.Page.EmoteWheel, KinetixAnalytics.Event_type.DragDrop, tile, page);
        }

        protected void OnSelectAnimation(AnimationIds _IDs)
        {
            KinetixUI.HideAll();
            KinetixUI.OnPlayedAnimationWithEmoteSelector?.Invoke(_IDs);
            KinetixCore.Animation.PlayAnimationOnLocalPlayer(_IDs);
        }

        // On Should Reload Views
        protected abstract void OnLoadData();
    }
}
