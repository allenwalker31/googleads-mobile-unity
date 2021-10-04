// Copyright (C) 2017 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;
using System.Reflection;
using System.Runtime;

using GoogleMobileAds;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class MobileAds
    {
        public static class Utils
        {
            public static float GetDeviceScale()
            {
                return Instance.client.GetDeviceScale();
            }

            public static int GetDeviceSafeWidth()
            {
                return Instance.client.GetDeviceSafeWidth();

            }
        }

        private readonly IMobileAdsClient client = GetMobileAdsClient();

        private static IClientFactory clientFactory;

        private static MobileAds instance;

        public static MobileAds Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MobileAds();
                }
                return instance;
            }
        }

        public static void Initialize(Action<InitializationStatus> initCompleteAction)
        {
            Instance.client.Initialize((initStatusClient) =>
            {

                if (initCompleteAction != null)
                {
                    initCompleteAction.Invoke(new InitializationStatus(initStatusClient));
                }
            });
            MobileAdsEventExecutor.Initialize();
        }

        public static void DisableMediationInitialization()
        {
            Instance.client.DisableMediationInitialization();
        }

        public static void SetApplicationMuted(bool muted)
        {
            Instance.client.SetApplicationMuted(muted);
        }

        /// <summary>
        /// Policy changes in China will require that the Google Mobile Ads SDK
        /// be configured specifically to operate in China.
        /// </summary>
        /// <remarks>
        /// This must be done before MobileAds.Initialization
        /// </remarks>
        public static void SetAppPrimaryRegionChina(bool isPrimaryAppRegionChina)
        {
            Instance.client.SetAppPrimaryRegionChina(isPrimaryAppRegionChina);
        }

        public static void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            Instance.client.SetRequestConfiguration(requestConfiguration);
        }

        public static RequestConfiguration GetRequestConfiguration()
        {

            return Instance.client.GetRequestConfiguration();
        }

        public static void SetApplicationVolume(float volume)
        {
            Instance.client.SetApplicationVolume(volume);
        }

        public static void SetiOSAppPauseOnBackground(bool pause)
        {
            Instance.client.SetiOSAppPauseOnBackground(pause);
        }

        /// <summary>
        /// Opens ad inspector UI.
        /// </summary>
        /// <param name="adInspectorClosedAction">Called when ad inspector UI closes.</param>
        public static void OpenAdInspector(Action<AdInspectorError> adInspectorClosedAction)
        {
            Instance.client.OpenAdInspector(args =>
            {
                if(adInspectorClosedAction != null)
                {
                    AdInspectorError error = null;
                    if (args != null && args.AdErrorClient != null)
                    {
                        error = new AdInspectorError(args.AdErrorClient);
                    }
                    adInspectorClosedAction(error);
                }
            });
        }

        internal static IClientFactory GetClientFactory() {
          if (clientFactory == null) {
            String typeName = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.iOS";
            } else if (Application.platform == RuntimePlatform.Android) {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.Android";
            } else {
              typeName = "GoogleMobileAds.GoogleMobileAdsClientFactory,GoogleMobileAds.Unity";
            }
            Type type = Type.GetType(typeName);
            clientFactory = (IClientFactory)System.Activator.CreateInstance(type);
          }
          return clientFactory;
        }

        private static IMobileAdsClient GetMobileAdsClient()
        {
            return GetClientFactory().MobileAdsInstance();
        }
    }
}
