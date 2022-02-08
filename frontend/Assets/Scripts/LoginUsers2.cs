using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

//using WebSocketSharp;
using HybridWebSocket;
using TMPro;

public class LoginUsers2 : MonoBehaviour
{
    public static string content;
    public static List<CrudComments.ReviewModel> contentArray;
    public static List<Crud.ImageModel> contentArrayImage;
    public static LoginUsers.OverUserModel contentUser = null;
    public static string access_token = "";
    public static string username = "";
    public static Boolean Dark = false;
    public static LoginUsers.OverUserModel getUser() { return contentUser; }
    public static void setUser(LoginUsers.OverUserModel user) { contentUser = user; }
    public static string getUsername() { return username; }
    public static void setUsername(string usern) { username = usern; }
    public static Boolean getDark() { return Dark; }
    public static void setDark(Boolean valueDark) { Dark = valueDark; }
    public static string getToken() { return access_token; }
    public int cuenta = 0;
    public int cuentaElegida = 0;
    public int cuentaMaxima = 0;

    WebSocket ws;
    string mensaje = "";
    public static string recibido = "";

    public string ip = "localhost";

    public GameObject form_login, txt_username, txt_mail, btn_settings, btn_login, btn_logout, login_error,
    form_register, register_error, password_window, password_error, user_settings,
    email_window, email_error, delete_window, main_menu_white, main_menu_black, places,
    infoPlaya, infoArrecife, infoGraciosa, infoTimanfaya, itemParent, item, txt_place,
    comment_log, comment_write, scrollView, comments, images, txt_place_image, itemImage, itemParentImage,
    chat_input, chat_parent, chat_child, chat, chatLog, chatWrite, chat_window;

    void Start()
    {
        cuentaElegida = 0;
        closeWindows();
        comments.SetActive(false);
        images.SetActive(false);
        chat_window.SetActive(false);
        places.SetActive(true);
        contentUser = LoginUsers.getUser();
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
        ip = LoginUsers.getIp();
        ws = WebSocketFactory.CreateInstance("ws://" + ip + ":8000");
        ws.OnMessage += (byte[] msg) =>
        {
            recibido = System.Text.Encoding.UTF8.GetString(msg);
            Debug.Log(recibido);
        };
        ws.Connect();
    }

    public void AddText(string t)
    {
        GameObject tmp_item = Instantiate(chat_child, chat_parent.transform);
        tmp_item.transform.GetChild(1).GetComponent<TMP_Text>().text = t;
        chat_input.GetComponent<TMP_InputField>().text = "";
    }

