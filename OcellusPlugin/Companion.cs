using System;
using System.Net;


// **************************************************************************
// *  Functions for accessing Frontier's Companion API for Elite Dangerous  *
// **************************************************************************

class Companion
{
    private const string loginURL = "https://companion.orerve.net/user/login";
    private const string confirmURL = "https://companion.orerve.net/user/confirm";
    private const string profileURL = "https://companion.orerve.net/profile";

    public static Tuple<CookieContainer, string> loginToAPI()
    {
        CookieContainer cookieContainer = new CookieContainer();
        string email = PluginRegistry.getStringValue("email");
        string password = PluginRegistry.getStringValue("password");
        if (email == null || email == string.Empty || password == null || password == string.Empty)
        {
            return Tuple.Create(cookieContainer, "credentials");
        }
        string returnString;

        Tuple<CookieContainer, string> tInitialGet = Web.sendRequest(loginURL, cookieContainer);

        cookieContainer = tInitialGet.Item1;
        string loginPageHTML = tInitialGet.Item2;

        if (loginPageHTML.Contains("Login"))
        {
            string sendData = "email=" + email + "&password=" + password;
            Tuple<CookieContainer, string> tLoginResponse = Web.sendRequest(loginURL, cookieContainer, loginURL, sendData);

            cookieContainer = tLoginResponse.Item1;
            string postPageHTML = tLoginResponse.Item2;
            if (postPageHTML.Contains("Verification"))
            {
                returnString = "verification";
            }
            else if (postPageHTML.Contains("password"))
            {
                returnString = "credentials";
            }
            else
            {
                // When verification works it doesn't return content, assume we are logged in
                returnString = "ok";
            }
        }
        else
        {
            returnString = "error";
        }

        return Tuple.Create(cookieContainer, returnString);

    }

    public static Tuple<CookieContainer, string> verifyWithAPI(CookieContainer cookieContainer, string verificationCode)
    {
        string sendData = "code=" + verificationCode;
        Tuple<CookieContainer, string> tResponse = Web.sendRequest(confirmURL, cookieContainer, confirmURL, sendData);
        string postVerifyHTML = tResponse.Item2;
        if (postVerifyHTML.Contains("Verification Code") )
        {
            return Tuple.Create(tResponse.Item1, "verification");
        }
        else
        {
            return Tuple.Create(tResponse.Item1, "ok");
        }
    }

    public static Tuple<CookieContainer, string> getProfile(CookieContainer cookieContainer)
    {
        Tuple<CookieContainer, string> tRespon = Web.sendRequest(profileURL, cookieContainer);
        return Tuple.Create(tRespon.Item1, tRespon.Item2);
    }
}