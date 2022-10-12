using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    static class BioAssemblyDefinition
    {
        // Comments
        static readonly internal string commentsRegex = @"#.*$"; // Matches the comments

        // Sections
        static readonly internal string textSectionRegex = @"^\.text$"; // Matches the tag for the text section
        static readonly internal string dataSectionRegex = @"^\.data$"; // Matches the tag for the data section
        static readonly internal string configurationSectionRegex = @"^\.configuration$"; // Matches the tag for the configuration section

        // Definitions
        static readonly internal string defintionRegex = @"def\s(?<defKey>\w+)\s(?<defValue>[\w\+\-\*\/\.\(\)]+)$"; // returns: defKey: name - defValue: value
        static readonly internal string defintionMalformedRegex = @"^def\s.*$";
        static readonly internal string definitionTargetRegex = @"^.*[^:]$"; // Matches string that are target by the definition substitution (matches instruction, does not match labels)

        // Labels
        static readonly internal string labelRegex = @"^(?<labelKey>\w+):$"; // returns: labelKey: label name
        static readonly internal string labelMalformedRegex = @"^.*:$";
        static readonly internal string labelTargetRegex = @"^.*;$"; // Matches string that are target by the label substitution (matches instruction)

        // Data segment labels
        static readonly internal string dataWithLabelRegex = @"^(?<labelKey>\w+):\s(?<dataType>int|real)\s(?<dataAddr>[\w\+\-\*\/\.\(\)]+)\s(?<dataValue>[\w\+\-\*\/\.\(\)]+)$";
        static readonly internal string dataWithoutLabelRegex = @"^(?<dataType>int|real)\s(?<dataAddr>[\w\+\-\*\/\.\(\)]+)\s(?<dataValue>[\w\+\-\*\/\.\(\)]+)$";

        // General
        static readonly internal string keywordsRegex = @"^(floor|ceil|abs|max|min|def|int|real|seteli|clreli|setel|clrall|clrel|devwr|adevrd|adevwr|adevex|adevcl|tstart|tstop|tick|barr|li|move|add|sub|and|or|xor|not|addi|subi|andi|ori|xori|sll|srl|sra|mult|div|ji|j|jial|beq|bge|ble|f_li|f_add|f_sub|f_mult|f_div|f_neg|f_abs|f_ceq|f_cge|f_cle|f_cvti2f|f_cvtf2i)$"; // Matches strings that the user cannot use for defines and variables
        static readonly internal string numericFloatRegex = @"^[\-\+]?\d+\.\d+$"; // Matches a float number with or without sign
        static readonly internal string numericIntegerRegex = @"^[\-\+]?\d+$"; // Matches an integer number with or without sign
        static readonly internal string numericIntegerFloatRegex = @"^[\+\-]?\d+(?:\.\d+)?$"; //Check if it is an integer or a float number with or without sign
        static readonly internal string numericFloatExpressionRegex = @"^[\d\.\+\-\*\/\(\)]+$"; //Check if it is a float numeric expresion
        static readonly internal string numericIntegerExpressionRegex = @"^[\d\+\-\*\/\(\)]+$"; //Check if it is an integer numeric expresion

        /*
         * Output writing
         * When printing out the assembled BioAssembly file, the following variables are used. The same are used by the VM when loading the assembled file.
         */
        static readonly internal char instructionTerminator = ';'; // Used when writing the output file
        static readonly internal char dataTerminator = ';'; // Used when writing the output file
        static readonly internal char configurationTerminator = ';'; // Used when writing the output file
        static readonly internal char instructionAddressSeparator = ':'; // Used when writing the output file
        static readonly internal char dataAddressSeparator = ':'; // Used when writing the output file
        static readonly internal string floatNumberFormat = "0.0#############################"; // Format of the real numbers in the assember output file
        static readonly internal string textSectionTag = @".text";
        static readonly internal string dataSectionTag = @".data";
        static readonly internal string configurationSectionTag = @".configuration";

        //Data
        static readonly internal string integerDataTypeTag = "int";
        static readonly internal string realDataTypeTag = "real";

        // Configuration and Instructions templates abbreviations
        static readonly internal char integerAbbreviation = 'i';
        static readonly internal char floatAbbreviation = 'f';
        static readonly internal char boolAbbreviation = 'b';

        // Configuration
        static readonly internal string genericConfigurationRegex = @"^(?<configuration>[a-z].*)$";

        static readonly internal string configBoolTrueTag = "true";
        static readonly internal string configBoolFalseTag = "false";

        internal struct ConfigurationInfo
        {
            public int operandsCount;
            public string operandsType;

            internal ConfigurationInfo(int operandsCountArg, string operandsTypeArg)
            {
                operandsCount = operandsCountArg;
                operandsType = operandsTypeArg;
            }
        }

        static readonly internal Dictionary<string, ConfigurationInfo> configurationTemplates = new Dictionary<string, ConfigurationInfo>() {
            {"data_memory_size", new ConfigurationInfo(1,"i")},
            {"private_data_memory_start_address", new ConfigurationInfo(1,"i")},
            {"debug_log_data_types_for_print_formatting", new ConfigurationInfo(1,"b")},
            {"debug_keep_all_private_data_memories", new ConfigurationInfo(1,"b")},
            //{"logger_log_debug_to_console", new ConfigurationInfo(1,"b")},
            //{"logger_log_info_to_console", new ConfigurationInfo(1,"b")},
            //{"logger_log_warning_to_console", new ConfigurationInfo(1,"b")},
            //{"logger_log_error_to_console", new ConfigurationInfo(1,"b")},
            //{"logger_log_fatal_to_console", new ConfigurationInfo(1,"b")},
            //{"logger_log_trace_to_console", new ConfigurationInfo(1,"b")},
            //{"logger_log_debug_to_file", new ConfigurationInfo(1,"b")},
            //{"logger_log_info_to_file", new ConfigurationInfo(1,"b")},
            //{"logger_log_warning_to_file", new ConfigurationInfo(1,"b")},
            //{"logger_log_error_to_file", new ConfigurationInfo(1,"b")},
            //{"logger_log_fatal_to_file", new ConfigurationInfo(1,"b")},
            //{"logger_log_trace_to_file", new ConfigurationInfo(1,"b")}
        };

        // Instructions
        static readonly internal string genericInstructionRegex = @"^(?<instruction>[a-z].*);$"; // Matches a generic instruction, returns only the instruction text without the instruction terminator (;).
        internal struct InstructionInfo
        {
            public int operandsCount;
            public string operandsType;

            internal InstructionInfo(int operandsCountArg, string operandsTypeArg)
            {
                operandsCount = operandsCountArg;
                operandsType = operandsTypeArg;
            }
        }

        static readonly internal Dictionary<string, InstructionInfo> instructionTemplates = new Dictionary<string, InstructionInfo>() {
            {"seteli", new InstructionInfo(1,"i")},
            {"clreli", new InstructionInfo(1,"i")},
            {"setel", new InstructionInfo(1,"i")},
            {"clrel", new InstructionInfo(1,"i")},
            {"devwr", new InstructionInfo(2,"ii")},
            {"adevrd", new InstructionInfo(2,"ii")},
            {"adevwr", new InstructionInfo(2,"ii")},
            {"adevex", new InstructionInfo(0,"")},
            {"adevcl", new InstructionInfo(0,"")},
            {"tstart", new InstructionInfo(1,"i")},
            {"tstop", new InstructionInfo(0,"")},
            {"tick", new InstructionInfo(0,"")},
            {"barr", new InstructionInfo(2,"ii")},
            {"li", new InstructionInfo(2,"ii")},
            {"move", new InstructionInfo(2,"ii")},
            {"add", new InstructionInfo(3,"iii")},
            {"sub", new InstructionInfo(3,"iii")},
            {"and", new InstructionInfo(3,"iii")},
            {"or", new InstructionInfo(3,"iii")},
            {"xor", new InstructionInfo(3,"iii")},
            {"not", new InstructionInfo(2,"ii")},
            {"addi", new InstructionInfo(3,"iii")},
            {"subi", new InstructionInfo(3,"iii")},
            {"andi", new InstructionInfo(3,"iii")},
            {"ori", new InstructionInfo(3,"iii")},
            {"xori", new InstructionInfo(3,"iii")},
            {"sll", new InstructionInfo(3,"iii")},
            {"srl", new InstructionInfo(3,"iii")},
            {"sra", new InstructionInfo(3,"iii")},
            {"mult", new InstructionInfo(3,"iii")},
            {"div", new InstructionInfo(3,"iii")},
            {"ji", new InstructionInfo(1,"i")},
            {"j", new InstructionInfo(1,"i")},
            {"jial", new InstructionInfo(2,"ii")},
            {"beq", new InstructionInfo(3,"iii")},
            {"bge", new InstructionInfo(3,"iii")},
            {"ble", new InstructionInfo(3,"iii")},
            {"f_li", new InstructionInfo(2,"if")}, // Load immediate float
            {"f_add", new InstructionInfo(3,"iii")}, // Addition float
            {"f_sub", new InstructionInfo(3,"iii")}, // Subtraction float
            {"f_mult", new InstructionInfo(3,"iii")}, // Multiplication float
            {"f_div", new InstructionInfo(3,"iii")}, // Division float
            {"f_neg", new InstructionInfo(2,"ii")}, // Change sign float
            {"f_abs", new InstructionInfo(2,"ii")}, // Absolute float
            {"f_ceq", new InstructionInfo(3,"iii")}, // Compare equal -> Target zero if false
            {"f_cge", new InstructionInfo(3,"iii")}, // Compare greater equal -> Target zero if false
            {"f_cle", new InstructionInfo(3,"iii")}, // Compare less equal -> Target zero if false
            {"f_cvti2f", new InstructionInfo(2,"ii")}, // Integer to float
            {"f_cvtf2i", new InstructionInfo(2,"ii")} // Float to integer

        };

    }
}
