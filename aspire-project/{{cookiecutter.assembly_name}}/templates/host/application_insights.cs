var appInsights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights("appInsights")
    : builder.AddConnectionString("appInsights", "APPLICATIONINSIGHTS_CONNECTION_STRING");