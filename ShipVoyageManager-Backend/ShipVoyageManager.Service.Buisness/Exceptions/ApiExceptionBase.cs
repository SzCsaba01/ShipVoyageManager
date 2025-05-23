﻿using System.Net;

namespace ShipVoyageManager.Service.Buisness.Exceptions;
public class ApiExceptionBase : Exception
{
    public readonly HttpStatusCode Code;
    public string Title { get; }

    public ApiExceptionBase(HttpStatusCode statusCode, string title)
    {
        Code = statusCode;
        Title = title;
    }

    public ApiExceptionBase(HttpStatusCode statusCode, string message, string title) : base(message)
    {
        Code = statusCode;
        Title = title;
    }

    public ApiExceptionBase(HttpStatusCode statusCode, string message, Exception innerException, string title) : base(message, innerException)
    {
        Code = statusCode;
        Title = title;
    }
}