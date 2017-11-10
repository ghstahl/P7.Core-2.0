# USING A CUSTOM HOSTNAME WITH IIS EXPRESS AND VISUAL STUDIO 2015

reference
[USING A CUSTOM HOSTNAME WITH IIS EXPRESS AND VISUAL STUDIO 2015 (VS2015)](http://10printhello.com/using-a-custom-hostname-with-iis-express-with-visual-studio-2015-vs2015/)

## Setup


applicationhost.config is located in the following location;
```
../src/.vs/config/applicationhost.config
```

For the most part this seems to work, but I have run into a bunch of problems with vs2017 not being able to launch IIS Express when I make the following change;

Simply add the following;
```
<binding protocol="https" bindingInformation="*:44311:p7core.127.0.0.1.xip.io" />
```
to the following site node;
```
<site name="WebApplication1" id="2">
...
</site>
```
My final looks like this;
```
<site name="WebApplication1" id="2">
<application path="/" applicationPool="Clr4IntegratedAppPool">
  <virtualDirectory path="/" physicalPath="H:\Github\ghstahl\P7.Core-2.0\src\WebApplication1" />
</application>
<bindings>
  <binding protocol="https" bindingInformation="*:44380:p7core2.127.0.0.1.xip.io" />
  <binding protocol="http" bindingInformation="*:11344:p7core2.127.0.0.1.xip.io" />
  <binding protocol="http" bindingInformation="*:11344:localhost" />
  <binding protocol="https" bindingInformation="*:44380:localhost" />
</bindings>
</site>
<site name="ReferenceWebApp.ExternalIdentity" id="3">
<application path="/" applicationPool="Clr4IntegratedAppPool">
  <virtualDirectory path="/" physicalPath="H:\Github\ghstahl\P7.Core-2.0\src\ReferenceWebApp.ExternalIdentity" />
</application>
<bindings>
  <binding protocol="http" bindingInformation="*:19901:localhost" />
  <binding protocol="https" bindingInformation="*:44311:p7core.127.0.0.1.xip.io" />
  <binding protocol="https" bindingInformation="*:44311:localhost" />
</bindings>
</site>
<site name="ReferenceWebApp.ExternalIdentity.NoIdentityServer4" id="3">
  <application path="/" applicationPool="Clr4IntegratedAppPool">
    <virtualDirectory path="/" physicalPath="C:\work\github\ghstahl\P7.Core-2.0\src\ReferenceWebApp.ExternalIdentity.NoIdentityServer4" />
  </application>
  <bindings>
    <binding protocol="http" bindingInformation="*:19902:localhost" />
    <binding protocol="http" bindingInformation="*:19902:p7core.127.0.0.1.xip.io" />
    <binding protocol="https" bindingInformation="*:44312:localhost" />
    <binding protocol="https" bindingInformation="*:44312:p7core.127.0.0.1.xip.io" />
  </bindings>
</site>
```


[launchSettings.json](../src/WebApplication1/Properties/launchSettings.json) is configured to launch to the following;
```
https://localhost:44311/
```

Using the following url should work as well;
```
https://p7core.127.0.0.1.xip.io:44311/
```

I have whitelisted the following domains in my Google OpenID configuration;
```
https://localhost:44311/
https://p7core.127.0.0.1.xip.io:44311/
```



