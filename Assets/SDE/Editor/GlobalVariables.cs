﻿using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  GlobalFlags holds the list of global variables being used in this project
*/
[InitializeOnLoad]
public static class GlobalVariables {
	
	public static readonly string[] variables =
	{
		"write variables here",
		"like so",
		"these are NOT flags",
		"these have point values"
	};
	
	static GlobalVariables() {
		CheckFlags();
		ExportFlags();
	}
	
	private static void CheckFlags() {
		// check for duplicates
		HashSet<string> variableSet = new HashSet<string>();

		foreach (string variable in variables) {
			if (!variableSet.Add(variable)) {
				throw new UnityException("DUPLICATE GLOBAL VARIABLE FOUND: " + variable);
			}
		}
	}
	
	private static void ExportFlags() {
		string path = Application.dataPath + "/SDE/_GlobalVariablesBuild.cs";
		Encoding encoding = Encoding.GetEncoding("UTF-8");
		
		// build the custom class string
		string output = 
			"using System.Collections;\n" +
			"using System.Collections.Generic;\n\n" +
			"// THIS IS A PROCEDURALLY GENERATED FILE!\n" +
			"// DO NOT EDIT, MODIFY, OR WRITE TO EXCEPT TO WHEN CHECKING VARIABLES!\n" +
			"public static class GlobalVariableBuild {\n" +
			"    public static Dictionary<string, int> variables;\n\n" +
			"    static GlobalVariableBuild() {\n" +
			"        variables = new Dictionary<string, int>() {\n";
		
		for (int i = 0; i < variables.Length; i++) {
			output += 
				"            {\"" + variables[i] + "\", 0}";
			if (i == variables.Length - 1) {
				output += "\n";
			} else {
				output += ",\n";
			}
		}
		
		
		output += 
			"        };\n" +
			"    }\n" +
			"}";
		
		using (StreamWriter stream = new StreamWriter(path, false, encoding)) {
			stream.Write(output);
		}
	}
}
