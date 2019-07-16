using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Google;


public class AuthenticationManager : MonoBehaviour
{
    public InputField LoginEmail, LoginPassword, RegUserName, RegEmail, RegPassword;

    public GameObject SignupPanel, LoginPanel;

    public Button SignupPanelButton, LoginPanelButton;

    public Text SignupPanelText, LoginPanelText, ErrorMessage;

    public GameObject LoadingPanel;

    string EmptyMessage = string.Empty;

    public Color ActiveColor = new Color(255, 255, 255, 255);
    public Color InActiveColor = new Color(100, 100, 100, 255);

    public Button GoogleLoginButton;

    public Button FacebookLoginButton;
    public Button FacebookRegisterButton;
    GoogleSignInStatusCode LoginStatus;
    public Button GoogleRegisterButton;

    protected FirebaseAuth auth;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    public void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
                auth = FirebaseAuth.DefaultInstance;
            else Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        });

        GoogleLoginButton.onClick.AddListener(GoogleLogIn);
        GoogleRegisterButton.onClick.AddListener(GoogleLogIn);

        //FacebookLoginButton.onClick.AddListener(FacebookLogin);
        //FacebookRegisterButton.onClick.AddListener(FacebookLogin);

        SignupPanelButton.onClick.AddListener(OnSignupPanel);
        LoginPanelButton.onClick.AddListener(OnLoginPanel);


        auth = FirebaseAuth.DefaultInstance;

    }

    void GoogleLogIn()
    {
        LoadingPanel.SetActive(true);

        Firebase.Analytics.FirebaseAnalytics
        .LogEvent("Google Login Button Press", "Google Login", 1f);

        Firebase.Analytics.FirebaseAnalytics.LogEvent("session_start", "Logged In", "Google Login Done");

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();


        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task => {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                DisplayMessage(2, "GoogleLogin Cancelled");
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
                DisplayMessage(2, "GoogleLogin Cancelled");
            }
            else
            {

                Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask => {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                        LoadingPanel.SetActive(false);
                    }
                    else if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                        LoadingPanel.SetActive(false);
                    }
                    else
                    {
                        signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                        Debug.Log(auth.CurrentUser.Email);
                        string GoogleEmailId = auth.CurrentUser.Email;
                        Debug.Log("Google Email Id: " + GoogleEmailId);
                        DisplayMessage(2, "GoogleLogin SuccessFull");
                        LoadingPanel.SetActive(false);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("FaceTrackingMesh");
                    }
                });
            }
        });
    }

    public void Signup()
    {
        string email = RegEmail.text;
        string password = RegPassword.text;

        LoadingPanel.SetActive(true);
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            DisplayMessage(3, "Please Enter Valid Credentials");
            LoadingPanel.SetActive(false);
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {

            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                LoadingPanel.SetActive(false);
                DisplayMessage(2, "Unable to create new user");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync error: " + task.Exception);
                LoadingPanel.SetActive(false);
                DisplayMessage(2, "Unable to create new user");
                if (task.Exception.InnerExceptions.Count > 0)
                    Debug.Log(task.Exception.InnerExceptions[0].Message);
                return;
            }

            FirebaseUser newUser = task.Result; // Firebase user has been created.

            newUser.SendEmailVerificationAsync().ContinueWith(t =>
            {
                Debug.Log("Confirmation Mail Sent");
                LoadingPanel.SetActive(false);
                DisplayMessage(2, "Confirmation Mail sent");
            });
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            Debug.Log("Signup Success");
            OnLoginPanel();
        });
    }

    public void Login()
    {

        string email = LoginEmail.text;
        string password = LoginPassword.text;

        LoadingPanel.SetActive(true);

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync canceled.");
                LoadingPanel.SetActive(false);
                DisplayMessage(2, "Unable to Login");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync error: " + task.Exception);
                LoadingPanel.SetActive(false);
                DisplayMessage(2, "Unable to Login");
                if (task.Exception.InnerExceptions.Count > 0)
                    Debug.Log(task.Exception.InnerExceptions[0].Message);
                return;
            }
            
            FirebaseUser user = task.Result;
            if (user.IsEmailVerified)
            {
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    user.DisplayName, user.UserId); LoadingPanel.SetActive(false);
                DisplayMessage(2, "Welcome " + user.DisplayName);
                LoadingPanel.SetActive(false);
                UnityEngine.SceneManagement.SceneManager.LoadScene("FaceTrackingMesh");
            }
            else
            {
                auth.SignOut();
            }
        });
    }

    public void ShowHidePassword(InputField Password)
    {
        if (Password.contentType == InputField.ContentType.Password)
        {
            string temp = Password.text;
            Password.text = string.Empty;
            Password.contentType = InputField.ContentType.Standard;
            Password.text = temp;
        }
        else
        {
            string temp = Password.text;
            Password.text = string.Empty;
            Password.contentType = InputField.ContentType.Password;
            Password.text = temp;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ErrorMessage.text = EmptyMessage;
        LoadingPanel.SetActive(false);
        //GoogleSignIn.DefaultInstance.SignOut();
        OnLoginPanel();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLoginPanel()
    {
        Debug.Log("Login Panel Active");
        LoginPanelButton.GetComponentInChildren<Text>().color = ActiveColor;
        LoginPanelText.color = ActiveColor;
        SignupPanelText.color = InActiveColor;
        SignupPanelButton.GetComponentInChildren<Text>().color = InActiveColor;
        LoginPanel.SetActive(true);
        SignupPanel.SetActive(false);
    }

    void OnSignupPanel()
    {
        LoginPanelButton.GetComponentInChildren<Text>().color = InActiveColor;
        LoginPanelText.color = InActiveColor;
        SignupPanelText.color = ActiveColor;
        SignupPanelButton.GetComponentInChildren<Text>().color = ActiveColor;
        LoginPanel.SetActive(false);
        SignupPanel.SetActive(true);
    }

    void DisplayMessage(int TimeToFade, string DisplayMessageText)
    {
        ErrorMessage.text = DisplayMessageText;
        WaitFor(TimeToFade);
        ErrorMessage.text = EmptyMessage;
    }

    IEnumerator WaitFor(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
