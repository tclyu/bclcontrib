﻿using System.Text;
namespace System.Interop.CSyntax
{
    public partial class PrintfFormat
    {
        private class ConversionSpecification
        {
            private bool thousands = false;
            private bool leftJustify = false;
            private bool leadingSign = false;
            private bool leadingSpace = false;
            private bool alternateForm = false;
            private bool leadingZeros = false;
            private bool variableFieldWidth = false;
            private int fieldWidth = 0;
            private bool fieldWidthSet = false;
            private int precision = 0;
            private static int defaultDigits = 6;
            private bool variablePrecision = false;
            private bool precisionSet = false;
            private bool positionalSpecification = false;
            private int argumentPosition = 0;
            private bool positionalFieldWidth = false;
            private int argumentPositionForFieldWidth = 0;
            private bool positionalPrecision = false;
            private int argumentPositionForPrecision = 0;
            private bool optionalh = false;
            private bool optionall = false;
            private bool optionalL = false;
            private char conversionCharacter = '\0';
            private int pos = 0;
            private string fmt;

            public ConversionSpecification() { }
            public ConversionSpecification(string fmtArg)
            {
                if (string.IsNullOrEmpty(fmtArg))
                    throw new ArgumentNullException("fmtArg");
                if (fmtArg[0] == '%')
                {
                    fmt = fmtArg;
                    pos = 1;
                    setArgPosition();
                    setFlagCharacters();
                    setFieldWidth();
                    setPrecision();
                    setOptionalHL();
                    if (setConversionCharacter())
                    {
                        if (pos == fmtArg.Length)
                        {
                            if (leadingZeros && leftJustify)
                                leadingZeros = false;
                            if (precisionSet && leadingZeros)
                            {
                                if (conversionCharacter == 'd'
                                    || conversionCharacter == 'i'
                                    || conversionCharacter == 'o'
                                    || conversionCharacter == 'x')
                                {
                                    leadingZeros = false;
                                }
                            }
                        }
                        else
                            throw new ArgumentException("fmtArg", "Malformed conversion specification=" + fmtArg);
                    }
                    else
                        throw new ArgumentException("fmtArg", "Malformed conversion specification=" + fmtArg);
                }
                else
                    throw new ArgumentException("fmtArg", "Control strings must begin with %.");
            }

            internal void setLiteral(string s)
            {
                fmt = s;
            }

            internal string getLiteral()
            {
                var sb = new StringBuilder();
                int i = 0;
                while (i < fmt.Length)
                {
                    if (fmt[i] == '\\')
                    {
                        i++;
                        if (i < fmt.Length)
                        {
                            char c = fmt[i];
                            switch (c)
                            {
                                case 'a':
                                    sb.Append((char)0x07);
                                    break;
                                case 'b':
                                    sb.Append('\b');
                                    break;
                                case 'f':
                                    sb.Append('\f');
                                    break;
                                case 'n':
                                    sb.Append('\n');// TODO(jgw): sb.append(System.getProperty("line.separator"));
                                    break;
                                case 'r':
                                    sb.Append('\r');
                                    break;
                                case 't':
                                    sb.Append('\t');
                                    break;
                                case 'v':
                                    sb.Append((char)0x0b);
                                    break;
                                case '\\':
                                    sb.Append('\\');
                                    break;
                            }
                            i++;
                        }
                        else
                            sb.Append('\\');
                    }
                    else
                        i++;
                }
                return fmt;
            }

            internal char getConversionCharacter()
            {
                return conversionCharacter;
            }

            internal bool isVariableFieldWidth()
            {
                return variableFieldWidth;
            }

            internal void setFieldWidthWithArg(int fw)
            {
                if (fw < 0)
                    leftJustify = true;
                fieldWidthSet = true;
                fieldWidth = Math.Abs(fw);
            }

            internal bool isVariablePrecision()
            {
                return variablePrecision;
            }

            internal void setPrecisionWithArg(int pr)
            {
                precisionSet = true;
                precision = Math.Max(pr, 0);
            }

            private string internalsprintf(int s)
            {
                string s2 = "";
                switch (conversionCharacter)
                {
                    case 'd':
                    case 'i':
                        if (optionalh)
                            s2 = printDFormat((short)s);
                        else if (optionall)
                            s2 = printDFormat((long)s);
                        else
                            s2 = printDFormat(s);
                        break;
                    case 'x':
                    case 'X':
                        if (optionalh)
                            s2 = printXFormat((short)s);
                        else if (optionall)
                            s2 = printXFormat((long)s);
                        else
                            s2 = printXFormat(s);
                        break;
                    case 'o':
                        if (optionalh)
                            s2 = printOFormat((short)s);
                        else if (optionall)
                            s2 = printOFormat((long)s);
                        else
                            s2 = printOFormat(s);
                        break;
                    case 'c':
                    case 'C':
                        s2 = printCFormat((char)s);
                        break;
                    default:
                        throw new ArgumentException("conversionCharacter", "Cannot format a int with a format using a " + conversionCharacter + " conversion character.");
                }
                return s2;
            }

            private string internalsprintf(long s)
            {
                string s2 = "";
                switch (conversionCharacter)
                {
                    case 'd':
                    case 'i':
                        if (optionalh)
                            s2 = printDFormat((short)s);
                        else if (optionall)
                            s2 = printDFormat(s);
                        else
                            s2 = printDFormat((int)s);
                        break;
                    case 'x':
                    case 'X':
                        if (optionalh)
                            s2 = printXFormat((short)s);
                        else if (optionall)
                            s2 = printXFormat(s);
                        else
                            s2 = printXFormat((int)s);
                        break;
                    case 'o':
                        if (optionalh)
                            s2 = printOFormat((short)s);
                        else if (optionall)
                            s2 = printOFormat(s);
                        else
                            s2 = printOFormat((int)s);
                        break;
                    case 'c':
                    case 'C':
                        s2 = printCFormat((char)s);
                        break;
                    default:
                        throw new ArgumentException("conversionCharacter", "Cannot format a long with a format using a " + conversionCharacter + " conversion character.");
                }
                return s2;
            }

            private string internalsprintf(double s)
            {
                string s2 = "";
                switch (conversionCharacter)
                {
                    case 'f':
                        s2 = printFFormat(s);
                        break;
                    case 'E':
                    case 'e':
                        s2 = printEFormat(s);
                        break;
                    case 'G':
                    case 'g':
                        s2 = printGFormat(s);
                        break;
                    default:
                        throw new ArgumentException("conversionCharacter", "Cannot " + "format a double with a format using a " + conversionCharacter + " conversion character.");
                }
                return s2;
            }

            private string internalsprintf(string s)
            {
                string s2 = "";
                if (conversionCharacter == 's' || conversionCharacter == 'S')
                    s2 = printSFormat(s);
                else
                    throw new ArgumentException("conversionCharacter", "Cannot " + "format a String with a format using a " + conversionCharacter + " conversion character.");
                return s2;
            }

