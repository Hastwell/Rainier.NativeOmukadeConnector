using System;
using System.Collections.Generic;
using System.Text;

namespace Rainier.NativeOmukadeConnector
{
    internal class FakeEnumerator : System.Collections.IEnumerator
    {
        public static readonly FakeEnumerator INSTANCE = new FakeEnumerator();

        public object Current => null;

        public bool MoveNext() => false;

        public void Reset() { }
    }
}
