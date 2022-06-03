1. Enable IIS and ASP.NET Core 3.1 Runtime - Windows Hosting Bundle;
2. Install MS SQL Server and MS SSMS;
3. Create ASP.Net Core 3.1 project with Angular and Individual User Accounts authentication;
4. Create table CodeTester and user for it;
5. Apply migrations
4. Create certificate for Identity Server...
    $cert = New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -Subject "IdentityServerCN" -Provider "Microsoft Strong Cryptographic Provider" -HashAlgorithm "SHA512"
    Export-PfxCertificate -cert ('Cert:\LocalMachine\My\' + $cert.thumbprint) -FilePath PATH_TO_YOUR_IdentityServerCertificate.pfx -Password (ConvertTo-SecureString -String "YOUR_PASSWORD" -Force -AsPlainText)
5. Configure appsettings.json file...
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=DESKTOP-KB4FSP8\\SQLEXPRESS;Initial Catalog=CodeTester;User ID=dev;Password=14412133;"
    }
    ...and...
    "Key": {
      "Type": "File",
      "FilePath": "C:\\Projects\\CodeTester\\CodeTester\\IdentityServerCertificate.pfx",
      "Password": "14412133"
    }

$cert = New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -Subject "IdentityServerCN" -Provider "Microsoft Strong Cryptographic Provider" -HashAlgorithm "SHA512"
Export-PfxCertificate -cert ('Cert:\LocalMachine\My\' + $cert.thumbprint) -FilePath D:\IdentityServerCertificate.pfx -Password (ConvertTo-SecureString -String "14412133" -Force -AsPlainText)