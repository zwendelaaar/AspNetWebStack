﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.TestCommon
{
    public class CultureReplacer : IDisposable
    {
        private const string _defaultCultureName = "en-GB";
        private const string _defaultUICultureName = "en-US";
#if NETSTANDARD1_3
        private static readonly CultureInfo _defaultCulture = new CultureInfo(_defaultCultureName);
#else
        private static readonly CultureInfo _defaultCulture = CultureInfo.GetCultureInfo(_defaultCultureName);
#endif
        private readonly CultureInfo _originalCulture;
        private readonly CultureInfo _originalUICulture;
        private readonly long _threadId;

        // Culture => Formatting of dates/times/money/etc, defaults to en-GB because en-US is the same as InvariantCulture
        // We want to be able to find issues where the InvariantCulture is used, but a specific culture should be.
        //
        // UICulture => Language
        public CultureReplacer(string culture = _defaultCultureName, string uiCulture = _defaultUICultureName)
        {
#if NETSTANDARD1_3
            _originalCulture = CultureInfo.CurrentCulture;
            _originalUICulture = CultureInfo.CurrentUICulture;
#else
            _originalCulture = Thread.CurrentThread.CurrentCulture;
            _originalUICulture = Thread.CurrentThread.CurrentUICulture;
#endif

            _threadId = Thread.CurrentThread.ManagedThreadId;

#if NETSTANDARD1_3
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(uiCulture);
#else
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(uiCulture);
#endif
        }

        /// <summary>
        /// The name of the culture that is used as the default value for Thread.CurrentCulture when CultureReplacer is used.
        /// </summary>
        public static string DefaultCultureName
        {
            get { return _defaultCultureName; }
        }

        /// <summary>
        /// The name of the culture that is used as the default value for Thread.UICurrentCulture when CultureReplacer is used.
        /// </summary>
        public static string DefaultUICultureName
        {
            get { return _defaultUICultureName; }
        }

        /// <summary>
        /// The culture that is used as the default value for Thread.CurrentCulture when CultureReplacer is used.
        /// </summary>
        public static CultureInfo DefaultCulture
        {
            get { return _defaultCulture; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Assert.True(Thread.CurrentThread.ManagedThreadId == _threadId, "The current thread is not the same as the thread invoking the constructor. This should never happen.");
#if NETSTANDARD1_3
                CultureInfo.CurrentCulture = _originalCulture;
                CultureInfo.CurrentUICulture = _originalUICulture;
#else
                Thread.CurrentThread.CurrentCulture = _originalCulture;
                Thread.CurrentThread.CurrentUICulture = _originalUICulture;
#endif
            }
        }
    }
}
