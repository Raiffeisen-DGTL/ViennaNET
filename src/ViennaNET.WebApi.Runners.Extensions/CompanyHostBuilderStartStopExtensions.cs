using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Runners.Extensions
{
    /// <summary>
    ///   Методы расширения для удобной работы со Start/Stop действиями через SimpleInjector
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Тип будет удалён в последующем рефакторинге.")]
    public static class CompanyHostBuilderStartStopExtensions
    {
        /// <summary>
        ///   Добавляет действие при старте сервиса
        /// </summary>
        /// <typeparam name="T">Класс, содержащий действие</typeparam>
        /// <param name="companyHostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ICompanyHostBuilder AddOnStartAction<T>(this ICompanyHostBuilder companyHostBuilder,
            Action<T> action)
            where T : class
        {
            companyHostBuilder.AddOnStartAction(container => GetAction(container, action));
            return companyHostBuilder;
        }

        /// <summary>
        ///   Добавляет действие перед остановкой сервиса
        /// </summary>
        /// <typeparam name="T">Класс, содержащий действие</typeparam>
        /// <param name="companyHostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ICompanyHostBuilder AddOnStoppingAction<T>(this ICompanyHostBuilder companyHostBuilder,
            Action<T> action)
            where T : class
        {
            companyHostBuilder.AddOnStoppingAction(container => GetAction(container, action));
            return companyHostBuilder;
        }

        /// <summary>
        ///   Добавляет действие при остановке сервиса
        /// </summary>
        /// <typeparam name="T">Класс, содержащий действие</typeparam>
        /// <param name="companyHostBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ICompanyHostBuilder AddOnStoppedAction<T>(this ICompanyHostBuilder companyHostBuilder,
            Action<T> action)
            where T : class
        {
            companyHostBuilder.AddOnStoppedAction(container => GetAction(container, action));
            return companyHostBuilder;
        }

        private static Action GetAction<T>(this object container, Action<T>? action) where T : class
        {
            // Удалить в послед. рефакторинге, как и класс в целом.
            var serviceProvider = (Container)container;
            var loggerFactory = serviceProvider.GetInstance<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Start/Stop Action"); 
            
            try
            {
                if (action != null)
                {
                    return () => action(serviceProvider.GetInstance<T>());
                }

                return () =>
                {
                };
            }
            catch (ActivationException exception)
            {
                logger.LogError(exception, "Could not to get instance of type: {Type}", typeof(T));
                return () =>
                {
                };
            }
        }
    }
}