    public void closeWindows()
    {
        form_login.SetActive(false);
        login_error.SetActive(false);
        form_register.SetActive(false);
        register_error.SetActive(false);
        user_settings.SetActive(false);
        password_window.SetActive(false);
        password_error.SetActive(false);
        email_window.SetActive(false);
        email_error.SetActive(false);
        delete_window.SetActive(false);
    }
    public void destroyImages()
    {
        for (int i = 0; i < itemParentImage.transform.childCount; i++)
        {
            Destroy(itemParentImage.transform.GetChild(i).gameObject);
        }
    }
    public void readImages()
    {
        StartCoroutine(readImagesI());
    }
    IEnumerator readImagesI()
    {

        txt_place_image.GetComponent<Text>().text = TourManager.getPlace();
        /*using var client = new HttpClient();
        content = await client.GetStringAsync("http://localhost:4000/api/reviews");*/
        UnityWebRequest request = new UnityWebRequest("http://localhost:4000/api/images", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
        contentArrayImage = JsonConvert.DeserializeObject<List<Crud.ImageModel>>(request.downloadHandler.text);
        //contentArrayImage.Reverse();
        itemImage.SetActive(true);
        foreach (Crud.ImageModel model in contentArrayImage)
        {
            if (model.place == TourManager.getPlace())
            {
                if (cuenta == cuentaElegida)
                {
                    GameObject tmp_item = Instantiate(itemImage, itemParentImage.transform);
                    tmp_item.transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(model.fileName);
                }
                cuenta++;
            }
        }
        itemImage.SetActive(false);
        cuentaMaxima = cuenta - 1;
        cuenta = 0;
    }

    public void imageNext()
    {
        if (cuentaElegida == cuentaMaxima)
        {
            cuentaElegida = 0;
        }
        else
        {
            cuentaElegida++;
        }
        readImages();
    }

    public void imageBefore()
    {
        if (cuentaElegida == 0)
        {
            cuentaElegida = cuentaMaxima;
        }
        else
        {
            cuentaElegida--;
        }
        readImages();
    }

    public void read()
    {
        StartCoroutine(readI());
    }
    IEnumerator readI()
    {
        txt_place.GetComponent<Text>().text = TourManager.getPlace();
        if (Dark)
        {
            for (int i = 0; i < itemParent.transform.childCount; i++)
            {
                Destroy(itemParent.transform.GetChild(i).gameObject);
            }
            /*using var client = new HttpClient();
            content = await client.GetStringAsync("http://localhost:4000/api/reviews");*/
            UnityWebRequest request = new UnityWebRequest("http://localhost:4000/api/reviews", "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            contentArray = JsonConvert.DeserializeObject<List<CrudComments.ReviewModel>>(request.downloadHandler.text);
            contentArray.Reverse();
            foreach (CrudComments.ReviewModel model in contentArray)
            {
                if (model.target == TourManager.getPlace())
                {
                    GameObject tmp_item = Instantiate(item, itemParent.transform);
                    tmp_item.transform.GetChild(0).GetComponent<Image>().color = new Color32(52, 52, 55, 255);
                    tmp_item.transform.GetChild(1).GetComponent<Text>().text = model.username + " says:";
                    tmp_item.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().color = Color.white;
                    tmp_item.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = model.content;
                }
            }
        }
        else
        {
            for (int i = 0; i < itemParent.transform.childCount; i++)
            {
                Destroy(itemParent.transform.GetChild(i).gameObject);
            }
            /*using var client = new HttpClient();
            content = await client.GetStringAsync("http://localhost:4000/api/reviews");*/
            UnityWebRequest request = new UnityWebRequest("http://localhost:4000/api/reviews", "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            contentArray = JsonConvert.DeserializeObject<List<CrudComments.ReviewModel>>(request.downloadHandler.text);
            contentArray.Reverse();
            foreach (CrudComments.ReviewModel model in contentArray)
            {
                if (model.target == TourManager.getPlace())
                {
                    GameObject tmp_item = Instantiate(item, itemParent.transform);
                    tmp_item.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    tmp_item.transform.GetChild(1).GetComponent<Text>().text = model.username + " says:";
                    tmp_item.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().color = Color.black;
                    tmp_item.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = model.content;
                }
            }
        }
    }

    public void createComment()
    {
        StartCoroutine(createCommentI());
    }
    IEnumerator createCommentI()
    {
        var review = new CrudComments.ReviewModel();
        review.content = comment_write.transform.GetChild(0).transform.GetChild(2).GetComponent<InputField>().text;
        review.username = contentUser.user.username;
        review.target = TourManager.getPlace();
        review.userId = contentUser.user.id;
        WWWForm form = new WWWForm();
        form.AddField("content", review.content);
        form.AddField("username", review.username);
        form.AddField("target", review.target);
        form.AddField("userId", review.userId.ToString());
        UnityWebRequest request = UnityWebRequest.Post("http://localhost:4000/api/reviews", form);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        comment_write.transform.GetChild(0).transform.GetChild(2).GetComponent<InputField>().text = "";
        read();
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
            var userUpdate = new LoginUsers.UserModel();
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
            var userUpdate = new LoginUsers.UserModel();
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

    public void login()
    {
        StartCoroutine(loginI());
    }

    IEnumerator loginI()
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
            var user = new LoginUsers.UserModel();
            user.username = form_login.transform.GetChild(1).GetComponent<InputField>().text;
            user.password = form_login.transform.GetChild(2).GetComponent<InputField>().text;
            var byteArray = System.Text.Encoding.UTF8.GetBytes($"{user.username}:{user.password}");
            string encodedText = Convert.ToBase64String(byteArray);
            //using var client = new HttpClient();
            UnityWebRequest request = new UnityWebRequest("http://localhost:4000/api/users/signin", "POST");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            request.SetRequestHeader("Authorization", "Basic " + encodedText);
            yield return request.SendWebRequest();
            //Debug.Log(request.downloadHandler.text);

            /*client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedText);
            client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));*/

            //HttpResponseMessage response = await client.PostAsync("http://localhost:4000/api/users/signin", null);
            //var responseString = await response.Content.ReadAsStringAsync();

            if (request.downloadHandler.text.StartsWith("{\"user\":{\"id\""))
            {
                login_error.SetActive(false);
                contentUser = JsonConvert.DeserializeObject<LoginUsers.OverUserModel>(request.downloadHandler.text);
                //Debug.Log(contentUser.access_token);
                contentUser.user.password = form_login.transform.GetChild(2).GetComponent<InputField>().text;
                form_login.transform.GetChild(1).GetComponent<InputField>().text = "";
                form_login.transform.GetChild(2).GetComponent<InputField>().text = "";
                form_login.SetActive(false);
                access_token = contentUser.access_token;
                LoginUsers.setUser(contentUser);
                isLoggedin();
            }
            else
            {
                login_error.SetActive(true);
                login_error.transform.GetChild(0).GetComponent<Image>().transform.GetChild(0).GetComponent<Text>().text = "Wrong username or password";
            }
        }
    }

    public void isLoggedin()
    {
        if (access_token != "")
        {
            username = contentUser.user.username;
            txt_username.GetComponent<Text>().text = username;
            txt_mail.GetComponent<Text>().text = contentUser.user.mail;
            email_window.transform.GetChild(1).GetComponent<InputField>().text = txt_mail.GetComponent<Text>().text;
            chatLog.SetActive(false);
            chatWrite.SetActive(true);
            btn_settings.SetActive(true);
            btn_logout.SetActive(true);
            btn_login.SetActive(false);
            comment_log.SetActive(false);
            comment_write.SetActive(true);
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
        LoginUsers.setUser(contentUser);
        access_token = "";
        closeWindows();
        chatLog.SetActive(true);
        chatWrite.SetActive(false);
        btn_logout.SetActive(false);
        btn_login.SetActive(true);
        comment_log.SetActive(true);
        comment_write.SetActive(false);
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
            var user = new LoginUsers.UserModel();
            user.username = form_register.transform.GetChild(1).GetComponent<InputField>().text;
            user.password = form_register.transform.GetChild(2).GetComponent<InputField>().text;
            user.mail = form_register.transform.GetChild(4).GetComponent<InputField>().text;
            WWWForm form = new WWWForm();
            form.AddField("username", user.username);
            form.AddField("password", user.password);
            form.AddField("mail", user.mail);
            /*var byteArray = System.Text.Encoding.UTF8.GetBytes($"{user.username}:{user.password}");
            string encodedText = Convert.ToBase64String(byteArray);
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedText);
            client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            HttpResponseMessage response = await client.PostAsync("http://localhost:4000/api/users", data);ç
            var responseString = await response.Content.ReadAsStringAsync();*/
            //Debug.Log(responseString);
            UnityWebRequest request = UnityWebRequest.Post("http://localhost:4000/api/users", form);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return request.SendWebRequest();
            //Debug.Log(request.downloadHandler.text);

            if (request.downloadHandler.text.StartsWith("{\"user\":{\"id\""))
            {
                register_error.SetActive(false);
                contentUser = JsonConvert.DeserializeObject<LoginUsers.OverUserModel>(request.downloadHandler.text);
                //Debug.Log(contentUser.access_token);
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
        }
        else
        {
            if (access_token != "")
            {
                contentUser.user.darkmode = true;
                StartCoroutine(changeDarknessI());

            }
            Dark = true;
        }
        LoginUsers.setDark(Dark);
        settingDarkness();
    }

    IEnumerator changeDarknessI()
    {
        var userUpdate = new LoginUsers.UserModel();
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
        read(); readImages();
        var white = new Color32(255, 255, 255, 255);
        var black = new Color32(52, 52, 55, 255);
        if (Dark)
        {
            places.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = black;
            places.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = black;
            places.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>().color = black;
            places.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>().color = black;
            main_menu_black.SetActive(true);
            main_menu_white.SetActive(false);
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
            if (access_token != "")
            {
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
                comment_write.transform.GetChild(0).GetComponent<Image>().color = black;
                comment_write.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().color = white;
            }
            infoPlaya.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = black;
            infoPlaya.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = white;
            infoArrecife.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = black;
            infoArrecife.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = white;
            infoGraciosa.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = black;
            infoGraciosa.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = white;
            infoTimanfaya.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = black;
            infoTimanfaya.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = white;
            comment_log.transform.GetChild(0).GetComponent<Image>().color = black;
            comment_log.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().color = white;
            scrollView.GetComponent<Image>().color = black;
        }
        else
        {
            places.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = white;
            places.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = white;
            places.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>().color = white;
            places.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>().color = white;
            main_menu_black.SetActive(false);
            main_menu_white.SetActive(true);
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
            if (access_token != "")
            {
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
                comment_write.transform.GetChild(0).GetComponent<Image>().color = white;
                comment_write.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().color = black;
            }
            infoPlaya.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = white;
            infoPlaya.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = black;
            infoArrecife.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = white;
            infoArrecife.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = black;
            infoGraciosa.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = white;
            infoGraciosa.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = black;
            infoTimanfaya.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().color = white;
            infoTimanfaya.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().color = black;
            comment_log.transform.GetChild(0).GetComponent<Image>().color = white;
            comment_log.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().color = black;
            scrollView.GetComponent<Image>().color = white;
        }
    }

    public void buttonChat()
    {
        mensaje = "<color=blue>" + contentUser.user.username + ": </color>" + chat_input.GetComponent<TMP_InputField>().text;
        if (chat_input.GetComponent<TMP_InputField>().text.Trim().Length > 0)
        {
            //ws.Send(mensaje);
            ws.Send(System.Text.Encoding.UTF8.GetBytes(mensaje));
        }
        else
        {
            chat_input.GetComponent<TMP_InputField>().text = "";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            destroyImages();
            read();
            readImages();
            cuentaElegida = 0;
            cuentaMaxima = 0;
        }
        if (ws == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            mensaje = "<color=blue>" + contentUser.user.username + ": </color>" + chat_input.GetComponent<TMP_InputField>().text;
            if (chat_input.GetComponent<TMP_InputField>().text.Trim().Length > 0)
            {
                ws.Send(System.Text.Encoding.UTF8.GetBytes(mensaje));
            }
            else
            {
                chat_input.GetComponent<TMP_InputField>().text = "";
            }
        }
        if (recibido != "")
        {
            AddText(recibido);
            Debug.Log("Se recibió un mensaje: " + recibido);
            recibido = "";
        }
        chat.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}