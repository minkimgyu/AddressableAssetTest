using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System;

public class AddressableAssetLoader : MonoBehaviour
{
    public enum PlaySceneLabel
    {
        SkillIcons,
        Weapons,
    }

    // 어드레서블의 Label을 얻어올 수 있는 필드.
    [SerializeField] AssetLabelReference assetLabel;
    [SerializeField] StringSpriteDictionary _spriteDictionary;
    [SerializeField] StringGameObjectDictionary _gameObjectDictionary;

    [SerializeField] Image[] _images;

    private void Start()
    {
        _spriteDictionary = new StringSpriteDictionary();
        _gameObjectDictionary = new StringGameObjectDictionary();

        foreach (var item in collection)
        {

        }

        GetLocations(LoadAssetsInLabel);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            foreach (var item in _spriteDictionary)
            {
                Addressables.Release(item.Value);
                _spriteDictionary.Remove(item.Key);
            }
        }
    }

    public void GetLocations(Action<IList<IResourceLocation>> OnComplete)
    {
        // 빌드타겟의 경로를 가져온다.
        // 경로이기 때문에 메모리에 에셋이 로드되진 않는다.
        Addressables.LoadResourceLocationsAsync(assetLabel.labelString, typeof(Sprite)).Completed +=
        (handle) =>
        {
            OnComplete?.Invoke(handle.Result);
        };
    }

    void LoadAssetsInLabel(IList<IResourceLocation> locationList)
    {
        for (int i = 0; i < locationList.Count; i++)
        {
            LoadAsset(locationList[i], _spriteDictionary, 
                (value, index) => 
                { 
                    _images[index].sprite = value; 
                }
            );
        }
    }

    void LoadAsset<T>(IResourceLocation location, Dictionary<string, T> dictionary, Action<T, int> OnComplete)
    {
        Addressables.LoadAssetAsync<T>(location).Completed +=
        (handle) =>
        {
            switch (handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    Debug.Log(location.PrimaryKey);
                    Debug.Log(handle.Result);
                    dictionary.Add(location.PrimaryKey, handle.Result);
                    OnComplete?.Invoke(handle.Result, dictionary.Count - 1);
                    break;

                case AsyncOperationStatus.Failed:
                    Debug.Log("로드 실패");
                    break;

                default:
                    break;
            }
        };
    }  
}
