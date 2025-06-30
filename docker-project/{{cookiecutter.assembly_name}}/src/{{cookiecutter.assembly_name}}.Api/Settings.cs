
namespace {{cookiecutter.assembly_name }}.Api;

public class ApiSettings
{
    public string AppName { get; set; }
    public string WebUrl { get; set; }
}
{% if cookiecutter.include_azure == "yes" %}
public class ApplicationInsights
{
    public string ConnectionString { get; set; }
}

public class AzureDetailSettings
{
    public string TenantId { get; set; }
    public string SubscriptionId { get; set; }
    public string Location { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public class AzureKeyVaultSettings
{
    public string VaultName { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public class AzureStorageSettings
{
    public string AccountName { get; set; }
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}
public class AzureContainerRegistrySettings
{
    public string Server { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}
{% endif %}
