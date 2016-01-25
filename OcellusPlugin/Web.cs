using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;


// *************************************************************
// *  Functions for handling cookies and retrieving web pages  *
// *************************************************************

class Web
{
    private static List<string> cookieStringsFromHeader(string header)
    {
        // Look for delimiters - either ; or ,.  Deal with commas that may exist in the attribute values
        // Deal with multiple cookies
        // Deal with boolean values

        header = header.Replace("\r", "").Replace("\n", "");

        int semiIndex;
        int equalIndex;

        string[] pieces = header.Split(',');

        List<string> cookieStrings = new List<string>();

        Boolean firstCookie = true;

        // assemble complete cookies back together
        foreach (string piece in pieces)
        {
            if (firstCookie)
            {
                cookieStrings.Add(piece);
                firstCookie = false;
            }
            else
            {
                semiIndex = piece.IndexOf(';');
                equalIndex = piece.IndexOf('=');

                if (semiIndex != -1 && equalIndex != -1)
                {
                    if (semiIndex < equalIndex) //this piece is part of the last cookie
                    {
                        int last = cookieStrings.Count - 1;
                        string temp = cookieStrings[last];
                        temp = temp + ',' + piece;
                        cookieStrings[last] = temp;

                    }
                    else if (equalIndex < semiIndex && equalIndex != -1)  // fragment must be the beginning of next cookie
                    {
                        cookieStrings.Add(piece);
                    }

                }
                else if ((semiIndex < equalIndex && semiIndex != -1) ||
                    (semiIndex != -1 && equalIndex == -1))  // fragment to be added to last
                {
                    int last = cookieStrings.Count - 1;
                    string temp = cookieStrings[last];
                    temp = temp + ',' + piece;
                    cookieStrings[last] = temp;
                }
            }
        }
        return cookieStrings;
    }

    private static Cookie convertCookieStringToCookie(string cookieString, string strHost)
    {

        string[] strEachCookParts;

        strEachCookParts = cookieString.Split(';');
        int intEachCookPartsCount = strEachCookParts.Length;
        string strCNameAndCValue = string.Empty;
        string strPNameAndPValue = string.Empty;
        string strDNameAndDValue = string.Empty;
        string[] NameValuePairTemp;
        Boolean doNotAdd = false;
        Cookie cookTemp = new Cookie();

        for (int j = 0; j < intEachCookPartsCount; j++)
        {
            if (j == 0)
            {
                strCNameAndCValue = strEachCookParts[j];
                if (strCNameAndCValue != string.Empty)
                {
                    int firstEqual = strCNameAndCValue.IndexOf("=");
                    string firstName = strCNameAndCValue.Substring(0, firstEqual);
                    string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                    cookTemp.Name = firstName;
                    cookTemp.Value = allValue;
                }
                continue;
            }

            if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                strPNameAndPValue = strEachCookParts[j];
                if (strPNameAndPValue != string.Empty)
                {
                    NameValuePairTemp = strPNameAndPValue.Split('=');
                    if (NameValuePairTemp[1] != string.Empty)
                    {
                        cookTemp.Path = NameValuePairTemp[1];
                    }
                    else
                    {
                        cookTemp.Path = "/";
                    }
                }
                continue;
            }

            if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                strPNameAndPValue = strEachCookParts[j];
                if (strPNameAndPValue != string.Empty)
                {
                    NameValuePairTemp = strPNameAndPValue.Split('=');

                    if (NameValuePairTemp[1] != string.Empty)
                    {
                        cookTemp.Domain = NameValuePairTemp[1];
                    }
                    else
                    {
                        cookTemp.Domain = strHost;
                    }
                }
                continue;
            }

            if (strEachCookParts[j].IndexOf("expires", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                strPNameAndPValue = strEachCookParts[j];
                if (strPNameAndPValue != string.Empty)
                {
                    NameValuePairTemp = strPNameAndPValue.Split('=');
                    if (NameValuePairTemp[1] != string.Empty)
                    {
                        // store the date in the expires field of the cookie
                        string expireStr = NameValuePairTemp[1].Replace("UTC", "GMT");
                        DateTime expires = DateTime.Parse(expireStr);
                        DateTime now = DateTime.Now;
                        if (expires.CompareTo(now) < 0)
                        {
                            doNotAdd = true;
                        }
                    }
                }
            }

            if (strEachCookParts[j].ToLower().Trim() == "secure")
            {
                cookTemp.Secure = true;
            }

        }

        if (cookTemp.Path == string.Empty)
        {
            cookTemp.Path = "/";
        }

        cookTemp.Domain = strHost;

