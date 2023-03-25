using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FAuth : SingletonComponent<FAuth>
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [ContextMenu("SingOut")]
    public void SignOut()
    {
        auth.SignOut();
        user = null;
        DataManager.Instance.SignOut();
    }

    #region Properties
    public System.Action<string> OnFAuthLoginFailed;
    public System.Action<string> OnFAuthLoginSucceeded;
    #endregion

    #region Unity Methods
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                UIManager.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    
    #endregion

    #region Private Methods

    private void InitializeFirebase()
    {
        UIManager.LogError("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        //AuthStateChanged(this, null);
    }

    

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signIn)
            {
                if (OnFAuthLoginFailed != null)
                {
                    OnFAuthLoginFailed("Signed Out");
                    
                }
                return;
            }

            user = auth.CurrentUser;
            if (signIn)
            {
                Debug.LogErrorFormat("Signed in {0}", user.UserId);
                if (OnFAuthLoginSucceeded != null)
                {
                    OnFAuthLoginSucceeded(user.UserId);
                }
                
            }
        }

        if (auth.CurrentUser == null && user == null)
        {
            if (OnFAuthLoginFailed != null)
            {
                OnFAuthLoginFailed("Signed Out");
            }
        }
    }

    private IEnumerator CreateUserWithEmailAndPassword(string username, string email, string password, System.Action<bool, string, string> callback)
    {
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
            callback(false, "Register Failed:" + GetErrorMessage(firebaseException), "");
        }
        else
        {
            FirebaseUser newUser = registerTask.Result;
            callback(true, string.Format("Firebase user created successfully: {0}", newUser.UserId), newUser.UserId);
        }
    }

    private IEnumerator SignInWithEmailAndPasswordAsync(string email, string password, System.Action<bool, string, string> callback)
    {
        var signInTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => signInTask.IsCompleted);

        if (signInTask.Exception != null)
        {
            FirebaseException firebaseException = signInTask.Exception.GetBaseException() as FirebaseException;
            callback(false, "Sign Failed:" + GetErrorMessage(firebaseException), "");
        }
        else
        {
            FirebaseUser newUser = signInTask.Result;
            callback(true, string.Format("Firebase user signed in successfully: {0}, {1}", newUser.DisplayName, newUser.UserId), newUser.UserId);
        }
    }

    private IEnumerator SendPasswordResetEmailAsync(string email, System.Action<bool, string> callback)
    {
        var resetTask = auth.SendPasswordResetEmailAsync(email);
        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.Exception != null)
        {
            callback(false, "SendPasswordResetEmailAsync encountered an error: " + resetTask.Exception);
        }
        else
        {
            callback(true, "Password reset email sent successfully.");
        }

    }

    private string GetErrorMessage(FirebaseException exception)
    {
        AuthError errorCode = (AuthError)exception.ErrorCode;
        return GetErrorMessage(errorCode);
    }

    private string GetErrorMessage(AuthError errorCode)
    {
        var result = "";
        switch (errorCode)
        {
            case AuthError.MissingPassword:
                result =  "Missing password.";
                break;
            case AuthError.WeakPassword:
                result = "Too weak of a password.";
                break;
            case AuthError.InvalidEmail:
                result = "Invalid email.";
                break;
            case AuthError.MissingEmail:
                result = "Missing email.";
                break;
            case AuthError.UserNotFound:
                result = "Account not found.";
                break;
            case AuthError.EmailAlreadyInUse:
                result = "Email already in use.";
                break;
            default:
                result = "Unknown error occurred.";
                break;
        }

        return result;
    }
    #endregion

    #region Public_Memebers
    public void Register(string username, string email, string password, System.Action<bool, string, string> callback)
    {
        StartCoroutine(CreateUserWithEmailAndPassword(username, email, password, callback));
    }

    public void SignIn(string email, string password, System.Action<bool, string, string> callback)
    {
        StartCoroutine(SignInWithEmailAndPasswordAsync(email, password, callback));
    }

    public void SendPasswordResetEmail(string email, System.Action<bool, string> callback)
    {
        StartCoroutine(SendPasswordResetEmailAsync(email, callback));
    }
    #endregion
}
