using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Util;

public class EntryCheck
{
    private readonly List<string> _errors = new();

    public IReadOnlyList<string> Errors => _errors;

    public bool IsValid => _errors.Count == 0;


    // Required: string cannot be null/empty/whitespace
    public EntryCheck IsNotNullOrWhitespace(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add($"{fieldName} is required.");
        }

        return this;
    }

    // integer cannot be -1 or less
    public EntryCheck IsNotNegative(int value, string fieldName)
    {
        if (value < 0)
        {
            _errors.Add($"{fieldName} must be 0(zero) or more.");
        }

        return this;
    }

    // integer cannot be 0 or less
    public EntryCheck IsNotZeroOrLess(int value, string fieldName)
    {
        if (value <= 0)
        {
            _errors.Add($"{fieldName} must be 1(one) or more.");
        }

        return this;
    }

    // future validations (max length, regex, etc)
    public EntryCheck RequireMaxLength(string? value, int max, string fieldName)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > max)
        {
            _errors.Add($"{fieldName} cannot exceed {max} characters.");
        }

        return this;
    }
}
