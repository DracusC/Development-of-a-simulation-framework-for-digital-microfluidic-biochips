using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BioVirtualMachine
{
    /// <summary>
    /// Class to process the basm or bioasm files and transform them form a human-friendly format into the format needed by the virtual machine.
    /// </summary>
    class Assembler
    {
        private struct Statement
        {
            internal string line;
            internal uint lineNumber;

            internal Statement(uint lineNumberArg, string lineArg)
            {
                lineNumber = lineNumberArg;
                line = lineArg;
            }
        }


        /// <summary>
        /// This is the main method to be used to translate from a 'friendly BioAssembly' into 
        /// a format where all the addressing, constants, expressions, and aliases are resolved.
        /// The latter format is the one executed by the VirtualMachine.
        /// </summary>
        /// <param name="inFile">Input file stream</param>
        /// <param name="outFile">Output file stream</param>
        public void Translate(ref FileStream inFile, ref FileStream outFile)
        {
            List<Statement> loadBuffer = new List<Statement>();
            List<string> forbiddenKeys = new List<string>();
            List<Statement> textBuffer_0 = new List<Statement>();
            List<Statement> textBuffer_1 = new List<Statement>();
            List<Statement> dataBuffer_0 = new List<Statement>();
            List<Statement> dataBuffer_1 = new List<Statement>();
            Dictionary<string, string> dataLabels = new Dictionary<string, string>();
            List<Statement> configurationBuffer_0 = new List<Statement>();
            List<Statement> configurationBuffer_1 = new List<Statement>();

            LoadFile(ref inFile, ref loadBuffer);
            SplitSegments(ref loadBuffer, ref textBuffer_0, ref dataBuffer_0, ref configurationBuffer_0);
            // Processing the data segment
            ResolveDefinitions(ref dataBuffer_0, ref dataBuffer_1, ref forbiddenKeys);
            dataBuffer_0.Clear();
            ResolveDataAndCheck(ref dataBuffer_1, ref dataBuffer_0, ref dataLabels, ref forbiddenKeys);
            dataBuffer_1.Clear();
            // Processing the text segment
            ResolveDefinitions(ref textBuffer_0, ref textBuffer_1, ref forbiddenKeys);
            textBuffer_0.Clear();
            ResolveTextLabels(ref textBuffer_1, ref textBuffer_0, ref dataLabels, ref forbiddenKeys);
            textBuffer_1.Clear();
            ResolveInstructionsAndCheck(ref textBuffer_0, ref textBuffer_1);
            // Processing the configuration segment
            ResolveConfigurationAndCheck(ref configurationBuffer_0, ref configurationBuffer_1);
            SaveFileWithSegments(ref textBuffer_1, ref dataBuffer_0, ref configurationBuffer_1, ref outFile);
        }


        /// <summary>
        /// This metod loads the content of the inFile into the list outBuffer and: 
        /// - Removes the comments
        /// - Makes all lowercase
        /// - Remove line start and line end whitespaces
        /// - Removes empity lines
        /// - Reducing multiple whitespaces(including tabs) to a single one
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outBuffer"></param>
        private void LoadFile(ref FileStream inFile, ref List<Statement> outBuffer)
        {
            StreamReader inFileStream = new StreamReader(inFile);

            string line;
            uint lineNumber = 1;

            while ((line = inFileStream.ReadLine()) != null)
            {
                line = Regex.Replace(line, BioAssemblyDefinition.commentsRegex, ""); // Removing comments
                line = line.Trim();
                line = line.ToLower();
                line = Regex.Replace(line, @"\s+", " "); // Reducing multiple whitespaces to a single one
                if (!line.Equals(""))
                {
                    outBuffer.Add(new Statement(lineNumber, line));
                }
                lineNumber++;
            }
            inFileStream.Close();
        }


        /// <summary>
        /// This method dumps the content of the inBuffer into the outFile for each segment
        /// </summary>
        /// <param name="inTextBuffer"></param>
        /// <param name="inDataBuffer"></param>
        /// <param name="inConfigurationBuffer"></param>
        /// <param name="outFile"></param>
        private void SaveFileWithSegments(ref List<Statement> inTextBuffer,
                                ref List<Statement> inDataBuffer,
                                ref List<Statement> inConfigurationBuffer,
                                ref FileStream outFile)
        {
            // Dumping the buffer to the file
            StreamWriter outFileStream = new StreamWriter(outFile);
            outFileStream.WriteLine(BioAssemblyDefinition.configurationSectionTag);
            foreach (Statement statement in inConfigurationBuffer)
            {
                outFileStream.WriteLine(statement.line);
            }
            outFileStream.WriteLine(BioAssemblyDefinition.dataSectionTag);
            foreach (Statement statement in inDataBuffer)
            {
                outFileStream.WriteLine(statement.line);
            }
            int address = 0;
            outFileStream.WriteLine(BioAssemblyDefinition.textSectionTag);
            foreach (Statement statement in inTextBuffer)
            {
                string line = address.ToString() + BioAssemblyDefinition.instructionAddressSeparator + " " + statement.line;
                outFileStream.WriteLine(line);
                address++;
            }
            outFileStream.Close();
        }


        /*
        * This metod splits the content of inBuffer into three buffers containing .text segment, .data segment, and .configuration segment.
        */
        private void SplitSegments(ref List<Statement> inBuffer,
                                   ref List<Statement> outTextBuffer,
                                   ref List<Statement> outDataBuffer,
                                   ref List<Statement> outConfigurationBuffer)
        {
            bool isText = true; // By default, what is read is .text
            bool isData = false;
            bool isConfiguration = false;
            foreach (Statement statement in inBuffer)
            {
                if (Regex.IsMatch(statement.line, BioAssemblyDefinition.textSectionRegex))
                {
                    isText = true;
                    isData = false;
                    isConfiguration = false;
                    continue;
                }
                else if (Regex.IsMatch(statement.line, BioAssemblyDefinition.dataSectionRegex))
                {
                    isText = false;
                    isData = true;
                    isConfiguration = false;
                    continue;
                }
                else if (Regex.IsMatch(statement.line, BioAssemblyDefinition.configurationSectionRegex))
                {
                    isText = false;
                    isData = false;
                    isConfiguration = true;
                    continue;
                }
                if (isText)
                {
                    outTextBuffer.Add(statement);
                }
                else if (isData)
                {
                    outDataBuffer.Add(statement);
                }
                else if (isConfiguration)
                {
                    outConfigurationBuffer.Add(statement);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // CONFIGURATION SEGMENT
        ///////////////////////////////////////////////////////////////////////////////

        private void ResolveConfigurationAndCheck(ref List<Statement> inBuffer, ref List<Statement> outBuffer)
        {
            foreach (Statement statement in inBuffer)
            {
                //int lineNumber = statement.Item1;
                //string line = statement.Item2;
                Match match = Regex.Match(statement.line, BioAssemblyDefinition.genericConfigurationRegex);
                if (!match.Success)
                {
                    // It does not look like an instruction
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Syntax issue. Maybe a missed ';'?";
                    throw new AssemblerException(exceptionMessage);
                }
                // It looks like a configuration line
                string configuration = match.Groups["configuration"].Value;
                configuration = configuration.Trim();
                List<string> operands = configuration.Split(' ').ToList(); // Temporarely contains also the configTag at index 0
                string configTag = operands[0];
                operands.RemoveAt(0);
                if (!BioAssemblyDefinition.configurationTemplates.ContainsKey(configTag))
                {
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Unknown configuration tag.";
                    throw new AssemblerException(exceptionMessage);
                }
                BioAssemblyDefinition.ConfigurationInfo configurationInfo = BioAssemblyDefinition.configurationTemplates[configTag];
                if (operands.Count > configurationInfo.operandsCount)
                {
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Configuration directive has too many operands.";
                    throw new AssemblerException(exceptionMessage);
                }
                if (operands.Count < configurationInfo.operandsCount)
                {
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Configuration directive has too few operands.";
                    throw new AssemblerException(exceptionMessage);
                }
                // Are operands types correct? Yes, evaluate expression
                for (int i = 0; i < operands.Count; i++)
                {
                    if (configurationInfo.operandsType[i] == BioAssemblyDefinition.integerAbbreviation)
                    {
                        int result;
                        try
                        {
                            result = EvaluateIntegerExpression(operands[i]);
                        }
                        catch (Exception ex)
                        {
                            //code for any other type of exception
                            string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": " + ex.Message;
                            throw new AssemblerException(exceptionMessage);
                        }
                        operands[i] = result.ToString();

                    }
                    else if (configurationInfo.operandsType[i] == BioAssemblyDefinition.floatAbbreviation)
                    {
                        // Float operand
                        //if (!Regex.IsMatch(operands[i], BioAssemblyDefinition.numericFloatExpressionRegex))
                        //{
                        //    string exceptionMessage = "Line " + lineNumber.ToString() + ": Malformed expression in one of the operands.";
                        //    throw new AssemblerException(exceptionMessage);
                        //}
                        float result;
                        try
                        {
                            result = EvaluateFloatExpression(operands[i]);
                        }
                        catch (Exception ex)
                        {
                            //code for any other type of exception
                            string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": " + ex.Message;
                            throw new AssemblerException(exceptionMessage);
                        }
                        //operands[i] = result.ToString();
                        //operands[i] = BitConverter.ToInt32(BitConverter.GetBytes(result), 0).ToString();
                        operands[i] = result.ToString(BioAssemblyDefinition.floatNumberFormat, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (configurationInfo.operandsType[i] == BioAssemblyDefinition.boolAbbreviation)
                    {
                        // Bool operand
                        if (!operands[i].Equals(BioAssemblyDefinition.configBoolFalseTag) && !operands[i].Equals(BioAssemblyDefinition.configBoolTrueTag))
                        {
                            string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Malformed expression in one of the operands. Expected " + BioAssemblyDefinition.configBoolTrueTag + " or " + BioAssemblyDefinition.configBoolFalseTag + ".";
                            throw new AssemblerException(exceptionMessage);
                        }
                    }
                    else
                    {
                        string exceptionMessage = "Error in configuration template.";
                        throw new AssemblerException(exceptionMessage);
                    }
                }
                string line = configTag;
                foreach (string operand in operands)
                {
                    line += " " + operand;
                }
                line += BioAssemblyDefinition.configurationTerminator;
                outBuffer.Add(new Statement(statement.lineNumber, line));
            }
        }


        ///////////////////////////////////////////////////////////////////////////////
        // COMMON
        ///////////////////////////////////////////////////////////////////////////////
        private void ResolveDefinitions(ref List<Statement> inBuffer,
                                         ref List<Statement> outBuffer,
                                         ref List<string> forbiddenKeys)
        {
            List<Statement> tmpBuffer = new List<Statement>();
            // Looking for defs and populating a dictionary
            Dictionary<string, string> definitions = new Dictionary<string, string>();
            Dictionary<string, uint> definitionsLineNumbers = new Dictionary<string, uint>();
            foreach (Statement statement in inBuffer)
            {
                //string line = entry.Item2;
                //int lineNumber = entry.Item1;
                Match match = Regex.Match(statement.line, BioAssemblyDefinition.defintionRegex);
                if (match.Success)
                {
                    // It is a def line
                    string defKey = match.Groups["defKey"].Value;
                    if (Regex.IsMatch(defKey, BioAssemblyDefinition.keywordsRegex))
                    {
                        // Key is a keyword
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Definition is a BioAssembly keyword or an expression keyword.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    string defValue = match.Groups["defValue"].Value;
                    if (definitions.ContainsKey(defKey))
                    {
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Definition key was already used as definition.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    if (forbiddenKeys.Contains(defKey))
                    {
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Definition key was already used in the file.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    definitions.Add(defKey, defValue);
                    definitionsLineNumbers.Add(defKey, statement.lineNumber);
                    forbiddenKeys.Add(defKey);
                }
                else if (Regex.IsMatch(statement.line, BioAssemblyDefinition.defintionMalformedRegex))
                {
                    // It is a malformed def
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Malformed definition.";
                    throw new AssemblerException(exceptionMessage);
                }
                else
                {
                    // It is not a def line, save it in the outBuffer
                    tmpBuffer.Add(statement);
                }
            }
            Dictionary<string, string> resolvedDefinitions = new Dictionary<string, string>();
            // Resolving cross dependencies between defs
            // Moving definitions that are already resolved
            bool definitionWasResolved = true;
            bool replacementHappened = true;
            while (definitionWasResolved || replacementHappened)
            {
                definitionWasResolved = false;
                replacementHappened = false;
                for (int i = definitions.Count - 1; i >= 0; i--)
                {
                    string defKey = definitions.ElementAt(i).Key;
                    string defValue = definitions.ElementAt(i).Value;
                    if (Regex.IsMatch(defValue, BioAssemblyDefinition.numericIntegerFloatRegex)) // Is integer number or float number with or without sign?
                    {
                        // Define is resolved
                        resolvedDefinitions.Add(defKey, defValue);
                        definitions.Remove(defKey);
                        definitionsLineNumbers.Remove(defKey);
                        definitionWasResolved = true;
                    }
                    else
                    {
                        // Define is not resolved
                        // Try to replace from resolved definitions use (). Did replacement happened? Yes: flag true
                        foreach (KeyValuePair<string, string> resolvedDefinition in resolvedDefinitions)
                        {
                            if (Regex.IsMatch(definitions[defKey], @"(?<preamble>^|[\ \+\-\*\/\%\^\(\)\,])" + resolvedDefinition.Key + @"(?<postamble>$|[\ \+\-\*\/\%\^\(\)\,])"))
                            {
                                definitions[defKey] = ReplaceString(definitions[defKey], @"(?<preamble>^|[\ \+\-\*\/\%\^\(\)]\,)" + resolvedDefinition.Key + @"(?<postamble>$|[\ \+\-\*\/\%\^\(\)\,])", "(" + resolvedDefinition.Value + ")");
                                replacementHappened = true;
                            }
                        }
                        // Check if now it is an integer numeric expresion
                        if (Regex.IsMatch(definitions[defKey], BioAssemblyDefinition.numericIntegerExpressionRegex))
                        {
                            // Yes: evaluate it, move it, remove it, flag true (rise exception)
                            int result;
                            try
                            {
                                result = EvaluateIntegerExpression(definitions[defKey]);
                            }
                            catch (Exception ex)
                            {
                                //code for any other type of exception
                                string exceptionMessage = "Line " + definitionsLineNumbers[defKey].ToString() + ": " + ex.Message;
                                throw new AssemblerException(exceptionMessage);
                            }
                            definitions[defKey] = result.ToString();
                            replacementHappened = true;
                        }
                        else if (Regex.IsMatch(definitions[defKey], BioAssemblyDefinition.numericFloatExpressionRegex)) //Check if it is a float numeric expresion
                        {
                            // Yes: evaluate it, move it, remove it, flag true (rise exception)
                            float result;
                            try
                            {
                                result = EvaluateFloatExpression(definitions[defKey]);
                            }
                            catch (Exception ex)
                            {
                                //code for any other type of exception
                                string exceptionMessage = "Line " + definitionsLineNumbers[defKey].ToString() + ": " + ex.Message;
                                throw new AssemblerException(exceptionMessage);
                            }
                            definitions[defKey] = result.ToString();
                            replacementHappened = true;
                        }
                        // No: proceed next loop
                    }
                }
            }
            if (definitions.Count != 0)
            {
                string exceptionMessage = "Line";
                foreach (KeyValuePair<string, string> definition in definitions)
                {
                    string defKey = definition.Key;
                    uint lineNumber = definitionsLineNumbers[defKey];
                    exceptionMessage += " " + lineNumber.ToString();
                }

                exceptionMessage += ": Definition cannot be resolved.";
                throw new AssemblerException(exceptionMessage);
            }

            //Replacing definition in the inBuffer
            foreach (Statement statement in tmpBuffer)
            {
                string line = statement.line;
                if (Regex.IsMatch(line, BioAssemblyDefinition.definitionTargetRegex))
                {
                    foreach (KeyValuePair<string, string> resolvedDefinition in resolvedDefinitions)
                    {
                        line = ReplaceString(line, @"(?<preamble>[\ \+\-\*\/\%\^\(\)\,])" + resolvedDefinition.Key + @"(?<postamble>[\ \+\-\*\/\%\^\(\);\,])", "(" + resolvedDefinition.Value + ")");
                    }
                    outBuffer.Add(new Statement(statement.lineNumber, line));
                }
                else
                {
                    outBuffer.Add(new Statement(statement.lineNumber, line));
                }
            }
        }


        /*
        * This method resolves the labels in the data segments and produces a dictionary with labelName and varAddres value to be used in the text segment
        */
        private void ResolveDataAndCheck(ref List<Statement> inBuffer,
                                        ref List<Statement> outBuffer,
                                        ref Dictionary<string, string> dataLabels,
                                        ref List<string> forbiddenKeys)
        {
            foreach (Statement statement in inBuffer)
            {
                string labelKey = "";
                string dataType, dataAddr, dataValue;
                bool dataHasLabel = false;
                Match matchLabel = Regex.Match(statement.line, BioAssemblyDefinition.dataWithLabelRegex); //@"^(?<labelKey>\w+):\s(?<dataType>int|real)\s(?<dataAddr>[\w\+\-\*\/\.\(\)]+)\s(?<dataValue>[\w\+\-\*\/\.\(\)]+)$";
                Match matchNoLabel = Regex.Match(statement.line, BioAssemblyDefinition.dataWithoutLabelRegex);
                if (matchLabel.Success)
                {
                    // It is a data line with a label
                    labelKey = matchLabel.Groups["labelKey"].Value;
                    if (Regex.IsMatch(labelKey, BioAssemblyDefinition.keywordsRegex))
                    {
                        // Key is a keyword
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Label is a BioAssembly keyword or an expression keyword.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    if (dataLabels.ContainsKey(labelKey))
                    {
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Label was already used as label.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    if (forbiddenKeys.Contains(labelKey))
                    {
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Label key was already used in the file.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    dataHasLabel = true;
                    forbiddenKeys.Add(labelKey);
                    dataType = matchLabel.Groups["dataType"].Value;
                    dataAddr = matchLabel.Groups["dataAddr"].Value;
                    dataValue = matchLabel.Groups["dataValue"].Value;
                }
                else if (matchNoLabel.Success)
                {
                    // It is a data line without a label
                    dataType = matchNoLabel.Groups["dataType"].Value;
                    dataAddr = matchNoLabel.Groups["dataAddr"].Value;
                    dataValue = matchNoLabel.Groups["dataValue"].Value;
                }
                else
                {
                    // It is a malformed line
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Malformed data declaration.";
                    throw new AssemblerException(exceptionMessage);
                }

                int result;
                try
                {
                    result = EvaluateIntegerExpression(dataAddr);
                }
                catch (Exception ex)
                {
                    //code for any other type of exception
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": " + ex.Message;
                    throw new AssemblerException(exceptionMessage);
                }
                dataAddr = result.ToString();

                // Adding label, if it exists, to the dictionary
                if (dataHasLabel)
                {
                    dataLabels.Add(labelKey, dataAddr);
                }

                // Evaluating data value depending on data type
                if (dataType.Equals(BioAssemblyDefinition.integerDataTypeTag))
                {
                    try
                    {
                        result = EvaluateIntegerExpression(dataValue);
                    }
                    catch (Exception ex)
                    {
                        //code for any other type of exception
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": " + ex.Message;
                        throw new AssemblerException(exceptionMessage);
                    }
                    dataValue = result.ToString();
                }
                else if (dataType.Equals(BioAssemblyDefinition.realDataTypeTag))
                {
                    float floatResult;
                    try
                    {
                        floatResult = EvaluateFloatExpression(dataValue);
                    }
                    catch (Exception ex)
                    {
                        //code for any other type of exception
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": " + ex.Message;
                        throw new AssemblerException(exceptionMessage);
                    }
                    //result = BitConverter.ToInt32(BitConverter.GetBytes(floatResult), 0);
                    dataValue = floatResult.ToString(BioAssemblyDefinition.floatNumberFormat, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    // It is a malformed line
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Data type not recognized.";
                    throw new AssemblerException(exceptionMessage);
                }
                outBuffer.Add(new Statement(statement.lineNumber, dataAddr + BioAssemblyDefinition.dataAddressSeparator + " " + dataValue + BioAssemblyDefinition.dataTerminator));
            }
        }

        /*
        * This method resolves the labels and replaces them in the outBuffer for text segment
        */
        private void ResolveTextLabels(ref List<Statement> inBuffer,
                                        ref List<Statement> outBuffer,
                                        ref Dictionary<string, string> dataLabels,
                                        ref List<string> forbiddenKeys)
        {
            List<Statement> tmpBuffer_0 = new List<Statement>();
            List<Statement> tmpBuffer_1 = new List<Statement>();
            Dictionary<string, string> labels = new Dictionary<string, string>();
            int address = 0;
            // Looking for lables and populating a dictionary
            foreach (Statement statement in inBuffer)
            {
                //string line = statement.Item2;
                //int lineNumber = statement.Item1;
                Match match = Regex.Match(statement.line, BioAssemblyDefinition.labelRegex);
                if (match.Success)
                {
                    // It is a label
                    string labelKey = match.Groups["labelKey"].Value;
                    if (Regex.IsMatch(labelKey, BioAssemblyDefinition.keywordsRegex))
                    {
                        // Key is a keyword
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Label is a BioAssembly keyword or an expression keyword.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    if (labels.ContainsKey(labelKey))
                    {
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Label was already used as label.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    if (dataLabels.ContainsKey(labelKey))
                    {
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Label was already used as data label.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    if (forbiddenKeys.Contains(labelKey))
                    {
                        string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Label key was already used in the file.";
                        throw new AssemblerException(exceptionMessage);
                    }
                    string labelValue = address.ToString();
                    labels.Add(labelKey, labelValue);
                    forbiddenKeys.Add(labelKey);
                }
                else if (Regex.IsMatch(statement.line, BioAssemblyDefinition.labelMalformedRegex))
                {
                    // It is a malformed label
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Malformed label.";
                    throw new AssemblerException(exceptionMessage);
                }
                else
                {
                    // It is not a label line, save it in the outBuffer
                    tmpBuffer_0.Add(statement);
                    address++;
                }
            }

            //Replacing data labels
            foreach (Statement statement in tmpBuffer_0)
            {
                string line = statement.line;
                if (Regex.IsMatch(line, BioAssemblyDefinition.labelTargetRegex))
                {
                    foreach (KeyValuePair<string, string> label in dataLabels)
                    {
                        line = ReplaceString(line, @"(?<preamble>[\ \+\-\*\/\%\^\(\)\,])" + label.Key + @"(?<postamble>[\ \+\-\*\/\%\^\(\);\,])", "(" + label.Value + ")");
                    }
                    tmpBuffer_1.Add(new Statement(statement.lineNumber, line));
                }
                else
                {
                    tmpBuffer_1.Add(new Statement(statement.lineNumber, line));
                }
            }

            //Replacing labels in the inBuffer
            foreach (Statement statement in tmpBuffer_1)
            {
                string line = statement.line;
                if (Regex.IsMatch(line, BioAssemblyDefinition.labelTargetRegex))
                {
                    foreach (KeyValuePair<string, string> label in labels)
                    {
                        line = ReplaceString(line, @"(?<preamble>[\ \+\-\*\/\%\^\(\)\,])" + label.Key + @"(?<postamble>[\ \+\-\*\/\%\^\(\);\,])", "(" + label.Value + ")");
                    }
                    outBuffer.Add(new Statement(statement.lineNumber, line));
                }
                else
                {
                    outBuffer.Add(new Statement(statement.lineNumber, line));
                }
            }
        }

        /*
        * This method resolves the expressions in the instructions and check that the instructions comply with the template
        */
        private void ResolveInstructionsAndCheck(ref List<Statement> inBuffer,
                                         ref List<Statement> outBuffer)
        {
            List<Statement> tmpBuffer = new List<Statement>();
            // Looking for lables and populating a dictionary
            foreach (Statement statement in inBuffer)
            {
                //int lineNumber = entry.Item1;
                //string line = entry.Item2;
                Match match = Regex.Match(statement.line, BioAssemblyDefinition.genericInstructionRegex);
                if (!match.Success)
                {
                    // It does not look like an instruction
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Syntax issue. Maybe a missed ';'?";
                    throw new AssemblerException(exceptionMessage);
                }
                // It looks like an instruction
                string instruction = match.Groups["instruction"].Value;
                instruction = instruction.Trim();
                List<string> operands = instruction.Split(' ').ToList(); // Temporarely contains also the opcode at index 0
                string opcode = operands[0];
                operands.RemoveAt(0);
                if (!BioAssemblyDefinition.instructionTemplates.ContainsKey(opcode))
                {
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": Unknown opcode.";
                    throw new AssemblerException(exceptionMessage);
                }
                BioAssemblyDefinition.InstructionInfo instructionInfo = BioAssemblyDefinition.instructionTemplates[opcode];
                if (operands.Count > instructionInfo.operandsCount)
                {
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": The instruction has too many operands.";
                    throw new AssemblerException(exceptionMessage);
                }
                if (operands.Count < instructionInfo.operandsCount)
                {
                    string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": The instruction has too few operands.";
                    throw new AssemblerException(exceptionMessage);
                }
                // Are operands types correct? Yes, evaluate expression
                for (int i = 0; i < operands.Count; i++)
                {
                    if (instructionInfo.operandsType[i] == BioAssemblyDefinition.integerAbbreviation)
                    {
                        int result;
                        try
                        {
                            result = EvaluateIntegerExpression(operands[i]);
                        }
                        catch (Exception ex)
                        {
                            //code for any other type of exception
                            string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": " + ex.Message;
                            throw new AssemblerException(exceptionMessage);
                        }
                        operands[i] = result.ToString();

                    }
                    else if (instructionInfo.operandsType[i] == BioAssemblyDefinition.floatAbbreviation)
                    {
                        float result;
                        try
                        {
                            result = EvaluateFloatExpression(operands[i]);
                        }
                        catch (Exception ex)
                        {
                            //code for any other type of exception
                            string exceptionMessage = "Line " + statement.lineNumber.ToString() + ": " + ex.Message;
                            throw new AssemblerException(exceptionMessage);
                        }
                        //operands[i] = result.ToString();
                        //operands[i] = BitConverter.ToInt32(BitConverter.GetBytes(result), 0).ToString();
                        operands[i] = result.ToString(BioAssemblyDefinition.floatNumberFormat, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        string exceptionMessage = "Error in instruction template.";
                        throw new AssemblerException(exceptionMessage);
                    }
                }
                string line = opcode;
                foreach (string operand in operands)
                {
                    line += " " + operand;
                }
                line += BioAssemblyDefinition.instructionTerminator;
                outBuffer.Add(new Statement(statement.lineNumber, line));
            }
        }

        

        /*
         * SUPPORT METHODS AND WRAPPERS
         */


        /*
         * This method evaluate a string expression into an integer result
         */
        private int EvaluateIntegerExpression(string expression)
        {
            //return Convert.ToInt32(new System.Data.DataTable().Compute(expression, null));
            return ExpressionSolver.Evaluate<int>(expression);
        }


        /*
        * This method evaluate a string expression into a long integer result
        */
        private long EvaluateLongIntegerExpression(string expression)
        {
            //return Convert.ToInt32(new System.Data.DataTable().Compute(expression, null));
            return ExpressionSolver.Evaluate<long>(expression);
        }


        /*
         * This method evaluate a string expression into a float result
         */
        private float EvaluateFloatExpression(string expression)
        {
            //return (float)Convert.ToDouble(new System.Data.DataTable().Compute(expression, null));
            return Convert.ToSingle(ExpressionSolver.Evaluate<double>(expression));
        }


        /*
         * This method replaces ALL occurrences of a regex match in bewteen the groups preamble and postamble
         */
        private string ReplaceString(string line, string oldStringRegex, string newString)
        {
            Regex rgx = new Regex(oldStringRegex);
            string replacement = @"${preamble}" + newString + @"${postamble}";
            while (Regex.IsMatch(line, oldStringRegex))
            {
                line = rgx.Replace(line, replacement, 1);
            }
            return line;
        }
    }

    public class AssemblerException : Exception
    {
        public AssemblerException(string message) : base(message)
        {
        }
    }
}