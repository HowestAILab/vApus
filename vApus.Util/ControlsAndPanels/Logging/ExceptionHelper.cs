/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Glenn Desmadryl
 */

using System;
using System.Diagnostics;

namespace vApus.Util {
    public static class ExceptionHelper {
        /// <summary>
        ///     Use this to parse an exeption to an usable string. Note on this that this only gets the last frame from the stacktrace.
        /// </summary>
        public static string ParseExceptionToString(Exception ex) {
            var trace = new StackTrace(ex, true);

            string methodName = trace.GetFrame(0).GetMethod().Name;
            string lineNumber = trace.GetFrame(0).GetFileLineNumber() + "";
            string columnNumber = trace.GetFrame(0).GetFileColumnNumber() + "";

            return "Method:" + methodName + ";Line:" + lineNumber + ";Column:" + columnNumber + ";Message:" + ex.Message;
        }
    }
}