using Newtonsoft.Json;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreatorWizard
{
    public class ProfileManager : MonoBehaviour
    {
        private const string TAG = nameof(ProfileManager);
        private const string DIRECTORY_NAME = "Ready Player Me";
        private const string FILE_NAME = "User";

        [SerializeField] private ProfileUI profileUI;

        private string filePath;
        private string directoryPath;
        private string lastModifiedAvatarId;

        private void Awake()
        {
            directoryPath = $"{Application.persistentDataPath}/{DIRECTORY_NAME}";
            filePath = $"{directoryPath}/{FILE_NAME}";
        }

        private void OnEnable()
        {
            profileUI.SignedOut += AuthManager.Logout;
            AuthManager.OnSignedOut += DeleteSession;
        }

        private void OnDisable()
        {
            SaveSession(AuthManager.UserSession);
            profileUI.SignedOut -= AuthManager.Logout;
            AuthManager.OnSignedOut -= DeleteSession;
        }

        public async Task LoadSession(CancellationToken cancellationToken)
        {
            if (!File.Exists(filePath))
            {
                await AuthManager.LoginAsAnonymous(cancellationToken);
                SetProfileData(AuthManager.UserSession);

                SDKLogger.Log(TAG, $"Session started as anonymous and saved in {filePath}");
                return;
            }

            var bytes = File.ReadAllBytes(filePath);
            var json = Encoding.UTF8.GetString(bytes);
            var userSession = JsonConvert.DeserializeObject<UserSession>(json);
            AuthManager.SetUser(userSession);

            SetProfileData(userSession);

            SDKLogger.Log(TAG, $"Loaded session from {filePath}");
        }

        public void SaveSession(UserSession userSession)
        {
            var json = JsonConvert.SerializeObject(userSession);
            DirectoryUtility.ValidateDirectory(directoryPath);
            File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(json));
            SetProfileData(userSession);

            SDKLogger.Log(TAG, $"Saved session to {filePath}");
        }

        private void SetProfileData(UserSession userSession)
        {
            if (string.IsNullOrEmpty(userSession.Name))
            {
                userSession.Name = userSession.Id;
            }
            profileUI.SetProfileData(
                userSession.Name,
                char.ToUpperInvariant(userSession.Name[0]).ToString()
            );
        }

        private void DeleteSession()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            profileUI.ClearProfile();

            SDKLogger.Log(TAG, $"Deleted session at {filePath}");
        }
    }
}
