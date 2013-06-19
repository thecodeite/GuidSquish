using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GuidSquish
{
    [TestFixture]
    class Program
    {
        private const string Base16Values = "0123456789abcdef";
        private const string Base32Values = "0123456789bcdfghjklmnpqrstuvwxyz";
        private const string Base64Values = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ$+";
        private const string Base = Base64Values;

        [TestCase(Base16Values)]
        [TestCase(Base32Values)]
        [TestCase(Base64Values)]
        public void TestSquish(string baseValues)
        {
            // var guid = Guid.Parse("1f9ab66b-24d9-4f5a-8a2e-d4a71216cce2");
            var guid = Guid.Parse("d31d0643-e80e-4f86-8d82-820afb59fd3a");
            //var guid = Guid.NewGuid();

            Console.WriteLine("Guid:        " + guid);

            var squished = Squish(guid, baseValues);

            Console.WriteLine("Squished:    " + squished);

            var unsquished = Unsquish(squished, baseValues);

            Console.WriteLine("Un-squished: " + unsquished);
        }

        [TestCase(1, "1")]
        [TestCase(10, "b")]
        [TestCase(31, "z")]
        [TestCase(32, "10")]
        [TestCase(-32, "-10")]
        public void TestToBase(int num, string expected)
        {
            // Arrange

            // Act
            var result = ToBase(num, Base);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(1, "1")]
        [TestCase(10, "b")]
        [TestCase(31, "z")]
        [TestCase(32, "10")]
        [TestCase(-32, "-10")]
        public void TestFromBase(int expected, string num)
        {
            // Arrange

            // Act
            var result = (int)FromBase(num, Base);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        private static string Squish(Guid guid, string baseValues)
        {
            var hex = "0"+guid.ToString().Replace("-", "");
            var num = BigInteger.Parse(hex, NumberStyles.HexNumber);

            return ToBase(num, baseValues);
        }

        public static string ToBase(BigInteger num, string baseValuesString)
        {
            var builder = new List<char>();
            var baseValues = baseValuesString.ToCharArray();
            var baseLength = baseValues.Length;

            var neg = num < 0;
            if (neg)
            {
                num = -num;
            }

            while (num > 0)
            {
                var segment = (int)(num % baseLength);
                builder.Add(baseValues[segment]);
                num /= baseLength;
            }

            if (neg)
            {
                builder.Add('-');
            }

            builder.Reverse();
            return new string(builder.ToArray());

        }

        public static BigInteger FromBase(string num, string baseValuesString)
        {
            var value = new BigInteger(0);
            var baseValues = baseValuesString.Select((v, i) => new { v, i }).ToDictionary(x => x.v, x => x.i);

            var neg = num.StartsWith("-");
            if (neg)
            {
                num = num.Substring(1);

            }

            foreach (var val in num)
            {
                value *= baseValuesString.Length;
                var intVal = baseValues[val];
                value += intVal;
            }

            if (neg)
            {
                value = -value;
            }

            return value;

        }

        static string Unsquish(string squished, string baseValues)
        {
            var number = FromBase(squished, baseValues);

            var hex = number.ToString("x");

            hex = string.Join("", hex.Select((x, i) => new[] { 8, 12, 16, 20 }.Contains(i) ? "-" + x : "" + x));

            return hex;
        }

        static void Main()
        {
            new Program().TestSquish(Base32Values);
        }
    }
}
