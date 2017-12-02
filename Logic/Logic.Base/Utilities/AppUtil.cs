namespace codingfreaks.cfUtils.Logic.Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using codingfreaks.cfUtils.Logic.Base.Structures;
    using codingfreaks.cfUtils.Logic.Standard.Extensions;

    /// <summary>
    /// Provides useful helper methods for applications.
    /// </summary>
    public static class AppUtil
    {
        #region methods

        /// <summary>
        /// Calls <see cref="CheckCommandArguments"/> and checks, if there are errors or not.
        /// </summary>
        /// <param name="args">The arguments given to the application.</param>
        /// <param name="appInfo">Informations on the app.</param>    
        /// <returns><c>True</c> if the command arguments are valid, otherwise <c>false</c>.</returns>
        public static bool AreCommandArgumentsValid(string[] args, ApplicationInfo appInfo)
        {
            var result = CheckCommandArguments(args, appInfo);
#if TRACE
            result.ForEach(line => Trace.WriteLine(line));
#endif
            return result.Count == 0;
        }

        /// <summary>
        /// Checks, if arguments provides match the criterias defined for an app.
        /// </summary>
        /// <param name="args">The arguments given to the application.</param>
        /// <param name="appInfo">Informations on the app.</param>        
        /// <returns>A list of error messages if one error occured.</returns>
        public static List<string> CheckCommandArguments(string[] args, ApplicationInfo appInfo)
        {
            var result = new List<string>();
            var argsFound = new List<CommandlineArgumentInfo>();
            var pos = 0;
            args.ToList().ForEach(
                arg =>
                {
                    pos++;
                    string givenValue = null;
                    string givenName = null;
                    if (arg.Contains(appInfo.ParameterDelimiter))
                    {
                        // the delimeter was found in this argument -> split it out
                        var parts = arg.Split(appInfo.ParameterDelimiter);
                        if (parts.Length == 2)
                        {
                            givenName = parts[0];
                            givenValue = parts[1];
                        }
                    }
                    else
                    {
                        // no delimter found -> use the complete argument as the value
                        if (arg.StartsWith(appInfo.ParameterPraefix.ToString(CultureInfo.InvariantCulture)))
                        {
                            givenName = arg;
                        }
                        else
                        {
                            givenValue = arg;
                        }
                    }
                    CommandlineArgumentInfo currentArgInfo;
                    if (string.IsNullOrEmpty(givenName))
                    {
                        // no parameter-name was found in this argument so get it from position
                        currentArgInfo = appInfo.CommandlineArgumentInfos[pos - 1];
                    }
                    else
                    {
                        // try to get the argument-info using the parameter name
                        givenName = givenName.Replace(appInfo.ParameterPraefix, " ".ToChar()).Trim().ToLower();
                        currentArgInfo = appInfo.CommandlineArgumentInfos.FirstOrDefault(a => a.ArgumentName.ToLower() == givenName || a.Abbreviation.ToLower() == givenName);
                    }
                    if (currentArgInfo == null)
                    {
                        result.Add($"Can not associate parameter {pos} ({givenName})!");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(givenValue))
                        {
                            // go on because we've got the value                                        
                            {
                                if (CheckValue(givenValue, currentArgInfo))
                                {
                                    // the value is valid for the current 
                                    argsFound.Add(currentArgInfo);
                                }
                                else
                                {
                                    // for some reason we couldn't use the value
                                    result.Add($"The value for parameter {pos} could not be matched!");
                                }
                            }
                        }
                        else
                        {
                            if (!currentArgInfo.IsFlag)
                            {
                                // this is not a flag-parameter so we need to provide some value, which didn't happen
                                result.Add($"No value for parameter {pos} provided!");
                            }
                        }
                    }
                });
            // Now check if all mandatory parameters are given
            var mandatoriesAllGiven = true;
            appInfo.CommandlineArgumentInfos.Where(a => a.IsMandatory).ToList().ForEach(
                a =>
                {
                    mandatoriesAllGiven &= argsFound.FirstOrDefault(f => f.Abbreviation == a.Abbreviation) != null;
                });
            if (!mandatoriesAllGiven)
            {
                result.Add("Not all mandatory arguments provided!");
            }
            return result;
        }

        /// <summary>
        /// Is used to map the given arguments back into a list of all arguments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Consider calling <see cref="CheckCommandArguments"/> before this call!
        /// </para>
        /// <para>
        /// The caller could use this method to try to map all given arguments back to a structure
        /// which he can use for iterations.
        /// </para>
        /// </remarks>
        /// <param name="args">The arguments given to the application.</param>
        /// <param name="appInfo">Informations on the app.</param>
        /// <returns>A list of all arguments, not only the provided ones.</returns>
        public static List<CommandlineArgumentInfo> MapCommandArguments(string[] args, ApplicationInfo appInfo)
        {
            var result = new List<CommandlineArgumentInfo>();
            var pos = 0;

            args.ToList().ForEach(
                arg =>
                {
                    pos++;
                    string givenValue = null;
                    string givenName = null;
                    if (arg.Contains(appInfo.ParameterDelimiter))
                    {
                        // the delimeter was found in this argument -> split it out
                        var parts = arg.Split(appInfo.ParameterDelimiter);
                        if (parts.Length == 2)
                        {
                            givenName = parts[0];
                            givenValue = parts[1];
                        }
                    }
                    else
                    {
                        // no delimter found -> use the complete argument as the value
                        if (arg.StartsWith(appInfo.ParameterPraefix.ToString(CultureInfo.InvariantCulture)))
                        {
                            givenName = arg;
                        }
                        else
                        {
                            givenValue = arg;
                        }
                    }
                    // go on because we've got the value                                        
                    CommandlineArgumentInfo currentArgInfo;
                    if (string.IsNullOrEmpty(givenName))
                    {
                        // no parameter-name was found in this argument
                        currentArgInfo = appInfo.CommandlineArgumentInfos[pos - 1];
                    }
                    else
                    {
                        // try to get the argument-info using the parameter name
                        givenName = givenName.Replace(appInfo.ParameterPraefix, " ".ToChar()).Trim().ToLower();
                        currentArgInfo = appInfo.CommandlineArgumentInfos.FirstOrDefault(a => a.ArgumentName.ToLower() == givenName || a.Abbreviation.ToLower() == givenName);
                    }
                    if (currentArgInfo != null)
                    {
                        if (currentArgInfo.IsFlag)
                        {
                            currentArgInfo.GivenValue = "true";
                            result.Add(currentArgInfo);
                        }
                        else if (CheckValue(givenValue, currentArgInfo))
                        {
                            // the value is valid for the current 
                            currentArgInfo.GivenValue = givenValue;
                            result.Add(currentArgInfo);
                        }
                    }
                });
            // add those parameter-definitions which wheren't passed by the app-caller and which are no flags
            appInfo.CommandlineArgumentInfos.Where(c => !c.IsFlag && c.DefaultValue != null).ToList().ForEach(
                c =>
                {
                    if (result.FirstOrDefault(r => r.Abbreviation == c.Abbreviation) == null)
                    {
                        // this one isn't included but we can add it and use it's default value later on
                        result.Add(c);
                    }
                });
            // remove all of the previous if not all mandatory parameters are given
            var ok = true;
            appInfo.CommandlineArgumentInfos.Where(c => c.IsMandatory).ToList().ForEach(c => ok &= result.FirstOrDefault(r => r.Abbreviation == c.Abbreviation) != null);
            if (!ok)
            {
                result.Clear();
            }
            return result;
        }

        /// <summary>
        /// Checks, if a supplied value can be assigned to an argument.
        /// </summary>
        /// <param name="val">The value supplied.</param>
        /// <param name="currentArgInfo">The argument definition.</param>
        /// <returns><c>True</c> if the value matches, otherwise <c>false</c>.</returns>
        private static bool CheckValue(string val, CommandlineArgumentInfo currentArgInfo)
        {
            if (string.IsNullOrEmpty(val) || currentArgInfo == null)
            {
                return false;
            }
            if (currentArgInfo.IsFlag)
            {
                // if the given argument expects to be a flag return true, if no value
                return string.IsNullOrEmpty(val);
            }
            if (currentArgInfo.IsNumeric)
            {
                long tmp;
                if (currentArgInfo.CanBeCommaSeparated)
                {
                    // we have to check all values
                    var values = val.Split(',').ToList();
                    if (!values.Any())
                    {
                        return false;
                    }
                    var result = true;
                    values.ForEach(v => result &= long.TryParse(v.Trim(), out tmp));
                    return result;
                }
                if (currentArgInfo.CanBeRanged)
                {
                    var values = val.Split('-').ToList();
                    if (values.Any())
                    {
                        if (long.TryParse(values[0], out var fromValue) && long.TryParse(values[1], out var toValue))
                        {
                            if (fromValue < toValue)
                            {
                                return true;
                            }
                        }
                        // it can be ranged, it contains a '-' but the values are invalid
                        return false;
                    }
                }
                return long.TryParse(val, out tmp);
            }
            if (currentArgInfo.IsBool)
            {
                bool tmp;
                return bool.TryParse(val, out tmp);
            }
            if (currentArgInfo.IsUri)
            {
                return Uri.IsWellFormedUriString(val, UriKind.Absolute);
            }
            return true;
        }

        #endregion
    }
}