            internal string internalsprintf(object s)
            {
                string s2 = "";
                if (conversionCharacter == 's' || conversionCharacter == 'S')
                    s2 = printSFormat(s.ToString());
                else
                    throw new ArgumentException("conversionCharacter", "Cannot format a String with a format using" + " a " + conversionCharacter + " conversion character.");
                return s2;
            }

            private char[] fFormatDigits(double x)
            {
                // int defaultDigits=6;
                string sx;
                int i, j, k;
                int n1In, n2In;
                int expon = 0;
                bool minusSign = false;
                if (x > 0.0)
                    sx = Convert.ToString(x);
                else if (x < 0.0)
                {
                    sx = Convert.ToString(-x);
                    minusSign = true;
                }
                else
                {
                    sx = Convert.ToString(x);
                    if (sx[0] == '-')
                    {
                        minusSign = true;
                        sx = sx.Substring(1);
                    }
                }
                int ePos = sx.IndexOf('E');
                int rPos = sx.IndexOf('.');
                if (rPos != -1)
                    n1In = rPos;
                else if (ePos != -1)
                    n1In = ePos;
                else
                    n1In = sx.Length;
                if (rPos != -1)
                {
                    if (ePos != -1)
                        n2In = ePos - rPos - 1;
                    else
                        n2In = sx.Length - rPos - 1;
                }
                else
                    n2In = 0;
                if (ePos != -1)
                {
                    int ie = ePos + 1;
                    expon = 0;
                    if (sx[ie] == '-')
                    {
                        for (++ie; ie < sx.Length; ie++)
                            if (sx[ie] != '0')
                                break;
                        if (ie < sx.Length)
                            expon = -int.Parse(sx.Substring(ie));
                    }
                    else
                    {
                        if (sx[ie] == '+')
                            ++ie;
                        for (; ie < sx.Length; ie++)
                            if (sx[ie] != '0')
                                break;
                        if (ie < sx.Length)
                            expon = int.Parse(sx.Substring(ie));
                    }
                }
                int p;
                if (precisionSet)
                    p = precision;
                else
                    p = defaultDigits - 1;
                char[] ca1 = sx.ToCharArray();
                char[] ca2 = new char[n1In + n2In];
                char[] ca3, ca4, ca5;
                for (j = 0; j < n1In; j++)
                    ca2[j] = ca1[j];
                i = j + 1;
                for (k = 0; k < n2In; j++, i++, k++)
                    ca2[j] = ca1[i];
                if (n1In + expon <= 0)
                {
                    ca3 = new char[-expon + n2In];
                    for (j = 0, k = 0; k < (-n1In - expon); k++, j++)
                        ca3[j] = '0';
                    for (i = 0; i < (n1In + n2In); i++, j++)
                        ca3[j] = ca2[i];
                }
                else
                    ca3 = ca2;
                bool carry = false;
                if (p < -expon + n2In)
                {
                    if (expon < 0)
                        i = p;
                    else
                        i = p + n1In;
                    carry = checkForCarry(ca3, i);
                    if (carry)
                        carry = startSymbolicCarry(ca3, i - 1, 0);
                }
                if (n1In + expon <= 0)
                {
                    ca4 = new char[2 + p];
                    if (!carry)
                        ca4[0] = '0';
                    else
                        ca4[0] = '1';
                    if (alternateForm || !precisionSet || precision != 0)
                    {
                        ca4[1] = '.';
                        for (i = 0, j = 2; i < Math.Min(p, ca3.Length); i++, j++)
                            ca4[j] = ca3[i];
                        for (; j < ca4.Length; j++)
                            ca4[j] = '0';
                    }
                }
                else
                {
                    if (!carry)
                    {
                        if (alternateForm || !precisionSet || precision != 0)
                            ca4 = new char[n1In + expon + p + 1];
                        else
                            ca4 = new char[n1In + expon];
                        j = 0;
                    }
                    else
                    {
                        if (alternateForm || !precisionSet || precision != 0)
                            ca4 = new char[n1In + expon + p + 2];
                        else
                            ca4 = new char[n1In + expon + 1];
                        ca4[0] = '1';
                        j = 1;
                    }
                    for (i = 0; i < Math.Min(n1In + expon, ca3.Length); i++, j++)
                        ca4[j] = ca3[i];
                    for (; i < n1In + expon; i++, j++)
                        ca4[j] = '0';
                    if (alternateForm || !precisionSet || precision != 0)
                    {
                        ca4[j] = '.';
                        j++;
                        for (k = 0; i < ca3.Length && k < p; i++, j++, k++)
                            ca4[j] = ca3[i];
                        for (; j < ca4.Length; j++)
                            ca4[j] = '0';
                    }
                }
                int nZeros = 0;
                if (!leftJustify && leadingZeros)
                {
                    int xThousands = 0;
                    if (thousands)
                    {
                        int xlead = 0;
                        if (ca4[0] == '+' || ca4[0] == '-' || ca4[0] == ' ')
                            xlead = 1;
                        int xdp = xlead;
                        for (; xdp < ca4.Length; xdp++)
                            if (ca4[xdp] == '.')
                                break;
                        xThousands = (xdp - xlead) / 3;
                    }
                    if (fieldWidthSet)
                        nZeros = fieldWidth - ca4.Length;
                    if ((!minusSign && (leadingSign || leadingSpace)) || minusSign)
                        nZeros--;
                    nZeros -= xThousands;
                    if (nZeros < 0)
                        nZeros = 0;
                }
                j = 0;
                if ((!minusSign && (leadingSign || leadingSpace)) || minusSign)
                {
                    ca5 = new char[ca4.Length + nZeros + 1];
                    j++;
                }
                else
                    ca5 = new char[ca4.Length + nZeros];
                if (!minusSign)
                {
                    if (leadingSign)
                        ca5[0] = '+';
                    if (leadingSpace)
                        ca5[0] = ' ';
                }
                else
                    ca5[0] = '-';
                for (i = 0; i < nZeros; i++, j++)
                    ca5[j] = '0';
                for (i = 0; i < ca4.Length; i++, j++)
                    ca5[j] = ca4[i];

                int lead = 0;
                if (ca5[0] == '+' || ca5[0] == '-' || ca5[0] == ' ')
                    lead = 1;
                int dp = lead;
                for (; dp < ca5.Length; dp++)
                    if (ca5[dp] == '.')
                        break;
                int nThousands = (dp - lead) / 3;
                // Localize the decimal point.
                if (dp < ca5.Length)
                    ca5[dp] = '.'; // TODO(jgw) dfs.getDecimalSeparator();
                char[] ca6 = ca5;
                if (thousands && nThousands > 0)
                {
                    ca6 = new char[ca5.Length + nThousands + lead];
                    ca6[0] = ca5[0];
                    for (i = lead, k = lead; i < dp; i++)
                    {
                        if (i > 0 && (dp - i) % 3 == 0)
                        {
                            // ca6[k]=',';
                            ca6[k] = ','; // TODO(jgw) dfs.getGroupingSeparator();
                            ca6[k + 1] = ca5[i];
                            k += 2;
                        }
                        else
                        {
                            ca6[k] = ca5[i];
                            k++;
                        }
                    }
                    for (; i < ca5.Length; i++, k++)
                    {
                        ca6[k] = ca5[i];
                    }
                }
                return ca6;
            }

