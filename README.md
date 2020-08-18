# SpawtzOAuthClientDemo
A sample client using Spawtz as an OAuth server

Remember to set the `ClientId`, `ClientSecret` and `SpawtzBaseUrl` App Settings in `Web.Config` before running and make sure that the site is accessible via the internet (so that Spawtz can redirect back).

# Issues #

If you get an exception where it cannot find part of the path .../bin/roslyn/csc.exe then run the following command in the package manager console:

```
Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r
```

The older versions of the DotNetCompilerPlatform have a known bug. It gets fixed in subsequent versions.
