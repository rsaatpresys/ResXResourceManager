﻿namespace tomenglertde.ResXManager.Infrastructure
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Threading;

    [Export]
    public class PerformanceTracer
    {
        private readonly ITracer _tracer;
        private int _index;

        [ImportingConstructor]
        public PerformanceTracer(ITracer tracer)
        {
            Contract.Requires(tracer != null);

            _tracer = tracer;
        }

        public IDisposable Start([Localizable(false)] string message)
        {
            Contract.Requires(message != null);
            Contract.Ensures(Contract.Result<IDisposable>() != null);

            return new Tracer(_tracer, Interlocked.Increment(ref _index), message);
        }

        private sealed class Tracer : IDisposable
        {
            private readonly ITracer _tracer;
            private readonly int _index;
            private readonly string _message;
            private readonly Stopwatch _stopwatch = new Stopwatch();

            public Tracer(ITracer tracer, int index, string message)
            {
                Contract.Requires(tracer != null);
                Contract.Requires(message != null);

                _tracer = tracer;
                _index = index;
                _message = message;

                _stopwatch.Start();

                _tracer.WriteLine(">>> {0}: {1} @{2}", _index, _message, DateTime.Now.ToString("HH:mm:ss.f", CultureInfo.InvariantCulture));
            }


            public void Dispose()
            {
                _tracer.WriteLine("<<< {0}: {1} {2}", _index, _message, _stopwatch.Elapsed);
            }

            [ContractInvariantMethod]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_tracer != null);
                Contract.Invariant(_message != null);
                Contract.Invariant(_stopwatch != null);
            }
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_tracer != null);
        }
    }
}