            private string fFormatString(double x)
            {
                //bool noDigits = false;
                char[] ca6, ca7;
                if (double.IsInfinity(x))
                {
                    if (x == double.PositiveInfinity)
                    {
                        if (leadingSign)
                            ca6 = "+Inf".ToCharArray();
                        else if (leadingSpace)
                            ca6 = " Inf".ToCharArray();
                        else
                            ca6 = "Inf".ToCharArray();
                    }
                    else
                        ca6 = "-Inf".ToCharArray();
                    //noDigits = true;
                }
                else if (double.IsNaN(x))
                {
                    if (leadingSign)
                        ca6 = "+NaN".ToCharArray();
                    else if (leadingSpace)
                        ca6 = " NaN".ToCharArray();
                    else
                        ca6 = "NaN".ToCharArray();
                    //noDigits = true;
                }
                else
                    ca6 = fFormatDigits(x);
                ca7 = applyFloatPadding(ca6, false);
                return new String(ca7);
            }

            private char[] eFormatDigits(double x, char eChar)
            {
                char[] ca1, ca2, ca3;
                // int defaultDigits=6;
                string sx;
                int i, j, k, p;
                int n1In, n2In;
                int expon = 0;
                int ePos, rPos, eSize;
                bool minusSign = false;
                if (x > 0.0)
                    sx = Convert.ToString(x);
                else if (x < 0.0)
                {
                    sx = Convert.ToString(-x);
                    minusSign = true;
                }
                else
                {
                    sx = Convert.ToString(x);
                    if (sx[0] == '-')
                    {
                        minusSign = true;
                        sx = sx.Substring(1);
                    }
                }
                ePos = sx.IndexOf('E');
                if (ePos == -1)
                    ePos = sx.IndexOf('e');
                rPos = sx.IndexOf('.');
                if (rPos != -1)
                    n1In = rPos;
                else if (ePos != -1)
                    n1In = ePos;
                else
                    n1In = sx.Length;
                if (rPos != -1)
                {
                    if (ePos != -1)
                        n2In = ePos - rPos - 1;
                    else
                        n2In = sx.Length - rPos - 1;
                }
                else
                    n2In = 0;
                if (ePos != -1)
                {
                    int ie = ePos + 1;
                    expon = 0;
                    if (sx[ie] == '-')
                    {
                        for (++ie; ie < sx.Length; ie++)
                            if (sx[ie] != '0')
                                break;
                        if (ie < sx.Length)
                            expon = -int.Parse(sx.Substring(ie));
                    }
                    else
                    {
                        if (sx[ie] == '+')
                            ++ie;
                        for (; ie < sx.Length; ie++)
                            if (sx[ie] != '0')
                                break;
                        if (ie < sx.Length)
                            expon = int.Parse(sx.Substring(ie));
                    }
                }
                if (rPos != -1)
                    expon += rPos - 1;
                if (precisionSet)
                    p = precision;
                else
                    p = defaultDigits - 1;
                if (rPos != -1 && ePos != -1)
                    ca1 = (sx.Substring(0, rPos) + sx.Substring(rPos + 1, ePos)).ToCharArray();
                else if (rPos != -1)
                    ca1 = (sx.Substring(0, rPos) + sx.Substring(rPos + 1)).ToCharArray();
                else if (ePos != -1)
                    ca1 = sx.Substring(0, ePos).ToCharArray();
                else
                    ca1 = sx.ToCharArray();
                bool carry = false;
                int i0 = 0;
                if (ca1[0] != '0')
                    i0 = 0;
                else
                    for (i0 = 0; i0 < ca1.Length; i0++)
                        if (ca1[i0] != '0')
                            break;
                if (i0 + p < ca1.Length - 1)
                {
                    carry = checkForCarry(ca1, i0 + p + 1);
                    if (carry)
                        carry = startSymbolicCarry(ca1, i0 + p, i0);
                    if (carry)
                    {
                        ca2 = new char[i0 + p + 1];
                        ca2[i0] = '1';
                        for (j = 0; j < i0; j++)
                            ca2[j] = '0';
                        for (i = i0, j = i0 + 1; j < p + 1; i++, j++)
                            ca2[j] = ca1[i];
                        expon++;
                        ca1 = ca2;
                    }
                }
                if (Math.Abs(expon) < 100 && !optionalL)
                    eSize = 4;
                else
                    eSize = 5;
                if (alternateForm || !precisionSet || precision != 0)
                    ca2 = new char[2 + p + eSize];
                else
                    ca2 = new char[1 + eSize];
                if (ca1[0] != '0')
                {
                    ca2[0] = ca1[0];
                    j = 1;
                }
                else
                {
                    for (j = 1; j < (ePos == -1 ? ca1.Length : ePos); j++)
                        if (ca1[j] != '0')
                            break;
                    if ((ePos != -1 && j < ePos) || (ePos == -1 && j < ca1.Length))
                    {
                        ca2[0] = ca1[j];
                        expon -= j;
                        j++;
                    }
                    else
                    {
                        ca2[0] = '0';
                        j = 2;
                    }
                }
                if (alternateForm || !precisionSet || precision != 0)
                {
                    ca2[1] = '.';
                    i = 2;
                }
                else
                    i = 1;
                for (k = 0; k < p && j < ca1.Length; j++, i++, k++)
                    ca2[i] = ca1[j];
                for (; i < ca2.Length - eSize; i++)
                    ca2[i] = '0';
                ca2[i++] = eChar;
                if (expon < 0)
                    ca2[i++] = '-';
                else
                    ca2[i++] = '+';
                expon = Math.Abs(expon);
                if (expon >= 100)
                {
                    switch (expon / 100)
                    {
                        case 1:
                            ca2[i] = '1';
                            break;
                        case 2:
                            ca2[i] = '2';
                            break;
                        case 3:
                            ca2[i] = '3';
                            break;
                        case 4:
                            ca2[i] = '4';
                            break;
                        case 5:
                            ca2[i] = '5';
                            break;
                        case 6:
                            ca2[i] = '6';
                            break;
                        case 7:
                            ca2[i] = '7';
                            break;
                        case 8:
                            ca2[i] = '8';
                            break;
                        case 9:
                            ca2[i] = '9';
                            break;
                    }
                    i++;
                }
                switch ((expon % 100) / 10)
                {
                    case 0:
                        ca2[i] = '0';
                        break;
                    case 1:
                        ca2[i] = '1';
                        break;
                    case 2:
                        ca2[i] = '2';
                        break;
                    case 3:
                        ca2[i] = '3';
                        break;
                    case 4:
                        ca2[i] = '4';
                        break;
                    case 5:
                        ca2[i] = '5';
                        break;
                    case 6:
                        ca2[i] = '6';
                        break;
                    case 7:
                        ca2[i] = '7';
                        break;
                    case 8:
                        ca2[i] = '8';
                        break;
                    case 9:
                        ca2[i] = '9';
                        break;
                }
                i++;
                switch (expon % 10)
                {
                    case 0:
                        ca2[i] = '0';
                        break;
                    case 1:
                        ca2[i] = '1';
                        break;
                    case 2:
                        ca2[i] = '2';
                        break;
                    case 3:
                        ca2[i] = '3';
                        break;
                    case 4:
                        ca2[i] = '4';
                        break;
                    case 5:
                        ca2[i] = '5';
                        break;
                    case 6:
                        ca2[i] = '6';
                        break;
                    case 7:
                        ca2[i] = '7';
                        break;
                    case 8:
                        ca2[i] = '8';
                        break;
                    case 9:
                        ca2[i] = '9';
                        break;
                }
                int nZeros = 0;
                if (!leftJustify && leadingZeros)
                {
                    int xThousands = 0;
                    if (thousands)
                    {
                        int xlead = 0;
                        if (ca2[0] == '+' || ca2[0] == '-' || ca2[0] == ' ')
                            xlead = 1;
                        int xdp = xlead;
                        for (; xdp < ca2.Length; xdp++)
                            if (ca2[xdp] == '.')
                                break;
                        xThousands = (xdp - xlead) / 3;
                    }
                    if (fieldWidthSet)
                        nZeros = fieldWidth - ca2.Length;
                    if ((!minusSign && (leadingSign || leadingSpace)) || minusSign)
                        nZeros--;
                    nZeros -= xThousands;
                    if (nZeros < 0)
                        nZeros = 0;
                }
                j = 0;
                if ((!minusSign && (leadingSign || leadingSpace)) || minusSign)
                {
                    ca3 = new char[ca2.Length + nZeros + 1];
                    j++;
                }
                else
                    ca3 = new char[ca2.Length + nZeros];
                if (!minusSign)
                {
                    if (leadingSign)
                        ca3[0] = '+';
                    if (leadingSpace)
                        ca3[0] = ' ';
                }
                else
                    ca3[0] = '-';
                for (k = 0; k < nZeros; j++, k++)
                    ca3[j] = '0';
                for (i = 0; i < ca2.Length && j < ca3.Length; i++, j++)
                    ca3[j] = ca2[i];

                int lead = 0;
                if (ca3[0] == '+' || ca3[0] == '-' || ca3[0] == ' ')
                    lead = 1;
                int dp = lead;
                for (; dp < ca3.Length; dp++)
                    if (ca3[dp] == '.')
                        break;
                int nThousands = dp / 3;
                // Localize the decimal point.
                if (dp < ca3.Length)
                    ca3[dp] = '.'; // TODO(jgw) dfs.getDecimalSeparator();
                char[] ca4 = ca3;
                if (thousands && nThousands > 0)
                {
                    ca4 = new char[ca3.Length + nThousands + lead];
                    ca4[0] = ca3[0];
                    for (i = lead, k = lead; i < dp; i++)
                    {
                        if (i > 0 && (dp - i) % 3 == 0)
                        {
                            // ca4[k]=',';
                            ca4[k] = ','; // TODO(jgw) dfs.getGroupingSeparator();
                            ca4[k + 1] = ca3[i];
                            k += 2;
                        }
                        else
                        {
                            ca4[k] = ca3[i];
                            k++;
                        }
                    }
                    for (; i < ca3.Length; i++, k++)
                        ca4[k] = ca3[i];
                }
                return ca4;
            }

