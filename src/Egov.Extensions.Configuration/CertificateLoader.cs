using System.Security.Cryptography.X509Certificates;

namespace Egov.Extensions.Configuration;

/// <summary>
/// A helper class for loading certificates.
/// </summary>
public static class CertificateLoader
{
    /// <summary>
    /// Loads the certificate with its public key.
    /// </summary>
    /// <param name="certificatePath">Path to the certificate file or a folder that contains "tls.crt" file.</param>
    /// <returns>The loaded <see cref="X509Certificate2"/>.</returns>
    public static X509Certificate2 Public(string certificatePath)
    {
        if (string.IsNullOrWhiteSpace(certificatePath))
            throw new ArgumentException("Path is required.", nameof(certificatePath));

        // load CER/CRT
        if (File.Exists(certificatePath)) return X509CertificateLoader.LoadCertificateFromFile(certificatePath);

        // load from mounted Kubernetes secret
        if (!Directory.Exists(certificatePath)) 
            throw new ArgumentException("No certificate file found.", nameof(certificatePath));
        var certificateFile = Path.Combine(certificatePath, "tls.crt");
        if (!File.Exists(certificateFile))
            throw new ArgumentException("No certificate file found.", nameof(certificatePath));
        return X509CertificateLoader.LoadCertificateFromFile(certificateFile);
    }

    /// <summary>
    /// Loads the certificate with its private key.
    /// </summary>
    /// <param name="certificatePath">Path to the certificate file or a folder that contains "tls.crt" and "tls.key" files.</param>
    /// <param name="certificatePassword">Certificate password, if required.</param>
    /// <returns>Loaded <see cref="X509Certificate2"/>.</returns>
    /// <exception cref="ArgumentException">When path is invalid.</exception>
    public static X509Certificate2 Private(string certificatePath, string? certificatePassword = null)
    {
        if (string.IsNullOrWhiteSpace(certificatePath))
            throw new ArgumentException("Path is required.", nameof(certificatePath));

        // load PFX
        if (File.Exists(certificatePath))
        {
            return X509CertificateLoader.LoadPkcs12FromFile(certificatePath, certificatePassword,
                loaderLimits: Pkcs12LoaderLimits.Defaults);
        }

        // load from a mounted Kubernetes secret
        return PrivateKubernetes(certificatePath, certificatePassword, null);
    }

    /// <summary>
    /// Loads a certificate with its private key and any intermediate certificates.
    /// </summary>
    /// <param name="certificatePath">Path to the certificate file or a folder that contains "tls.crt" and "tls.key" files.</param>
    /// <param name="certificatePassword">Certificate password, if required.</param>
    /// <param name="intermediateCertificates">Output parameter that will contain any intermediate certificates.</param>
    /// <returns>The loaded <see cref="X509Certificate2"/>.</returns>
    /// <exception cref="ArgumentException">When path is invalid.</exception>
    public static X509Certificate2 PrivateChain(string certificatePath, string? certificatePassword, out X509Certificate2Collection? intermediateCertificates)
    {
        intermediateCertificates = null;
        if (string.IsNullOrWhiteSpace(certificatePath))
            throw new ArgumentException("Path is required.", nameof(certificatePath));

        X509Certificate2 certificate;

        // load PFX
        if (File.Exists(certificatePath))
        {
            intermediateCertificates = X509CertificateLoader.LoadPkcs12CollectionFromFile(certificatePath, certificatePassword,
                loaderLimits: Pkcs12LoaderLimits.Defaults);
            var found = intermediateCertificates.FirstOrDefault(c => c.HasPrivateKey)
                ?? throw new ArgumentException("No certificate with private key found in the specified path.", nameof(certificatePath));
            // Load the private certificate independently so its handle remains valid
            // after LeaveOnlyIntermediates removes entries from the shared collection.
            certificate = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, certificatePassword,
                loaderLimits: Pkcs12LoaderLimits.Defaults);
            LeaveOnlyIntermediates(intermediateCertificates, found);
        }
        else // load from a mounted Kubernetes secret
        {
            intermediateCertificates = new X509Certificate2Collection();
            certificate = PrivateKubernetes(certificatePath, certificatePassword, intermediateCertificates);
        }
        
        if (intermediateCertificates.Count == 0) intermediateCertificates = null;
        return certificate;
    }

    private static X509Certificate2 PrivateKubernetes(string certificatePath, string? certificatePassword,
        X509Certificate2Collection? intermediateCertificates)
    {
        if (!Directory.Exists(certificatePath))
            throw new ArgumentException("No certificate file found.", nameof(certificatePath));
        var certificateFile = Path.Combine(certificatePath, "tls.crt");
        var keyFile = Path.Combine(certificatePath, "tls.key");
        if (!File.Exists(keyFile))
            throw new ArgumentException("No certificate file found.", nameof(certificatePath));

        var certificate = string.IsNullOrEmpty(certificatePassword) ?
            X509Certificate2.CreateFromPemFile(certificateFile, keyFile) :
            X509Certificate2.CreateFromEncryptedPemFile(certificateFile, certificatePassword, keyFile);

        if (OperatingSystem.IsWindows())
        {
            var certificateBytes = certificate.Export(X509ContentType.Pkcs12);
            certificate.Dispose();
            certificate = X509CertificateLoader.LoadPkcs12(certificateBytes, null);
        }

        if (intermediateCertificates != null)
        {
            intermediateCertificates.ImportFromPemFile(certificateFile);
            LeaveOnlyIntermediates(intermediateCertificates, certificate);
        }

        return certificate;
    }

    private static void LeaveOnlyIntermediates(X509Certificate2Collection certificates, X509Certificate2 privateCertificate)
    {
        for (int i = 0; i < certificates.Count; i++)
        {
            var certificate = certificates[i];
            if (certificate.Subject == certificate.Issuer)
            {
                certificates.RemoveAt(i);
                certificate.Dispose();
                i--;
            }
            else if (certificate.Thumbprint == privateCertificate.Thumbprint)
            {
                certificates.RemoveAt(i);
                i--;
            }
        }
    }
}