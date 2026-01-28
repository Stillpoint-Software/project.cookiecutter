
namespace {{cookiecutter.assembly_name }}.AppHost.Extensions;

internal static class ParameterExtensions
{
    internal static IResourceBuilder<ParameterResource> GetOrAddParameter(
        this IDistributedApplicationBuilder builder, string parameterName)
    {
        // Check if parameter already exists
        var existingResource = builder.Resources
            .FirstOrDefault(r => r.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase)
                                 && r is ParameterResource);

        if (existingResource != null)
        {
            // Return existing parameter wrapped in resource builder
            return builder.CreateResourceBuilder((ParameterResource)existingResource);
        }

        // Parameter doesn't exist, create new one
        return builder.AddParameter(parameterName);
    }
}
