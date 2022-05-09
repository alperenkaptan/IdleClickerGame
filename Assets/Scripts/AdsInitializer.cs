using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [HideInInspector] string _androidGameId = "4743163";
    [HideInInspector] string _iOSGameId = "4743162";
    [SerializeField] bool _testMode;
    private string _gameId;
    [SerializeField] Button _showAdButton;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        //Debug.Log("Unity Ads initialization complete. ");
        Instantiate(_showAdButton, this.transform, false);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        //Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}