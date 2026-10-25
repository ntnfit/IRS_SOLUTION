using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Apzon.Commons;

namespace IrsConfigTool;

/// <summary>
/// Uses the RSA key pair compiled into Apzon.Commons (internal class Apzon.Commons.Constants.RSA)
/// so values written here are readable by the API exactly like the originals.
/// </summary>
internal static class Program
{
    // appSettings keys the API reads through Function.Decrypt (see Apzon.Api Global.cs)
    private static readonly string[] EncryptedKeys =
    {
        "LicenseServer", "CommonDbUserName", "CommonDbPassword", "CommonDbServer", "CommonDb", "ServiceAddress",
        "ServiceType", "DatabaseSystemType", "Pass", "SuperUser", "LoginSap", "CloudSystem"
    };

    // never printed in clear text by show-config
    private static readonly HashSet<string> SecretKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CommonDbUserName", "CommonDbPassword", "Pass", "SuperUser"
    };

    private static int Main(string[] args)
    {
        try
        {
            switch (args.FirstOrDefault()?.ToLowerInvariant())
            {
                case "encrypt" when args.Length == 2:
                    Console.WriteLine(Function.Encrypt(Key("PublicKey"), args[1]));
                    return 0;
                case "show-config" when args.Length == 2:
                    return ShowConfig(args[1]);
                case "set-config" when args.Length >= 3:
                    return SetConfig(args[1], args.Skip(2));
                case "decrypt-aef" when args.Length >= 2:
                    return DecryptAef(args[1], args.Length > 2 ? args[2] : null);
                case "hardware-key" when args.Length == 1:
                    Console.WriteLine(Function.GetHardwareKey());
                    return 0;
                case "create-qc-license" when args.Length >= 2 && args.Length <= 5:
                    return CreateQcLicense(
                        args[1],
                        args.Length > 2 ? args[2] : Function.GetHardwareKey(),
                        args.Length > 3 ? args[3] : "2.10.776",
                        args.Length > 4 ? args[4] : DateTime.Today.AddDays(30).ToString("yyyyMMdd"));
                case "verify-license" when args.Length == 2:
                    return VerifyLicense(args[1]);
                default:
                    Console.Error.WriteLine(
                        "Usage:\n" +
                        "  IrsConfigTool encrypt <text>\n" +
                        "  IrsConfigTool show-config <Web.config>            (secrets are masked)\n" +
                        "  IrsConfigTool set-config <Web.config> Key=Value ... (encrypts the keys the API decrypts)\n" +
                        "  IrsConfigTool decrypt-aef <file.AEF|folder> [outFolder]\n" +
                        "  IrsConfigTool hardware-key\n" +
                        "  IrsConfigTool create-qc-license <output> [hardwareKey] [version] [expiry yyyyMMdd]\n" +
                        "  IrsConfigTool verify-license <file>");
                    return 2;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("ERROR: " + ex.Message);
            return 1;
        }
    }

    private static string Key(string name)
    {
        Type rsa = typeof(Function).Assembly.GetType("Apzon.Commons.Constants.RSA", throwOnError: true);
        return (string)rsa.GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).GetValue(null);
    }

    private static IEnumerable<XElement> AppSettings(XDocument doc) =>
        doc.Root.Element("appSettings")?.Elements("add") ?? Enumerable.Empty<XElement>();

    private static int ShowConfig(string path)
    {
        XDocument doc = XDocument.Load(path);
        foreach (XElement add in AppSettings(doc))
        {
            string key = (string)add.Attribute("key");
            string value = (string)add.Attribute("value");
            if (!EncryptedKeys.Contains(key))
            {
                Console.WriteLine($"{key} = {value}");
            }
            else if (SecretKeys.Contains(key))
            {
                Console.WriteLine($"{key} = <secret, encrypted>");
            }
            else
            {
                string plain;
                try { plain = Function.Decrypt(Key("PrivateKey"), value); }
                catch { plain = "<cannot decrypt>"; }
                Console.WriteLine($"{key} = {plain}   (encrypted)");
            }
        }
        return 0;
    }

    private static int SetConfig(string path, IEnumerable<string> pairs)
    {
        XDocument doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        XElement appSettings = doc.Root.Element("appSettings") ?? throw new InvalidOperationException("No <appSettings> in " + path);
        foreach (string pair in pairs)
        {
            int eq = pair.IndexOf('=');
            if (eq <= 0)
            {
                throw new ArgumentException("Expected Key=Value, got: " + pair);
            }
            string key = pair.Substring(0, eq);
            string value = pair.Substring(eq + 1);
            string stored = EncryptedKeys.Contains(key) ? Function.Encrypt(Key("PublicKey"), value) : value;
            XElement add = appSettings.Elements("add").FirstOrDefault(e => (string)e.Attribute("key") == key);
            if (add == null)
            {
                appSettings.Add(add = new XElement("add", new XAttribute("key", key)));
            }
            add.SetAttributeValue("value", stored);
            Console.WriteLine($"set {key}{(EncryptedKeys.Contains(key) ? " (encrypted)" : "")}");
        }
        doc.Save(path, SaveOptions.DisableFormatting);
        return 0;
    }

    private static int DecryptAef(string input, string outFolder)
    {
        string[] files = Directory.Exists(input)
            ? Directory.GetFiles(input, "*.AEF", SearchOption.AllDirectories)
            : new[] { input };
        foreach (string file in files)
        {
            string error = Function.DecryptFile(file, out List<string> statements);
            if (!string.IsNullOrEmpty(error))
            {
                Console.Error.WriteLine($"{file}: {error}");
                continue;
            }
            string sql = string.Join(Environment.NewLine + "GO" + Environment.NewLine, statements);
            if (outFolder == null)
            {
                Console.WriteLine($"-- ===== {file}");
                Console.WriteLine(sql);
                continue;
            }
            string relative = Directory.Exists(input) ? file.Substring(input.TrimEnd('\\', '/').Length + 1) : Path.GetFileName(file);
            string target = Path.Combine(outFolder, Path.ChangeExtension(relative, ".sql"));
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.WriteAllText(target, sql);
        }
        Console.Error.WriteLine($"processed {files.Length} file(s)");
        return 0;
    }

    private static int CreateQcLicense(string output, string hardwareKey, string version, string expiry)
    {
        if (string.IsNullOrWhiteSpace(hardwareKey))
        {
            throw new InvalidOperationException("The local hardware key is empty.");
        }
        if (!DateTime.TryParseExact(expiry, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime expiryDate))
        {
            throw new ArgumentException("Expiry must use yyyyMMdd format.");
        }
        if (expiryDate < DateTime.Today)
        {
            throw new ArgumentException("Expiry cannot be earlier than today.");
        }

        string fullOutput = Path.GetFullPath(output);
        string outputDirectory = Path.GetDirectoryName(fullOutput);
        if (!string.IsNullOrEmpty(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var lines = new List<string>
        {
            "IRS QC LICENSE",
            "QC-LOCAL",
            "LOCAL DEVELOPMENT ONLY",
            "Bound to one hardware key",
            "Short-lived test license",
            "Not for production",
            "Generated by recovered IrsConfigTool",
            "Version-bound",
            "END HEADER"
        };

        AddLicenseValue(lines, hardwareKey);
        AddLicenseValue(lines, "QC-" + DateTime.Today.ToString("yyyyMMdd"));
        AddLicenseValue(lines, Guid.NewGuid().ToString("N"));
        AddLicenseValue(lines, "DEV");
        AddLicenseValue(lines, version);
        AddLicenseValue(lines, DateTime.Today.ToString("yyyyMMdd"));
        AddLicenseValue(lines, $"QC;QC Full Access;5;{expiry};S");
        AddLicenseValue(lines, "END");
        AddLicenseValue(lines, "ENDLICENSE");

        File.WriteAllLines(fullOutput, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        VerifyLicenseData(fullOutput, hardwareKey, version, expiryDate);
        Console.WriteLine(fullOutput);
        return 0;
    }

    private static void AddLicenseValue(ICollection<string> lines, string value)
    {
        string encrypted = Function.Encrypt(Key("PublicKey"), value);
        if (!encrypted.EndsWith("==", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Unexpected RSA payload padding.");
        }
        encrypted = encrypted.Substring(0, encrypted.Length - 2);
        lines.Add(encrypted);
        int fillerCount = Convert.ToChar(encrypted.Substring(15, 1)) % 5;
        for (int i = 0; i < fillerCount; i++)
        {
            lines.Add("QC-FILLER");
        }
    }

    private static int VerifyLicense(string path)
    {
        Function.ReadLicense(File.ReadAllText(path), out string bpCode, out string installNumber,
            out string hardwareKey, out string fileId, out string sysType, out string dateCreate,
            out string version, out List<LicenseInfoImpl> licenses);
        Console.WriteLine($"BP code: {bpCode}");
        Console.WriteLine($"Install number: {installNumber}");
        Console.WriteLine($"Hardware key: {hardwareKey}");
        Console.WriteLine($"File ID: {fileId}");
        Console.WriteLine($"System type: {sysType}");
        Console.WriteLine($"Created: {dateCreate}");
        Console.WriteLine($"Version: {version}");
        foreach (LicenseInfoImpl license in licenses)
        {
            Console.WriteLine($"License: {license.TypeCode}; {license.TypeName}; amount={license.Amount}; expiry={license.ExpiredDate:yyyyMMdd}; rule={license.LicenseRule}");
        }
        return 0;
    }

    private static void VerifyLicenseData(string path, string expectedHardwareKey, string expectedVersion, DateTime expectedExpiry)
    {
        Function.ReadLicense(File.ReadAllText(path), out string bpCode, out _, out string hardwareKey,
            out _, out string sysType, out _, out string version, out List<LicenseInfoImpl> licenses);
        if (bpCode != "QC-LOCAL" || hardwareKey != expectedHardwareKey || sysType != "DEV" ||
            version != expectedVersion || licenses.Count != 1 || licenses[0].ExpiredDate.Date != expectedExpiry.Date)
        {
            throw new InvalidDataException("Generated QC license failed round-trip validation.");
        }
    }
}
