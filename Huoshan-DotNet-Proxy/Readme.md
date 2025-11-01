
# HuoshanAI-DotNet-Proxy

[![NuGet version (HuoshanAI-DotNet-Proxy)](https://img.shields.io/nuget/v/HuoshanAI-DotNet-Proxy.svg?label=HuoshanAI-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/HuoshanAI-DotNet-Proxy/)

A simple Proxy API gateway for [HuoshanAI-DotNet](https://github.com/RageAgainstThePixel/HuoshanAI-DotNet) to make authenticated requests from a front end application without exposing your API keys.

## Getting started

### Install from NuGet

Install package [`HuoshanAI-DotNet-Proxy` from Nuget](https://www.nuget.org/packages/HuoshanAI-DotNet-Proxy/).  Here's how via command line:

```powershell
Install-Package HuoshanAI-DotNet-Proxy
```

## Documentation

Using either the [HuoshanAI-DotNet](https://github.com/RageAgainstThePixel/HuoshanAI-DotNet) or [com.HuoshanAI.unity](https://github.com/RageAgainstThePixel/com.HuoshanAI.unity) packages directly in your front-end app may expose your API keys and other sensitive information. To mitigate this risk, it is recommended to set up an intermediate API that makes requests to HuoshanAI on behalf of your front-end app. This library can be utilized for both front-end and intermediary host configurations, ensuring secure communication with the HuoshanAI API.

### Front End Example

In the front end example, you will need to securely authenticate your users using your preferred OAuth provider. Once the user is authenticated, exchange your custom auth token with your API key on the backend.

Follow these steps:

1. Setup a new project using either the [HuoshanAI-DotNet](https://github.com/RageAgainstThePixel/HuoshanAI-DotNet) or [com.HuoshanAI.unity](https://github.com/RageAgainstThePixel/com.HuoshanAI.unity) packages.
2. Authenticate users with your OAuth provider.
3. After successful authentication, create a new `HuoshanAIAuthentication` object and pass in the custom token with the prefix `sess-`.
4. Create a new `HuoshanAISettings` object and specify the domain where your intermediate API is located.
5. Pass your new `auth` and `settings` objects to the `HuoshanAIClient` constructor when you create the client instance.

Here's an example of how to set up the front end:

```csharp
var authToken = await LoginAsync();
var auth = new HuoshanAIAuthentication($"sess-{authToken}");
var settings = new HuoshanAISettings(domain: "api.your-custom-domain.com");
var api = new HuoshanAIClient(auth, settings);
```

This setup allows your front end application to securely communicate with your backend that will be using the HuoshanAI-DotNet-Proxy, which then forwards requests to the HuoshanAI API. This ensures that your HuoshanAI API keys and other sensitive information remain secure throughout the process.

### Back End Example

In this example, we demonstrate how to set up and use `HuoshanAIProxy` in a new ASP.NET Core web app. The proxy server will handle authentication and forward requests to the HuoshanAI API, ensuring that your API keys and other sensitive information remain secure.

1. Create a new [ASP.NET Core minimal web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0) project.
2. Add the HuoshanAI-DotNet nuget package to your project.
    - Powershell install: `Install-Package HuoshanAI-DotNet-Proxy`
    - Dotnet install: `dotnet add package HuoshanAI-DotNet-Proxy`
    - Manually editing .csproj: `<PackageReference Include="HuoshanAI-DotNet-Proxy" />`
3. Create a new class that inherits from `AbstractAuthenticationFilter` and override the `ValidateAuthentication` method. This will implement the `IAuthenticationFilter` that you will use to check user session token against your internal server.
4. In `Program.cs`, create a new proxy web application by calling `HuoshanAIProxy.CreateWebApplication` method, passing your custom `AuthenticationFilter` as a type argument.
5. Create `HuoshanAIAuthentication` and `HuoshanAISettings` as you would normally with your API keys, org id, or Azure settings.

```csharp
public partial class Program
{
    private class AuthenticationFilter : AbstractAuthenticationFilter
    {
        public override async Task ValidateAuthenticationAsync(IHeaderDictionary request)
        {
            await Task.CompletedTask; // remote resource call to verify token

            // You will need to implement your own class to properly test
            // custom issued tokens you've setup for your end users.
            if (!request.Authorization.ToString().Contains(TestUserToken))
            {
                throw new AuthenticationException("User is not authorized");
            }
        }
    }

    public static void Main(string[] args)
    {
        var auth = HuoshanAIAuthentication.LoadFromEnvironment();
        var settings = new HuoshanAISettings(/* your custom settings if using Azure HuoshanAI */);
        using var HuoshanAIClient = new HuoshanAIClient(auth, settings);
        HuoshanAIProxy.CreateWebApplication<AuthenticationFilter>(args, HuoshanAIClient).Run();
    }
}
```

Once you have set up your proxy server, your end users can now make authenticated requests to your proxy api instead of directly to the HuoshanAI API. The proxy server will handle authentication and forward requests to the HuoshanAI API, ensuring that your API keys and other sensitive information remain secure.