            private bool checkForCarry(char[] ca1, int icarry)
            {
                bool carry = false;
                if (icarry < ca1.Length)
                {
                    if (ca1[icarry] == '6' || ca1[icarry] == '7' || ca1[icarry] == '8' || ca1[icarry] == '9')
                        carry = true;
                    else if (ca1[icarry] == '5')
                    {
                        int ii = icarry + 1;
                        for (; ii < ca1.Length; ii++)
                            if (ca1[ii] != '0')
                                break;
                        carry = ii < ca1.Length;
                        if (!carry && icarry > 0)
                        {
                            carry =
                                (ca1[icarry - 1] == '1'
                                    || ca1[icarry - 1] == '3'
                                    || ca1[icarry - 1] == '5'
                                    || ca1[icarry - 1] == '7'
                                    || ca1[icarry - 1] == '9');
                        }
                    }
                }
                return carry;
            }

            private bool startSymbolicCarry(char[] ca, int cLast, int cFirst)
            {
                bool carry = true;
                for (int i = cLast; carry && i >= cFirst; i--)
                {
                    carry = false;
                    switch (ca[i])
                    {
                        case '0':
                            ca[i] = '1';
                            break;
                        case '1':
                            ca[i] = '2';
                            break;
                        case '2':
                            ca[i] = '3';
                            break;
                        case '3':
                            ca[i] = '4';
                            break;
                        case '4':
                            ca[i] = '5';
                            break;
                        case '5':
                            ca[i] = '6';
                            break;
                        case '6':
                            ca[i] = '7';
                            break;
                        case '7':
                            ca[i] = '8';
                            break;
                        case '8':
                            ca[i] = '9';
                            break;
                        case '9':
                            ca[i] = '0';
                            carry = true;
                            break;
                    }
                }
                return carry;
            }

            private string eFormatString(double x, char eChar)
            {
                //bool noDigits = false;
                char[] ca4, ca5;
                if (double.IsInfinity(x))
                {
                    if (x == double.PositiveInfinity)
                    {
                        if (leadingSign)
                            ca4 = "+Inf".ToCharArray();
                        else if (leadingSpace)
                            ca4 = " Inf".ToCharArray();
                        else
                            ca4 = "Inf".ToCharArray();
                    }
                    else
                        ca4 = "-Inf".ToCharArray();
                    //noDigits = true;
                }
                else if (double.IsNaN(x))
                {
                    if (leadingSign)
                        ca4 = "+NaN".ToCharArray();
                    else if (leadingSpace)
                        ca4 = " NaN".ToCharArray();
                    else
                        ca4 = "NaN".ToCharArray();
                    //noDigits = true;
                }
                else
                    ca4 = eFormatDigits(x, eChar);
                ca5 = applyFloatPadding(ca4, false);
                return new String(ca5);
            }

