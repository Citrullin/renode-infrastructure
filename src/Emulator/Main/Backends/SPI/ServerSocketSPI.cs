//
// Copyright (c) 2010-2018 Antmicro
// Copyright (c) 2011-2015 Realtime Embedded
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//

using System;
using Antmicro.Renode.Core;
using Antmicro.Renode.Utilities;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Antmicro.Renode.Backends.SPI
{
    public static class ServerSocketSPIExtensions
    {
        public static void CreateServerSocketSPI(this Emulation emulation, int port, string name, bool emitConfig = true)
        {
            emulation.ExternalsManager.AddExternal(new ServerSocketSPI(port, emitConfig), name);
        }
    }

    public class ServerSocketSPI : BackendSPI, IDisposable
    {
        public ServerSocketSPI(int port, bool emitConfigBytes = true)
        {
            server = new SocketServerProvider(emitConfigBytes);
            server.DataReceived += b => CallCharReceived((byte)b);

            server.Start(port);
        }

        public override byte Transmit(byte value)
        {
            Task t = Task.Run(() => server.SendByte(value));
            t.Wait();
            byte response = (byte)buffer.Dequeue();
            return response;
        }

        public void Dispose()
        {
            server.Stop();
        }

        private SocketServerProvider server;
    }
}

