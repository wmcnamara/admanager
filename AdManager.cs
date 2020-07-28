using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    [Header("Store IDs")]
    [SerializeField] private string androidStoreID = null;
    [SerializeField] private string iosStoreID = null;

    [Header("Placement IDs")]
    [SerializeField] private string rewardedVideoID = null;
    [SerializeField] private string nonRewardedVideoID = null;
    [SerializeField] private string bannerID = null;

    [Header("Configuration")]
    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;
    [Tooltip("Shows the banner when the game begins")]
    [SerializeField] private bool StartShowBanner = true;

    [Header("Rewarded Ad Events")]
    [SerializeField] private UnityEvent OnAdCompleted = null;
    [SerializeField] private UnityEvent OnAdSkipped = null;
    [SerializeField] private UnityEvent OnAdFailed = null;

    //Reward Events
    public void OnUnityAdsDidError(string message) => Debug.LogError("Internal Unity Error: " + message);
    public void OnUnityAdsDidStart(string placementId) { }
    public void OnUnityAdsReady(string placementId) { }

    private string storeID;

#if UNITY_EDITOR
    private readonly static bool testing = true;
#else
    private readonly static bool testing = false;
#endif

#if UNITY_ANDROID
    private static readonly Platform platform = Platform.Android;
#elif UNITY_IOS
    private static readonly Platform platform = Platform.IOS;
#endif

    private void DetermineStoreIDBasedOnPlatform()
    {
        switch (platform)
        {
            case Platform.Android:
                storeID = androidStoreID;
                break;
            case Platform.IOS:
                storeID = iosStoreID;
                break;
        }   
    }

    //Dispatch events for rewarded ads.
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == rewardedVideoID)
        {
            switch (showResult)
            {
                case ShowResult.Finished:
                    Debug.Log("Ad Completed");
                    OnAdCompleted.Invoke();
                    break;
                case ShowResult.Skipped:
                    Debug.Log("Ad Skipped");
                    OnAdSkipped.Invoke();
                    break;
                case ShowResult.Failed:
                    Debug.Log("Ad Error. Check network and configuration.");
                    OnAdFailed.Invoke();
                    break;
            }
        }
    }

    //Displays an ad. Does not dispatch any reward callbacks.
    public void ShowNonRewardedAd()
    {
        if (Advertisement.IsReady(nonRewardedVideoID))
            Advertisement.Show(nonRewardedVideoID);
    }
    
    //Attempts to show the banner every second.
    private IEnumerator AttemptShowBanner()
    {
        while (!Advertisement.IsReady(bannerID))
            yield return new WaitForSeconds(1.0f);

        Advertisement.Banner.SetPosition(bannerPosition);
        Advertisement.Banner.Show(bannerID);
    }

    //Displays a banner ad. Does not dispatch reward events.
    public void ShowBannerAd() => Instance.StartCoroutine(AttemptShowBanner());

    //Hides banner ad.
    public void HideBannerAd() => Advertisement.Banner.Hide();

    //Shows a rewarded ad, and dispatches reward events.
    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady(rewardedVideoID))
            Advertisement.Show(rewardedVideoID);
    }

    //Singleton implementation
    private static AdManager m_instance;
    public static AdManager Instance { get { return m_instance; } }

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            //Initialize singleton instance
            m_instance = this;
            DetermineStoreIDBasedOnPlatform();

            Advertisement.AddListener(this);
            Advertisement.Initialize(storeID, testing);

            if (StartShowBanner) { ShowBannerAd(); }
            DontDestroyOnLoad(gameObject);
        }
    }

    //Used internally. Do not do anything with this.
    private enum Platform { IOS, Android }
}

public enum AdTypes { Rewarded, NonRewarded, Banner}