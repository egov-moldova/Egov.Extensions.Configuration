using Microsoft.Extensions.Options;

namespace Egov.Extensions.Configuration;

internal class SystemCertificateOptionsPostConfigure: IPostConfigureOptions<SystemCertificateOptions>
{
    public void PostConfigure(string? name, SystemCertificateOptions options)
    {
        if ((options.Certificate == null) && !string.IsNullOrWhiteSpace(options.Path))
        {
            options.Certificate = CertificateLoader.PrivateChain(options.Path, options.Password, out var intermediateCertificates);
            options.IntermediateCertificates = intermediateCertificates;
        }

        if (options.Certificate == null)
        {
            throw new InvalidOperationException("Cannot load service certificate");
        }
        if (!options.Certificate.HasPrivateKey)
        {
            throw new InvalidOperationException("Service certificate does not contain a private key");
        }
    }
}