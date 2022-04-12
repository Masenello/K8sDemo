using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace K8sBackendShared.Logging
{
    public static class LoggingUtilities
    {
    
    public static string AddException(this string source, Exception ex)
        {
            string message;
            try
            {
                var actualException = ex;

                var exceptionsMessage = new List<string>();
                var stackTracesMessage = new List<string>();

                var counterInnerException = 0;
                var exceptionInner = ex;

                while (exceptionInner.InnerException != null)
                {
                    counterInnerException++;
                    exceptionInner = exceptionInner.InnerException;

                }

                for (var c = 0; c <= counterInnerException; c++)
                {
                    if (actualException != null)
                    {
                        var exceptionMessage = actualException.Message;


                        var st = new StackTrace(actualException, true);


                        var lineNumbers = new List<string>();
                        var methods = new List<string>();
                        var classes = new List<string>();
                        var files = new List<string>();

                        for (var i = 0; i < st.FrameCount; i++)
                        {
                            // Note that high up the call stack, there is only
                            // one stack frame.
                            var sf = st.GetFrame(i);

                            if (sf.GetMethod().ReflectedType?.Name != nameof(Timer) &&
                                sf.GetMethod().ReflectedType?.Name != nameof(ExecutionContext) &&
                                sf.GetMethod().ReflectedType?.Name != "TimerQueueTimer" &&
                                sf.GetMethod().ReflectedType?.Name != "TimerQueue" && sf.GetFileLineNumber() != 0)
                            {
                                lineNumbers.Add(sf.GetFileLineNumber().ToString());
                                methods.Add(sf.GetMethod().Name);
                                classes.Add(sf.GetMethod().ReflectedType.Name);
                                files.Add(sf.GetFileName());

                            }
                        }

                        var stackComplete = "      generated from : \n";


                        if (lineNumbers.Count != 0)
                        {
                            var maxLengthLineNumbers = lineNumbers.Max(x => x.Length);
                            var maxLengthMethods = methods.Max(x => x.Length);
                            var maxLengthClasses = classes.Max(x => x.Length);
                            var maxLengthFiles = files.Max(x => x.Length);

                            for (var i = 0; i < lineNumbers.Count; i++)
                            {

                                var stackToAdd = $"  -> Line: {lineNumbers[i].AddSpaceToLength(maxLengthLineNumbers)} | " +
                                                    $"Method: {methods[i].AddSpaceToLength(maxLengthMethods)} | " +
                                                    $"Class: {classes[i].AddSpaceToLength(maxLengthClasses)} | " +
                                                    $"Filename: {files[i].AddSpaceToLength(maxLengthFiles)}";

                                if (i == lineNumbers.Count - 1)
                                {
                                    stackComplete += stackToAdd;
                                }
                                else
                                {
                                    stackComplete += $"{stackToAdd}\n";
                                }
                            }


                        }
                        else
                        {
                            stackComplete += $"  -> External source: {actualException.Source}";
                        }

                        exceptionsMessage.Add(exceptionMessage);
                        stackTracesMessage.Add(stackComplete);
                    }

                    actualException = actualException?.InnerException;
                }

                message = $"{source}\n";

                for (var i = exceptionsMessage.Count - 1; i >= 0; i--)
                {
                    if (i == exceptionsMessage.Count - 1)
                    {
                        message += $"  Exception core with message: {exceptionsMessage[i]}\n{stackTracesMessage[i] }\n";
                    }
                    else
                    {
                        message += $"  Exception relaunched from previously exception with message: {exceptionsMessage[i]}\n{stackTracesMessage[i] } \n";
                    }


                }

            }
            catch (Exception a)
            {
                message = $"{source}. Exception to take exception:{ a.GetOriginalException().Message} \n {a.StackTrace}" +
                    $"Original exception {ex.GetOriginalException().Message} \n {ex.StackTrace}";
            }

            return message;


        }

        /// <summary>
        /// Returns a description of the most internal exception of an exception stack
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Innermost exception</returns>
        public static Exception GetOriginalException(this Exception ex)
        {
            return ex.InnerException == null ? ex : ex.InnerException.GetOriginalException();
        }

        /// <summary>
        /// Extension to add spaces to a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lengthWithSpace"></param>
        /// <returns>Formatted string</returns>
        public static string AddSpaceToLength(this string value, int lengthWithSpace)
        {
            var lengthString = value.Length;
            var spaceToAdd = lengthWithSpace - lengthString;

            if (spaceToAdd > 0)
            {
                value += new string(' ', spaceToAdd);
            }

            return value;

        }
    }
}