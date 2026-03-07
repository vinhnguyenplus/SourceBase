// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// Verify file type
/// </summary>
public static class VerifyFileExtensionName
{
    private static readonly IDictionary<string, string> DicsExt = new Dictionary<string, string>();
    private static readonly IDictionary<string, HashSet<int>> ExtDics = new Dictionary<string, HashSet<int>>();

    static VerifyFileExtensionName()
    {
        DicsExt.Add("FFD8FFE0", ".jpg");
        DicsExt.Add("FFD8FFE1", ".jpg");
        DicsExt.Add("89504E47", ".png");
        DicsExt.Add("47494638", ".gif");
        DicsExt.Add("49492A00", ".tif");
        DicsExt.Add("424D", ".bmp");

        // PS and CAD
        DicsExt.Add("38425053", ".psd");
        DicsExt.Add("41433130", ".dwg"); // CAD
        DicsExt.Add("252150532D41646F6265", ".ps");

        // Office documents
        DicsExt.Add("D0CF11E0", ".ppt,.doc,.xls"); // ppt、doc、xls
        DicsExt.Add("504B0304", ".pptx,.docx,.xlsx"); // pptx、docx、xlsx

        /* Note that because there is too much input content in the text document, the file header will be more variable when reading - START */
        DicsExt.Add("0D0A0D0A", ".txt"); // txt
        DicsExt.Add("0D0A2D2D", ".txt"); // txt
        DicsExt.Add("0D0AB4B4", ".txt"); // txt
        DicsExt.Add("B4B4BDA8", ".txt"); // The file header is Chinese characters
        DicsExt.Add("73646673", ".txt"); // txt, the file header is English letters
        DicsExt.Add("32323232", ".txt"); // txt, the file header content is a number
        DicsExt.Add("0D0A09B4", ".txt"); // txt, the file header content is a number
        DicsExt.Add("3132330D", ".txt"); // txt, the file header content is a number
        /* Note that because there is too much input content in the text document, the file header will be more variable when reading - END */

        DicsExt.Add("7B5C727466", ".rtf"); // diary

        DicsExt.Add("255044462D312E", ".pdf");

        // Video or audio class
        DicsExt.Add("3026B275", ".wma");
        DicsExt.Add("57415645", ".wav");
        DicsExt.Add("41564920", ".avi");
        DicsExt.Add("4D546864", ".mid");
        DicsExt.Add("2E524D46", ".rm");
        DicsExt.Add("000001BA", ".mpg");
        DicsExt.Add("000001B3", ".mpg");
        DicsExt.Add("6D6F6F76", ".mov");
        DicsExt.Add("3026B2758E66CF11", ".asf");

        // Compressed package
        DicsExt.Add("52617221", ".rar");
        DicsExt.Add("504B03040A000000", ".zip");
        DicsExt.Add("504B030414000000", ".zip");
        DicsExt.Add("1F8B08", ".gz");

        // program files
        DicsExt.Add("3C3F786D6C", ".xml");
        DicsExt.Add("68746D6C3E", ".html");
        DicsExt.Add("04034b50", ".apk");
        //dics_ext.Add("7061636B", ".java");
        //dics_ext.Add("3C254020", ".jsp");
        //dics_ext.Add("4D5A9000", ".exe");

        DicsExt.Add("44656C69766572792D646174653A", ".eml"); // mail
        DicsExt.Add("5374616E64617264204A", ".mdb"); // Access database file

        DicsExt.Add("46726F6D", ".mht");
        DicsExt.Add("4D494D45", ".mhtml");

        foreach (var dics in DicsExt)
        {
            foreach (var ext in dics.Value.Split(","))
            {
                if (!ExtDics.ContainsKey(ext))
                    ExtDics.Add(ext, new HashSet<int> { dics.Key.Length / 2 });
                else
                    ExtDics[ext].Add(dics.Key.Length / 2);
            }
        }
    }

    /// <summary>
    /// Are the file format and file content format consistent?
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static bool IsSameType(Stream stream, string suffix = ".jpg")
    {
        if (stream == null)
            return false;

        suffix = suffix.ToLower();
        if (!ExtDics.TryGetValue(suffix, out HashSet<int> dic)) return false;

        try
        {
            foreach (var len in dic)
            {
                byte[] b = new byte[len];
                stream.ReadExactly(b);
                // string fileType = System.Text.Encoding.UTF8.GetString(b);
                string fileKey = GetFileHeader(b);
                if (DicsExt.ContainsKey(fileKey))
                    return true;
            }
        }
        catch (IOException)
        {
        }
        return false;
    }

    /**
     * Number of bytes converted from the filegroupGet fileHeader information
     * @param File path
     * @return File header information
     */

    private static string GetFileHeader(byte[] b)
    {
        string value = BytesToHexString(b);
        return value;
    }

    /**
     * of the file whose header information is about to be readbytenumbergroupconvert tostringTypeexpress; indicate; show
     * The following piece of codeYesused to correspond toFile typeMethod of verification，
     * Convert bytesgroupfront ofFourconvert to bits16radix string，andConvertedtimewait，Need to first and0xFFDooneBitwise AND operation。
     * ThisYesbecause，wholeThe number of bytes in a file streamgroupin，There are manyYesNegative number，After performing the AND operation，You can remove all the preceding sign bits，
     * Converted like this16The binary string retains up to two digits，IfYesjustnumber againless than10，Then after the conversion, onlyoneposition，
     * Need to add in front0，The purpose of doing thisYesConvenient for comparison，Before takingFourThis loop can then be terminated
     * @param srcThe file whose header information needs to be readbytenumbergroup
     * @return File header information
     */

    private static string BytesToHexString(byte[] src)
    {
        var builder = new StringBuilder();
        if (src == null || src.Length <= 0)
            return null;

        for (int i = 0; i < src.Length; i++)
        {
            // Returns the string representation of an integer argument as a hexadecimal (base 16) unsigned integer, converted to uppercase
            string hVal = Convert.ToString(src[i] & 0xFF, 16).ToUpper();
            if (hVal.Length < 2) builder.Append(0);
            builder.Append(hVal);
        }
        return builder.ToString();
    }
}