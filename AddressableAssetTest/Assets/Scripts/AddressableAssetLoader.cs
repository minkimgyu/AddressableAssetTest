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

    // ��巹������ Label�� ���� �� �ִ� �ʵ�.
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
        // ����Ÿ���� ��θ� �����´�.
        // ����̱� ������ �޸𸮿� ������ �ε���� �ʴ´�.
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
                    Debug.Log("�ε� ����");
                    break;

                default:
                    break;
            }
        };
    }  
}
