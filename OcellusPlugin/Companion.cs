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

        Tuple<bool, string, CookieContainer, string> tInitialGet = Web.sendRequest(loginURL, cookieContainer);
        // XXX handle returned errors
        cookieContainer = tInitialGet.Item3;
        string loginPageHTML = tInitialGet.Item4;

        if (loginPageHTML.Contains("Login"))
        {
            string sendData = "email=" + email + "&password=" + password;
            Tuple<bool, string, CookieContainer, string> tLoginResponse = Web.sendRequest(loginURL, cookieContainer, loginURL, sendData);
            // XXX handle returned errors
            cookieContainer = tLoginResponse.Item3;
            string postPageHTML = tLoginResponse.Item4;
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
        Tuple<bool, string, CookieContainer, string> tResponse = Web.sendRequest(confirmURL, cookieContainer, confirmURL, sendData);

        // XXX handle returned error code
        Debug.Write("return code: " + tResponse.Item1.ToString());
        Debug.Write("return message: " + tResponse.Item2.ToString());
        Debug.Write("return HTML:  " + tResponse.Item4);

        string postVerifyHTML = tResponse.Item4;
        if (postVerifyHTML.Contains("Verification Code"))
        {
            return Tuple.Create(tResponse.Item3, "verification");
        }
        else if (postVerifyHTML.Contains("Please correct"))
        {
            return Tuple.Create(tResponse.Item3, "login");
        }
        else
        {
            return Tuple.Create(tResponse.Item3, "ok");
        }
    }

    public static Tuple<CookieContainer, string> getProfile(CookieContainer cookieContainer)
    {
        Tuple<bool, string, CookieContainer, string> tRespon = Web.sendRequest(profileURL, cookieContainer);
        // XXX handle error messages
        return Tuple.Create(tRespon.Item3, tRespon.Item4);
    }
}