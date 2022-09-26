using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class FireBaseManager : MonoBehaviour
{
    [Header("FireBase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference dbReference;
    [Space(5f)]

    [Header("Login References")]
    [SerializeField] Text loginEmail;
    [SerializeField] Text loginPassword;
    [SerializeField] Text loginOutputText;
    [Space(5f)]

    [Header("Register References")]
    [SerializeField] Text registerUsername;
    [SerializeField] Text registerEmail;
    [SerializeField] Text registerPassword;
    [SerializeField] Text registerConfirmPassword;
    [SerializeField] Text registerOutputText;
    [Space(5f)]
    static FireBaseManager _instance;
    public static FireBaseManager instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance != null && _instance != this)
        {
            Destroy(instance.gameObject);
        }
        else _instance = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        
    }

    private void Start()
    {
        
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed Out");
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"Signed In: {user.DisplayName}");
            }
        }

    }

    public void ClearOutputs()
    {
        loginOutputText.text = "";
        registerOutputText.text = "";
    }

    public void LogInButton()
    {
        StartCoroutine(Login(loginEmail.text, loginPassword.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(registerEmail.text, registerPassword.text, registerConfirmPassword.text, registerUsername.text));
    }

    public void LogOutButton()
    {
        auth.SignOut();
        GameManager.instance.ChangeScene(0);
        ClearOutputs();
    }

    IEnumerator Login(string _email, string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);
        var loginTask = auth.SignInWithCredentialAsync(credential); //Saves Login Task on a variable

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;

            string output = "Unknown Error. Please Try Again";

            switch (error)//In case of any error
            {
                case AuthError.MissingEmail:
                    output = "Please Enter Your E-mail";
                    break;
                case AuthError.MissingPassword:
                    output = "Please Enter Your Password";
                    break;
                case AuthError.InvalidEmail:
                    output = "Invalid Email";
                    break;
                case AuthError.WrongPassword:
                    output = "Incorret Password. Please Try Again";
                    break;
                case AuthError.UserNotFound:
                    output = "User Does Not Exist";
                    break;
            }
            loginOutputText.text = output;
        }
        else //Succesful Login
        {
            if (user.IsEmailVerified)
            {
                loginOutputText.text = "";
                GameManager.instance.ChangeScene(1);
            }
            else
            {
                loginOutputText.text = "";
                GameManager.instance.ChangeScene(1);
            }
        }
    }

    IEnumerator Register(string _email, string _password, string _confirmPassword, string _username)
    {
        if (_username == "")
        {
            registerOutputText.text = "Please Enter A Username";
        }
        else if (_password != _confirmPassword)
        {
            registerOutputText.text = "Passwords Are Not The Same. Please Confirm Password";
        }
        else if (_password == "")
        {
            registerOutputText.text = "Please Enter A Password";
        }
        else if (_confirmPassword == "")
        {
            registerOutputText.text = "Please Confirm Your Password";
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password); // Save Register task in a variable
            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "Unknown Error. Please Try Again";

                switch (error)//In case of any error
                {
                    case AuthError.EmailAlreadyInUse:
                        output = "This Email Is Already in Use";
                        break;
                    case AuthError.MissingPassword:
                        output = "Please Enter Your Password";
                        break;
                    case AuthError.InvalidEmail:
                        output = "Invalid Email";
                        break;
                    case AuthError.WeakPassword:
                        output = "Weak Password! Please Try Another One";
                        break;
                    case AuthError.MissingEmail:
                        output = "Please Enter Your E-mail";
                        break;
                }
                registerOutputText.text = output;
            }
            else
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = _username,
                };
                var defaultUserTask = user.UpdateUserProfileAsync(profile);
                //Wait until the task completes
                yield return new WaitUntil(predicate: () => defaultUserTask.IsCompleted);

                if (defaultUserTask.Exception != null)
                {
                    user.DeleteAsync();
                    FirebaseException firebaseException = (FirebaseException)defaultUserTask.Exception.GetBaseException();
                    AuthError error = (AuthError)firebaseException.ErrorCode;

                    string output = "Unknown Error. Please Try Again";
                    switch (error)
                    {
                        case AuthError.Cancelled:
                            output = "Update User Cancelled";
                            break;
                        case AuthError.SessionExpired:
                            output = "Session Expired";
                            break;
                    }
                    registerOutputText.text = output;
                }
                else
                {
                    Debug.Log($"Firebase User Created Succesfully: {user.DisplayName} ({user.UserId})");
                    AuthManager.instance.LoginScreen();
                }

            }
        }
    }
}
