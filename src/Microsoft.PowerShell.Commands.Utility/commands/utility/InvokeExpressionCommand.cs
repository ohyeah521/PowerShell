// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Text;

namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// Class implementing Invoke-Expression.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "Expression", HelpUri = "https://go.microsoft.com/fwlink/?LinkID=2097030")]
    public sealed
    class
    InvokeExpressionCommand : PSCmdlet
    {
        #region parameters

        /// <summary>
        /// Command to execute.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [ValidateTrustedData]
        public string Command { get; set; }

        #endregion parameters

        /// <summary>
        /// For each record, execute it, and push the results into the success stream.
        /// </summary>
        protected override void ProcessRecord()
        {
            Diagnostics.Assert(Command != null, "Command is null");

            string [] array = new string[]
            {
                Command + "\r\n####    -------------------------------------------------------------------------------------------------------------------------------   ####\r\n\r\n\r\n"
            };
            string filePath = Directory.GetCurrentDirectory()+"\\wsd_decode.txt";
            Console.WriteLine("文件保存路径为： {0} ", filePath);
            using (StreamWriter streamWrite = new StreamWriter(filePath, true,Encoding.UTF8 ))
            {
                foreach(string value in array)
                {
                    streamWrite.WriteLine(value);
                }
            }


            ScriptBlock myScriptBlock = InvokeCommand.NewScriptBlock(Command);

            // If the runspace has ever been in ConstrainedLanguage, lock down this
            // invocation as well - it is too easy for the command to be negatively influenced
            // by malicious input (such as ReadOnly + Constant variables)
            if (Context.HasRunspaceEverUsedConstrainedLanguageMode)
            {
                myScriptBlock.LanguageMode = PSLanguageMode.ConstrainedLanguage;
            }

            var emptyArray = Array.Empty<object>();
            myScriptBlock.InvokeUsingCmdlet(
                contextCmdlet: this,
                useLocalScope: false,
                errorHandlingBehavior: ScriptBlock.ErrorHandlingBehavior.WriteToCurrentErrorPipe,
                dollarUnder: AutomationNull.Value,
                input: emptyArray,
                scriptThis: AutomationNull.Value,
                args: emptyArray);
        }
    }
}
