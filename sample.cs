/*
     Server Based Converter .NET Interface sample program

     The following indicates the standard sample for converting office
     document and outputting to PDF.
     
     Copyright 2004 Antenna House, Inc.
*/

using System;
using System.Collections;
using DfvDotNetCtl;

public class MainApp
{
	static public void Main(string[] args)
	{
		if (args.Length < 2)
		{
			Console.WriteLine("usage: sample office-file out-file");
			return;
		}
		
		DfvObj.Initialize();
		DfvObj adfv = null;
		try
		{
			adfv = new DfvObj();
			adfv.DocumentURI = args[0];
			adfv.OutputFilePath = args[1];
//			adfv.PrinterName = "@SVG";
			adfv.PrinterName = "@PDF";
			adfv.ExitLevel = 4;
			adfv.Execute();

			ArrayList errList = new ArrayList();
			adfv.GetFormattingError(errList);
			for (int i = 0; i < errList.Count; i++)
			{
				DfvErrorInformation ei = (DfvErrorInformation)errList[i];
				Console.WriteLine("ErrorLevel : " + ei.ErrorLevel + "\nErrorCode  : " + 
				ei.ErrorCode + "\n" + ei.ErrorMessage);
			}
			
			Console.WriteLine("\nFormatting finished: '" + args[1] + "' created.");
		}
		catch(DfvException e)
		{
			Console.WriteLine("ErrorLevel : " + e.ErrorLevel + "\nErrorCode : " + e.ErrorCode + "\n" + e.Message);
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
		}
		finally
		{
			if (adfv != null)
				adfv.Dispose();
			DfvObj.Terminate();
		}
	}
}

