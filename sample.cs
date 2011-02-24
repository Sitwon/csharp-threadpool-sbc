/*
     Server Based Converter .NET Interface sample program

     The following indicates the standard sample for converting office
     document and outputting to PDF.
     
     Copyright 2004 Antenna House, Inc.
*/

using System;
using System.Collections;
using System.Threading;
using DfvDotNetCtl;

public class MainApp
{
	private string _inputdoc;
	private ManualResetEvent _doneEvent;

	public MainApp(string inputdoc, ManualResetEvent doneEvent)
	{
		_inputdoc = inputdoc;
		_doneEvent = doneEvent;
	}

	static public void Main(string[] args)
	{
		if (args.Length < 2)
		{
			Console.WriteLine("usage: sample office-file out-file");
			return;
		}
		ManualResetEvent[] doneEvents = new ManualResetEvent[args.Length];
		MainApp[] sbcJobs = new MainApp[args.Length];

		Console.WriteLine("launching {0} tasks...", args.Length);
		for (int i = 0; i < args.Length; i++)
		{
			doneEvents[i] = new ManualResetEvent(false);
			MainApp job = new MainApp(args[i], doneEvents[i]);
			sbcJobs[i] = job;
			ThreadPool.QueueUserWorkItem(job.ThreadProc, i);
		}
		WaitHandle.WaitAll(doneEvents);
		Console.WriteLine("All conversions are finished.");
	}

	public void ThreadProc(Object stateInfo)
	{
		Console.WriteLine("starting job {0}", (int)stateInfo);
		DfvObj.Initialize();
		DfvObj adfv = null;
		string inputFilename = _inputdoc;
		string outputFilename = inputFilename + ".pdf";
		try
		{
			adfv = new DfvObj();
			adfv.DocumentURI = inputFilename;
			adfv.OutputFilePath = outputFilename;
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
			
			Console.WriteLine("\nFormatting finished: '" + outputFilename + "' created.");
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
			Console.WriteLine("finishing job {0}", (int)stateInfo);
			_doneEvent.Set();
		}
	}
}