            private char[] applyFloatPadding(char[] ca4, bool noDigits)
            {
                char[] ca5 = ca4;
                if (fieldWidthSet)
                {
                    int i, j, nBlanks;
                    if (leftJustify)
                    {
                        nBlanks = fieldWidth - ca4.Length;
                        if (nBlanks > 0)
                        {
                            ca5 = new char[ca4.Length + nBlanks];
                            for (i = 0; i < ca4.Length; i++)
                                ca5[i] = ca4[i];
                            for (j = 0; j < nBlanks; j++, i++)
                                ca5[i] = ' ';
                        }
                    }
                    else if (!leadingZeros || noDigits)
                    {
                        nBlanks = fieldWidth - ca4.Length;
                        if (nBlanks > 0)
                        {
                            ca5 = new char[ca4.Length + nBlanks];
                            for (i = 0; i < nBlanks; i++)
                                ca5[i] = ' ';
                            for (j = 0; j < ca4.Length; i++, j++)
                                ca5[i] = ca4[j];
                        }
                    }
                    else if (leadingZeros)
                    {
                        nBlanks = fieldWidth - ca4.Length;
                        if (nBlanks > 0)
                        {
                            ca5 = new char[ca4.Length + nBlanks];
                            i = 0;
                            j = 0;
                            if (ca4[0] == '-')
                            {
                                ca5[0] = '-';
                                i++;
                                j++;
                            }
                            for (int k = 0; k < nBlanks; i++, k++)
                                ca5[i] = '0';
                            for (; j < ca4.Length; i++, j++)
                                ca5[i] = ca4[j];
                        }
                    }
                }
                return ca5;
            }

            private string printFFormat(double x)
            {
                return fFormatString(x);
            }
            private string printEFormat(double x)
            {
                if (conversionCharacter == 'e')
                    return eFormatString(x, 'e');
                else
                    return eFormatString(x, 'E');
            }
            private string printGFormat(double x)
            {
                string sx, sy, sz, ret;
                int savePrecision = precision;
                int i;
                char[] ca4, ca5;
                //bool noDigits = false;
                if (double.IsInfinity(x))
                {
                    if (x == double.PositiveInfinity)
                    {
                        if (leadingSign)
                            ca4 = "+Inf".ToCharArray();
                        else if (leadingSpace)
                            ca4 = " Inf".ToCharArray();
                        else
                            ca4 = "Inf".ToCharArray();
                    }
                    else
                        ca4 = "-Inf".ToCharArray();
                    //noDigits = true;
                }
                else if (double.IsNaN(x))
                {
                    if (leadingSign)
                        ca4 = "+NaN".ToCharArray();
                    else if (leadingSpace)
                        ca4 = " NaN".ToCharArray();
                    else
                        ca4 = "NaN".ToCharArray();
                    //noDigits = true;
                }
                else
                {
                    if (!precisionSet)
                        precision = defaultDigits;
                    if (precision == 0)
                        precision = 1;
                    int ePos = -1;
                    if (conversionCharacter == 'g')
                    {
                        sx = eFormatString(x, 'e').Trim();
                        ePos = sx.IndexOf('e');
                    }
                    else
                    {
                        sx = eFormatString(x, 'E').Trim();
                        ePos = sx.IndexOf('E');
                    }
                    i = ePos + 1;
                    int expon = 0;
                    if (sx[i] == '-')
                    {
                        for (++i; i < sx.Length; i++)
                            if (sx[i] != '0')
                                break;
                        if (i < sx.Length)
                            expon = -int.Parse(sx.Substring(i));
                    }
                    else
                    {
                        if (sx[i] == '+')
                            ++i;
                        for (; i < sx.Length; i++)
                            if (sx[i] != '0')
                                break;
                        if (i < sx.Length)
                            expon = int.Parse(sx.Substring(i));
                    }
                    // Trim trailing zeros.
                    // If the radix character is not followed by a digit, trim it, too.
                    if (!alternateForm)
                    {
                        if (expon >= -4 && expon < precision)
                            sy = fFormatString(x).Trim();
                        else
                            sy = sx.Substring(0, ePos);
                        i = sy.Length - 1;
                        for (; i >= 0; i--)
                            if (sy[i] != '0')
                                break;
                        if (i >= 0 && sy[i] == '.')
                            i--;
                        if (i == -1)
                            sz = "0";
                        else if (!char.IsDigit(sy[i]))
                            sz = sy.Substring(0, i + 1) + "0";
                        else
                            sz = sy.Substring(0, i + 1);
                        if (expon >= -4 && expon < precision)
                            ret = sz;
                        else
                            ret = sz + sx.Substring(ePos);
                    }
                    else
                    {
                        if (expon >= -4 && expon < precision)
                            ret = fFormatString(x).Trim();
                        else
                            ret = sx;
                    }
                    // leading space was trimmed off during construction
                    if (leadingSpace)
                        if (x >= 0)
                            ret = " " + ret;
                    ca4 = ret.ToCharArray();
                }
                // Pad with blanks or zeros.
                ca5 = applyFloatPadding(ca4, false);
                precision = savePrecision;
                return new String(ca5);
            }

            private string printDFormat(short x)
            {
                return printDFormat(Convert.ToString(x));
            }

