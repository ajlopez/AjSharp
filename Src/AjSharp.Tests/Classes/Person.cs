namespace AjSharp.Tests.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Person
    {
        public Person()
        {
        }

        public Person(string name, int age)
        {
            this.Name = name;
            this.Age = age;
        }

        public string Name { get; set; }

        public int Age { get; set; }
    }
}
