using Moq;
using VaultSharp;
using VaultSharp.V1;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;
using VaultSharp.V1.SecretsEngines.KeyValue;
using VaultSharp.V1.SecretsEngines.KeyValue.V2;

namespace ViennaNET.Extensions.Configuration.HashicorpVault.Tests;

/// <summary>
/// </summary>
public static class Given
{
    /// <summary>
    /// </summary>
    public static string AppSettingsJsonV1 = @"{
        ""ConnectionStrings"": {
            ""TestDB"": ""Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;""
        },
        ""RootIntProp"": 12345,
        ""RootTimeSpanProp"": ""02:00:10"",
        ""RootArraySimple"": [
            ""Item1"",
            ""Item2""
        ],
        ""RootArrayOfObj"": [
            {
                ""BoolProp"": true,
                ""DateTimeProp"": ""2023-02-15"",
            },
            {
                ""BoolProp"": true,
                ""DateTimeProp"": ""2022-02-15""
            }
        ],
        ""RootObj"": {
            ""UriProp"": ""https://test.raiffeisen.ru""
        },
        ""ChildSection"": {
            ""ArraySimple"": [
                ""Item1"",
                ""Item2""
            ],
        }
    }";

    public static string AppSettingsJsonV2 = @"{
        ""ConnectionStrings"": {
            ""TestDB"": ""Server=(localdb)\\mssqllocaldbv2;Database=TestDB;""
        },
        ""RootIntProp"": 54321,
        ""RootTimeSpanProp"": ""01:00:10"",
        ""RootObj"": {
            ""UriProp"": ""https://test.raiffeisen.ru""
        },
        ""ChildSection"": {
            ""ArraySimple"": [
                ""Item1"",
                ""Item2""
            ],
        }
    }";

    /// <summary>
    ///     Возвращает фиктивный <see cref="IVaultClient" />, который при 3-й по счёту попытке получить секрет,
    ///     возвращает исключение <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="firstCallResult">
    ///     Результат, который возвращается при первом вызове, <see cref="IKeyValueSecretsEngineV2.ReadSecretAsync" />.
    /// </param>
    /// <param name="secondCallResult">
    ///     Результат, который возвращается при следующем вызове,
    ///     <see cref="IKeyValueSecretsEngineV2.ReadSecretAsync" />.
    ///     Если не задано, тогда возвращается <paramref name="firstCallResult" />.
    /// </param>
    /// <returns>Ссылка на объект <see cref="Mock{T}"/>.</returns>
    public static Mock<IVaultClient> GetClientMockWithSetupReadSecretAsync(SecretData firstCallResult,
        SecretData? secondCallResult = null)
    {
        var clientMock = new Mock<IVaultClient>();
        var clientV1Mock = new Mock<IVaultClientV1>();
        var secretEngineMock = new Mock<ISecretsEngine>();
        var keyValueSecretEngineMock = new Mock<IKeyValueSecretsEngine>();
        var keyValueSecretEngineV2Mock = new Mock<IKeyValueSecretsEngineV2>();

        clientV1Mock.Setup(v1 => v1.Secrets).Returns(secretEngineMock.Object);
        secretEngineMock.Setup(engine => engine.KeyValue).Returns(keyValueSecretEngineMock.Object);
        keyValueSecretEngineMock.Setup(engine => engine.V2).Returns(keyValueSecretEngineV2Mock.Object);
        keyValueSecretEngineV2Mock
            .SetupSequence(engine =>
                engine.ReadSecretAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new Secret<SecretData> { Data = firstCallResult, RequestId = Guid.NewGuid().ToString() })
            .ReturnsAsync(
                new Secret<SecretData>
                {
                    Data = secondCallResult ?? firstCallResult, RequestId = Guid.NewGuid().ToString()
                })
            .ThrowsAsync(new InvalidOperationException());

        clientMock.Setup(client => client.V1).Returns(clientV1Mock.Object);

        return clientMock;
    }

    public static VaultConfigurationSource GetSource(
        TimeSpan? reloadInterval = null,
        string? mountPath = null,
        string? path = null,
        int? version = null)
    {
        var source = new VaultConfigurationSource
        {
            ClientOptions = new VaultClientOptions(),
            ReloadInterval = reloadInterval,
            Version = version,
            OnPollException = TestContext.WriteLine
        };

        if (mountPath is not null)
        {
            source.MountPath = mountPath;
        }

        if (path is not null)
        {
            source.Path = path;
        }

        return source;
    }
}