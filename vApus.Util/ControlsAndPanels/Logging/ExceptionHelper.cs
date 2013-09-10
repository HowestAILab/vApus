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
            var traceFrame = new StackTrace(ex, true).GetFrame(0);

            string methodName = traceFrame.GetMethod().Name;
            string lineNumber = traceFrame.GetFileLineNumber() + "";
            string columnNumber = traceFrame.GetFileColumnNumber() + "";

            return "Method:" + methodName + ";Line:" + lineNumber + ";Column:" + columnNumber + ";Message:" + ex.Message;
        }
    }
}