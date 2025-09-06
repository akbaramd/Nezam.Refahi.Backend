using System;
using System.Linq; // Added missing import for .All()

// Test the NationalId validation for 2741153671
public class TestNationalId
{
    public static void Main()
    {
        string testId = "2741153671";
        
        // Test the validation algorithm manually
        Console.WriteLine($"Testing National ID: {testId}");
        
        // Basic validation
        if (testId.Length != 10 || !testId.All(char.IsDigit))
        {
            Console.WriteLine("❌ Basic validation failed");
            return;
        }
        Console.WriteLine("✅ Basic validation passed");
        
        // Check for repeated digits
        bool allDigitsSame = true;
        for (int i = 1; i < testId.Length; i++)
        {
            if (testId[i] != testId[0])
            {
                allDigitsSame = false;
                break;
            }
        }
        if (allDigitsSame)
        {
            Console.WriteLine("❌ All digits are the same");
            return;
        }
        Console.WriteLine("✅ Repeated digits check passed");
        
        // Check digit validation
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (testId[i] - '0') * (10 - i);
        }
        
        int remainder = sum % 11;
        int checkDigit = testId[9] - '0';
        
        Console.WriteLine($"Sum: {sum}");
        Console.WriteLine($"Remainder: {remainder}");
        Console.WriteLine($"Check digit: {checkDigit}");
        
        bool isValid;
        if (remainder < 2)
        {
            isValid = checkDigit == remainder;
            Console.WriteLine($"Expected check digit: {remainder}");
        }
        else
        {
            isValid = checkDigit == (11 - remainder);
            Console.WriteLine($"Expected check digit: {11 - remainder}");
        }
        
        if (isValid)
        {
            Console.WriteLine("✅ Check digit validation passed");
            Console.WriteLine($"✅ National ID {testId} is VALID");
        }
        else
        {
            Console.WriteLine("❌ Check digit validation failed");
            Console.WriteLine($"❌ National ID {testId} is INVALID");
        }
    }
}
