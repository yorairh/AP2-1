﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IViewModel : INotifyPropertyChanged
    {
        void UploadFile(string path);
    }
}
