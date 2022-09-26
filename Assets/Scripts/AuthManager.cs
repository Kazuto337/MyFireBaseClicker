using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    static AuthManager _instance;

    [Header("References")]
    [SerializeField] GameObject checking4AccountUI;
    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject registerUI;
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject verifyEmailUI;
    [SerializeField] Text verifyEmailText;
    public static AuthManager instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else _instance = this;
    }
    public void ClearUI()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        titleUI.SetActive(false);
        verifyEmailUI.SetActive(false);
        FireBaseManager.instance.ClearOutputs();
    }
    public void LoginScreen()
    {
        ClearUI();
        loginUI.SetActive(true);
    }
    public void RegisterScreen()
    {
        ClearUI();
        registerUI.SetActive(true);
    }
    public void TitleScreen()
    {
        ClearUI();
        titleUI.SetActive(true);
    }

    public void AwaitVerification(bool _emailSent, string _email, string _output)
    {
        ClearUI();
        verifyEmailUI.SetActive(true);

        if (_emailSent)
        {
            verifyEmailText.text = $"Email Sent\n Please Verify {_email} ";
        }
        else
        {
            verifyEmailText.text = $"Email not Sent : {_output}\n Please Verify {_email} ";
        }
    }
}
