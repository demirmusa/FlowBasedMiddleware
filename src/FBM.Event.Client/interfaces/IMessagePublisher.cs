﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FBM.Event.Client.interfaces
{
    public interface IMessagePublisher
    {
        void Publish(string message);
    }
}