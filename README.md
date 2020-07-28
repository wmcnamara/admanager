# AdManager
Singleton Ad Manager For Unity.

## Features
1. Automatic Platform Detection
2. Simple to set up, and use.
3. Dispatches UnityEvents for rewarded ads.
4. Easy banner configuration

## How To Use:
1. Attach AdManager.cs to any GameObject in the hierarchy.
2. Set the Store ID and Placement ID values in the inspector. You can find these on your Unity Ads dashboard.
3. Select the banner position you wish to use.
4. Run `AdManager.Instance.ShowAd(AdType.NonRewarded)`. You can also use `AdType.Rewarded`, and `AdType.Banner`.
