﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoUniversity.Core.Courses
{
    public class Course : RootEntity
    {
        public string Title { get; set; }
        public int Credits { get; set; }
    }
}
