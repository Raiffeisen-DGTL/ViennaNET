using k8s;
using k8s.Autorest;
using k8s.Models;
using Moq;
using ViennaNET.Extensions.Configuration.Kubernetes.Internals;

namespace ViennaNET.Extensions.Configuration.Kubernetes.Tests;

internal static class Given
{
    public const string FileName = "test.json";
    public const string OtherFileName = "other-test.json";
    public const string FileJsonContent = "{ \"TestOption\": \"TestOptValue\"}";
    public const string InvalidFileJsonContent = "{ \"TestOption\": \"TestOptValue\"";
    public static readonly V1ObjectMeta Meta = new() { Name = "test", NamespaceProperty = "default" };

    public static Mock<IKubernetes> GetClientMock<T>(T result) where T : IKubernetesObject
    {
        var mock = new Mock<IKubernetes>();

        return result switch
        {
            V1ConfigMap body => mock.WithV1ConfigMap(body),
            V1Secret body => mock.WithV1Secret(body),
            V1ConfigMapList body => mock.WithV1ConfigMapList(body),
            V1SecretList body => mock.WithV1SecretList(body),
            _ => mock
        };
    }

    public static Mock<IKubernetes> WithV1ConfigMap(this Mock<IKubernetes> mock, V1ConfigMap result)
    {
        mock.Setup(kubernetes =>
                kubernetes.CoreV1.ReadNamespacedConfigMapWithHttpMessagesAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
                    It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new HttpOperationResponse<V1ConfigMap> { Body = result }));

        return mock;
    }

    public static Mock<IKubernetes> WithV1Secret(this Mock<IKubernetes> mock, V1Secret result)
    {
        mock.Setup(kubernetes =>
                kubernetes.CoreV1.ReadNamespacedSecretWithHttpMessagesAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(),
                    It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new HttpOperationResponse<V1Secret> { Body = result }));

        return mock;
    }

    public static Mock<IKubernetes> WithV1ConfigMapList(this Mock<IKubernetes> mock, V1ConfigMapList result)
    {
        mock.Setup(kubernetes =>
                kubernetes.CoreV1.ListNamespacedConfigMapWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(),
                    It.IsAny<bool?>(), It.IsAny<bool?>(),
                    It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new HttpOperationResponse<V1ConfigMapList> { Body = result }));

        return mock;
    }

    public static Mock<IKubernetes> WithV1SecretList(this Mock<IKubernetes> mock, V1SecretList result)
    {
        mock.Setup(kubernetes =>
                kubernetes.CoreV1.ListNamespacedSecretWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(),
                    It.IsAny<bool?>(), It.IsAny<bool?>(),
                    It.IsAny<IReadOnlyDictionary<string, IReadOnlyList<string>>>(),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new HttpOperationResponse<V1SecretList> { Body = result }));

        return mock;
    }

    public static KubernetesConfigurationSource GetSource(Kinds kind, DataTypes dataTypes, bool reloadOnChange,
        string? fileName = null)
    {
        return new KubernetesConfigurationSource
        {
            Name = "test",
            Namespace = "default",
            FileName = fileName,
            Kind = kind,
            ReloadOnChange = reloadOnChange,
            DataType = dataTypes,
            OnWatchClose = () => TestContext.WriteLine("WatchCloseTest"),
            OnWatchException = TestContext.WriteLine
        };
    }
}