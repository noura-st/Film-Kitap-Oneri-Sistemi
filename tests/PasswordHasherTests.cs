using System;
using RecommendationApp;

class Program
{
    static int pass = 0, fail = 0;

    static void Check(string name, bool ok)
    {
        Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + name);
        if (ok) pass++; else fail++;
    }

    static void Main()
    {
        Console.WriteLine("PasswordHasher");

        string h1 = PasswordHasher.Hash("dogru-sifre-123");
        Console.WriteLine("  stored as: " + h1);

        Check("correct password verifies", PasswordHasher.Verify("dogru-sifre-123", h1));
        Check("wrong password rejected", !PasswordHasher.Verify("dogru-sifre-124", h1));
        Check("empty password rejected", !PasswordHasher.Verify("", h1));
        Check("null password rejected", !PasswordHasher.Verify(null, h1));
        Check("plaintext claim rejected", !PasswordHasher.Verify(h1, h1));

        string h2 = PasswordHasher.Hash("dogru-sifre-123");
        Check("same password -> different hash (salt)", h1 != h2);
        Check("both hashes still verify", PasswordHasher.Verify("dogru-sifre-123", h2));

        Check("no plaintext in stored value", h1.IndexOf("dogru-sifre", StringComparison.Ordinal) < 0);
        Check("format is pbkdf2-sha256$iter$salt$hash", h1.Split('$').Length == 4 && h1.StartsWith("pbkdf2-sha256$")); Check("sha256 is named, not the sha1 default", h1.Contains("sha256"));

        Check("garbage hash rejected", !PasswordHasher.Verify("x", "not-a-hash"));
        Check("truncated hash rejected", !PasswordHasher.Verify("x", "pbkdf2$100000$abc"));
        Check("bad base64 rejected", !PasswordHasher.Verify("x", "pbkdf2$100000$!!!$???"));

        Console.WriteLine();
        Console.WriteLine("  " + pass + " passed, " + fail + " failed");
        Environment.Exit(fail == 0 ? 0 : 1);
    }
}