            private string printDFormat(long x)
            {
                return printDFormat(Convert.ToString(x));
            }
            private string printDFormat(int x)
            {
                return printDFormat(Convert.ToString(x));
            }
            private string printDFormat(string sx)
            {
                int nLeadingZeros = 0;
                int nBlanks = 0, n = 0;
                int i = 0, jFirst = 0;
                bool neg = sx[0] == '-';
                if (sx.Equals("0") && precisionSet && precision == 0)
                    sx = "";
                if (!neg)
                {
                    if (precisionSet && sx.Length < precision)
                        nLeadingZeros = precision - sx.Length;
                }
                else
                {
                    if (precisionSet && (sx.Length - 1) < precision)
                        nLeadingZeros = precision - sx.Length + 1;
                }
                if (nLeadingZeros < 0)
                    nLeadingZeros = 0;
                if (fieldWidthSet)
                {
                    nBlanks = fieldWidth - nLeadingZeros - sx.Length;
                    if (!neg && (leadingSign || leadingSpace))
                        nBlanks--;
                }
                if (nBlanks < 0)
                    nBlanks = 0;
                if (leadingSign)
                    n++;
                else if (leadingSpace)
                    n++;
                n += nBlanks;
                n += nLeadingZeros;
                n += sx.Length;
                char[] ca = new char[n];
                if (leftJustify)
                {
                    if (neg)
                        ca[i++] = '-';
                    else if (leadingSign)
                        ca[i++] = '+';
                    else if (leadingSpace)
                        ca[i++] = ' ';
                    char[] csx = sx.ToCharArray();
                    jFirst = neg ? 1 : 0;
                    for (int j = 0; j < nLeadingZeros; i++, j++)
                        ca[i] = '0';
                    for (int j = jFirst; j < csx.Length; j++, i++)
                        ca[i] = csx[j];
                    for (int j = 0; j < nBlanks; i++, j++)
                        ca[i] = ' ';
                }
                else
                {
                    if (!leadingZeros)
                    {
                        for (i = 0; i < nBlanks; i++)
                            ca[i] = ' ';
                        if (neg)
                            ca[i++] = '-';
                        else if (leadingSign)
                            ca[i++] = '+';
                        else if (leadingSpace)
                            ca[i++] = ' ';
                    }
                    else
                    {
                        if (neg)
                            ca[i++] = '-';
                        else if (leadingSign)
                            ca[i++] = '+';
                        else if (leadingSpace)
                            ca[i++] = ' ';
                        for (int j = 0; j < nBlanks; j++, i++)
                            ca[i] = '0';
                    }
                    for (int j = 0; j < nLeadingZeros; j++, i++)
                        ca[i] = '0';
                    char[] csx = sx.ToCharArray();
                    jFirst = neg ? 1 : 0;
                    for (int j = jFirst; j < csx.Length; j++, i++)
                        ca[i] = csx[j];
                }
                return new String(ca);
            }
            private string printXFormat(short x)
            {
                string sx = null;
                if (x == short.MinValue)
                    sx = "8000";
                else if (x < 0)
                {
                    string t;
                    if (x == short.MinValue)
                        t = "0";
                    else
                    {
                        t = Convert.ToString((~(-x - 1)) ^ short.MinValue, 16);
                        if (t[0] == 'F' || t[0] == 'f')
                            t = t.Substring(16, 32);
                    }
                    switch (t.Length)
                    {
                        case 1:
                            sx = "800" + t;
                            break;
                        case 2:
                            sx = "80" + t;
                            break;
                        case 3:
                            sx = "8" + t;
                            break;
                        case 4:
                            switch (t[0])
                            {
                                case '1':
                                    sx = "9" + t.Substring(1, 4);
                                    break;
                                case '2':
                                    sx = "a" + t.Substring(1, 4);
                                    break;
                                case '3':
                                    sx = "b" + t.Substring(1, 4);
                                    break;
                                case '4':
                                    sx = "c" + t.Substring(1, 4);
                                    break;
                                case '5':
                                    sx = "d" + t.Substring(1, 4);
                                    break;
                                case '6':
                                    sx = "e" + t.Substring(1, 4);
                                    break;
                                case '7':
                                    sx = "f" + t.Substring(1, 4);
                                    break;
                            }
                            break;
                    }
                }
                else
                    sx = Convert.ToString((int)x, 16);
                return printXFormat(sx);
            }
            private string printXFormat(long x)
            {
                string sx = null;
                if (x == long.MinValue)
                    sx = "8000000000000000";
                else if (x < 0)
                {
                    string t = Convert.ToString((~(-x - 1)) ^ long.MinValue, 16);
                    switch (t.Length)
                    {
                        case 1:
                            sx = "800000000000000" + t;
                            break;
                        case 2:
                            sx = "80000000000000" + t;
                            break;
                        case 3:
                            sx = "8000000000000" + t;
                            break;
                        case 4:
                            sx = "800000000000" + t;
                            break;
                        case 5:
                            sx = "80000000000" + t;
                            break;
                        case 6:
                            sx = "8000000000" + t;
                            break;
                        case 7:
                            sx = "800000000" + t;
                            break;
                        case 8:
                            sx = "80000000" + t;
                            break;
                        case 9:
                            sx = "8000000" + t;
                            break;
                        case 10:
                            sx = "800000" + t;
                            break;
                        case 11:
                            sx = "80000" + t;
                            break;
                        case 12:
                            sx = "8000" + t;
                            break;
                        case 13:
                            sx = "800" + t;
                            break;
                        case 14:
                            sx = "80" + t;
                            break;
                        case 15:
                            sx = "8" + t;
                            break;
                        case 16:
                            switch (t[0])
                            {
                                case '1':
                                    sx = "9" + t.Substring(1, 16);
                                    break;
                                case '2':
                                    sx = "a" + t.Substring(1, 16);
                                    break;
                                case '3':
                                    sx = "b" + t.Substring(1, 16);
                                    break;
                                case '4':
                                    sx = "c" + t.Substring(1, 16);
                                    break;
                                case '5':
                                    sx = "d" + t.Substring(1, 16);
                                    break;
                                case '6':
                                    sx = "e" + t.Substring(1, 16);
                                    break;
                                case '7':
                                    sx = "f" + t.Substring(1, 16);
                                    break;
                            }
                            break;
                    }
                }
                else
                    sx = Convert.ToString(x, 16);
                return printXFormat(sx);
            }

            private string printXFormat(int x)
            {
                string sx = null;
                if (x == int.MinValue)
                    sx = "80000000";
                else if (x < 0)
                {
                    string t = Convert.ToString((~(-x - 1)) ^ int.MinValue, 16);
                    switch (t.Length)
                    {
                        case 1:
                            sx = "8000000" + t;
                            break;
                        case 2:
                            sx = "800000" + t;
                            break;
                        case 3:
                            sx = "80000" + t;
                            break;
                        case 4:
                            sx = "8000" + t;
                            break;
                        case 5:
                            sx = "800" + t;
                            break;
                        case 6:
                            sx = "80" + t;
                            break;
                        case 7:
                            sx = "8" + t;
                            break;
                        case 8:
                            switch (t[0])
                            {
                                case '1':
                                    sx = "9" + t.Substring(1, 8);
                                    break;
                                case '2':
                                    sx = "a" + t.Substring(1, 8);
                                    break;
                                case '3':
                                    sx = "b" + t.Substring(1, 8);
                                    break;
                                case '4':
                                    sx = "c" + t.Substring(1, 8);
                                    break;
                                case '5':
                                    sx = "d" + t.Substring(1, 8);
                                    break;
                                case '6':
                                    sx = "e" + t.Substring(1, 8);
                                    break;
                                case '7':
                                    sx = "f" + t.Substring(1, 8);
                                    break;
                            }
                            break;
                    }
                }
                else
                    sx = Convert.ToString(x, 16);
                return printXFormat(sx);
            }
            private string printXFormat(string sx)
            {
                int nLeadingZeros = 0;
                int nBlanks = 0;
                if (sx.Equals("0") && precisionSet && precision == 0)
                    sx = "";
                if (precisionSet)
                    nLeadingZeros = precision - sx.Length;
                if (nLeadingZeros < 0)
                    nLeadingZeros = 0;
                if (fieldWidthSet)
                {
                    nBlanks = fieldWidth - nLeadingZeros - sx.Length;
                    if (alternateForm)
                        nBlanks = nBlanks - 2;
                }
                if (nBlanks < 0)
                    nBlanks = 0;
                int n = 0;
                if (alternateForm)
                    n += 2;
                n += nLeadingZeros;
                n += sx.Length;
                n += nBlanks;
                char[] ca = new char[n];
                int i = 0;
                if (leftJustify)
                {
                    if (alternateForm)
                    {
                        ca[i++] = '0';
                        ca[i++] = 'x';
                    }
                    for (int j = 0; j < nLeadingZeros; j++, i++)
                        ca[i] = '0';
                    char[] csx = sx.ToCharArray();
                    for (int j = 0; j < csx.Length; j++, i++)
                        ca[i] = csx[j];
                    for (int j = 0; j < nBlanks; j++, i++)
                        ca[i] = ' ';
                }
                else
                {
                    if (!leadingZeros)
                        for (int j = 0; j < nBlanks; j++, i++)
                            ca[i] = ' ';
                    if (alternateForm)
                    {
                        ca[i++] = '0';
                        ca[i++] = 'x';
                    }
                    if (leadingZeros)
                        for (int j = 0; j < nBlanks; j++, i++)
                            ca[i] = '0';
                    for (int j = 0; j < nLeadingZeros; j++, i++)
                        ca[i] = '0';
                    char[] csx = sx.ToCharArray();
                    for (int j = 0; j < csx.Length; j++, i++)
                        ca[i] = csx[j];
                }
                string caReturn = new String(ca);
                if (conversionCharacter == 'X')
                    caReturn = caReturn.ToUpper();
                return caReturn;
            }

