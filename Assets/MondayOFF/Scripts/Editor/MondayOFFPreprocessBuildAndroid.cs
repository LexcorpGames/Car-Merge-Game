#if UNITY_ANDROID
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MondayOFF {
    public class MondayOFFPreprocessBuildAndroid : IPreprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) {
            ConfigAndroidManifest();
            ConfigProguard();
        }

        private static void ConfigProguard() {
            List<string> PROGUARD_LIST = new List<string>(){
                "# Facebook",
                "-keep class com.facebook.** { *; }",
            };

            string proguardFilename =
#if UNITY_2021_2_OR_NEWER
                "proguard-user.txt"
#else
                "proguard.txt"
#endif
            ;

            string proguardPath = Path.Combine("Assets/Plugins/Android", proguardFilename);

            List<string> lines;
            if (File.Exists(proguardPath)) {
                lines = File.ReadAllLines(proguardPath).Union(PROGUARD_LIST).ToList();
            } else {
                string proguardDirectory = Path.GetDirectoryName(proguardPath);
                if (!Directory.Exists(proguardDirectory)) {
                    Directory.CreateDirectory(proguardDirectory);
                }
                File.Create(proguardPath).Dispose();
                lines = PROGUARD_LIST;
            }

            File.WriteAllLines(proguardPath, lines);
        }

        private static void ConfigAndroidManifest() {
            const string manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
            string manifestDirectory = Path.GetDirectoryName(manifestPath);
            if (!Directory.Exists(manifestDirectory)) {
                Directory.CreateDirectory(manifestDirectory);
            }

            // Check for FB 
            if (Facebook.Unity.Settings.FacebookSettings.AppIds == null || Facebook.Unity.Settings.FacebookSettings.AppIds.Count == 0) {
                UnityEditor.Selection.activeObject = Facebook.Unity.Settings.FacebookSettings.Instance;
                throw new UnityEditor.Build.BuildFailedException("[EVERYDAY] Facebook App ID is empty!");
            }
            if (string.IsNullOrEmpty(Facebook.Unity.Settings.FacebookSettings.ClientToken)) {
                UnityEditor.Selection.activeObject = Facebook.Unity.Settings.FacebookSettings.Instance;
                throw new UnityEditor.Build.BuildFailedException("[EVERYDAY] Facebook Client Token is empty!");
            }

            Facebook.Unity.Editor.ManifestMod.GenerateManifest();

            var androidManifestDocument = XDocument.Load(manifestPath);

            XNamespace androidNamespace = androidManifestDocument.Root.Attribute(XNamespace.Xmlns + "android").Value;

            var applicationNode = androidManifestDocument.Root
                                        .Descendants("application")
                                        .FirstOrDefault();

            if (applicationNode == null) {
                Debug.LogWarning("Application node doest not exist! Are you sure AndroidManifest.xml is valid?");
                return;
            }

            bool isModified = false;

            if (applicationNode.Attribute(androidNamespace + "debuggable").Value == "true") {
                applicationNode.SetAttributeValue(androidNamespace + "debuggable", "false");
            }

            // Add required permissions
            string[] permissions = new string[]{
                "android.permission.ACCESS_NETWORK_STATE",
                "android.permission.INTERNET",
                "com.google.android.gms.permission.AD_ID"
            };

            foreach (var permission in permissions) {
                var existingPermission
                        = androidManifestDocument.Root
                            .Descendants()
                            .FirstOrDefault(element => element.Name == "uses-permission" && element.Attribute(androidNamespace + "name").Value == permission);

                if (existingPermission == null) {
                    var element = new XElement("uses-permission");
                    element.SetAttributeValue(androidNamespace + "name", permission);
                    applicationNode.Parent.Add(element);
                    isModified = true;
                }
            }

            if (isModified) {
                applicationNode.Document.Save(manifestPath);
                Debug.Log("Updated AndroidManifest.xml");
            }
        }
    }
}
#endif