using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    private enum SceneMode
    {
        Login,
        Register
    }

    private SceneMode _mode = SceneMode.Login;

    [SerializeField] TextMeshProUGUI _messageTextUI;
    [SerializeField] GameObject _passwordConfirmObject;
    [SerializeField] Button _gotoRegisterButton;
    [SerializeField] Button _loginButton;
    [SerializeField] Button _gotoLoginButton;
    [SerializeField] Button _registerButton;

    [SerializeField] TMP_InputField _idInputField;
    [SerializeField] TMP_InputField _passwordInputField;
    [SerializeField] TMP_InputField _passwordConfirmInputField;

    private string _idPattern = @"^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,}$";
    private string _passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_])[a-zA-Z0-9\W_]{7,20}$";
    private void Start()
    {
        Refresh();
        AddButtonEvents();
        CheckLastID();
    }

    private void CheckLastID()
    {
        string lastID = PlayerPrefs.GetString("lastID");

        if (string.IsNullOrEmpty(lastID))
        {
            return;
        }
        else
        {
            _idInputField.text = lastID;
        }
    }

    private void AddButtonEvents()
    {
        _gotoRegisterButton.onClick.AddListener(GotoRegister);
        _loginButton.onClick.AddListener(Login);
        _gotoLoginButton.onClick.AddListener(GotoLogin);
        _registerButton.onClick.AddListener(Register);
    }
    private void Refresh()
    {
        // 2차 비밀번호 오브젝트는 회원가입 모드일때만 노출
        _passwordConfirmObject.SetActive(_mode == SceneMode.Register);
        _gotoRegisterButton.gameObject.SetActive(_mode == SceneMode.Login);
        _loginButton.gameObject.SetActive(_mode == SceneMode.Login);
        _gotoLoginButton.gameObject.SetActive(_mode == SceneMode.Register);
        _registerButton.gameObject.SetActive(_mode == SceneMode.Register);
    }

    private void Login()
    {
        // 입력이 되어있는지 확인
        string id = _idInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }
        if (!Regex.IsMatch(id, _idPattern))
        {
            _messageTextUI.text = "이메일 형식이 아닙니다.";
            return;
        }
        string password = _passwordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "비밀번호를 입력해주세요.";
            return;
        }

        // 실제 저장된 아이디 비밀번호 계정이 있는 지 확인
        // 먼저 아이디가 있는지 확인한다.
        if (!PlayerPrefs.HasKey(id))
        {
            _messageTextUI.text = "아이디 혹은 비밀번호가 틀립니다.";
            return;
        }
        if (PlayerPrefs.GetString(id) != password)
        {
            _messageTextUI.text = "비밀번호가 틀립니다.";
            return;
        }
        PlayerPrefs.SetString("lastID", id);
        // 동기 방식 (SceneManager.LoadScene)
        SceneManager.LoadScene("LoadingScene");
    }

    private void Register()
    {
        string id = _idInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }
        if (!Regex.IsMatch(id, _idPattern))
        {
            _messageTextUI.text = "이메일 형식이 아닙니다.";
            return;
        }
        string password = _passwordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "비밀번호를 입력해주세요.";
            return;
        }
        if (!Regex.IsMatch(password, _passwordPattern))
        {
            _messageTextUI.text = "비밀번호 형식을 지켜주세요.";
            return;
        }
        string password2 = _passwordConfirmInputField.text;
        if (string.IsNullOrEmpty(password2) || password != password2)
        {
            _messageTextUI.text = "비밀번호를 확인해주세요.";
            return;
        }

        // 실제 저장된 아이디 비밀번호 계정이 있는 지 확인
        // 먼저 아이디가 있는지 확인한다.
        if (PlayerPrefs.HasKey(id))
        {
            _messageTextUI.text = "중복된 아이디입니다.";
            return;
        }
        PlayerPrefs.SetString(id, password);
        _messageTextUI.text = "회원가입이 완료되었습니다.";
        GotoLogin();
    }

    private void GotoLogin()
    {
        _mode = SceneMode.Login;
        Refresh();
    }

    private void GotoRegister()
    {
        _mode = SceneMode.Register;
        Refresh();
    }
}
