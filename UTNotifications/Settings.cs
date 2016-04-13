using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTNotifications
{
    /// <summary>
    /// UTNotifications settings. Edit in Unity Editor: <c>"Edit -> Project Settings -> UTNotifications"</c>
    /// </summary>
    public class Settings : ScriptableObject
    {
    //public
        public static Settings Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = Resources.Load(m_assetName) as Settings;
                    if (m_instance == null)
                    {
                        m_instance = CreateInstance<Settings>();
#if UNITY_EDITOR
                        string path = Path.Combine(Application.dataPath, "UTNotifications/Resources");
                        if (!Directory.Exists(path))
                        {
                            AssetDatabase.CreateFolder("Assets/UTNotifications", "Resources");
                        }
                        
                        AssetDatabase.CreateAsset(m_instance, "Assets/UTNotifications/Resources/" + m_assetName + ".asset");
#endif
                    }
                }

                return m_instance;
            }
        }

        public List<NotificationProfile> NotificationProfiles
        {
            get
            {
                return m_notificationProfiles;
            }
        }

        public bool PushNotificationsEnabledIOS
        {
            get
            {
                return m_pushNotificationsEnabledIOS;
            }
            set
            {
                if (m_pushNotificationsEnabledIOS != value)
                {
                    m_pushNotificationsEnabledIOS = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public bool PushNotificationsEnabledGooglePlay
        {
            get
            {
                return m_pushNotificationsEnabledGooglePlay;
            }
            set
            {
                if (m_pushNotificationsEnabledGooglePlay != value)
                {
                    m_pushNotificationsEnabledGooglePlay = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }
        
        public bool PushNotificationsEnabledAmazon
        {
            get
            {
                return m_pushNotificationsEnabledAmazon;
            }
            set
            {
                if (m_pushNotificationsEnabledAmazon != value)
                {
                    m_pushNotificationsEnabledAmazon = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public bool PushNotificationsEnabledWindows
        {
            get
            {
                return m_pushNotificationsEnabledWindows;
            }
            set
            {
                if (m_pushNotificationsEnabledWindows != value)
                {
                    m_pushNotificationsEnabledWindows = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public TokenEncoding IOSTokenEncoding
        {
            get
            {
                return m_iOSTokenEncoding;
            }
            set
            {
                if (m_iOSTokenEncoding != value)
                {
                    m_iOSTokenEncoding = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public ShowNotifications AndroidShowNotificationsMode
        {
            get
            {
                return m_androidShowNotificationsMode;
            }
            set
            {
                if (m_androidShowNotificationsMode != value)
                {
                    m_androidShowNotificationsMode = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

        public bool AndroidRestoreScheduledNotificationsAfterReboot
        {
            get
            {
                return m_androidRestoreScheduledNotificationsAfterReboot;
            }
#if UNITY_EDITOR
            set
            {
                if (m_androidRestoreScheduledNotificationsAfterReboot != value)
                {
                    m_androidRestoreScheduledNotificationsAfterReboot = value;
                    Save();
                }
            }
#endif
        }

        public bool Android4CompatibilityMode
        {
            get
            {
                return m_android4CompatibilityMode;
            }
#if UNITY_EDITOR
            set
            {
				if (value && FacebookPluginPresent())
				{
					value = false;
				}
				
                if (m_android4CompatibilityMode != value)
                {
                    m_android4CompatibilityMode = value;
                    Save();
                }
            }
#endif
        }

        public NotificationsGroupingMode AndroidNotificationsGrouping
        {
            get
            {
                return m_androidNotificationsGrouping;
            }
            set
            {
                if (m_androidNotificationsGrouping != value)
                {
                    m_androidNotificationsGrouping = value;
#if UNITY_EDITOR
                    Save();
#endif
                }
            }
        }

		public bool AndroidShowLatestNotificationOnly
		{
			get
			{
				return m_androidShowLatestNotificationOnly;
			}

#if UNITY_EDITOR
			set
			{
				if (value != m_androidShowLatestNotificationOnly)
				{
					m_androidShowLatestNotificationOnly = value;
					Save();
				}
			}
#endif
		}
        
        public string GooglePlaySenderID
        {
            get
            {
                return m_googlePlaySenderID;
            }
#if UNITY_EDITOR
            set
            {
                if (m_googlePlaySenderID != value)
                {
                    m_googlePlaySenderID = value;
                    Save();
                }
            }
#endif
        }

#if UNITY_EDITOR
        public static string GetAndroidDebugSignatureMD5()
        {
#if UNITY_EDITOR_WIN
            string homeDir = System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            string javaHome = JavaHome;

            if (string.IsNullOrEmpty(javaHome))
            {
                string error = "JDK path not found. Please make sure that JDK is installed";
                Debug.LogError(error);
                return "<" + error + ">";
            }

            string keytool = javaHome + "\\bin\\keytool.exe";
#else
            string homeDir = System.Environment.GetEnvironmentVariable("HOME");
            string keytool = "keytool";
#endif

            string debugKeystore = Path.Combine(homeDir, ".android/debug.keystore");
            if (!File.Exists(debugKeystore))
            {
                string error = "debug.keystore file not found. Please build an Android version at least once.";
                Debug.LogError(error);
                return "<" + error + ">";
            }

            string keytoolOutput = RunCommand(keytool, "-list -v -alias androiddebugkey -storepass android -keystore " + debugKeystore);
            int index = keytoolOutput.IndexOf("MD5:");
            if (index < 0)
            {
                string message = "Unable to read \"debug.keystore\" file. Look http://stackoverflow.com/questions/8576732/there-is-no-debug-keystore-in-android-folder";
                Debug.LogError(message + "\n" + keytoolOutput);
                return message;
            }
            else
            {
                index += 4;
                while (char.IsWhiteSpace(keytoolOutput[index])) ++index;
                return keytoolOutput.Substring(index, keytoolOutput.IndexOf('\n', index) - index);
            }
        }

        public static string GetAmazonAPIKey()
        {
            string file = Path.Combine(Application.dataPath, "Plugins/Android/assets/api_key.txt");

            if (File.Exists(file))
            {
                return File.ReadAllText(file);
            }
            else
            {
                return "";
            }
        }

        public static void SetAmazonAPIKey(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                string file = Path.Combine(Application.dataPath, "Plugins/Android/assets/api_key.txt");
                
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            else
            {
                string dir = Path.Combine(Application.dataPath, "Plugins/Android/assets");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(dir + "/api_key.txt", value);
            }

            AssetDatabase.Refresh();
        }
#endif

        public bool WindowsDontShowWhenRunning
        {
            get
            {
                return m_windowsDontShowWhenRunning;
            }

#if UNITY_EDITOR
            set
            {
                if (m_windowsDontShowWhenRunning != value)
                {
                    m_windowsDontShowWhenRunning = value;
                    Save();
                }
            }
#endif
        }

#if UNITY_EDITOR
        public string WindowsIdentityName
        {
            get
            {
#if !UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_4_8 && !UNITY_4_9
                return PlayerSettings.WSA.packageName;
#else
                return PlayerSettings.Metro.packageName;
#endif
            }

            set
            {
#if !UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_4_8 && !UNITY_4_9
                PlayerSettings.WSA.packageName = value;
#else
                PlayerSettings.Metro.packageName = value;
#endif
            }
        }

        public string WindowsCertificatePublisher
        {
            get
            {
#if !UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_4_8 && !UNITY_4_9
                return PlayerSettings.WSA.certificateIssuer;
#else
                return PlayerSettings.Metro.certificateIssuer;
#endif
            }
        }

        public bool WindowsCertificateIsCorrect(string publisher)
        {
            //Correct certificate publisher format: 00E3DE9D-D280-4DAF-907B-9DC894310E32
            return System.Text.RegularExpressions.Regex.IsMatch(publisher, "^[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}$");
        }

        public const string WRONG_CERTIFICATE_MESSAGE = "Wrong Windows Store certificate! Please create right one in Unity platform settings or associate app with the store in Visual Studio in order to make push notifications work. For details please read the UTNotifications manual.";
#endif

#if UNITY_EDITOR
        public static string GetAndroidResourceFolder(string resType)
        {
            return Path.Combine(Application.dataPath, "Plugins/Android/UTNotifications/res/" + resType);
        }

        public static string GetIOSResourceFolder()
        {
            return Path.Combine(Application.dataPath, "Plugins/iOS/Raw");
        }

		public static bool FacebookPluginPresent()
		{
			return Directory.Exists(Path.Combine(Application.dataPath, "FacebookSDK"));
		}

        [MenuItem(m_settingsMenuItem)]
        public static void EditSettigns()
        {
            EditorPrefs.SetBool(m_shownEditorPrefKey, true);
            Selection.activeObject = Instance;
        }

        public void Save()
        {
            AndroidManifestManager.Update();
            EditorUtility.SetDirty(this);
        }
#endif
        
        [System.Serializable]
        public struct NotificationProfile
        {
            public string profileName;
            public string iosSound;
            public string androidIcon;
            public string androidLargeIcon;
            public string androidIcon5Plus;
            public string androidSound;
        }

        public enum ShowNotifications
        {
            WHEN_CLOSED_OR_IN_BACKGROUND = 0,
            WHEN_CLOSED = 1,
            ALWAYS = 2
        }

        public enum TokenEncoding
        {
            Base64 = 0,
            HEX = 1
        }

        public enum NotificationsGroupingMode
        {
            /// <summary>
            /// Don't group
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Group by notifications profiles
            /// </summary>
            BY_NOTIFICATION_PROFILES = 1,

            /// <summary>
            /// Use "notification_group" user data value as a grouping key
            /// </summary>
            FROM_USER_DATA = 2,

            /// <summary>
            /// All the app's notifications will belong to a single group
            /// </summary>
            ALL_IN_A_SINGLE_GROUP = 3
        }

    //private
#if UNITY_EDITOR
        private static string RunCommand(string command, string args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            
            string output = process.StandardOutput.ReadToEnd();
            
            process.WaitForExit();

            return output;
        }
#endif

        [SerializeField]
        private List<NotificationProfile> m_notificationProfiles = new List<NotificationProfile>();

        [SerializeField]
        private TokenEncoding m_iOSTokenEncoding = TokenEncoding.HEX;

        [SerializeField]
        private ShowNotifications m_androidShowNotificationsMode = ShowNotifications.WHEN_CLOSED_OR_IN_BACKGROUND;

        [SerializeField]
        private bool m_androidRestoreScheduledNotificationsAfterReboot = true;
        
        [SerializeField]
        private bool m_android4CompatibilityMode = true;

        [SerializeField]
        private NotificationsGroupingMode m_androidNotificationsGrouping = NotificationsGroupingMode.NONE;

		[SerializeField]
		private bool m_androidShowLatestNotificationOnly = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledIOS = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledGooglePlay = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledAmazon = false;

        [SerializeField]
        private bool m_pushNotificationsEnabledWindows = false;

        [SerializeField]
        private string m_googlePlaySenderID = "";

#pragma warning disable 414
        [SerializeField]
        private bool m_windowsDontShowWhenRunning = true;
#pragma warning restore 414

        private const string m_assetName = "UTNotificationsSettings";
        private const string m_settingsMenuItem = "Edit/Project Settings/UTNotifications";
        private static Settings m_instance;

#if UNITY_EDITOR
        private static readonly string m_shownEditorPrefKey = "UTNotificationsSettingsShown." + PlayerSettings.productName;

        [InitializeOnLoad]
        public class SettingsHelper : ScriptableObject
        {
            static SettingsHelper()
            {
                EditorApplication.update += Update;
            }

            private static void Update()
            {
                if (!EditorPrefs.GetBool(m_shownEditorPrefKey, false) &&
                    !File.Exists(Path.Combine(Application.dataPath, "UTNotifications/Resources/" + m_assetName + ".asset")))
                {
                    if (EditorUtility.DisplayDialog("UTNotifications", "Please configure UTNotifications.\nYou can always edit its settings in menu:\n" + m_settingsMenuItem, "Now", "Later"))
                    {
                        EditSettigns();
                    }
                }
                EditorPrefs.SetBool(m_shownEditorPrefKey, true);

                Instance.CheckAndroidPlugin();
                Instance.CheckWSAPlugin();

                EditorApplication.update -= Update;
            }
        }

        private void CheckAndroidPlugin()
        {
            CheckAndroidRes();
            CheckEclipseProjectFiles();
            CheckAndroidManifest();
			CheckSupportLibrary();
			CheckFacebookPlugin();
        }

        private void CheckAndroidRes()
        {
            bool moved = false;

            List<NotificationProfile> profiles = NotificationProfiles;
            if (profiles != null)
            {
                string obsoleteAndroidResPath = Path.Combine(Application.dataPath, "Plugins/Android/res");
                if (Directory.Exists(obsoleteAndroidResPath))
                {
                    foreach (NotificationProfile profile in profiles)
                    {
                        moved |= MoveFilesStartingWith(obsoleteAndroidResPath + "/raw", GetAndroidResourceFolder("raw"), profile.profileName);
                        moved |= MoveFilesStartingWith(obsoleteAndroidResPath + "/drawable", GetAndroidResourceFolder("drawable"), profile.profileName);
                    }
                }
            }

            if (moved)
            {
                AssetDatabase.Refresh();
            }
        }

        private void CheckEclipseProjectFiles()
        {
            CheckEclipseProjectFiles("google-play-services_lib");
            CheckEclipseProjectFiles("UTNotifications");
        }

        private void CheckEclipseProjectFiles(string nativeLibraryName)
        {
            string path = Path.Combine(Application.dataPath, "Plugins/Android/" + nativeLibraryName);
            RemoveEclipseADTPrefix(path, "project");
            RemoveEclipseADTPrefix(path, "classpath");
        }

        private void RemoveEclipseADTPrefix(string path, string extenstion)
        {
            string targetFileName = path + "/." + extenstion;
            if (!File.Exists(targetFileName))
            {
                string originalFileName = path + "/eclipse_adt." + extenstion;
                if (File.Exists(originalFileName))
                {
                    File.Copy(originalFileName, targetFileName);
                }
            }
        }

        private void CheckAndroidManifest()
        {
            string manifestFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            if (!File.Exists(manifestFile) || File.ReadAllText(manifestFile).IndexOf("universal.tools.notifications") < 0)
            {
                AndroidManifestManager.Update();
            }
        }

		private void CheckSupportLibrary()
		{
			string supportLibraryDefaultPath = Path.Combine(Application.dataPath, "Plugins/Android/libs/android-support-v4.jar");
			if (File.Exists(supportLibraryDefaultPath) && File.Exists(GetUTNotificationsAndroidSupportLibraryPath()))
			{
				File.Delete(GetUTNotificationsAndroidSupportLibraryPath());
				AssetDatabase.Refresh();
			}
		}

		private void CheckFacebookPlugin()
		{
			if (FacebookPluginPresent())
			{
				string supportLibraryFacebookPath = Path.Combine(Application.dataPath, "FacebookSDK/Plugins/Android/libs/android-support-v4.jar");
				if (File.Exists(supportLibraryFacebookPath) && File.Exists(GetUTNotificationsAndroidSupportLibraryPath()))
				{
					File.Delete(GetUTNotificationsAndroidSupportLibraryPath());
					AssetDatabase.Refresh();
				}

				Android4CompatibilityMode = false;
			}
		}

		private static string GetUTNotificationsAndroidSupportLibraryPath()
		{
			return Path.Combine(Application.dataPath, "Plugins/Android/UTNotifications/libs/android-support-v4.jar");
		}

        private void CheckWSAPlugin()
        {
            CheckWSAUnprocessed();

            string metroPath = Path.Combine(Application.dataPath, "Plugins/Metro");
            string wsaPath = Path.Combine(Application.dataPath, "Plugins/WSA");
            string rightPath;
            string wrongPath;
#if !UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_4_8 && !UNITY_4_9
            rightPath = wsaPath;
            wrongPath = metroPath;
#else
            rightPath = metroPath;
            wrongPath = wsaPath;
#endif

            if (Directory.Exists(wrongPath))
            {
                if (!Directory.Exists(rightPath))
                {
                    Directory.CreateDirectory(rightPath);
                }

                if (Directory.Exists(wrongPath + "/UTNotifications"))
                {
                    Directory.Move(wrongPath + "/UTNotifications", rightPath + "/UTNotifications");
                }

                string[] filesToMove = Directory.GetFiles(wrongPath, "UTNotifications.*");
                if (filesToMove != null && filesToMove.Length > 0)
                {
                    foreach (string file in filesToMove)
                    {
                        File.Move(file, rightPath + "/" + Path.GetFileName(file));
                    }
                }

                AssetDatabase.Refresh();
            }

            //Delete old bin folder
            if (Directory.Exists(rightPath + "/UTNotifications/bin"))
            {
                Directory.Delete(rightPath + "/UTNotifications/bin", true);
                AssetDatabase.Refresh();
            }
        }

        private void CheckWSAUnprocessed()
        {
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
            string dllName = "UTNotifications.dll";
            if (System.Array.IndexOf(PlayerSettings.Metro.unprocessedPlugins, dllName) < 0)
            {
                List<string> unprocessedPlugins = PlayerSettings.Metro.unprocessedPlugins != null ? new List<string>(PlayerSettings.Metro.unprocessedPlugins) : new List<string>();
                unprocessedPlugins.Add(dllName);
                PlayerSettings.Metro.unprocessedPlugins = unprocessedPlugins.ToArray();
            }
#endif
        }

        private bool MoveFilesStartingWith(string pathFrom, string pathTo, string startsWith)
        {
            bool moved = false;

            if (Directory.Exists(pathFrom))
            {
                string[] files = Directory.GetFiles(pathFrom, startsWith + "*");
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        if (!Directory.Exists(pathTo))
                        {
                            Directory.CreateDirectory(pathTo);
                        }

                        string fullPathTo = pathTo + "/" + Path.GetFileName(file);
                        if (File.Exists(fullPathTo))
                        {
                            File.Delete(fullPathTo);
                        }
                        File.Move(file, fullPathTo);
                        moved = true;
                    }
                }
            }

            return moved;
        }
#endif

#if UNITY_EDITOR_WIN
        private static string m_javaHome;

        private static string JavaHome
        {
            get
            {
                string home = m_javaHome;
                if (home == null)
                {
                    home = System.Environment.GetEnvironmentVariable("JAVA_HOME");
                    if (string.IsNullOrEmpty(home) || !Directory.Exists(home))
                    {
                        home = CheckForJavaHome(Microsoft.Win32.Registry.CurrentUser);
                        if (home == null)
                        {
                            home = CheckForJavaHome(Microsoft.Win32.Registry.LocalMachine);
                        }
                    }
                    
                    if (home != null && !Directory.Exists(home))
                    {
                        home = null;
                    }
                    
                    m_javaHome = home;
                }
                
                return m_javaHome;
            }
        }
        
        private static string CheckForJavaHome(Microsoft.Win32.RegistryKey key)
        {
            using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(@"SOFTWARE\JavaSoft\Java Development Kit"))
            {
                if (subkey == null)
                {
                    return null;
                }
                
                object value = subkey.GetValue("CurrentVersion", null, Microsoft.Win32.RegistryValueOptions.None);
                if (value != null)
                {
                    using (Microsoft.Win32.RegistryKey currentHomeKey = subkey.OpenSubKey(value.ToString()))
                    {
                        if (currentHomeKey == null)
                        {
                            return null;
                        }
                        
                        value = currentHomeKey.GetValue("JavaHome", null, Microsoft.Win32.RegistryValueOptions.None);
                        if (value != null)
                        {
                            return value.ToString();
                        }
                    }
                }
            }
            
            return null;
        }
#endif
    }
}