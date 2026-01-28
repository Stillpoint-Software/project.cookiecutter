using Aspire.Hosting.Azure;

namespace {{cookiecutter.assembly_name}}.AppHost.Extensions;

public static class ApplicationInsightExtensions
{
    public static IResourceBuilder<AzureApplicationInsightsResource>? AddAppInsightsResource(
       this IDistributedApplicationBuilder builder)
    {
        //get's user secrets
        var appInsightsName = builder.Configuration["Parameters:AppInsightsName"];
        var appInsightsConn = builder.Configuration["Parameters:AppInsightsConnection"];
        var resourceGroup = builder.Configuration["Parameters:ResourceGroup"];

        if (!string.IsNullOrEmpty(resourceGroup) && !string.IsNullOrEmpty(appInsightsConn))
        {
            // Create parameter resources for use with .AsExisting()
            var existingResourceGroup = builder.GetOrAddParameter("ResourceGroup");
            var existingAppInsightsName = builder.GetOrAddParameter("AppInsightsName");

            return builder.AddAzureApplicationInsights("appInsights")
                 .AsExisting(existingAppInsightsName, existingResourceGroup);
        }
        return null;
    }
}
