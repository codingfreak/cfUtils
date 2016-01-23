namespace s2.s2Utils.Logic.Base.Utilities
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using s2.s2Utils.Logic.Base.Structures;

    /// <summary>
    /// Provides helpers methods for using the console.
    /// </summary>
    public static class ConsoleUtil
    {
        #region methods

        /// <summary>
        /// Initiates a console-window on startup.
        /// </summary>
        /// <param name="width">The desired width of the visible area of the window.</param>
        /// <param name="height">The desired height of the visible area of the window.</param>
        /// <param name="bufferWidth">The amount of columns in the buffer.</param>
        /// <param name="bufferHeight">The amount of rows in the buffer.</param>
        /// <param name="backgroundColor">The default background color of the console.</param>
        public static void InitConsole(int width, int height, int bufferWidth, int bufferHeight, ConsoleColor backgroundColor)
        {
            if (width > Console.LargestWindowWidth)
            {
                width = Console.LargestWindowWidth;
            }
            if (height > Console.LargestWindowHeight)
            {
                height = Console.LargestWindowHeight;
            }
            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Console.BufferWidth = bufferWidth;
            Console.BufferHeight = bufferHeight;
            Console.BackgroundColor = backgroundColor;
        }

        /// <summary>
        /// Writes a standardized app-specific header to the current cursor-position in the console.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        /// <param name="description">The name of the application.</param>
        /// <param name="appVersion">The version information.</param>
        /// <param name="legalInformations">Informations on trademarks, copyrights etc.</param>
        /// <param name="companyName">The name of the manufacturer or a person.</param>
        public static void ShowAppHeader(string appName, string description, string appVersion, string legalInformations, string companyName)
        {
            PrintConsoleLine('*', Console.WindowWidth - 1);
            PrintFullWidthText(appName, '*');
            if (!string.IsNullOrEmpty(description))
            {
                PrintConsoleLine('-', Console.WindowWidth - 1, '*');
                PrintFullWidthText(description, '*');
            }
            PrintConsoleLine('-', Console.WindowWidth - 1, '*');
            if (!string.IsNullOrEmpty(companyName))
            {
                PrintFullWidthText(companyName, '*');
            }
            if (!string.IsNullOrEmpty(appVersion))
            {
                PrintFullWidthText("Version " + appVersion, '*');
            }
            if (!string.IsNullOrEmpty(legalInformations))
            {
                PrintFullWidthText(legalInformations, '*');
            }
            PrintConsoleLine('*', Console.WindowWidth - 1);
            Console.WriteLine();
        }

        /// <summary>
        /// Writes a standardized app-specific header to the current cursor-position in the console.
        /// </summary>
        /// <param name="assemblyInfo">The assembly for which to write the header.</param>
        public static void ShowAppHeader(Assembly assemblyInfo)
        {
            ShowAppHeader(
                Portable.Utilities.ReflectionUtil.GetAssemblyProduct(assemblyInfo),
                Portable.Utilities.ReflectionUtil.GetAssemblyDescription(assemblyInfo),
                Portable.Utilities.ReflectionUtil.GetAssemblyVersion(assemblyInfo),
                Portable.Utilities.ReflectionUtil.GetAssemblyCopyright(assemblyInfo),
                Portable.Utilities.ReflectionUtil.GetAssemblyCompany(assemblyInfo));
        }

        /// <summary>
        /// Is used to inform the caller on error and give him help.
        /// </summary>        
        /// <param name="errorMessage">The message to display to the user.</param>        
        /// <param name="appInfo">Metadata on the calling application.</param>        
        public static void PrintArgumentError(ApplicationInfo appInfo, string errorMessage)
        {
            PrintColoredLine(errorMessage, ConsoleColor.Red);
            Console.WriteLine();
            PrintShortArgumentHelp(appInfo);
        }

        /// <summary>
        /// Prints the 
        /// </summary>
        /// <param name="appInfo">Metadata on the calling application.</param>
        public static void PrintShortArgumentHelp(ApplicationInfo appInfo)
        {
            // Step 1: Build the calling convention line
            var exeName = Portable.Utilities.ReflectionUtil.GetAssemblyTitle(appInfo.AssemblyInfo) + ".exe ";
            var callingBuilder = new StringBuilder(exeName);
            var shortSampleBuilder = new StringBuilder(exeName);
            var sampleBuilder = new StringBuilder(exeName);
            appInfo.CommandlineArgumentInfos.OrderByDescending(arg => arg.IsMandatory).ThenBy(arg => arg.OrderPosition).ToList().ForEach(arg =>
            {
                callingBuilder.AppendFormat("{0}{1}{2}{3} ", arg.IsMandatory ? string.Empty : "[", appInfo.ParameterPraefix, arg.ArgumentName, arg.IsMandatory ? string.Empty : "]");
                if (arg.IsFlag)
                {
                    sampleBuilder.AppendFormat(
                        "{0}{1} ",                        
                        appInfo.ParameterPraefix,
                        arg.ArgumentName);
                }
                else
                {
                    sampleBuilder.AppendFormat(
                        "{0}{1}{2}{3} ",                        
                        appInfo.ParameterPraefix,
                        arg.ArgumentName,
                        appInfo.ParameterDelimiter,
                        arg.SampleValue);
                }
                if (arg.IsMandatory)
                {
                    shortSampleBuilder.AppendFormat("{0} ", arg.SampleValue);
                }
            });
            Console.WriteLine("Usage:\n  {0}", callingBuilder);
            Console.WriteLine("Short Sample call:\n  {0}", shortSampleBuilder);
            Console.WriteLine("Full Sample call:\n  {0}", sampleBuilder);
        }

        /// <summary>
        /// Prints a line using a <paramref name="charToPrint"/> with a length of <paramref name="lineLength"/> to
        /// the current position of the console.
        /// </summary>
        /// <param name="charToPrint">The char with which the line should be written.</param>
        /// <param name="lineLength">The amount of chars to repeat.</param>
        public static void PrintConsoleLine(char charToPrint, int lineLength)
        {
            Console.WriteLine(new string(charToPrint, lineLength));
        }

        /// <summary>
        /// Prints a line using a <paramref name="charToPrint"/> with a length of <paramref name="lineLength"/> to
        /// the current position of the console.
        /// </summary>
        /// <param name="charToPrint">The char with which the line should be written.</param>
        /// <param name="lineLength">The amount of chars to repeat.</param>
        /// <param name="firstAndLastChar">The char at the beginning and end of the line.</param>
        public static void PrintConsoleLine(char charToPrint, int lineLength, char firstAndLastChar)
        {
            var text = new string(charToPrint, lineLength - 4);
            Console.WriteLine("{0} {1} {0}", firstAndLastChar, text);
        }

        /// <summary>
        /// Writes a given text to the console. The text is encapsulated with a special char at the beginning and the end of the line.
        /// </summary>
        /// <param name="text">The text to write to the console.</param>
        /// <param name="firstAndLastChar">The char to put at the beginning and the end of the line.</param>
        public static void PrintFullWidthText(string text, char firstAndLastChar)
        {
            var fullWidthText = text + new string(Convert.ToChar(" "), Console.WindowWidth - text.Length - 5);
            Console.WriteLine("{0} {1} {0}", firstAndLastChar, fullWidthText);
        }

        /// <summary>
        /// Prints a line using a <paramref name="charToPrint"/> with a length of <paramref name="lineLength"/>
        /// and a specific color to the current position of the console.
        /// </summary>
        /// <param name="charToPrint">The char with which the line should be written.</param>
        /// <param name="lineLength">The amount of chars to repeat.</param>
        /// <param name="color">The color to use</param>
        public static void PrintConsoleLine(char charToPrint, int lineLength, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(new string(charToPrint, lineLength));
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a text to the console and uses a specified foreground-color.
        /// </summary>
        /// <param name="text">The text to print to the console.</param>
        /// <param name="color">The color to use as the text color.</param>
        public static void PrintColoredLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        #endregion
    }
}