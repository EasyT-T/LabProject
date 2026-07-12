namespace LabProject.Abstractions;

using System;

public interface IExceptionHandler
{
    void HandleException(Exception exception, string message);
}