namespace AjLanguage.Language
{
    using System;

    public interface IChannel
    {
        object Receive();

        void Send(object value);
    }
}
