using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TextParser
{
    /// <summary>
    /// Rozdziela kolejne s�owa po przecinkach do tablicy String
    /// </summary>
    /// <param name="input">
    /// Zdanie zawieraj�ce s�owa rozdzielone przecinkami.
    /// </param>
    /// <returns>
    /// Tablic� wyraz�w
    /// </returns>
    public static string[] SplitText(string input)
    {
        // Rozdziel tekst na podstawie przecink�w
        var parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        // Usu� zb�dne spacje z ka�dego elementu
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].Trim();
        }

        return parts;
    }

    /// <summary>
    /// Analizuje typ danych s��w
    /// </summary>
    /// <param name="words">
    /// Tablica s��w
    /// </param>
    /// <returns>
    /// Tablic� liczb <c>int</c>
    /// <list type="bullet">
    /// <item>
    /// <description>0 - Rejestr</description>
    /// </item>
    /// <item>
    /// <description>1 - Liczba sta�a</description>
    /// </item>
    /// <item>
    /// <description>2 - Etykieta</description>
    /// </item>
    /// <item>
    /// <description>3 - Liczba sta�a z przesuni�ciem</description>
    /// </item>
    /// <item>
    /// <description>4 - Nieznany typ</description>
    /// </item>
    /// </list>
    /// </returns>
    public static int[] AnalyzeWords(string[] words)
    {
        List<int> types = new List<int>();

        // Wyra�enia regularne do rozpoznawania typ�w
        Regex registerRegex = new Regex(@"^R[0-9]{1,2}$"); // Dopasowuje rejestry R0-R32
        Regex constantRegex = new Regex(@"^0X[0-9A-Fa-f]+$"); // Dopasowuje liczby szesnastkowe (np. 0x03C)
        Regex constantDecimalRegex = new Regex(@"^\d+$"); // Dopasowuje liczby dziesiatkowe
        Regex offsetRegex = new Regex(@"^0X[0-9A-Fa-f]+\s*\(R[0-9]{1,2}\)$"); // Dopasowuje liczby z przesuni�ciem (np. 0x0300(R1))
        Regex labelRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$"); // Dopasowuje etykiety (np. secondLoop)

        foreach (var word in words)
        {
            string trimmedWord = word.Trim(); // Usu� niepotrzebne spacje
            trimmedWord = trimmedWord.ToUpper();

            if (registerRegex.IsMatch(trimmedWord))
            {
                types.Add(0); // Rejestr
            }
            else if (constantRegex.IsMatch(trimmedWord) || constantDecimalRegex.IsMatch(trimmedWord))
            {
                types.Add(1); // Liczba sta�a
            }
            else if (offsetRegex.IsMatch(trimmedWord))
            {
                types.Add(3); // Liczba sta�a z przesuni�ciem
            }
            else if (labelRegex.IsMatch(trimmedWord))
            {
                types.Add(2); // Etykieta
            }
            else
            {
                Debug.Log(trimmedWord);
                types.Add(4);
                //throw new Exception($"Nieznany typ: {trimmedWord}");
            }
        }

        return types.ToArray();
    }

    /// <summary>
    /// Wykrywa b��dy w zapisanej instrukcji i oznacza b��dne pozycje
    /// </summary>
    /// <param name="input">
    /// Instrukcja do przeanalizowania
    /// </param>
    /// <param name="labels">
    /// Lista wszystkich etykiet dost�pnych w li�cie instrukcji
    /// </param>
    /// <returns>
    /// Instrukcj� z naniesionymi oznaczeniami b��du, je�li zosta�y wykryte
    /// </returns>
    public static string IndicateErrors(string input, List<string> labels)
    {
        string[] analyzedWords = SplitText(input);
        int[] types = AnalyzeWords(analyzedWords);
        string result = "";
        for (int i = 0; i < types.Length; i++)
        {
            result += analyzedWords[i];
            if (!analyzedWords[i].Contains("<b��d>")) {
                if (types[i] == 2)
                {
                    if (!labels.Contains(analyzedWords[i]))
                    {
                        Debug.Log("Nie ma takiej etykiety!");
                        result += "<b��d>";
                    }
                }
                if (types[i] == 4)
                {
                    Debug.Log("Niedozwolony ci�g znak�w!");
                    result += "<b��d>";
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
            Console.WriteLine($"Tekst wej�ciowy: \"{input}\"");

            // Parsowanie
            var splitText = SplitText(input);
            Console.WriteLine("Rozdzielone s�owa: " + string.Join(", ", splitText));

            // Analiza znaczenia
            var types = AnalyzeWords(splitText);
            Console.WriteLine("Typy: " + string.Join(", ", types));
            Console.WriteLine();
        }
    }
}

