﻿//
// TextMatePlistFormat.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2016 Xamarin Inc. (http://xamarin.com)
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

using System;
using System.Collections.Generic;

namespace MonoDevelop.Ide.Editor.Highlighting
{
	static class TextMateFormat
	{
		public static EditorTheme LoadEditorTheme (string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException (nameof (fileName));

			var dictionary = PDictionary.FromFile (fileName);
			var name = (PString)dictionary ["name"];
			var contentArray = dictionary ["settings"] as PArray;
			if (contentArray == null || contentArray.Count == 0)
				return new EditorTheme(name);
			
			var settings = new List<ThemeSetting> ();
			for (int i = 0; i < contentArray.Count; i++) {
				var dict = contentArray [i] as PDictionary;
				if (dict == null)
					continue;
				settings.Add (LoadThemeSetting (dict));
			}

			return new EditorTheme (name, settings);
		}

		static ThemeSetting LoadThemeSetting(PDictionary dict)
		{
			string name = null;
			var scopes = new List<string> ();
			var settings = new Dictionary<string, string> ();

			PObject val;
			if (dict.TryGetValue ("name", out val))
				name = ((PString)val).Value;
			if (dict.TryGetValue ("scope", out val)) {
				scopes.AddRange (((PString)val).Value.Split (new [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			}
			if (dict.TryGetValue ("settings", out val)) {
				var settingsDictionary = val as PDictionary;
				foreach (var setting in settingsDictionary) {
					settings.Add (setting.Key, ((PString)setting.Value).Value);
				}
			}
			return new ThemeSetting (name, scopes, settings);
		}
	}
}