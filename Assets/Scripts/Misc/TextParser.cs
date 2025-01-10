using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TextParser
{
    public static string[] SplitText(string input)
    {
        // Rozdziel tekst na podstawie przecinków
        var parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        // Usuñ zbêdne spacje z ka¿dego elementu
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].Trim();
        }

        return parts;
    }

    public static int[] AnalyzeWords(string[] words)
    {
        List<int> types = new List<int>();

        // Wyra¿enia regularne do rozpoznawania typów
        Regex registerRegex = new Regex(@"^R[0-9]{1,2}$"); // Dopasowuje rejestry R0-R32
        Regex constantRegex = new Regex(@"^0X[0-9A-Fa-f]+$"); // Dopasowuje liczby szesnastkowe (np. 0x03C)
        Regex constantDecimalRegex = new Regex(@"^\d+$"); // Dopasowuje liczby dziesiatkowe
        Regex offsetRegex = new Regex(@" ^ 0X[0-9A-Fa-f]+\s*\(R[0-9]{1,2}\)$"); // Dopasowuje liczby z przesuniêciem (np. 0x0300(R1))
        Regex labelRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$"); // Dopasowuje etykiety (np. secondLoop)

        foreach (var word in words)
        {
            string trimmedWord = word.Trim(); // Usuñ niepotrzebne spacje
            trimmedWord = trimmedWord.ToUpper();

            if (registerRegex.IsMatch(trimmedWord))
            {
                types.Add(0); // Rejestr
            }
            else if (constantRegex.IsMatch(trimmedWord) || constantDecimalRegex.IsMatch(trimmedWord))
            {
                types.Add(1); // Liczba sta³a
            }
            else if (offsetRegex.IsMatch(trimmedWord))
            {
                types.Add(3); // Liczba sta³a z przesuniêciem
            }
            else if (labelRegex.IsMatch(trimmedWord))
            {
                types.Add(2); // Etykieta
            }
            else
            {
                types.Add(4);
                //throw new Exception($"Nieznany typ: {trimmedWord}");
            }
        }

        return types.ToArray();
    }

    public static string indicateErrors(string input, List<string> labels)
    {
        string[] analyzedWords = SplitText(input);
        int[] types = AnalyzeWords(analyzedWords);
        string result = "";
        for (int i = 0; i < types.Length; i++)
        {
            result += analyzedWords[i];
            if (!analyzedWords[i].Contains("<b³¹d>")) {
                Debug.Log("Prawdobodone zle slowo: " + analyzedWords[i]);
                if (types[i] == 2)
                {
                    if (!labels.Contains(analyzedWords[i]))
                    {
                        result += "<b³¹d>";
                    }
                }
                if (types[i] == 4)
                {
                    result += "<b³¹d>";
                }
            }
            if (i != types.Length - 1)
            {
                result += ", ";
            }
        }
        return result;
    }

    public static void TestParser()
    {
        var testInputs = new List<string>
        {
            "R4, 0x0004, R4",
            "R5, R2, R9, R10",
            "R3, 0x0300(R1)",
            "R5, nextStepInLoop"
        };

        foreach (var input in testInputs)
        {
            Console.WriteLine($"Tekst wejœciowy: \"{input}\"");

            // Parsowanie
            var splitText = SplitText(input);
            Console.WriteLine("Rozdzielone s³owa: " + string.Join(", ", splitText));

            // Analiza znaczenia
            var types = AnalyzeWords(splitText);
            Console.WriteLine("Typy: " + string.Join(", ", types));
            Console.WriteLine();
        }
    }
}

