﻿using System;
using System.Collections.Generic;
using System.Text;

namespace A18NAdapter.Dto
{
    public class SmsMessageList
    {
        public List<SmsMessage> SmsMessages { get; set; }
        public int Count { get; set; }
    }
}
