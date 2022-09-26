using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class ResetPassword : MonoBehaviour
{
    [SerializeField] InputField emailField;
    [SerializeField] Text output;

    public void ResetPasswordButton()
    {
        string email = emailField.text;
        StartCoroutine(SendEmail(email));
    }

    IEnumerator SendEmail(string email)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        var resetTask = auth.SendPasswordResetEmailAsync(email);

        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.Exception != null)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + resetTask.Exception);

        }
        else
        {
            output.text = "Password reset email sent successfully";
            Debug.Log("Password reset email sent successfully.");
        }
    }

    public void ResetOutput()
    {
        output.text = "";
    }
}