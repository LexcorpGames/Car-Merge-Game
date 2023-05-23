using UnityEngine;

namespace MondayOFF {
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class MondayOFF : MonoBehaviour {
        static MondayOFF _instance = null;

        [SerializeField] bool _alsoInitializeFacebookSDK = true;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnLoad() {
            var mondayoff = GameObject.Find("MondayOFF");
            if (mondayoff == null) {
                new GameObject("MondayOFF").AddComponent<MondayOFF>();
            }
        }

        private void OnFacebookInitialized() {
            Facebook.Unity.FB.ActivateApp();
            Debug.Log("[MondayOFF] Facebook SDK initialized");
        }

        private void InitFacebook() {
            Facebook.Unity.FB.Init(OnFacebookInitialized);
        }

        private void Awake() {
            if (_instance != null) {
                Debug.LogWarning("Instance of MondayOFF already exists!");
                DestroyImmediate(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start() {
            MaxSdk.SetSdkKey("-uBAP4IJbzlOMFq-KJUwdvW8bwGdhtGgmRr9V8T65CUSSIQocwhFqCNP7e2pVITFkJPERuLW5q-X7PJlJ_-7CM");
            MaxSdk.InitializeSdk();

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
                Debug.Log("[MondayOFF] MaxSdk initialized");
            };

            if (_alsoInitializeFacebookSDK) {
                if (Facebook.Unity.FB.IsInitialized) {
                    Facebook.Unity.FB.ActivateApp();
                } else {
                    Facebook.Unity.FB.Init(OnFacebookInitialized);
                }
            }
        }
    }
}