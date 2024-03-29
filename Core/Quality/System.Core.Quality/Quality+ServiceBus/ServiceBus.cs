#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System.Collections.Generic;
namespace System.Quality
{
    /// <summary>
    /// ServiceBus
    /// </summary>
    public static class ServiceBus
    {
        // Kuldge : Remove
        public static void Touch() { var o = ServiceBusManager.Current; }

        public static TMessage MakeMessage<TMessage>()
            where TMessage : IServiceMessage, new() { return ServiceBusManager.Current.MakeMessage<TMessage>(); }
        public static void SendSelf<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { ServiceBusManager.Current.SendSelf(messageBuilder); }
        public static void SendSelf(params IServiceMessage[] messages) { ServiceBusManager.Current.SendSelf(messages); }
        public static IServiceBusCallback SendTo<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return ServiceBusManager.Current.SendTo<TMessage>(null, messageBuilder); }
        public static IServiceBusCallback SendTo<TMessage>(string destination, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return ServiceBusManager.Current.SendTo<TMessage>(destination, messageBuilder); }
        public static IServiceBusCallback SendTo(params IServiceMessage[] messages) { return ServiceBusManager.Current.SendTo(null, messages); }
        public static IServiceBusCallback SendTo(string destination, params IServiceMessage[] messages) { return ServiceBusManager.Current.SendTo(destination, messages); }
    }
}