            private string printOFormat(short x)
            {
                string sx = null;
                if (x == short.MinValue)
                    sx = "100000";
                else if (x < 0)
                {
                    string t = Convert.ToString((~(-x - 1)) ^ short.MinValue, 8);
                    switch (t.Length)
                    {
                        case 1:
                            sx = "10000" + t;
                            break;
                        case 2:
                            sx = "1000" + t;
                            break;
                        case 3:
                            sx = "100" + t;
                            break;
                        case 4:
                            sx = "10" + t;
                            break;
                        case 5:
                            sx = "1" + t;
                            break;
                    }
                }
                else
                    sx = Convert.ToString(x, 8);
                return printOFormat(sx);
            }

            private string printOFormat(long x)
            {
                string sx = null;
                if (x == long.MinValue)
                    sx = "1000000000000000000000";
                else if (x < 0)
                {
                    string t = Convert.ToString((~(-x - 1)) ^ long.MinValue, 8);
                    switch (t.Length)
                    {
                        case 1:
                            sx = "100000000000000000000" + t;
                            break;
                        case 2:
                            sx = "10000000000000000000" + t;
                            break;
                        case 3:
                            sx = "1000000000000000000" + t;
                            break;
                        case 4:
                            sx = "100000000000000000" + t;
                            break;
                        case 5:
                            sx = "10000000000000000" + t;
                            break;
                        case 6:
                            sx = "1000000000000000" + t;
                            break;
                        case 7:
                            sx = "100000000000000" + t;
                            break;
                        case 8:
                            sx = "10000000000000" + t;
                            break;
                        case 9:
                            sx = "1000000000000" + t;
                            break;
                        case 10:
                            sx = "100000000000" + t;
                            break;
                        case 11:
                            sx = "10000000000" + t;
                            break;
                        case 12:
                            sx = "1000000000" + t;
                            break;
                        case 13:
                            sx = "100000000" + t;
                            break;
                        case 14:
                            sx = "10000000" + t;
                            break;
                        case 15:
                            sx = "1000000" + t;
                            break;
                        case 16:
                            sx = "100000" + t;
                            break;
                        case 17:
                            sx = "10000" + t;
                            break;
                        case 18:
                            sx = "1000" + t;
                            break;
                        case 19:
                            sx = "100" + t;
                            break;
                        case 20:
                            sx = "10" + t;
                            break;
                        case 21:
                            sx = "1" + t;
                            break;
                    }
                }
                else
                    sx = Convert.ToString(x, 8);
                return printOFormat(sx);
            }

            private string printOFormat(int x)
            {
                string sx = null;
                if (x == int.MinValue)
                    sx = "20000000000";
                else if (x < 0)
                {
                    String t = Convert.ToString((~(-x - 1)) ^ int.MinValue, 8);
                    switch (t.Length)
                    {
                        case 1:
                            sx = "2000000000" + t;
                            break;
                        case 2:
                            sx = "200000000" + t;
                            break;
                        case 3:
                            sx = "20000000" + t;
                            break;
                        case 4:
                            sx = "2000000" + t;
                            break;
                        case 5:
                            sx = "200000" + t;
                            break;
                        case 6:
                            sx = "20000" + t;
                            break;
                        case 7:
                            sx = "2000" + t;
                            break;
                        case 8:
                            sx = "200" + t;
                            break;
                        case 9:
                            sx = "20" + t;
                            break;
                        case 10:
                            sx = "2" + t;
                            break;
                        case 11:
                            sx = "3" + t.Substring(1);
                            break;
                    }
                }
                else
                    sx = Convert.ToString(x, 8);
                return printOFormat(sx);
            }

            private string printOFormat(string sx)
            {
                int nLeadingZeros = 0;
                int nBlanks = 0;
                if (sx.Equals("0") && precisionSet && precision == 0)
                    sx = "";
                if (precisionSet)
                    nLeadingZeros = precision - sx.Length;
                if (alternateForm)
                    nLeadingZeros++;
                if (nLeadingZeros < 0)
                    nLeadingZeros = 0;
                if (fieldWidthSet)
                    nBlanks = fieldWidth - nLeadingZeros - sx.Length;
                if (nBlanks < 0)
                    nBlanks = 0;
                int n = nLeadingZeros + sx.Length + nBlanks;
                char[] ca = new char[n];
                int i;
                if (leftJustify)
                {
                    for (i = 0; i < nLeadingZeros; i++)
                        ca[i] = '0';
                    char[] csx = sx.ToCharArray();
                    for (int j = 0; j < csx.Length; j++, i++)
                        ca[i] = csx[j];
                    for (int j = 0; j < nBlanks; j++, i++)
                        ca[i] = ' ';
                }
                else
                {
                    if (leadingZeros)
                        for (i = 0; i < nBlanks; i++)
                            ca[i] = '0';
                    else
                        for (i = 0; i < nBlanks; i++)
                            ca[i] = ' ';
                    for (int j = 0; j < nLeadingZeros; j++, i++)
                        ca[i] = '0';
                    char[] csx = sx.ToCharArray();
                    for (int j = 0; j < csx.Length; j++, i++)
                        ca[i] = csx[j];
                }
                return new String(ca);
            }

            private string printCFormat(char x)
            {
                int nPrint = 1;
                int width = fieldWidth;
                if (!fieldWidthSet)
                    width = nPrint;
                char[] ca = new char[width];
                int i = 0;
                if (leftJustify)
                {
                    ca[0] = x;
                    for (i = 1; i <= width - nPrint; i++)
                        ca[i] = ' ';
                }
                else
                {
                    for (i = 0; i < width - nPrint; i++)
                        ca[i] = ' ';
                    ca[i] = x;
                }
                return new String(ca);
            }

