using System.Security.Cryptography.X509Certificates;

namespace Egov.Extensions.Configuration;

/// <summary>
/// Options for system certificate loading.
/// </summary>
public class SystemCertificateOptions : IDisposable
{
    /// <summary>
    /// Path to the certificate file or a folder that contains "tls.crt" and "tls.key" files.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Certificate password, if required.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// You can directly assign to this property an actual certificate, loaded by other means.
    /// </summary>
    public X509Certificate2? Certificate { get; set; }

    /// <summary>
    /// You can directly assign to this property actual intermediate certificates, loaded by other means.
    /// This is useful when the certificate is loaded from a PFX/CRT file that contains the whole chain.
    /// </summary>
    public X509Certificate2Collection? IntermediateCertificates { get; set; }

    private bool disposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!disposed)
        {
            Certificate?.Dispose();
            if (IntermediateCertificates != null)
            {
                foreach (var certificate in IntermediateCertificates)
                {
                    certificate.Dispose();
                }
            }
            disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}