        if (doNotAdd)
        {
            return null;
        }
        else
        {
            return cookTemp;
        }
    }

    public static void WriteCookiesToDisk(string file, CookieContainer cookieJar)
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
        using (Stream stream = File.Create(file))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, cookieJar);
            }
            catch (Exception e)
            {
                Debug.Write("ERROR:  Problem writing cookies to disk: " + e.GetType());
            }
        }
    }

    public static CookieContainer ReadCookiesFromDisk(string file)
    {
        try
        {
            using (Stream stream = File.Open(file, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                CookieContainer cookieJar = (CookieContainer)formatter.Deserialize(stream);

                return cookieJar;
            }
        }
        catch (Exception e)
        {
            Debug.Write("ERROR:  Problem reading cookies from disk: " + e.GetType());
            return new CookieContainer();
        }
    }

    public static Boolean downloadFile(string url, string filename)
    {
        try
        {
            Stream response = WebRequest.Create(url).GetResponse().GetResponseStream();

            FileStream fs = File.Create(filename);
            Byte[] buffer = new Byte[32 * 1024];
            int read = response.Read(buffer, 0, buffer.Length);
            while (read > 0)
            {
                fs.Write(buffer, 0, read);
                read = response.Read(buffer, 0, buffer.Length);
            }
            fs.Close();
            response.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static Tuple<Boolean, string, CookieContainer, string> sendRequest(string url, CookieContainer cookieContainer = null, string referer = null, string sendData = null)
    {
        url = url.Replace("\n", "");
        url = url.Replace("\r", "");
        HttpWebRequest request = null;
        try
        {
            request = (HttpWebRequest)WebRequest.Create(url);
        }
        catch
        {
            return Tuple.Create(true, "Bad URL", cookieContainer, "");
        }

        if (cookieContainer != null)
        {
            request.CookieContainer = cookieContainer;
        }
        else
        {
            request.CookieContainer = new CookieContainer();
        }

        // Frontier's companion API currently requires an Apple User-Agent
        request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4";

        // WebRequest doesn't pick up cookies on 302 Redirects.  We have to do this manually
        request.AllowAutoRedirect = false;
        request.ProtocolVersion = HttpVersion.Version11;
        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        request.Headers["Cache-Control"] = "max-age=0";
        request.Headers["Upgrade-Insecure-Requests"] = "1";
        request.Headers["Accept-Language"] = "en-US,en;q=0.8";
        request.Headers["Cache-Control"] = "max-age=0";
        request.Timeout = 30000; // 30 seconds
        if (referer != null)
        {
            request.Referer = referer;
        }

        if (sendData != null)
        {
            request.Method = "POST";
            byte[] postBytes = Encoding.UTF8.GetBytes(sendData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();
        }
        else
        {
            request.Method = "GET";
        }

        string redirect = null;
        string cookieString = null;
        string htmldata = null;

        HttpWebResponse response;
        try
        {
            response = (HttpWebResponse)request.GetResponse();
        }

        catch (WebException ex)
        {
            Debug.Write("Exception Caught:  " + ex.ToString());
            return Tuple.Create(true, "Error", cookieContainer, "");
        }

        StreamReader sr = new StreamReader(response.GetResponseStream());
        htmldata = sr.ReadToEnd();
        sr.Close();

        // Manually handle the 302 Redirect
        redirect = response.Headers["Location"];
        cookieString = response.Headers[HttpResponseHeader.SetCookie];

        if (redirect != null)
        {
            redirect = redirect.Replace("\r", "").Replace("\n", "");
            redirect = "https://companion.orerve.net" + redirect;

            if (cookieString != null)
            {
                List<string> cookieList = cookieStringsFromHeader(cookieString);
                List<string> cookieNames = new List<string>();
                CookieContainer newCookieJar = new CookieContainer();
                foreach (string eachCookieString in cookieList)
                {
                    try
                    {
                        Cookie convertedCookie = convertCookieStringToCookie(eachCookieString, "companion.orerve.net");
                        if (convertedCookie != null)
                        {
                            newCookieJar.Add(convertedCookie);
                            string addName = convertedCookie.Name;
                            cookieNames.Add(addName);
                        }
                    }

                    catch (Exception ex)
                    {
                        Debug.Write("ERROR:  " + ex.ToString());
                    }
                }

                foreach (Cookie passedCookie in cookieContainer.GetCookies(request.RequestUri))
                {
                    if (!cookieNames.Contains(passedCookie.Name))
                    {
                        newCookieJar.Add(passedCookie);
                    }
                }

                Tuple<Boolean, string, CookieContainer, string> tRespon = sendRequest(redirect, newCookieJar, referer, sendData);

                CookieContainer returnedCookies = tRespon.Item3;
                string returnedHtmldata = tRespon.Item4;

                return Tuple.Create(false, "", returnedCookies, returnedHtmldata);
            }
        }
        return Tuple.Create(false, "", cookieContainer, htmldata);
    }
}