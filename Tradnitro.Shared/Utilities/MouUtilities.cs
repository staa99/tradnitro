using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;


namespace Tradnitro.Shared.Utilities
{
    public static class MouUtilities
    {
        private static readonly string[] TensMap =
        {
            "zero",
            "ten",
            "twenty",
            "thirty",
            "forty",
            "fifty",
            "sixty",
            "seventy",
            "eighty",
            "ninety"
        };

        private static readonly string[] UnitsMap =
        {
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "ten",
            "eleven",
            "twelve",
            "thirteen",
            "fourteen",
            "fifteen",
            "sixteen",
            "seventeen",
            "eighteen",
            "nineteen"
        };


        public static WordDocument DecodeFromString(string base64EncodedString)
        {
            var bytes = Convert.FromBase64String(base64EncodedString);
            var wordDocument = new WordDocument(new MemoryStream(bytes),
                                                FormatType.Docx);

            return wordDocument;
        }


        public static string EncodeToString(byte[] rawDocBytes) =>
            Convert.ToBase64String(rawDocBytes);


        public static Task GeneratePdf(string   templateString,
                                       string   reference,
                                       string   fullName,
                                       decimal  amountInvested,
                                       int      numberOfMonths,
                                       float    interestPercentage,
                                       DateTime datePaymentVerified,
                                       DateTime nextPayoutDate,
                                       bool     interestAndCapital,
                                       string   savePath,
                                       string   backupPath) =>
            Task.Run(() =>
            {
                var document = DecodeFromString(templateString);

                var name = fullName;
                var amount = GetNumberAsText(amountInvested);
                var percentage = $"{interestPercentage}%";
                var datePayIn = FormatDate(datePaymentVerified);
                var datePayOut = FormatDate(nextPayoutDate);
                var interestOrAll =
                    $"{(interestAndCapital ? "initial amount plus interest" : "interest")}";

                var resultingAmount = GetAmountToBePaid(amountInvested,
                                                        interestPercentage,
                                                        interestAndCapital);

                document.Replace("{{NAME}}",
                                 name,
                                 true,
                                 true);

                document.Replace("{{REFERENCE}}",
                                 reference,
                                 true,
                                 true);

                document.Replace("{{AMOUNT}}",
                                 amount,
                                 true,
                                 true);

                document.Replace("{{NO_OF_MONTHS}}",
                                 numberOfMonths.ToString(),
                                 true,
                                 true);

                document.Replace("{{PERCENTAGE}}",
                                 percentage,
                                 true,
                                 true);

                document.Replace("{{DATE_PAY_IN}}",
                                 datePayIn,
                                 true,
                                 true);

                document.Replace("{{DATE_PAY_OUT}}",
                                 datePayOut,
                                 true,
                                 true);

                document.Replace("{{INTEREST_OR_ALL}}",
                                 interestOrAll,
                                 true,
                                 true);

                document.Replace("{{RESULTING_AMOUNT}}",
                                 resultingAmount,
                                 true,
                                 true);

                var fileName = $"MOU-{reference}.pdf";
                //Instantiation of DocIORenderer for Word to PDF conversion
                var render = new DocIORenderer
                {
                    Settings =
                    {
                        AutoTag = true,
                        AutoDetectComplexScript = true,
                        OptimizeIdenticalImages = true,
                        PreserveFormFields = true,
                        UpdateDocumentFields = true,
                        EmbedFonts = true
                    }
                };

                //Sets true to preserve document structured tags in the converted PDF document 
                //Converts Word document into PDF document
                var pdfDocument = render.ConvertToPDF(document);
                //Releases all resources used by the Word document and DocIO Renderer objects
                render.Dispose();
                document.Dispose();
                //Saves the PDF file
                var mainStream = File.OpenWrite(Path.Combine(savePath,
                                                             fileName));

                pdfDocument.Save(mainStream);
                pdfDocument.Dispose();
                mainStream.Dispose();

                File.Copy(Path.Combine(savePath,
                                       fileName),
                          Path.Combine(backupPath,
                                       $"{fileName}.bak"),
                          true);
            });


        public static bool IsCompleted(int    payoutCounts,
                                       string formula)
        {
            var payoutPercents = formula.Split(',')
                                        .Select(float.Parse)
                                         //.Where(i => i > 0)
                                         // allow dropping zero as per the formula
                                        .ToImmutableList();

            return payoutCounts >= payoutPercents.Count;
        }


