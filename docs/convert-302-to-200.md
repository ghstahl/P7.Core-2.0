What I am doing might be considered evil, but whatev!

I ended up adding a middleware that turned a 302 into a 200

```
public class Convert302ResponseMiddleware
{
  private readonly RequestDelegate _next;
  public Convert302ResponseMiddleware(RequestDelegate next)
  {
    _next = next;
  }
  public async Task Invoke(HttpContext context)
  {
    await _next(context);
    if (context.Response.StatusCode == 302
                && context.Request.Headers.ContainsKey("X-302to200"))
    {
      context.Response.StatusCode = 200;
    }
  }
}
```
My angular guys can now make the following call
```
POST /Account/ExternalLogin HTTP/1.1
HOST: localhost:44301
content-type: application/x-www-form-urlencoded
x-302to200: true
cookie: .AspNetCore.Antiforgery.4sSYxSBLpJg=CfDJ8KQDvhHrlshPgwOtJGGbkau0w0wjo1uYCEH65BBBTQtDd4EkH8HYwKDCJDSJjynybApW5VovnUOUhBWeub3UERMtTQdV1o9XhuvFSaZ77u0tyQGZRNt806c3_BPrR_K2Ty1_8evftKRo_b1mzenmp5o; ASP.NET_SessionId=4kwnfeojxriopytdzavqgqj0
content-length: 15

provider=Google
```
resulting in a 200 response that contains the location of the redirect.
All nonce cookies are still in play, and they can then choose to do a
```
location:
https://accounts.google.com/o/oauth2/v2/auth?client_id=1096301616546-edbl612881t7rkpljp3qa3juminskulo.apps.googleusercontent.com&redirect_uri=https%3A%2F%2Flocalhost%3A44301%2Fsignin-google&response_type=code&scope=openid%20profile&response_mode=form_post&nonce=636447363938910795.ZTJlY2U4MzUtZTQ5OS00M2VlLTk4YjUtZTQ4NzZjN2Q1ZGE4N2NiOGRjZTItNzljMC00MmQ4LWEwYTYtZmU0NWRjNzlkMWE0&state=CfDJ8E4sZF7qpe5Hjw0cpmHSOvcjkspCz75vdaMbym5cCq59j26Ydw7FNxz9Y6fUbCPHSOkacn4rz7FgqcJ3-MifQqRtOsSRJJl-qsVWq4EZjV3y6ef-ukNwbikjBrMn6ZAHpnHdzrZ7nt4S6qKoospoqnzQ6hyXRE1HYVK94GCroCSTAMhfuNiwEHwoJU1LlgmUIFKo5d-l2L3UUX-hIvjl3x5-dGC6Fbf6hViAPzZCHPwDgBokO-L0e4OErGMmzEmyTQ9pouxYAqDjCSzgEETj7QtASLNU0S_s6To6OBEsSXtbKVDVtS-LZVkF-_G3a9tPjwb9zopAlSmKqJBk-WE7qc-VYdzK84yHfIE_TQSD8i7H&x-client-SKU=ID_NET&x-client-ver=2.1.4.0
server:
Kestrel
set-cookie:
.AspNetCore.OpenIdConnect.Nonce.CfDJ8E4sZF7qpe5Hjw0cpmHSOvdgj99_XCKpTYQYIInvZILDBS6xx-8C-H7DYmKFkAkrF1zNHeuhmndYtjPC_L6B4R-wTseGP5HPOyp-BAmWTsUKmRX085aSlJlRIOl-StmCAzgPFDBNpDViLCEAiwpfn4_46OTGaSu2vgDR5o_8kj3ifmPAK6VBGzb9EeRvl4LbbxBjVL-Ua6cx4i8QSw0Vq3tkTPmEypMidWj4UwMcZnnuPCFS_QkCbEmI5SA5wGTzNluOcM-gPSpqYmzXC70Iuoc=N; expires=Fri, 27 Oct 2017 21:41:33 GMT; path=/signin-google; secure; httponly; .AspNetCore.Correlation.Google.iA2XXFmbiM_rA4wcPmp2euRiyTJnBE47hqQoIgAexS4=N; expires=Fri, 27 Oct 2017 21:41:33 GMT; path=/signin-google; secure; httponly; .AspNetCore.Mvc.CookieTempDataProvider=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; samesite=strict
x-sourcefiles:
=?UTF-8?B?SDpcR2l0aHViXGdoc3RhaGxcUDcuQ29yZS0yLjBcc3JjXFJlZmVyZW5jZVdlYkFwcC5FeHRlcm5hbElkZW50aXR5XEFjY291bnRcRXh0ZXJuYWxMb2dpbldoYXRJZg==?=
x-powered-by:
ASP.NET
date:
Fri, 27 Oct 2017 21:26:33 GMT
content-length:
0
```
You might ask why the hell are we doing this?
To avoid a redirect, or better yet, to cover up a redirect, by getting the location of the redirect via REST