            private string printSFormat(string x)
            {
                int nPrint = x.Length;
                int width = fieldWidth;
                if (precisionSet && nPrint > precision)
                    nPrint = precision;
                if (!fieldWidthSet)
                    width = nPrint;
                int n = 0;
                if (width > nPrint)
                    n += width - nPrint;
                if (nPrint >= x.Length)
                    n += x.Length;
                else
                    n += nPrint;
                char[] ca = new char[n];
                int i = 0;
                if (leftJustify)
                {
                    if (nPrint >= x.Length)
                    {
                        char[] csx = x.ToCharArray();
                        for (i = 0; i < x.Length; i++)
                            ca[i] = csx[i];
                    }
                    else
                    {
                        char[] csx = x.Substring(0, nPrint).ToCharArray();
                        for (i = 0; i < nPrint; i++)
                            ca[i] = csx[i];
                    }
                    for (int j = 0; j < width - nPrint; j++, i++)
                        ca[i] = ' ';
                }
                else
                {
                    for (i = 0; i < width - nPrint; i++)
                        ca[i] = ' ';
                    if (nPrint >= x.Length)
                    {
                        char[] csx = x.ToCharArray();
                        for (int j = 0; j < x.Length; i++, j++)
                            ca[i] = csx[j];
                    }
                    else
                    {
                        char[] csx = x.Substring(0, nPrint).ToCharArray();
                        for (int j = 0; j < nPrint; i++, j++)
                            ca[i] = csx[j];
                    }
                }
                return new String(ca);
            }

            private bool setConversionCharacter()
            {
                /* idfgGoxXeEcs */
                bool ret = false;
                conversionCharacter = '\0';
                if (pos < fmt.Length)
                {
                    char c = fmt[pos];
                    if (c == 'i'
                        || c == 'd'
                        || c == 'f'
                        || c == 'g'
                        || c == 'G'
                        || c == 'o'
                        || c == 'x'
                        || c == 'X'
                        || c == 'e'
                        || c == 'E'
                        || c == 'c'
                        || c == 's'
                        || c == '%')
                    {
                        conversionCharacter = c;
                        pos++;
                        ret = true;
                    }
                }
                return ret;
            }

            private void setOptionalHL()
            {
                optionalh = false;
                optionall = false;
                optionalL = false;
                if (pos < fmt.Length)
                {
                    char c = fmt[pos];
                    if (c == 'h')
                    {
                        optionalh = true;
                        pos++;
                    }
                    else if (c == 'l')
                    {
                        optionall = true;
                        pos++;
                    }
                    else if (c == 'L')
                    {
                        optionalL = true;
                        pos++;
                    }
                }
            }

            private void setPrecision()
            {
                int firstPos = pos;
                precisionSet = false;
                if (pos < fmt.Length && fmt[pos] == '.')
                {
                    pos++;
                    if ((pos < fmt.Length) && (fmt[pos] == '*'))
                    {
                        pos++;
                        if (!setPrecisionArgPosition())
                        {
                            variablePrecision = true;
                            precisionSet = true;
                        }
                        return;
                    }
                    else
                    {
                        while (pos < fmt.Length)
                        {
                            char c = fmt[pos];
                            if (char.IsDigit(c))
                                pos++;
                            else
                                break;
                        }
                        if (pos > firstPos + 1)
                        {
                            string sz = fmt.Substring(firstPos + 1, pos);
                            precision = int.Parse(sz);
                            precisionSet = true;
                        }
                    }
                }
            }

            private void setFieldWidth()
            {
                int firstPos = pos;
                fieldWidth = 0;
                fieldWidthSet = false;
                if ((pos < fmt.Length) && (fmt[pos] == '*'))
                {
                    pos++;
                    if (!setFieldWidthArgPosition())
                    {
                        variableFieldWidth = true;
                        fieldWidthSet = true;
                    }
                }
                else
                {
                    while (pos < fmt.Length)
                    {
                        char c = fmt[pos];
                        if (char.IsDigit(c))
                            pos++;
                        else
                            break;
                    }
                    if (firstPos < pos && firstPos < fmt.Length)
                    {
                        string sz = fmt.Substring(firstPos, pos);
                        fieldWidth = int.Parse(sz);
                        fieldWidthSet = true;
                    }
                }
            }

            private void setArgPosition()
            {
                int xPos;
                for (xPos = pos; xPos < fmt.Length; xPos++)
                {
                    if (!char.IsDigit(fmt[xPos]))
                        break;
                }
                if (xPos > pos && xPos < fmt.Length)
                {
                    if (fmt[xPos] == '$')
                    {
                        positionalSpecification = true;
                        argumentPosition = int.Parse(fmt.Substring(pos, xPos));
                        pos = xPos + 1;
                    }
                }
            }

            private bool setFieldWidthArgPosition()
            {
                bool ret = false;
                int xPos;
                for (xPos = pos; xPos < fmt.Length; xPos++)
                {
                    if (!char.IsDigit(fmt[xPos]))
                        break;
                }
                if (xPos > pos && xPos < fmt.Length)
                {
                    if (fmt[xPos] == '$')
                    {
                        positionalFieldWidth = true;
                        argumentPositionForFieldWidth = int.Parse(fmt.Substring(pos, xPos));
                        pos = xPos + 1;
                        ret = true;
                    }
                }
                return ret;
            }

            private bool setPrecisionArgPosition()
            {
                bool ret = false;
                int xPos;
                for (xPos = pos; xPos < fmt.Length; xPos++)
                {
                    if (!char.IsDigit(fmt[xPos]))
                        break;
                }
                if (xPos > pos && xPos < fmt.Length)
                {
                    if (fmt[xPos] == '$')
                    {
                        positionalPrecision = true;
                        argumentPositionForPrecision = int.Parse(fmt.Substring(pos, xPos));
                        pos = xPos + 1;
                        ret = true;
                    }
                }
                return ret;
            }
            internal bool isPositionalSpecification()
            {
                return positionalSpecification;
            }
            internal int getArgumentPosition()
            {
                return argumentPosition;
            }
            internal bool isPositionalFieldWidth()
            {
                return positionalFieldWidth;
            }
            internal int getArgumentPositionForFieldWidth()
            {
                return argumentPositionForFieldWidth;
            }
            internal bool isPositionalPrecision()
            {
                return positionalPrecision;
            }
            internal int getArgumentPositionForPrecision()
            {
                return argumentPositionForPrecision;
            }

            private void setFlagCharacters()
            {
                // '-+ #0
                thousands = false;
                leftJustify = false;
                leadingSign = false;
                leadingSpace = false;
                alternateForm = false;
                leadingZeros = false;
                for (; pos < fmt.Length; pos++)
                {
                    char c = fmt[pos];
                    if (c == '\'')
                        thousands = true;
                    else if (c == '-')
                    {
                        leftJustify = true;
                        leadingZeros = false;
                    }
                    else if (c == '+')
                    {
                        leadingSign = true;
                        leadingSpace = false;
                    }
                    else if (c == ' ')
                    {
                        if (!leadingSign)
                            leadingSpace = true;
                    }
                    else if (c == '#')
                        alternateForm = true;
                    else if (c == '0')
                    {
                        if (!leftJustify)
                            leadingZeros = true;
                    }
                    else
                        break;
                }
            }
        }
    }
}
