﻿using System;

 namespace SuperGamblino.Infrastructure.DatabasesObjects
{
    public class DateTimeResult
    {
        public DateTimeResult(bool successful, DateTime? dateTime)
        {
            Successful = successful;
            DateTime = dateTime;
        }

        public bool Successful { get; set; }
        public DateTime? DateTime { get; set; }
    }
}