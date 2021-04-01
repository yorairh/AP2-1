﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class FileUploadEventArgs : PropertyChangedEventArgs
    {
        int length;
        public FileUploadEventArgs(InfoVal info, int length) : base(info)
        {
            this.length = length;
        }
        public int Length
        {
            set
            {
                this.length = value;
            }
            get
            {
                return this.length;
            }
        }
    }
}
