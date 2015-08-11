using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace KVSWebApplication.ImportExport
{
    /// <summary>
    /// Hilfsklasse fuer das Exportieren der Rechnung ins DateV
    /// </summary>
    public static class DateVExport
    {

        //ASCII code 30 = RS ( Record separator )
        private const byte rs = 30;
        //ASCII code 28 = FS ( separator )
        private const byte fs = 28;
        //ASCII code 24 = CAN ( Cancel ) 
        private const byte can = 24;
        //ASCII code 29 = GS ( Group separator ) 
        private const byte gs = 29;

        public static string CreateExport(List<VirtualInvoice> invoices)
        {
            var result = String.Empty;

            var rsAsString = ByteArrayToString(new byte[] { rs });
            var fsAsString = ByteArrayToString(new byte[] { fs });

            string decodeBegin = Encoding.ASCII.GetString(new byte[] { gs, can }, 0, new byte[] { gs, can }.Length);
            var beginText = String.Format("{0}{1}{2}{3}{4}{5}",
                decodeBegin,
                ConfigurationManager.AppSettings["DateVBeginText"],
                Convert.ToString(invoices.Max(q => q.Invoice.CreateDate).Year).Substring(2, 2),
                invoices.Min(q => q.Invoice.CreateDate).ToString("ddMMyy"),
                invoices.Max(q => q.Invoice.CreateDate).ToString("ddMMyy"),
                ConfigurationManager.AppSettings["DateVBeginTextAdditionalInfo"]
                );


            long totalSum = 0;
            var invoiceTexts = GetInvoicePositions(invoices, ref totalSum);
            var endText = String.Format("{0}{1}yz", totalSum > 0 ? "yx" : "yw", totalSum);

            var substring = String.Empty;
            for (int i = 0; i < invoiceTexts.Count; i++)
            {
                var checkString = String.Format("{0}{1}{2}",
                    String.IsNullOrEmpty(substring) ? (i == 0 ? beginText + invoiceTexts[0] : invoiceTexts[i]) : substring,
                    i < (invoiceTexts.Count - 1) ? invoiceTexts[i + 1] : String.Empty,
                    i == (invoiceTexts.Count - 1) ? endText : String.Empty
                    );

                if (!GreatherThan(checkString))
                {
                    if (String.IsNullOrEmpty(substring))
                    {
                        substring = checkString;
                    }
                    else if (i < (invoiceTexts.Count - 1))
                    {
                        substring += invoiceTexts[i + 1];
                    }

                    if (i == (invoiceTexts.Count - 1))
                    {
                        substring += endText;
                        result += CreateSpecialArray(substring);
                    }
                }
                else
                {
                    if (i < (invoiceTexts.Count - 1))
                    {
                        result += CreateSpecialArray(substring + " ", true);
                        substring = "";
                    }

                    if (i == (invoiceTexts.Count - 1))
                    {
                        result += CreateSpecialArray(substring + endText);
                    }
                }
            }

            return result;
        }

        private static List<string> GetInvoicePositions(List<VirtualInvoice> invoices, ref long totalSum)
        {
            var invoiceTexts = new List<string>();
            totalSum = 0;

            var selectedInvoiceId = 0;
            foreach (var invoice in invoices)
            {
                if (selectedInvoiceId != invoice.Invoice.Id)
                {
                    var groupedInvoices = invoices.FirstOrDefault(q => q.Invoice.Id == invoice.Invoice.Id);
                    selectedInvoiceId = groupedInvoices.Invoice.Id;
                    totalSum += long.Parse(groupedInvoices.Invoice.GrandTotal.ToString("F").Replace(".", "").Replace(",", ""));
                }

                invoiceTexts.Add(String.Format("{0}y+{1}a{2}b{3}d{4}e{5}{6}{7}{8}{9}{10}",
                    String.Empty,//ByteArrayToString(new byte[] { fs }),
                    long.Parse((invoice.accountSum).ToString("F").Replace(".", "").Replace(",", "")),
                    invoice.account,
                    invoice.Invoice.InvoiceNumber.Number,
                    invoice.Invoice.CreateDate.ToString("ddMM"),
                    !String.IsNullOrEmpty(invoice.Invoice.Customer.Debitornumber) ? invoice.Invoice.Customer.Debitornumber : invoice.account,
                    ByteArrayToString(new byte[] { rs }),
                    String.Empty,//String.Format("{0}-{1}", invoice.Invoice.InvoiceTypes.contraction, invoice.Invoice.InvoiceNumber.Number),
                    Purge(invoice.Invoice.Customer.Name),
                    String.Empty, //j{Steuersatz}o{WKZ},
                    ByteArrayToString(new byte[] { fs })
                    ));
            }

            return invoiceTexts;
        }

        private static bool GreatherThan(string customString)
        {
            byte[] currArray = StringToByteArray(customString);
            if (currArray.Length > 250)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string CreateSpecialArray(string customString, bool replaceEmptyFieldsWithBlanks = false)
        {
            byte[] returnArray = new byte[256];
            byte[] helperArray;

            if (replaceEmptyFieldsWithBlanks)
            {
                for (int i = 0; i < 250; i++)
                    returnArray[i] = (byte)' ';
            }

            int counter = 0;
            helperArray = StringToByteArray(customString);
            foreach (byte item in helperArray)
            {
                returnArray[counter] = item;
                counter++;
            }

            return ByteArrayToString(returnArray);
        }

        private static byte[] StringToByteArray(string str)
        {
            var enc = new ASCIIEncoding();
            return enc.GetBytes(str);
        }

        private static string ByteArrayToString(byte[] arr)
        {
            var enc = new ASCIIEncoding();
            return enc.GetString(arr);
        }

        public static String Purge(String text)
        {
            var htmlk = new StringBuilder();
            int ia = 0;
            for (int i = 0; i < text.Length; i++)
            {
                Char c = text[i];

                var criticals = HTMLCharacterConverter();

                if (criticals.ContainsKey(c))
                {
                    htmlk.Append(text.Substring(ia, i - ia));
                    htmlk.Append(criticals[c]);
                    ia = i + 1;
                }
            }
            htmlk.Append(text.Substring(ia));
            return htmlk.ToString();
        }

        public static Dictionary<char, string> HTMLCharacterConverter()
        {
            var criticals = new Dictionary<char, string>();

            criticals.Add('"', "”");
            criticals.Add('>', "”");
            criticals.Add('<', "”");
            criticals.Add('¡', "”");
            criticals.Add('¢', "”");
            criticals.Add('£', "”");
            criticals.Add('¤', "”");
            criticals.Add('¥', "”");
            criticals.Add('¦', "”");
            criticals.Add('§', "”");
            criticals.Add('¨', "”");
            criticals.Add('©', "”");
            criticals.Add('ª', "”");
            criticals.Add('«', "”");
            criticals.Add('¬', "”");
            criticals.Add('­', "”");
            criticals.Add('®', "”");
            criticals.Add('¯', "”");
            criticals.Add('°', "”");
            criticals.Add('±', "”");
            criticals.Add('²', "”");
            criticals.Add('³', "”");
            criticals.Add('´', "”");
            criticals.Add('µ', "”");
            criticals.Add('¶', "”");
            criticals.Add('·', "”");
            criticals.Add('¸', "”");
            criticals.Add('¹', "”");
            criticals.Add('º', "”");
            criticals.Add('»', "”");
            criticals.Add('¼', "”");
            criticals.Add('½', "”");
            criticals.Add('¾', "”");
            criticals.Add('¿', "”");
            criticals.Add('À', "”");
            criticals.Add('Á', "”");
            criticals.Add('Â', "”");
            criticals.Add('Ã', "”");
            criticals.Add('Ä', "”");
            criticals.Add('Å', "”");
            criticals.Add('Æ', "”");
            criticals.Add('Ç', "”");
            criticals.Add('È', "”");
            criticals.Add('É', "”");
            criticals.Add('Ê', "”");
            criticals.Add('Ë', "”");
            criticals.Add('Ì', "”");
            criticals.Add('Í', "”");
            criticals.Add('Î', "”");
            criticals.Add('Ï', "”");
            criticals.Add('Ð', "”");
            criticals.Add('Ñ', "”");
            criticals.Add('Ò', "”");
            criticals.Add('Ó', "”");
            criticals.Add('Ô', "”");
            criticals.Add('Õ', "”");
            criticals.Add('Ö', "”");
            criticals.Add('×', "”");
            criticals.Add('Ø', "”");
            criticals.Add('Ù', "”");
            criticals.Add('Ú', "”");
            criticals.Add('Û', "”");
            criticals.Add('Ü', "”");
            criticals.Add('Ý', "”");
            criticals.Add('Þ', "”");
            criticals.Add('ß', "”");
            criticals.Add('à', "”");
            criticals.Add('á', "”");
            criticals.Add('â', "”");
            criticals.Add('ã', "”");
            criticals.Add('ä', "”");
            criticals.Add('å', "”");
            criticals.Add('æ', "”");
            criticals.Add('ç', "”");
            criticals.Add('è', "”");
            criticals.Add('é', "”");
            criticals.Add('ê', "”");
            criticals.Add('ë', "”");
            criticals.Add('ì', "”");
            criticals.Add('í', "”");
            criticals.Add('î', "”");
            criticals.Add('ï', "”");
            criticals.Add('ð', "”");
            criticals.Add('ñ', "”");
            criticals.Add('ò', "”");
            criticals.Add('ó', "”");
            criticals.Add('ô', "”");
            criticals.Add('õ', "”");
            criticals.Add('ö', "”");
            criticals.Add('÷', "”");
            criticals.Add('ø', "”");
            criticals.Add('ù', "”");
            criticals.Add('ú', "”");
            criticals.Add('û', "”");
            criticals.Add('ü', "”");
            criticals.Add('ý', "”");
            criticals.Add('þ', "”");
            criticals.Add('ÿ', "”");


            //TODO delete
            criticals = new Dictionary<char, string>();

            return criticals;
        }
    }
}