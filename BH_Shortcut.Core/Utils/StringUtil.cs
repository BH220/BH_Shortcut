using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH_Shortcut.Core.Utils
{
    /// <summary>
    /// 문자열 변환 및 가공에 대한 유틸리티 메서드를 제공합니다.
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// 입력받은 데이터를 YYYYMMDD형식의 날짜 문자열로 변환합니다.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string DateConvert(object[] items)
        {
            string str, str2, str3;

            if (items.Length == 3)
            {
                str = "    ";
                str2 = "  ";
                str3 = "  ";
                if ((items[0] != null) && (items[0].ToString() != string.Empty))
                {
                    str = string.Format("{0,4}", items[0].ToString().Trim());
                }
                if ((items[1] != null) && (items[1].ToString() != string.Empty))
                {
                    str2 = string.Format("{0,2}", items[1].ToString().Trim());
                }
                if ((items[2] != null) && (items[2].ToString() != string.Empty))
                {
                    str3 = string.Format("{0,2}", items[2].ToString().Trim());
                }

                string res = string.Format("{0}{1}{2}", str, str2, str3);

                //res.Replace(' ', '0');

                return res;
            }
            return string.Empty;
        }

        public static string DateConvert(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        public static string DayConvert(int day)
        {
            if (day <= 30)
                return string.Format("{0}일", day);
            else if (day <= 365)
                return string.Format("{0}개월", (int)(day / 30));
            else
            {
                double year = (double)day / 365;
                double month = (double)day % 365;
                if (month <= 30)
                    return string.Format("{0}년", (int)year);
                else
                    return string.Format("{0}년 {1}개월", (int)year, (int)(month / 30));
            }
        }

        /// <summary>
        /// 입력받은 데이터를 YYYY-MM-DD형식의 날짜 문자열로 변환합니다.
        /// </summary>
        /// <param name="yyyyMMdd"></param>
        /// <returns></returns>
        public static string DateCommonViewConvert(string yyyyMMdd)
        {
            if (yyyyMMdd.Length != 8)
            {
                return yyyyMMdd;
            }
            string date = "";

            date += yyyyMMdd.SubstringEx(0, 4) + "-";
            date += yyyyMMdd.SubstringEx(4, 2) + "-";
            date += yyyyMMdd.SubstringEx(6, 2);

            return date;
        }

        public static string DateCommonViewConvert(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string RetrunTelNo(object obj)
        {
            string s = obj.ToStringEx().Replace("-", "");
            if (s.Length == 11)
                return string.Format("{0}-{1}-{2}", s.SubstringEx(0, 3), s.SubstringEx(3, 4), s.SubstringEx(7, 4));
            else if (s.Length == 10)
                return string.Format("{0}-{1}-{2}", s.SubstringEx(0, 3), s.SubstringEx(3, 3), s.SubstringEx(6, 4));
            else if (s.Length == 9)
                return string.Format("{0}-{1}-{2}", s.SubstringEx(0, 2), s.SubstringEx(2, 3), s.SubstringEx(5, 4));
            return s;
        }

        /// <summary>
        /// 입력받은 YYYYMMDD형식의 날짜 문자열을 년/월/일 로 분리합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<string> DateSpliter(object item)
        {
            List<string> list = new List<string>();

            if ((((item != DBNull.Value) && (item != null)) && (item.ToString().Trim() != string.Empty)) && (item.ToString().Length == 8))
            {
                Match match = new Regex(@"(?<YEAR>\d{4})(?<MONTH>\s{1}\d{1}|\d{2})(?<DAY>\s{1}\d{1}|\d{2})(?x)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Match(item.ToStringEx());
                if (match.Success)
                {
                    list.AddRange(new string[] { match.Groups["YEAR"].Value, match.Groups["MONTH"].Value, match.Groups["DAY"].Value });
                }
                else
                {
                    list.AddRange(new string[] { "", "", "" });
                }
                return list;
            }
            list.AddRange(new string[] { "", "", "" });
            return list;
        }

        /// <summary>
        /// 숫자형 FormatString 문자열을 생성합니다.
        /// </summary>
        /// <param name="length">데이터 길이</param>
        /// <param name="isShowZero">입력받은 데이터가 0일때 표시할 것인지..</param>
        /// <returns></returns>
        public static string GenerateNumericFormatString(int length, bool isShowZero)
        {
            string formatStr = string.Empty;

            if (length == 0)
            {
                length = 30;
            }

            for (int i = 0; i < length; i++)
            {
                if (isShowZero && i == length - 1)
                {
                    formatStr += "0";
                }
                else
                {
                    formatStr += "#";
                }
            }

            if (isShowZero)
            {
                formatStr = formatStr.SubstringEx(0, formatStr.Length - 1) + "0";
            }

            return formatStr;
        }

        /// <summary>
        /// 금액형 FormatString 문자열을 생성합니다.(3자리마다 컴마)
        /// </summary>
        /// <param name="length">데이터 길이</param>
        /// <param name="isShowZero">입력받은 데이터가 0일때 표시할 것인지..</param>
        /// <returns></returns>
        public static string GenerateCurrencyFormatString(int length, bool isShowZero)
        {
            string formatStr = string.Empty;

            if (length == 0)
            {
                length = 30;
            }

            for (int i = 0; i < length; i++)
            {
                formatStr += "#";
            }

            Regex regex = new Regex("(?<=#)(?=(###)+(?!#))");

            formatStr = regex.Replace(formatStr, ",");

            if (isShowZero)
            {
                formatStr = formatStr.SubstringEx(0, formatStr.Length - 1) + "0";
            }

            return formatStr;
        }

        /// <summary>
        /// 소수점형 FormatString 문자열을 생성합니다.
        /// </summary>
        /// <param name="maxLength">데이터의 총 길이(정수부분+소수부분)</param>
        /// <param name="decimalLength">소수점 자릿수</param>
        /// <returns></returns>
        public static string GenerateDecimalFormatString(int maxLength, int decimalLength)
        {
            return GenerateDecimalFormatString(maxLength, decimalLength, true);
        }

        /// <summary>
        /// 숫자(소수점 포함) FormatString 문자열을 생성합니다.(세자리 구분 콤마 제외)
        /// </summary>
        /// <param name="maxLength">전체 자릿수(소수점 자릿수 포함)</param>
        /// <param name="decimalLength">소수점 자릿수</param>
        /// <param name="includeComma">세자리 콤마 포함 여부</param>
        /// <returns></returns>
        public static string GenerateDecimalFormatString(int maxLength, int decimalLength, bool includeComma)
        {
            string formatStr = string.Empty;

            if (maxLength == 0)
            {
                maxLength = 30;
            }

            int totalIntegerLength = maxLength - decimalLength;

            if (includeComma)
            {
                formatStr = StringUtil.GenerateCurrencyFormatString(totalIntegerLength, true);
            }
            else
            {
                formatStr = StringUtil.GenerateNumericFormatString(totalIntegerLength, true);
            }

            if (decimalLength > 0)
            {
                formatStr += ".";
            }

            for (int i = 0; i < decimalLength; i++)
            {
                formatStr = formatStr.Insert(formatStr.Length, "0");
            }

            return formatStr;
        }

        /// <summary>
        /// string을 구분자로 문자열을 나눕니다.
        /// 2013.04.24 진병호
        /// </summary>
        /// <param name="testString">원본 문자열</param>
        /// <param name="split">구분 문자열</param>
        /// <returns></returns>
        public static string[] SplitByString(string testString, string split)
        {
            int offset = 0;
            int index = 0;
            int[] offsets = new int[testString.Length + 1];

            while (index < testString.Length)
            {
                int indexOf = testString.IndexOf(split, index);
                if (indexOf != -1)
                {
                    offsets[offset++] = indexOf;
                    index = (indexOf + split.Length);
                }
                else
                {
                    index = testString.Length;
                }
            }

            string[] final = new string[offset + 1];

            if (offset == 0)
            {
                final[0] = testString;
            }
            else
            {
                offset--;
                final[0] = testString.SubstringEx(0, offsets[0]);
                for (int i = 0; i < offset; i++)
                {
                    final[i + 1] = testString.SubstringEx(offsets[i] + split.Length, offsets[i + 1] - offsets[i] - split.Length);
                }
                final[offset + 1] = testString.Substring(offsets[offset] + split.Length);
            }
            return final;
        }

        /// <summary>
        /// string을 개행 처리로 하여 나눕니다.
        /// </summary>
        /// <param name="oriString"></param>
        /// <returns></returns>
        public static string[] SplitByEnterLine(string oriString)
        {
            return SplitByString(oriString, "\r\n");
        }

        /// <summary>
        /// 2013. 09. 12 엄태영
        /// -1이 스타터로 반환되면 끝까지 간것임
        /// </summary>
        /// <param name="pSource">변화없음</param>
        /// <param name="pLeft"></param>
        /// <param name="pRight"></param>
        /// <param name="pStart">반환된 스트링 끝 위치가 반영된다.</param>
        /// <returns></returns>
        static public string GetStrBetween(ref string pSource, string pLeft, string pRight, ref int pStart)
        {
            int iPos1 = -1;
            if (string.IsNullOrEmpty(pLeft))
            {
                iPos1 = pStart;
            }
            else
            {
                iPos1 = pSource.IndexOf(pLeft, pStart);
            }
            if (iPos1 == -1)
            {
                pStart = -1;
                return string.Empty;
            }
            int iPos2 = -1;
            if (string.IsNullOrEmpty(pRight))
            {
                iPos2 = pSource.Length;
            }
            else
            {
                iPos2 = pSource.IndexOf(pRight, iPos1 + pLeft.Length);
            }
            if (iPos2 == -1)
            {
                pStart = -1;
                return string.Empty;
            }
            pStart = iPos2 + pRight.Length;
            string strReturn = pSource.SubstringEx(iPos1 + pLeft.Length, iPos2 - iPos1 - pLeft.Length);
            return strReturn;
        }

        ///// <summary>
        ///// 2013. 09. 12 엄태영
        ///// </summary>
        ///// <param name="pSource"></param>
        ///// <param name="pLeft"></param>
        ///// <param name="pRight"></param>
        ///// <param name="pStart"></param>
        ///// <returns></returns>
        //static public string GetStrBetween(string pSource, string pLeft, string pRight, ref int pStart)
        //{
        //    return GetStrBetween(ref pSource, pLeft, pRight, ref pStart);
        //}

        /// <summary>
        /// 2013. 09. 12 엄태영
        /// </summary>
        /// <param name="pSource"></param>
        /// <param name="pLeft"></param>
        /// <param name="pRight"></param>
        /// <returns></returns>
        static public string GetStrBetween(ref string pSource, string pLeft, string pRight)
        {
            int ipos = 0;
            return GetStrBetween(ref pSource, pLeft, pRight, ref ipos);
        }

        ///// <summary>
        ///// 2013. 09. 12 엄태영
        ///// </summary>
        ///// <param name="pSource"></param>
        ///// <param name="pLeft"></param>
        ///// <param name="pRight"></param>
        ///// <returns></returns>
        //static public string GetStrBetween(string pSource, string pLeft, string pRight)
        //{
        //    return GetStrBetween(ref pSource, pLeft, pRight);
        //}

        /// <summary>
        /// 2013. 09. 12 엄태영
        /// </summary>
        /// <param name="pSource"></param>
        /// <param name="pFind"></param>
        /// <param name="pLeft"></param>
        /// <param name="pRight"></param>
        /// <param name="pStart"></param>
        /// <returns></returns>
        static public String GetStrFindBetween(ref String pSource, String pFind, String pLeft, String pRight, ref Int32 pStart)
        {
            pStart = pSource.IndexOf(pFind, pStart);
            if (pStart == -1)
            {
                return String.Empty;
            }
            pStart += pFind.Length;
            return GetStrBetween(ref pSource, pLeft, pRight, ref pStart);
        }

        /// <summary>
        /// 2013. 09. 12 엄태영
        /// </summary>
        /// <param name="pSource"></param>
        /// <param name="pFind"></param>
        /// <param name="pLeft"></param>
        /// <param name="pRight"></param>
        /// <returns></returns>
        static public String GetStrFindBetween(ref String pSource, String pFind, String pLeft, String pRight)
        {
            int iPos = 0;
            return GetStrFindBetween(ref pSource, pFind, pLeft, pRight, ref iPos);
        }

        /// <summary>
        /// 주어진 문자열에 대한 길이(Byte)를 제공합니다.
        /// </summary>
        /// <param name="Str">문자열</param>
        /// <returns>길이(Byte)</returns>
        static public int GetStringLength(string Str)
        {
            char[] obj = Str.ToCharArray();//입력 String을 char[]로 변경
            int maxB = 0;//바이트 길이를 계산할 변수
            for (int i = 0; i < obj.Length; i++)
            {
                byte oF = (byte)((obj[i] & 0xff00) >> 8);
                byte oB = (byte)(obj[i] & 0x00f);
                if (oF == 0)
                {
                    maxB++;
                }
                else
                {
                    maxB += 2;
                }
            }
            return maxB;
        }

        /// <summary>
        /// 전문을 만들때, 주어진 문자열 뒤에 공백을 덮붙여서 주어진 길이의 문자열을 생성하는 기능을 제공합니다.
        /// </summary>
        /// <param name="StringValue">전문에 들어갈 문자열</param>
        /// <param name="ByteCnt">전체 전문의 길이</param>
        /// <returns>공백이 포함된 문자열</returns>
        static public string GetStringByteCollection(string StringValue, int ByteCnt)
        {
            string lsTmpStringValue = string.Empty;
            string lsReturnValue = string.Empty;
            int liByteCnt = 0;
            try
            {
                StringValue = StringValue.PadRight(ByteCnt, ' ').ToString();
                for (int liStartPoint = 0; liStartPoint < StringValue.Length; liStartPoint++)
                {
                    lsTmpStringValue = lsTmpStringValue + StringValue.SubstringEx(liStartPoint, 1);
                    liByteCnt = Encoding.Default.GetByteCount(lsTmpStringValue);
                    if (liByteCnt == ByteCnt)
                    {
                        lsReturnValue = lsTmpStringValue.ToString();
                        break;
                    }
                    else if (liByteCnt > ByteCnt)
                    {
                        lsReturnValue = lsTmpStringValue.SubstringEx(0, lsTmpStringValue.Length - 1);
                        lsReturnValue = lsReturnValue + string.Empty.PadRight(1, ' ').ToString();
                        break;
                    }
                }
                return lsReturnValue.ToString();
            }
            catch
            {
                return string.Empty.PadRight(ByteCnt, ' ').ToString();
            }
        }

        /// <summary>
        /// 전문을 만들때, 주어진 문자열 뒤에 문자을 덮붙여서 주어진 길이의 문자열을 생성하는 기능을 제공합니다.
        /// </summary>
        /// <param name="StringValue">전문에 들어갈 문자열</param>
        /// <param name="ByteCnt">전체 전문의 길이</param>
        /// <param name="fillStr">채워질 문자열</param>
        /// <returns>공백이 포함된 문자열</returns>
        static public string GetStringByteCollection(string StringValue, int ByteCnt, char fillChar)
        {
            string lsTmpStringValue = string.Empty;
            string lsReturnValue = string.Empty;
            int liByteCnt = 0;
            try
            {
                StringValue = StringValue.PadRight(ByteCnt, fillChar).ToString();
                for (int liStartPoint = 0; liStartPoint < StringValue.Length; liStartPoint++)
                {
                    lsTmpStringValue = lsTmpStringValue + StringValue.SubstringEx(liStartPoint, 1);
                    liByteCnt = Encoding.Default.GetByteCount(lsTmpStringValue);
                    if (liByteCnt == ByteCnt)
                    {
                        lsReturnValue = lsTmpStringValue.ToString();
                        break;
                    }
                    else if (liByteCnt > ByteCnt)
                    {
                        lsReturnValue = lsTmpStringValue.SubstringEx(0, lsTmpStringValue.Length - 1);
                        lsReturnValue = lsReturnValue + string.Empty.PadRight(1, fillChar).ToString();
                        break;
                    }
                }
                return lsReturnValue.ToString();
            }
            catch
            {
                return string.Empty.PadRight(ByteCnt, ' ').ToString();
            }
        }

        /// <summary>
        /// DataTable의 컬럼 존재여부를 제공합니다.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool DataColumnExits(string columnName, DataTable table)
        {
            bool retVal = false;
            DataColumnCollection columns = table.Columns;

            if (columns.Contains(columnName))
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// 입력받은 텍스트를 전화번호의 형식으로 변환하여 줍니다.
        /// </summary>
        /// <param name="originValue"></param>
        /// <returns></returns>
        public static string MakePhoneString(string originValue)
        {
            string removeChar = string.Empty;
            foreach (char c in originValue.Trim())
            {
                if (c == '-' || char.IsNumber(c))
                    removeChar = removeChar + c;
            }
            originValue = removeChar;

            string[] splitPhone = originValue.Split('-');
            if (splitPhone.Length > 3) return originValue;

            string phoneNumber = originValue.Trim().Replace("-", "");

            string exp1 = @"(?<Area>\d{3})[-]{0,}(?<Exchange>\d{0,3})[-]{0,}(?<Number>\d{0,4})";
            string exp2 = @"(?<Area>\d{2})[-]{0,}(?<Exchange>\d{0,3})[-]{0,}(?<Number>\d{0,4})";
            string exp3 = @"(?<Area>\d{3})[-]{0,}(?<Exchange>\d{3})[-]{0,}(?<Number>\d{0,4})";
            string exp4 = @"(?<Area>\d{2})[-]{0,}(?<Exchange>\d{3})[-]{0,}(?<Number>\d{0,4})";
            string exp5 = @"(?<Area>\d{3})[-]{0,}(?<Exchange>\d{3,4})[-]{0,}(?<Number>\d{4})";
            string exp6 = @"(?<Area>\d{2})[-]{0,}(?<Exchange>\d{3,4})[-]{0,}(?<Number>\d{4})";
            string exp7 = @"(?<Area>\d{4})[-]{0,}(?<Exchange>\d{3,4})[-]{0,}(?<Number>\d{4})";

            Regex regex = null;
            bool isMatch = false;

            if (string.IsNullOrEmpty(phoneNumber))
                return originValue;

            if (phoneNumber.StartsWith("02"))
            {
                if (phoneNumber.Length < 5)
                {
                    regex = new Regex(exp2);
                }
                else
                {
                    if (phoneNumber.Length < 9)
                    {
                        regex = new Regex(exp4);
                    }
                    else
                    {
                        regex = new Regex(exp6);
                    }
                }
            }
            else
            {
                if (phoneNumber.Length < 6)
                {
                    regex = new Regex(exp1);
                }
                else
                {
                    if (phoneNumber.Length < 10)
                    {
                        regex = new Regex(exp3);
                    }
                    else if (phoneNumber.Length < 12)
                    {
                        regex = new Regex(exp5);
                    }
                    else if (phoneNumber.Length >= 12)
                    {
                        regex = new Regex(exp7);
                    }
                    else
                    {
                        return originValue;
                    }
                }
            }

            isMatch = regex.IsMatch(phoneNumber);
            if (!isMatch)
                return phoneNumber;

            Match match = regex.Match(phoneNumber);
            if (match == null)
                return phoneNumber;

            phoneNumber = (match.Groups["Area"].Value + "-");
            string exchangeValue = match.Groups["Exchange"].Value;
            string numberValue = match.Groups["Number"].Value;

            if (!string.IsNullOrEmpty(match.Groups["Exchange"].Value))
            {
                phoneNumber += (exchangeValue + (exchangeValue.Length >= 3 ? "-" : ""));
            }
            if (!string.IsNullOrEmpty(match.Groups["Number"].Value))
            {
                phoneNumber += (numberValue);
            }

            return phoneNumber;
        }
    }
}
