using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnglishTranslator
{
    public static Func<string, string> GetRawFactText = (varName) => varName;

    // Expression cache: maps serialized logic strings → english fact text
    private static Dictionary<string, string> _factTextCache = new Dictionary<string, string>();

    /// Clears the cache (call this at the start or end of a puzzle).
    public static void ClearCache()
    {
        _factTextCache.Clear();
    }

    public static string TranslateFactTextWithCache(ExpressionNode expr)
    {
        string logicKey = expr.ToLogicString();
        Debug.Log($"[Translator] Checking cache for: {logicKey}");

        if (_factTextCache.TryGetValue(logicKey, out string cached))
        {
            Debug.Log($"[Translator] Cache hit for: {logicKey} -> {cached}");
            return cached;
        }

        Debug.Log($"[Translator] Cache miss for: {logicKey}. Translating...");
        string factText = TranslateExpression(expr);
        Debug.Log($"[Translator] Translated '{logicKey}' to: {factText}");

        _factTextCache[logicKey] = factText;
        return factText;
    }

    public static string TranslateProperFactTextWithCache(ExpressionNode expr)
    {
        string factText = TranslateFactTextWithCache(expr);
        return ProperSentence(factText);
    }

    public static string TranslateVariable(string varName, int negationCount)
    {
        string logicKey = varName;
        for (int i = 0; i < negationCount; i++)
            logicKey = "~" + logicKey;

        // Check cache
        if (_factTextCache.TryGetValue(logicKey, out string cached))
        {
            Debug.Log($"[Translator Cache] Cache hit for variable: {logicKey} -> {cached}");
            return cached;
        }

        // Not in cache, so compute it
        string raw = GetRawFactText(varName);
        Debug.Log($"[Translator] Translating variable '{varName}' with {negationCount} negation(s). Raw text: {raw}");

        string notReplacement = "";
        for (int i = 0; i < negationCount; i++)
            notReplacement += "not ";

        string result = raw.Replace("(not) ", notReplacement);
        Debug.Log($"[Translator] Result after applying nots: {result}");

        // Cache the result
        _factTextCache[logicKey] = result;
        Debug.Log($"[Translator Cache] Caching variable: {logicKey} => {result}");

        return result;
    }

    // Capitalizes the first letter and adds a period at the end.
    public static string ProperSentence(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        text = text.Trim();
        text = char.ToUpper(text[0]) + text.Substring(1);
        if (!text.EndsWith("."))
            text += ".";
        return text;
    }

    // Recursively translates an expression node into its English factText.
    // The negationCount parameter allows you to “accumulate” Not operators.
    public static string TranslateExpression(ExpressionNode node, int negationCount = 0)
    {
        if (node is VariableNode varNode)
        {
            return TranslateVariable(varNode.variable, negationCount);
        }
        else if (node is ConstantNode constNode)
        {
            // You can choose to customize constants as needed.
            return constNode.value ? "true" : "false";
        }
        else if (node is OperatorNode opNode)
        {
            // If the operator is a Not, increment the negation count.
            if (opNode.op == OperatorType.Not)
            {
                return TranslateExpression(opNode.operands[0], negationCount + 1);
            }
            else
            {
                // For binary (or n-ary) operators, first translate each operand.
                // We assume the operator is binary for simplicity.
                // Cache the subexpressions too for better performance.
                string leftText = TranslateFactTextWithCache(opNode.operands[0]);
                string rightText = TranslateFactTextWithCache(opNode.operands[1]);
                // For operators, if there's an outer negation that hasn't been “canceled” we'll check for it.
                switch (opNode.op)
                {
                    case OperatorType.Or:
                        Debug.Log($"[Translator] Translating OR: {leftText} v {rightText}, Negated? {negationCount % 2 == 1}");
                        return (negationCount % 2 == 1)
                            ? $"neither {leftText} nor {rightText}"
                            : $"{leftText} or {rightText}";

                    case OperatorType.And:
                        Debug.Log($"[Translator] Translating AND: {leftText} ^ {rightText}, Negated? {negationCount % 2 == 1}");
                        return (negationCount % 2 == 1)
                            ? $"not both {leftText} and {rightText}"
                            : $"{leftText} and {rightText}";

                    case OperatorType.Implies:
                        Debug.Log($"[Translator] Translating IMPLIES: {leftText} => {rightText}, Negated? {negationCount % 2 == 1}");
                        return (negationCount % 2 == 1)
                            ? $"{leftText} doesn't imply {rightText}"
                            : $"if {leftText}, then {rightText}";

                    case OperatorType.Biconditional:
                        Debug.Log($"[Translator] Translating BICONDITIONAL: {leftText} <=> {rightText}, Negated? {negationCount % 2 == 1}");
                        return (negationCount % 2 == 1)
                            ? $"either {leftText} or {rightText}, never both"
                            : $"{leftText} if and only if {rightText}";

                    default:
                        Debug.LogError($"[Translator] Unknown operator type: {opNode.op}");
                        return "";
                }

            }
        }
        else
        {
            return "";
        }
    }
}