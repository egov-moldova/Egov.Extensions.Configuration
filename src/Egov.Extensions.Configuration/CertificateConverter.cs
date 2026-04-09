using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Egov.Extensions.Configuration;

/// <summary>
/// A <see cref="TypeConverter"/> that can convert a string into a certificate. Useful for configuration loading.
/// </summary>
public class CertificateConverter : TypeConverter
{
    /// <summary>
    /// Registers the <see cref="CertificateConverter"/> as type converter.
    /// </summary>
    public static void Register()
    {
        TypeDescriptor.AddAttributes(typeof(X509Certificate2), new TypeConverterAttribute(typeof(CertificateConverter)));
    }

    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if ((value is string valueString) && !string.IsNullOrWhiteSpace(valueString))
        {
            var indexOfPipe = valueString.IndexOf('|');
            if (indexOfPipe >= 0)
            {
                return CertificateLoader.Private(valueString[..indexOfPipe], valueString[(indexOfPipe + 1)..]);
            }
            return CertificateLoader.Public(valueString);
        }
        return base.ConvertFrom(context, culture, value);
    }
}