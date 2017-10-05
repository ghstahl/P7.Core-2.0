# USING A CUSTOM HOSTNAME WITH IIS EXPRESS AND VISUAL STUDIO 2015

reference
[USING A CUSTOM HOSTNAME WITH IIS EXPRESS AND VISUAL STUDIO 2015 (VS2015)](http://10printhello.com/using-a-custom-hostname-with-iis-express-with-visual-studio-2015-vs2015/)

## Setup

[applicationhost.config](../src/.vs/config/applicationhost.config)  

```
<site name="WebApplication1" id="2">
    <application path="/" applicationPool="Clr4IntegratedAppPool">
        <virtualDirectory path="/" physicalPath="H:\github\ghstahl\P7.Core-2.0\src\WebApplication1" />
    </application>
    <bindings>
                <binding protocol="http" bindingInformation="*:9815:localhost" />
                <binding protocol="https" bindingInformation="*:44311:localhost" />
                <binding protocol="https" bindingInformation="*:44311:p7core.127.0.0.1.xip.io" />
    </bindings>
</site>
```

[launchSettings.json](../src/WebApplication1/Properties/launchSettings.json) 

