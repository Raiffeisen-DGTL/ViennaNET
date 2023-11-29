using System.Diagnostics.CodeAnalysis;

namespace ViennaNET.Extensions.Mediator.Tests.Models;

[ExcludeFromCodeCoverage(Justification = "Используется для тестирования.")]
public class Event
{
    public Event(int value)
    {
        Value = value;
    }

    public int Value { get; set; }
}