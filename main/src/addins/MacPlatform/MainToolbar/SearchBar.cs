﻿//
// SearchBar.cs
//
// Author:
//       Marius Ungureanu <marius.ungureanu@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc (http://www.xamarin.com)
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
using AppKit;
using Foundation;
using Gtk;
using MonoDevelop.Components.Mac;

namespace MonoDevelop.MacIntegration.MainToolbar
{
	[Register]
	class SearchBar : NSSearchField
	{
		internal Widget gtkWidget;
		internal event EventHandler<Xwt.KeyEventArgs> KeyPressed;
		internal event EventHandler LostFocus;

		/// <summary>
		/// This tells whether events have been attached when created from the menu.
		/// </summary>
		internal bool EventsAttached;

		public SearchBar ()
		{
		}

		public SearchBar (IntPtr ptr) : base (ptr)
		{
			EventsAttached = false;
		}

		static Xwt.ModifierKeys TranslateMask (NSEventModifierMask mask)
		{
			Xwt.ModifierKeys xwtMask = Xwt.ModifierKeys.None;
			if (mask.HasFlag (NSEventModifierMask.CommandKeyMask))
				xwtMask |= Xwt.ModifierKeys.Command;
			if (mask.HasFlag (NSEventModifierMask.ControlKeyMask))
				xwtMask |= Xwt.ModifierKeys.Control;
			if (mask.HasFlag (NSEventModifierMask.ShiftKeyMask))
				xwtMask |= Xwt.ModifierKeys.Shift;
			if (mask.HasFlag (NSEventModifierMask.AlternateKeyMask))
				xwtMask |= Xwt.ModifierKeys.Alt;
			return xwtMask;
		}

		static Xwt.Key TranslateKey (NSKey key)
		{
			switch (key)
			{
			case NSKey.UpArrow:
				return Xwt.Key.Up;
			case NSKey.DownArrow:
				return Xwt.Key.Down;
			case NSKey.LeftArrow:
				return Xwt.Key.Left;
			case NSKey.RightArrow:
				return Xwt.Key.Right;
			case NSKey.Home:
				return Xwt.Key.Home;
			case NSKey.End:
				return Xwt.Key.End;
			case NSKey.PageUp:
				return Xwt.Key.PageUp;
			case NSKey.PageDown:
				return Xwt.Key.PageDown;
			}
			return 0;
		}

		bool SendKeyPressed (Xwt.Key key, Xwt.ModifierKeys mask)
		{
			var kargs = new Xwt.KeyEventArgs (key, mask, false, 0);
			if (KeyPressed != null)
				KeyPressed (this, kargs);

			return kargs.Handled;
		}

		public override bool PerformKeyEquivalent (NSEvent theEvent)
		{
			// TODO: Remove his hack when we get CommandManager in place for NSView handling.
			if ((theEvent.ModifierFlags & NSEventModifierMask.CommandKeyMask) == NSEventModifierMask.CommandKeyMask) {
				if (theEvent.KeyCode == (ushort)NSKey.X)
					return NSApplication.SharedApplication.SendAction (new ObjCRuntime.Selector ("cut:"), Window.FirstResponder, this);
				if (theEvent.KeyCode == (ushort)NSKey.C)
					return NSApplication.SharedApplication.SendAction (new ObjCRuntime.Selector ("copy:"), Window.FirstResponder, this);
				if (theEvent.KeyCode == (ushort)NSKey.V)
					return NSApplication.SharedApplication.SendAction (new ObjCRuntime.Selector ("paste:"), Window.FirstResponder, this);
				if (theEvent.KeyCode == (ushort)NSKey.A)
					return NSApplication.SharedApplication.SendAction (new ObjCRuntime.Selector ("selectAll:"), Window.FirstResponder, this);
				if (theEvent.KeyCode == (ushort)NSKey.Q)
					return NSApplication.SharedApplication.SendAction (new ObjCRuntime.Selector ("terminate:"), NSApplication.SharedApplication, this);
			}

			if (theEvent.KeyCode == (ushort)NSKey.Escape) {
				SendKeyPressed (Xwt.Key.Escape, Xwt.ModifierKeys.None);
				base.PerformKeyEquivalent (theEvent);
				return true;
			}

			// Use CharactersIgnoringModifiers instead of KeyCode. They don't match Xwt anyway.
			if (SendKeyPressed (TranslateKey ((NSKey)theEvent.CharactersIgnoringModifiers[0]), TranslateMask (theEvent.ModifierFlags)))
				return true;

			return base.PerformKeyEquivalent (theEvent);
		}

		public override void DidEndEditing (NSNotification notification)
		{
			nint value = ((NSNumber)notification.UserInfo.ValueForKey ((NSString)"NSTextMovement")).LongValue;
			if (value == (nint)(long)NSTextMovement.Return ||
				value == (nint)(long)NSTextMovement.Tab) {
				if (value == (nint)(long)NSTextMovement.Return)
					SendKeyPressed (Xwt.Key.Return, Xwt.ModifierKeys.None);

				Window.MakeFirstResponder (this);
				return;
			}

			// This means we've reached a focus loss event.
			var replacedWith = notification.UserInfo.ValueForKey ((NSString)"_NSFirstResponderReplacingFieldEditor");
			if (replacedWith != this && LostFocus != null)
				LostFocus (this, null);

			base.DidEndEditing (notification);
		}

		public override void ViewDidMoveToWindow ()
		{
			base.ViewDidMoveToWindow ();

			// Needs to be grabbed after it's parented.
			gtkWidget = GtkMacInterop.NSViewToGtkWidget (this);
		}
	}
}

