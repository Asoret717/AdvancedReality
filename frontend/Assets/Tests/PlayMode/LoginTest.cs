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
public class LoginTest
{
    public string result = "";
    public int testCounter = 0;
    public bool CreatedAccount = false;
    public LoginUsers.OverUserModel contentUser = null;

    [UnitySetUp]
    public IEnumerator CreateTestUser()
    {
        if (!CreatedAccount)
        {
            CreatedAccount = true;
            var user = new LoginUsers.UserModel();
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
                contentUser = JsonConvert.DeserializeObject<LoginUsers.OverUserModel>(request.downloadHandler.text);
                Debug.Log("The test user was created " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("The test user wasn't created");
            }
        }
    }
    
    [UnityTest]
    public IEnumerator LoginRight(){
        var loggedIn = false;
        yield return LoginUsers.loginI("test","1234");
        if (LoginUsers.DbData.StartsWith("{\"user\":{\"id\"")){ loggedIn = true; }
        testCounter += 1;
        Debug.Log("Test con los valores correctos");
        Assert.IsTrue(loggedIn, "El usuario no pudo logearse con los credenciales correctos");
    }

    [UnityTest]
    public IEnumerator LoginFail(){
        var loggedIn = false;
        yield return LoginUsers.loginI("test","123");
        if (LoginUsers.DbData.StartsWith("{\"user\":{\"id\"")){ loggedIn = true; }
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