using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.Services;

public class AuthenticationService : IDisposable
{
    private readonly HttpClient httpClient;

    public AuthenticationService()
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://auth.balya.net/v1/")
        };
    }

    public async Task<bool> IsRegistered()
    {
        var cc = await SecureStorage.Default.GetAsync("clientCertificate");
        return cc != null;
    }

    private record CreateAccountResponse
    {
        public Guid AccountId { get; set; }
    }

    public async Task<Guid> CreateAccount(string email, string password)
    {
        var data = new { EmailAddress = email, Password = password };
        var response = await httpClient.PostAsJsonAsync("/account/create", data);
        response.EnsureSuccessStatusCode();
        var car = await response.Content.ReadFromJsonAsync<CreateAccountResponse>();
        return car.AccountId;
    }

    private record ClientRegistrationResponse
    {
        public string CertificatePem { get; set; }
        public Guid AccountId { get; set; }
        public Guid ClientId { get; set; }
    }

    public async Task RegisterClient(string email, string password, string clientName)
    {
        var clientId = Guid.NewGuid();
        var clientIdStr = await SecureStorage.GetAsync("clientId");
        if (clientIdStr != null)
            clientId = new Guid(clientIdStr);
        else
            await SecureStorage.Default.SetAsync("clientId", clientId.ToString());

        var commonName = $"client-{clientId.ToString().ToLowerInvariant()}";
        var clientKeyPem = await SecureStorage.GetAsync("clientKeyRSA");
        RSA clientKey;
        if (clientKeyPem != null)
        {
            clientKey = RSA.Create();
            clientKey.ImportFromPem(clientKeyPem);
        }
        else
        {
            clientKey = RSA.Create(3192);
            await SecureStorage.Default.SetAsync("clientKeyRSA", clientKey.ExportRSAPrivateKeyPem());
        }

        var requestPem = await SecureStorage.GetAsync("clientCertificateRequest");
        if (requestPem == null)
        {
            var dnb = new X500DistinguishedNameBuilder();
            dnb.AddCommonName(commonName);
            dnb.AddOrganizationName("Omnicore");
            dnb.AddOrganizationalUnitName("Mobile Client");

            requestPem = new CertificateRequest(
                dnb.Build(),
                clientKey,
                HashAlgorithmName.SHA512,
                RSASignaturePadding.Pkcs1).CreateSigningRequestPem();
            await SecureStorage.Default.SetAsync("clientCertificateRequest", requestPem);
        }

        var response = await httpClient.PostAsJsonAsync("client/register",
            new { EmailAddress = email, Password = password, ClientName = clientName, RequestPem = requestPem, HashAlgorithm = HashAlgorithmName.SHA512.ToString() });
        response.EnsureSuccessStatusCode();

        var crr = await response.Content.ReadFromJsonAsync<ClientRegistrationResponse>();
        if (crr != null)
        {
            await SecureStorage.Default.SetAsync("clientCertificate", crr.CertificatePem);
            await SecureStorage.Default.SetAsync("accountId", crr.AccountId.ToString());
            await SecureStorage.Default.SetAsync("clientId", crr.ClientId.ToString());

            SecureStorage.Default.Remove("clientCertificateRequest");
        }
        clientKey.Dispose();
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}

