﻿using System;

namespace PaperX_SCG_forms.Views.CustomControls
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }
        public GenericEventArgs(T EventData)
        {
            this.EventData = EventData;
        }
    }
}
