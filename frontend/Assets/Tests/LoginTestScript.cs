using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LoginTestScript
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

    public int testCounter = 0;
    public bool CreatedAccount = false;
    public OverUserModel contentUser = null;

    [UnitySetUp]
    public IEnumerator CreateTestUser()
    {
        if (!CreatedAccount)
        {
            CreatedAccount = true;
            var user = new UserModel();
            user.username = "test";
            user.password = "1234";
            user.mail = "test@gmail.com";
            WWWForm form = new WWWForm();
            form.AddField("username", "test");
            form.AddField("password", "1234");
            form.AddField("mail", "test@gmail.com");
            UnityWebRequest request = UnityWebRequest.Post("http://localhost:4000/api/users", form);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return request.SendWebRequest();

            if (request.downloadHandler.text.StartsWith("{\"user\":{\"id\""))
            {
                contentUser = JsonConvert.DeserializeObject<OverUserModel>(request.downloadHandler.text);
                Debug.Log("The test user was created " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("The test user wasn't created");
            }
        }
    }

    [UnityTest]
    public IEnumerator LoginRight()
    {
        var loggedIn = false;
        var byteArray = System.Text.Encoding.UTF8.GetBytes("test:1234");
        string encodedText = Convert.ToBase64String(byteArray);
        //using var client = new HttpClient();
        UnityWebRequest request = new UnityWebRequest("http://localhost:4000/api/users/signin", "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.SetRequestHeader("Authorization", "Basic " + encodedText);
        yield return request.SendWebRequest();

        if (request.downloadHandler.text.StartsWith("{\"user\":{\"id\""))
        {
            loggedIn = true;
        }
        testCounter += 1;
        Debug.Log("Test con los valores correctos");
        Assert.IsTrue(loggedIn, "El usuario no pudo logearse con los credenciales correctos");
    }

    [UnityTest]
    public IEnumerator LoginFail()
    {
        var loggedIn = false;
        var byteArray = System.Text.Encoding.UTF8.GetBytes("test:123");
        string encodedText = Convert.ToBase64String(byteArray);
        UnityWebRequest request = new UnityWebRequest("http://localhost:4000/api/users/signin", "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.SetRequestHeader("Authorization", "Basic " + encodedText);
        yield return request.SendWebRequest();

        if (request.downloadHandler.text.StartsWith("{\"user\":{\"id\""))
        {
            Debug.Log("The test user logged in correctly" + request.downloadHandler.text);
            loggedIn = true;
        }
        testCounter += 1;
        Debug.Log("Test con la contraseña equivocada");
        Assert.IsFalse(loggedIn, "El usuario pudo conectarse con la contraseña incorrecta");
    }

    [UnityTearDown]
    public IEnumerator GlobalTeardown()
    {
        if (testCounter == 2)
        {
            UnityWebRequest request = UnityWebRequest.Delete("http://localhost:4000/api/users/" + contentUser.user.id);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + contentUser.access_token);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            Debug.Log("La Cuenta fue Borrada");
        }
    }
}
