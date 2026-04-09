using Egov.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Egov.Extensions.Configuration.Tests;

public class SystemCertificateExtensionsTests
{
    private static readonly string CertDir = Path.Combine(AppContext.BaseDirectory, "TestCertificates");
    private static readonly string PfxPath = Path.Combine(CertDir, "egov_library_test.pfx");
    private const string PfxPassword = "test";

    [Fact]
    public void AddSystemCertificate_WithAction_ResolvesCertificate()
    {
        var services = new ServiceCollection();
        services.AddSystemCertificate(options =>
        {
            options.Path = PfxPath;
            options.Password = PfxPassword;
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SystemCertificateOptions>>().Value;

        Assert.NotNull(options.Certificate);
        Assert.True(options.Certificate.HasPrivateKey);
    }

    [Fact]
    public void AddSystemCertificate_WithConfiguration_ResolvesCertificate()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Path"] = PfxPath,
                ["Password"] = PfxPassword
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSystemCertificate(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SystemCertificateOptions>>().Value;

        Assert.NotNull(options.Certificate);
        Assert.True(options.Certificate.HasPrivateKey);
    }

    [Fact]
    public void AddSystemCertificate_NoCertificateConfigured_ThrowsOnResolve()
    {
        var services = new ServiceCollection();
        services.AddSystemCertificate(options => { /* no path, no cert */ });

        var provider = services.BuildServiceProvider();

        Assert.Throws<InvalidOperationException>(
            () => provider.GetRequiredService<IOptions<SystemCertificateOptions>>().Value);
    }

    [Fact]
    public void AddSystemCertificate_RegistersPostConfigureOptions()
    {
        var services = new ServiceCollection();
        services.AddSystemCertificate(options =>
        {
            options.Path = PfxPath;
            options.Password = PfxPassword;
        });

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IPostConfigureOptions<SystemCertificateOptions>));

        Assert.NotNull(descriptor);
    }
}
