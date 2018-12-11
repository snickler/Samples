using PCLCrypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Auth;

namespace XamarinClient
{
    public class OAuth2AuthenticatorEx : OAuth2Authenticator
    {
        private string _codeVerifier = String.Empty;
        private string _redirectUrl = String.Empty;
        public OAuth2AuthenticatorEx(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl, GetUsernameAsyncFunc getUsernameAsync = null, bool isUsingNativeUI = false) : base(clientId, scope, authorizeUrl, redirectUrl, getUsernameAsync, isUsingNativeUI)
        {
        }

        public OAuth2AuthenticatorEx(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl, GetUsernameAsyncFunc getUsernameAsync = null, bool isUsingNativeUI = false) : base(clientId, clientSecret, scope, authorizeUrl, redirectUrl, accessTokenUrl, getUsernameAsync, isUsingNativeUI)
        {
          
        }
       
        protected override void OnCreatingInitialUrl(IDictionary<string, string> query)
        {
            _redirectUrl = Uri.UnescapeDataString(query["redirect_uri"]);
            _codeVerifier = CreateCodeVerifier();
            query["response_type"] = "code";
            query["nonce"] = Guid.NewGuid().ToString("N");
            query["code_challenge"] = CreateChallenge(_codeVerifier);
            query["code_challenge_method"] = "S256";
            base.OnCreatingInitialUrl(query);
        }
        private string CreateCodeVerifier()
        {
            var codeBytes = WinRTCrypto.CryptographicBuffer.GenerateRandom(64);
            return Convert.ToBase64String(codeBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
        private string CreateChallenge(string code)
        {
            var codeVerifier = code;
            var sha256 = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var challengeByteArray = sha256.HashData(WinRTCrypto.CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(codeVerifier)));
            WinRTCrypto.CryptographicBuffer.CopyToByteArray(challengeByteArray, out byte[] challengeBytes);
            return Convert.ToBase64String(challengeBytes).Replace("+","-").Replace("/","_").Replace("=","");
        }

        protected override async void OnRedirectPageLoaded(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            query["code_verifier"] = _codeVerifier;
            query["client_id"] = ClientId;
            query["grant_type"] = "authorization_code";
            query["redirect_uri"] = _redirectUrl;
            var token = await RequestAccessTokenAsync(query);
            foreach(var tokenSegment in token)
            {
                fragment.Add(tokenSegment);
            }
            base.OnRedirectPageLoaded(url, query, fragment);
        }
    }
}
