using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
//using NUnit.Framework;

public class LoginUsers : MonoBehaviour
{

    public class OverUserModel
    {
        public UserModel user { get; set; }
        public string access_token { get; set; }
    }

    public class UserModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string mail { get; set; }
        public bool darkmode { get; set; }
        public bool isAdmin { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class TextModel
    {
        public int id { get; set; }
        public string content { get; set; }
        public string mail { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int? userId { get; set; }
    }

    public static string content;
    public static OverUserModel contentUser = null;
    public static string access_token = "";
    public static string username = "";
    public static Boolean Dark = false;

    public static string DbData = "f";
    public static OverUserModel getUser() { return contentUser; }
    public static void setUser(OverUserModel user) { contentUser = user; }
    public static string getUsername() { return username; }
    public static void setUsername(string usern) { username = usern; }
    public static Boolean getDark() { return Dark; }
    public static void setDark(Boolean valueDark) { Dark = valueDark; }
    public static string getToken() { return access_token; }


    public GameObject form_login, txt_username, txt_mail, btn_settings, btn_login, btn_logout, btn_admin, login_error,
    form_register, register_error, contact_window, contact2_window, contact_success, password_window, password_error, user_settings, info,
    email_window, email_error, delete_window, main_menu_white, main_menu_black, ip_input;

    public static LoginUsers instance;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        closeWindows();
        contentUser = LoginUsers2.getUser();
        if (contentUser == null)
        {
            contentUser = Crud.getUser();
        }
        if (contentUser != null)
        {
            access_token = contentUser.access_token;
            Dark = contentUser.user.darkmode;
        }
        else
        {
            access_token = "";
        }
        isLoggedin();
        settingDarkness();
    }

    public void closeWindows()
    {
        form_login.SetActive(false);
        login_error.SetActive(false);
        form_register.SetActive(false);
        register_error.SetActive(false);
        contact_window.SetActive(false);
        contact2_window.SetActive(false);
        contact_success.SetActive(false);
        user_settings.SetActive(false);
        info.SetActive(false);
        password_window.SetActive(false);
        password_error.SetActive(false);
        email_window.SetActive(false);
        email_error.SetActive(false);
        delete_window.SetActive(false);
    }

    public static String ip = "localhost";
    public void changeIp()
    {
        ip = ip_input.GetComponent<TMP_InputField>().text;
        Debug.Log(ip);
    }

    public static String getIp()
    {
        return ip;
    }

    public void contact()
    {
        if (access_token != "")
        {
            contact2_window.SetActive(true);
        }
        else
        {
            contact_window.SetActive(true);
        }
    }

    public void changePassword()
    {
        StartCoroutine(changePasswordI());
    }
    IEnumerator changePasswordI()
    {
        if (password_window.transform.GetChild(1).GetComponent<InputField>().text == "")
        {
            password_error.SetActive(true);
            password_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Missing the old password";
        }
        else if (password_window.transform.GetChild(1).GetComponent<InputField>().text != contentUser.user.password)
        {
            password_error.SetActive(true);
            password_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Wrong old password";
        }
        else if (password_window.transform.GetChild(2).GetComponent<InputField>().text == "")
        {
            password_error.SetActive(true);
            password_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Enter a new password";
        }
        else if (password_window.transform.GetChild(2).GetComponent<InputField>().text != password_window.transform.GetChild(3).GetComponent<InputField>().text)
        {
            password_error.SetActive(true);
            password_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Passwords don't match";
        }
        else if (password_window.transform.GetChild(2).GetComponent<InputField>().text == contentUser.user.password)
        {
            password_error.SetActive(true);
            password_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "That isn't a new password";
        }
        else
        {
            var userUpdate = new UserModel();
            userUpdate.id = contentUser.user.id;
            userUpdate.username = contentUser.user.username;
            userUpdate.password = password_window.transform.GetChild(2).GetComponent<InputField>().text;
            userUpdate.mail = contentUser.user.mail;
            userUpdate.isAdmin = contentUser.user.isAdmin;
            userUpdate.darkmode = contentUser.user.darkmode;
            var json = JsonConvert.SerializeObject(userUpdate);
            //var updateData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var byteArray = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = UnityWebRequest.Put("http://localhost:4000/api/users/" + userUpdate.id, byteArray);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + access_token);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            password_window.transform.GetChild(1).GetComponent<InputField>().text = "";
            password_window.transform.GetChild(2).GetComponent<InputField>().text = "";
            password_window.transform.GetChild(3).GetComponent<InputField>().text = "";
            password_error.SetActive(true);
            password_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "The password was changed";
            contentUser.user.password = userUpdate.password;
            userUpdate = null;
        }
    }

    public void changeEmail()
    {
        StartCoroutine(changeEmailI());
    }
    IEnumerator changeEmailI()
    {
        if (email_window.transform.GetChild(2).GetComponent<InputField>().text == "")
        {
            email_error.SetActive(true);
            email_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Enter a new email";
        }
        else if (email_window.transform.GetChild(2).GetComponent<InputField>().text == contentUser.user.mail)
        {
            email_error.SetActive(true);
            email_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "That isn't a new email";
        }
        else
        {
            var userUpdate = new UserModel();
            userUpdate.id = contentUser.user.id;
            userUpdate.username = contentUser.user.username;
            userUpdate.password = contentUser.user.password;
            userUpdate.mail = email_window.transform.GetChild(2).GetComponent<InputField>().text;
            userUpdate.isAdmin = contentUser.user.isAdmin;
            userUpdate.darkmode = contentUser.user.darkmode;
            var json = JsonConvert.SerializeObject(userUpdate);
            //var updateData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var byteArray = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = UnityWebRequest.Put("http://localhost:4000/api/users/" + userUpdate.id, byteArray);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + access_token);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            email_window.transform.GetChild(1).GetComponent<InputField>().text = userUpdate.mail;
            email_window.transform.GetChild(2).GetComponent<InputField>().text = "";
            email_error.SetActive(true);
            email_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "The email was changed";
            contentUser.user.mail = userUpdate.mail;
            txt_mail.GetComponent<Text>().text = contentUser.user.mail;
            userUpdate = null;
        }
    }

    public void deleteUser()
    {
        StartCoroutine(deleteUserI());
    }
    IEnumerator deleteUserI()
    {
        UnityWebRequest request = UnityWebRequest.Delete("http://localhost:4000/api/users/" + contentUser.user.id);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + access_token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        logout();
        closeWindows();
    }
    public void sendMessage()
    {
        StartCoroutine(sendMessageI());
    }
    IEnumerator sendMessageI()
    {
        if (contact2_window.transform.GetChild(1).GetComponent<InputField>().text == "")
        {
            contact_success.SetActive(true);
            contact_success.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "You didn't write anything";
        }
        else
        {
            var text = new TextModel();
            text.content = contact2_window.transform.GetChild(1).GetComponent<InputField>().text;
            text.mail = contentUser.user.mail;
            text.userId = contentUser.user.id;
            WWWForm form = new WWWForm();
            form.AddField("content", text.content);
            form.AddField("mail", text.mail);
            form.AddField("userId", text.userId.ToString());
            /*using var client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync("http://localhost:4000/api/texts", data);*/
            UnityWebRequest request = UnityWebRequest.Post("http://localhost:4000/api/texts", form);
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            //Debug.Log(request.downloadHandler.text);

            contact2_window.transform.GetChild(1).GetComponent<InputField>().text = "";

            contact_success.SetActive(true);
            contact_success.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "The message was sent!";
        }

    }
    public void login()
    {
        if (form_login.transform.GetChild(1).GetComponent<InputField>().text == "")
        {
            login_error.SetActive(true);
            login_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "You didn't enter the username";
        }
        else if (form_login.transform.GetChild(2).GetComponent<InputField>().text == "")
        {
            login_error.SetActive(true);
            login_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "You didn't enter the password";
        }
        else
        {
            var username = form_login.transform.GetChild(1).GetComponent<InputField>().text;
            var password = form_login.transform.GetChild(2).GetComponent<InputField>().text;
            StartCoroutine(loginI(username, password));

            if (DbData.StartsWith("{\"user\":{\"id\""))
            {
                login_error.SetActive(false);
                contentUser = JsonConvert.DeserializeObject<OverUserModel>(DbData);
                //Debug.Log(contentUser.access_token);
                contentUser.user.password = form_login.transform.GetChild(2).GetComponent<InputField>().text;
                form_login.transform.GetChild(1).GetComponent<InputField>().text = "";
                form_login.transform.GetChild(2).GetComponent<InputField>().text = "";
                form_login.SetActive(false);
                access_token = contentUser.access_token;
                setUser(contentUser);
                isLoggedin();
            }
            else
            {
                login_error.SetActive(true);
                login_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Wrong username or password";
            }
        }
    }
    public static IEnumerator loginI(string username, string password)
    {
        var byteArray = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
        string encodedText = Convert.ToBase64String(byteArray);
        UnityWebRequest request = new UnityWebRequest("http://localhost:4000/api/users/signin", "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.SetRequestHeader("Authorization", "Basic " + encodedText);
        yield return request.SendWebRequest();
        DbData = request.downloadHandler.text;
    }

    public void isLoggedin()
    {
        if (access_token != "")
        {
            username = contentUser.user.username;
            txt_username.GetComponent<Text>().text = username;
            txt_mail.GetComponent<Text>().text = contentUser.user.mail;
            email_window.transform.GetChild(1).GetComponent<InputField>().text = txt_mail.GetComponent<Text>().text;
            btn_settings.SetActive(true);
            btn_logout.SetActive(true);
            btn_login.SetActive(false);
            if (contentUser.user.isAdmin)
            {
                btn_admin.SetActive(true);
            }
            Dark = contentUser.user.darkmode;
            settingDarkness();
        }

    }
    public void logout()
    {
        username = "";
        txt_username.GetComponent<Text>().text = username;
        btn_settings.SetActive(false);
        contentUser = null;
        setUser(contentUser);
        LoginUsers2.setUser(contentUser);
        access_token = "";
        closeWindows();
        btn_logout.SetActive(false);
        btn_admin.SetActive(false);
        btn_login.SetActive(true);
    }
    public void register()
    {
        StartCoroutine(registerI());
    }
    IEnumerator registerI()
    {
        if (form_register.transform.GetChild(1).GetComponent<InputField>().text == "")
        {
            register_error.SetActive(true);
            register_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "You didn't enter the username";
        }
        else if (form_register.transform.GetChild(2).GetComponent<InputField>().text == "" ||
        form_register.transform.GetChild(3).GetComponent<InputField>().text == "")
        {
            register_error.SetActive(true);
            register_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "You didn't enter the password";
        }
        else if (form_register.transform.GetChild(2).GetComponent<InputField>().text !=
       form_register.transform.GetChild(3).GetComponent<InputField>().text)
        {
            register_error.SetActive(true);
            register_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Passwords don't match";
        }
        else if (form_register.transform.GetChild(4).GetComponent<InputField>().text == "")
        {
            register_error.SetActive(true);
            register_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "You didn't enter the email";
        }
        else if (!form_register.transform.GetChild(4).GetComponent<InputField>().text.Contains("@") ||
       !form_register.transform.GetChild(4).GetComponent<InputField>().text.Contains(".") ||
       form_register.transform.GetChild(4).GetComponent<InputField>().text.Length < 5)
        {
            register_error.SetActive(true);
            register_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Enter a valid email";
        }
        else
        {
            var user = new UserModel();
            user.username = form_register.transform.GetChild(1).GetComponent<InputField>().text;
            user.password = form_register.transform.GetChild(2).GetComponent<InputField>().text;
            user.mail = form_register.transform.GetChild(4).GetComponent<InputField>().text;
            WWWForm form = new WWWForm();
            form.AddField("username", user.username);
            form.AddField("password", user.password);
            form.AddField("mail", user.mail);
            UnityWebRequest request = UnityWebRequest.Post("http://localhost:4000/api/users", form);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return request.SendWebRequest();

            if (request.downloadHandler.text.StartsWith("{\"user\":{\"id\""))
            {
                register_error.SetActive(false);
                contentUser = JsonConvert.DeserializeObject<OverUserModel>(request.downloadHandler.text);
                form_register.transform.GetChild(1).GetComponent<InputField>().text = "";
                form_register.transform.GetChild(2).GetComponent<InputField>().text = "";
                form_register.SetActive(false);
                access_token = contentUser.access_token;
                isLoggedin();
            }
            else
            {
                register_error.SetActive(true);
                register_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Something went wrong";
                //Debug.Log("Wrong username or password");
            }
        }
    }

    public void changeDarkness()
    {
        if (Dark)
        {
            if (access_token != "")
            {
                contentUser.user.darkmode = false;
                StartCoroutine(changeDarknessI());

            }
            Dark = false;
            LoginUsers2.setDark(Dark);
            settingDarkness();
        }
        else
        {
            if (access_token != "")
            {
                contentUser.user.darkmode = true;
                StartCoroutine(changeDarknessI());

            }
            Dark = true;
            LoginUsers2.setDark(Dark);
            settingDarkness();
        }
    }

    IEnumerator changeDarknessI()
    {
        var userUpdate = new UserModel();
        userUpdate.id = contentUser.user.id;
        userUpdate.username = contentUser.user.username;
        userUpdate.password = contentUser.user.password;
        userUpdate.mail = contentUser.user.mail;
        userUpdate.isAdmin = contentUser.user.isAdmin;
        userUpdate.darkmode = contentUser.user.darkmode;
        var json = JsonConvert.SerializeObject(userUpdate);
        //var updateData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var byteArray = System.Text.Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = UnityWebRequest.Put("http://localhost:4000/api/users/" + userUpdate.id, byteArray);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + access_token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        userUpdate = null;
    }

    public void settingDarkness()
    {
        Crud.setDark(Dark);
        var white = new Color32(255, 255, 255, 255);
        var black = new Color32(52, 52, 55, 255);
        if (Dark)
        {

            main_menu_black.SetActive(true);
            main_menu_white.SetActive(false);
            info.transform.GetChild(0).GetComponent<Image>().color = black;
            info.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
            form_login.transform.GetChild(0).GetComponent<Image>().color = black;
            form_login.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
            form_login.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = white;
            form_login.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().color = white;
            txt_mail.GetComponent<Text>().color = white;
            form_register.transform.GetChild(0).GetComponent<Image>().color = black;
            form_register.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
            form_register.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = white;
            form_register.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().color = white;
            form_register.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().color = white;
            contact_window.transform.GetChild(0).GetComponent<Image>().color = black;
            contact_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
            contact_window.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().color = white;
            if (access_token != "")
            {
                contact2_window.transform.GetChild(0).GetComponent<Image>().color = black;
                contact2_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
                contact2_window.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().color = white;
                user_settings.transform.GetChild(0).GetComponent<Image>().color = black;
                user_settings.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
                user_settings.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().color = white;
                user_settings.transform.GetChild(0).transform.GetChild(5).GetComponent<Text>().color = white;
                password_window.transform.GetChild(0).GetComponent<Image>().color = black;
                password_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
                password_window.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = white;
                email_window.transform.GetChild(0).GetComponent<Image>().color = black;
                email_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
                email_window.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = white;
                delete_window.transform.GetChild(0).GetComponent<Image>().color = black;
                delete_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = white;
            }
        }
        else
        {
            main_menu_black.SetActive(false);
            main_menu_white.SetActive(true);
            info.transform.GetChild(0).GetComponent<Image>().color = white;
            info.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
            form_login.transform.GetChild(0).GetComponent<Image>().color = white;
            form_login.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
            form_login.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = black;
            form_login.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().color = black;
            txt_mail.GetComponent<Text>().color = black;
            form_register.transform.GetChild(0).GetComponent<Image>().color = white;
            form_register.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
            form_register.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = black;
            form_register.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().color = black;
            form_register.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().color = black;
            contact_window.transform.GetChild(0).GetComponent<Image>().color = white;
            contact_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
            contact_window.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().color = black;
            if (access_token != "")
            {
                contact2_window.transform.GetChild(0).GetComponent<Image>().color = white;
                contact2_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
                contact2_window.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().color = black;
                user_settings.transform.GetChild(0).GetComponent<Image>().color = white;
                user_settings.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
                user_settings.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().color = black;
                user_settings.transform.GetChild(0).transform.GetChild(5).GetComponent<Text>().color = black;
                password_window.transform.GetChild(0).GetComponent<Image>().color = white;
                password_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
                password_window.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = black;
                email_window.transform.GetChild(0).GetComponent<Image>().color = white;
                email_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
                email_window.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().color = black;
                delete_window.transform.GetChild(0).GetComponent<Image>().color = white;
                delete_window.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().color = black;
            }
        }
    }

    void Update()
    {

    }
}