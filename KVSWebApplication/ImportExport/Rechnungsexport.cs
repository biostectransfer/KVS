using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KVSCommon.Database;
using System.Text;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
namespace KVSWebApplication.ImportExport
{
    /// <summary>
    /// Hilfsklasse fuer das Exportieren der Rechnung ins DAT
    /// </summary>
    public  class Rechnungsexport
    {
        private long bruttobetrag=0;
        private string completeByteString = string.Empty;
        public string CompleteByteString
        {
            get { return completeByteString; }            
        }
        private List<VirtualInvoice> invs;
        private Dictionary<char, string> criticals = new Dictionary<char, string>();
        public Rechnungsexport(List<VirtualInvoice> invoice)
        {
            this.invs = invoice;       
            //SCII code 29 = GS ( Group separator ) 
            //ASCII code 30 = RS ( Record separator )
            //ASCII code 24 = CAN ( Cancel )                                                                                                                      
            // HTMLCharacterConverter();
            string firstY = "y";
            string olySeparator = "o1y";
            string endX = "x";
            string endYZ = "yz";
            string myBeginText = string.Empty;
            int counter = 0;
            bool isGreather = false;
            bool hasFinished = false;
            byte rs = 30;
            byte fs = 28;
            List<string> myInvoices = new List<string>();
            byte can = 24; byte gs = 29;
            string decodeBegin = Encoding.ASCII.GetString(new byte[] { gs, can }, 0, new byte[] { gs, can }.Length);
            myBeginText = decodeBegin + "11  11FK0000001000010001" + Convert.ToString(invs.Max(q => q.Invoice.CreateDate).Year).Substring(2, 2) + invs.Min(q => q.Invoice.CreateDate).ToString("ddMMyy") + invs.Max(q => q.Invoice.CreateDate).ToString("ddMMyy")
                + "001                                    ";
            completeByteString += myBeginText;
            completeByteString += firstY;
            var selectedInvoiceId = 0;
            foreach (VirtualInvoice inv in invs)
            {
                if (selectedInvoiceId != inv.Invoice.Id)
                {
                    var groupedInvoices = invs.FirstOrDefault(q => q.Invoice.Id == inv.Invoice.Id);
                    selectedInvoiceId = groupedInvoices.Invoice.Id;
                    bruttobetrag += long.Parse(( 
                        groupedInvoices.Invoice.GrandTotal
                        .ToString("F").Replace(".", "").Replace(",", "")));
                }            
                myInvoices.Add(long.Parse((inv.accountSum).ToString("F").Replace(".", "").Replace(",", "")) + "a" +inv.account +
                     "b" + inv.Invoice.InvoiceNumber.Number + "d" + inv.Invoice.CreateDate.ToString("ddMM") +
                     "e" + inv.Invoice.Customer.Debitornumber + ByteArrayToString(new byte[] { rs }) + inv.Invoice.InvoiceTypes.contraction + "-" + inv.Invoice.InvoiceNumber.Number +
                     "," + Purge(inv.Invoice.Customer.Name));
            }
            string mySubstring = myInvoices[0];
            for (int i = 0; i < myInvoices.Count; i++)
            {
                if (counter == 0)
                {
                    while (!isGreather)
                    {
                        if (!GreatherThan(myBeginText + firstY + "+" + mySubstring + " " + olySeparator + (((myInvoices.Count - 1) >= (i + 1)) ? "+" + myInvoices[i + 1] +ByteArrayToString(new byte[] { fs })+ olySeparator : "") + endX + bruttobetrag.ToString() + endYZ) && i < (myInvoices.Count - 1))
                        {
                            i++;
                            mySubstring += ByteArrayToString(new byte[] { fs }) + olySeparator + "+" + myInvoices[i];
                        }
                        else
                        {
                            isGreather = true;
                            counter++;
                       

                            if (i < (myInvoices.Count - 1))
                            {
                                completeByteString = CreateSpecialArray(completeByteString + "+" + mySubstring + ByteArrayToString(new byte[] { fs }) + olySeparator);
                                mySubstring = "";
                                i++;
                                break;
                            }
                            if (i == (myInvoices.Count - 1))
                            {
                                string separatString = "+" + mySubstring + " " + ByteArrayToString(new byte[] { fs }) + olySeparator + endX + bruttobetrag.ToString() + endYZ;


                                completeByteString = CreateSpecialArray(completeByteString + separatString);
                                hasFinished = true;
                                mySubstring = "";
                                i++;
                                break;
                            }
                        }
                    }
                }
                if (i < (myInvoices.Count - 1) && isGreather == true)
                {
                    if (GreatherThan(mySubstring + " " + ByteArrayToString(new byte[] { fs }) +  olySeparator + myInvoices[i] + ((myInvoices.Count >= (i + 1)) ? "+" + myInvoices[i + 1] + olySeparator : "") + endX + bruttobetrag.ToString() + endYZ))
                    {
                        completeByteString += CreateSpecialArray(mySubstring + ByteArrayToString(new byte[] { fs }) + " " + olySeparator + "+" + myInvoices[i] + ByteArrayToString(new byte[] { fs }) + " " + olySeparator);
                        mySubstring = "";
                    }
                    else
                    {
                        if (mySubstring != string.Empty && !mySubstring.Substring(mySubstring.Count()-5).Contains(olySeparator))
                        {
                            mySubstring += ByteArrayToString(new byte[] { fs }) + olySeparator + "+" + myInvoices[i];
                        }
                        else
                        mySubstring += "+" + myInvoices[i];
                    }
                }
                else if (i == (myInvoices.Count - 1) && hasFinished == false)
                {
                    if (mySubstring == string.Empty)
                    {
                        string separatString = "+"+myInvoices[i]+ByteArrayToString(new byte[] { fs }) + olySeparator + endX + bruttobetrag.ToString() + endYZ;
                        completeByteString += CreateSpecialArray(separatString);
                        break;
                    }
                    else
                    {
                        string separatString = mySubstring + " " + ByteArrayToString(new byte[] { fs }) + olySeparator + "+" +myInvoices[i] + ByteArrayToString(new byte[] { fs }) + olySeparator + endX + bruttobetrag.ToString() + endYZ;
                        completeByteString += CreateSpecialArray(separatString);
                        break;
                    }
                }
            }
        }
        protected bool GreatherThan(string customString)
        {
            byte[] currArray = StringToByteArray(customString);
            if (currArray.Length > 246)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected string CreateSpecialArray(string customString)
        {
            byte[] returnArray = new byte[256];
            byte[] helperArray;

            int counter = 0;
            helperArray = StringToByteArray(customString);
            foreach (byte item in helperArray)
            {
                returnArray[counter] = item;
                counter++;
            }
            return ByteArrayToString(returnArray);
        }
        private byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }
        private string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }
        public String Purge(String text)
        {
            StringBuilder htmlk = new StringBuilder();
            int ia = 0;
            for (int i = 0; i < text.Length; i++)
            {
                Char c = text[i];
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
        public void HTMLCharacterConverter()
        {
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
        }
    }
}