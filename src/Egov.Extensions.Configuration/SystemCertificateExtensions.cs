using Egov.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Dependency injection extensions for configuration.
/// </summary>
public static class SystemCertificateExtensions
{
    /// <summary>
    /// Adds system certificate using a configuration. You can then use <see cref="IOptions{SystemCertificateOptions}"/> to access the certificate.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSystemCertificate(this IServiceCollection services, IConfiguration config) 
        => services.AddSystemCertificate().Configure<SystemCertificateOptions>(config);

    /// <summary>
    /// Adds system certificate using a configuration action. You can then use <see cref="IOptions{SystemCertificateOptions}"/> to access the certificate.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">The action used to configure the options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSystemCertificate(this IServiceCollection services, Action<SystemCertificateOptions> configureOptions) 
        => services.AddSystemCertificate().Configure(configureOptions);

    /// <summary>
    /// Helper that ensures post configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    private static IServiceCollection AddSystemCertificate(this IServiceCollection services) 
        => services.AddOptions().AddSingleton<IPostConfigureOptions<SystemCertificateOptions>, SystemCertificateOptionsPostConfigure>();
}