        public static decimal NextPayoutAmount(decimal amountInvested,
                                               int     payoutCounts,
                                               string  formula)
        {
            var payoutPercents = formula.Split(',')
                                        .Select(s => (int) (float.Parse(s) * 100))
                                        .ToImmutableList();

            while (payoutCounts < payoutPercents.Count && payoutPercents[payoutCounts] == 0)
            {
                payoutCounts++;
            }

            return payoutCounts >= payoutPercents.Count
                       ? 0
                       : amountInvested * payoutPercents[payoutCounts] / 10000;
        }


        public static DateTime? NextPayoutDate(DateTime dateEffectedOrLastPayout,
                                               int      payoutCounts,
                                               string   formula,
                                               int      noOfAdditionalDays,
                                               bool     skipFirstWeekend,
                                               bool     skipAllWeekends)
        {
            var payoutPercents = formula.Split(',')
                                        .Select(s => (int) float.Parse(s))
                                        .ToImmutableList();

            if (payoutCounts < 0)
            {
                payoutCounts = 0;
            }

            // short circuit failures as a temporary measure
            // prevent further payment for error cases
            if (payoutCounts == payoutPercents.Count(p => p > 0))
            {
                return null;
            }

            while (payoutCounts < payoutPercents.Count && payoutPercents[payoutCounts] == 0)
            {
                payoutCounts++;
            }

            if (payoutCounts >= payoutPercents.Count)
            {
                return null;
            }
            
            var month = dateEffectedOrLastPayout.Month + payoutCounts + 1;
            var year = dateEffectedOrLastPayout.Year;
            if (month >= 13)
            {
                month %= 12;
                if (month == 0)
                {
                    month = 12;
                }
                year++;
            }

            var noOfDaysInMonth = GetNumberOfDaysInMonth(month, year);
            var day = noOfDaysInMonth < dateEffectedOrLastPayout.Day ? noOfDaysInMonth : dateEffectedOrLastPayout.Day;
            var next = new DateTime(year, month, day, dateEffectedOrLastPayout.Hour, dateEffectedOrLastPayout.Minute, dateEffectedOrLastPayout.Second);

            if (dateEffectedOrLastPayout.Day > next.Day)
            {
                next = next.AddDays(dateEffectedOrLastPayout.Day - next.Day);
            }

            //return skipFirstWeekend
            //           ? next.AddDays(2)
            //           : skipAllWeekends
            //               ? next.AddDays(8)
            //               : next;
            return next.AddDays(noOfAdditionalDays);
        }


        private static string FormatDate(DateTime date)
        {
            var strDate = $"{date:MMMM d, yyyy}";
            return strDate;
        }


        private static string GetAmountToBePaid(decimal amount,
                                                float   percentage,
                                                bool    interestOrAll)
        {
            var interest = Convert.ToInt32(percentage * 100) / 10000M;
            var ret = amount *
                      (interestOrAll
                           ? 1 + interest
                           : interest);

            return GetNumberAsText(ret);
        }


        private static string GetNumberAsText(decimal amount) =>
            $"{amount:N} naira";


        private static int GetNumberOfDaysInMonth(int month, int year)
        {
            switch (month)
            {
                case 9:
                case 4:
                case 6:
                case 11:
                    return 30;
                case 2:
                    var isLeapYear = year % 100 != 0 && year % 4 == 0 || year % 400 == 0;
                    return isLeapYear ? 29 : 28;
                default:
                    return 31;
            }
        }


        private static string NumberToWords(int number)
        {
            if (number == 0)
            {
                return "zero";
            }

            if (number < 0)
            {
                return "minus " + NumberToWords(Math.Abs(number));
            }

            var words = "";

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number <= 0)
            {
                return words;
            }

            if (words != "")
            {
                words += "and ";
            }

            if (number < 20)
            {
                words += UnitsMap[number];
            }
            else
            {
                words += TensMap[number / 10];
                if (number % 10 > 0)
                {
                    words += "-" + UnitsMap[number % 10];
                }
            }

            return words;
        }


        private static string NumberToWords(decimal number)
        {
            // first resolve the decimal part
            var s = number.ToString(CultureInfo.InvariantCulture);
            s = Regex.Replace(s,
                              "[^\\d\\.]+",
                              "");

            var dotIndex = s.IndexOf('.');

            var n = (int) number;
            if (dotIndex == -1)
            {
                return n == 0
                           ? string.Empty
                           : $"{NumberToWords(n)} Naira only";
            }

            var decimalPart = int.Parse(s.Substring(dotIndex + 1,
                                                    Math.Min(2,
                                                             s.Length - dotIndex - 1)));

            return $"{NumberToWords(n)} Naira and {NumberToWords(decimalPart)} Kobo only";
        }
    }
}