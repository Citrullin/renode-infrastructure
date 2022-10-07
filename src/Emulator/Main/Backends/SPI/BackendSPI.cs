//
// Copyright (c) 2010-2022 Antmicro
// Copyright (c) 2011-2015 Realtime Embedded
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//
using System;
using System.Collections;
using Antmicro.Renode.Peripherals.SPI;
using Antmicro.Renode.Core;
using Antmicro.Renode.Peripherals;

namespace Antmicro.Renode.Backends.SPI
{
    public abstract class BackendSPI : IExternal, IConnectable<ISPIPeripheral>
    {
        public abstract byte Transmit(byte data);
        
        public virtual event Action<byte> CharReceived;

        public virtual void AttachTo(ISPIPeripheral spi)
        {
            this.spi = spi;
            this.machine = spi.GetMachine();
            buffer = new Queue();
        }

        public virtual void DetachFrom(ISPIPeripheral spi)
        {
            this.spi = null;
            this.machine = null;
            buffer.Clear();
        }

        protected void CallCharReceived(byte value)
        {
            buffer.Enqueue(value);
        }

        public Queue buffer;
        private readonly object innerLock = new object();

        private ISPIPeripheral spi;
        private Machine machine;
        private bool pendingTimeDomainEvent;
    }
}

