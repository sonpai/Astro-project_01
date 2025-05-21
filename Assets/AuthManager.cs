using UnityEngine;
using UnityEngine.UI; // Required for UI elements like InputField, Button
using TMPro;          // Required for TextMeshPro elements
using System.Security.Cryptography; // Required for SHA256 hashing
using System.Text;    // Required for Encoding

public class AuthManager : MonoBehaviour
{
    /// <summary>
    /// UI button for starting a new game from the main menu.
    /// </summary>
    [Header("Main Menu UI")]
    [SerializeField] private Button newGameButtonMainMenu;

    /// <summary>
    /// UI button for loading an existing game from the main menu.
    /// </summary>
    [SerializeField] private Button loadGameButtonMainMenu;

    [Header("Registration Panel UI")]
    /// <summary>
    /// GameObject representing the registration panel.
    /// </summary>
    [SerializeField] private GameObject registrationPanel;

    /// <summary>
    /// Input field for entering the email address during registration.
    /// </summary>
    [SerializeField] private TMP_InputField registerEmailInput;

    /// <summary>
    /// Input field for entering the password during registration.
    /// </summary>
    [SerializeField] private TMP_InputField registerPasswordInput;

    /// <summary>
    /// UI button for submitting the registration form.
    /// </summary>
    [SerializeField] private Button registerButton;

    /// <summary>
    /// UI button for navigating back to the main menu from the registration panel.
    /// </summary>
    [SerializeField] private Button backButtonReg;

    /// <summary>
    /// Text element for displaying messages during registration.
    /// </summary>
    [SerializeField] private TextMeshProUGUI registerMessageText;

    [Header("Login Panel UI")]
    /// <summary>
    /// GameObject representing the login panel.
    /// </summary>
    [SerializeField] private GameObject loginPanel;

    /// <summary>
    /// Input field for entering the email address during login.
    /// </summary>
    [SerializeField] private TMP_InputField loginEmailInput;

    /// <summary>
    /// Input field for entering the password during login.
    /// </summary>
    [SerializeField] private TMP_InputField loginPasswordInput;

    /// <summary>
    /// UI button for submitting the login form.
    /// </summary>
    [SerializeField] private Button loginButton;

    /// <summary>
    /// UI button for navigating back to the main menu from the login panel.
    /// </summary>
    [SerializeField] private Button backButtonLogin;

    /// <summary>
    /// Text element for displaying messages during login.
    /// </summary>
    [SerializeField] private TextMeshProUGUI loginMessageText;

    /// <summary>
    /// Prefix used to store hashed passwords in PlayerPrefs.
    /// </summary>
    private const string UserPasswordKeyPrefix = "UserPwd_";

    /// <summary>
    /// Initializes the UI and sets up button listeners.
    /// </summary>
    private void Start()
    {
        registrationPanel.SetActive(false);
        loginPanel.SetActive(false);

        // Setup button listeners with null checks.
        newGameButtonMainMenu?.onClick.AddListener(ShowRegistrationPanel);
        loadGameButtonMainMenu?.onClick.AddListener(ShowLoginPanel);

        registerButton?.onClick.AddListener(HandleRegistration);
        backButtonReg?.onClick.AddListener(HideAllPanels);

        loginButton?.onClick.AddListener(HandleLogin);
        backButtonLogin?.onClick.AddListener(HideAllPanels);
    }

    /// <summary>
    /// Displays the registration panel and clears any messages.
    /// </summary>
    public void ShowRegistrationPanel()
    {
        HideAllPanels();
        registrationPanel.SetActive(true);
        registerMessageText.text = "";
    }

    /// <summary>
    /// Displays the login panel and clears any messages.
    /// </summary>
    public void ShowLoginPanel()
    {
        HideAllPanels();
        loginPanel.SetActive(true);
        loginMessageText.text = "";
    }

    /// <summary>
    /// Hides all authentication-related panels.
    /// </summary>
    public void HideAllPanels()
    {
        registrationPanel.SetActive(false);
        loginPanel.SetActive(false);
    }

    /// <summary>
    /// Handles user registration by validating input, checking for duplicates, and storing hashed credentials.
    /// </summary>
    private void HandleRegistration()
    {
        string email = registerEmailInput.text.Trim();
        string password = registerPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            registerMessageText.text = "Error: Email and password cannot be empty.";
            registerMessageText.color = Color.red;
            return;
        }

        if (!IsValidEmail(email))
        {
            registerMessageText.text = "Error: Invalid email format.";
            registerMessageText.color = Color.red;
            return;
        }

        string passwordKey = UserPasswordKeyPrefix + email;

        if (PlayerPrefs.HasKey(passwordKey))
        {
            registerMessageText.text = "Error: This email is already registered.";
            registerMessageText.color = Color.red;
            return;
        }

        string hashedPassword = HashPassword(password);

        PlayerPrefs.SetString(passwordKey, hashedPassword);
        PlayerPrefs.Save();

        registerMessageText.text = "Registration successful! You can now log in.";
        registerMessageText.color = Color.green;

        registerEmailInput.text = "";
        registerPasswordInput.text = "";
    }

    /// <summary>
    /// Handles user login by validating input and verifying credentials.
    /// </summary>
    private void HandleLogin()
    {
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginMessageText.text = "Error: Email and password cannot be empty.";
            loginMessageText.color = Color.red;
            return;
        }

        string passwordKey = UserPasswordKeyPrefix + email;

        if (!PlayerPrefs.HasKey(passwordKey))
        {
            loginMessageText.text = "Error: Email not found or incorrect credentials.";
            loginMessageText.color = Color.red;
            return;
        }

        string storedHashedPassword = PlayerPrefs.GetString(passwordKey);

        if (VerifyPassword(password, storedHashedPassword))
        {
            loginMessageText.text = "Login successful! Access granted.";
            loginMessageText.color = Color.green;
            GrantGameAccess(email);
        }
        else
        {
            loginMessageText.text = "Error: Email not found or incorrect credentials.";
            loginMessageText.color = Color.red;
        }
    }

    /// <summary>
    /// Grants the user access to the game upon successful login.
    /// </summary>
    private void GrantGameAccess(string userEmail)
    {
        Debug.Log($"Game access granted to user: {userEmail}. Implement game loading logic here!");
    }

    /// <summary>
    /// Hashes the provided password using SHA256.
    /// </summary>
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
            return System.BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    /// <summary>
    /// Verifies if the entered password matches the stored hash.
    /// </summary>
    private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
    {
        return HashPassword(enteredPassword) == storedHashedPassword;
    }

    /// <summary>
    /// Validates an email address using basic checks.
    /// </summary>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
