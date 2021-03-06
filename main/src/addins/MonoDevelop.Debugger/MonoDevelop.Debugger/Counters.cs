﻿//
// Counters.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2018 Microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using MonoDevelop.Core.Instrumentation;
using System;

namespace MonoDevelop.Debugger
{
	static class Counters
	{
		public static Counter DebugSession = InstrumentationService.CreateCounter ("Debug Session", "Debugger", id: "Debugger.DebugSession");
		public static Counter EvaluationStats = InstrumentationService.CreateCounter ("Evaluation Statistics", "Debugger", id: "Debugger.EvaluationStatistics");
		public static TimerCounter<DebuggerStartMetadata> DebuggerStart = InstrumentationService.CreateTimerCounter<DebuggerStartMetadata> ("Debugger Start", "Debugger", id: "Debugger.Start");
		public static TimerCounter<DebuggerActionMetadata> DebuggerAction = InstrumentationService.CreateTimerCounter<DebuggerActionMetadata> ("Debugger Action", "Debugger", id: "Debugger.Action");
	}

	class DebuggerStartMetadata : CounterMetadata
	{
		public DebuggerStartMetadata ()
		{
		}

		public string Name {
			get => GetProperty<string> ();
			set => SetProperty (value);
		}
	}

	class DebuggerActionMetadata : CounterMetadata
	{
		public enum ActionType {
			Unknown,
			StepOver,
			StepInto,
			StepOut
		};

		public DebuggerActionMetadata ()
		{
		}

		public ActionType Type {
			get {
				var result = GetProperty<string> ();
				if (Enum.TryParse<ActionType> (result, out var eResult)) {
					return eResult;
				}

				return ActionType.Unknown;
			}

			set => SetProperty (value.ToString ());
		}
	}
}
