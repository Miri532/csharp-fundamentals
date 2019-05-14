using System;
using System.Collections.Generic;
using System.IO;

namespace GradeBook
{
    public delegate void GradeAddedDelegate(object sender, EventArgs args); 

    public class NamedObject : object
    {
        public NamedObject(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }
    }

    public interface IBook
    {
        void AddGrade(double grade);
        Statistics GetStatistics();
        string Name { get; }
        event GradeAddedDelegate GradeAdded;
    }

    // bookbase is a NamedObject
    public abstract class Book : NamedObject, IBook
    {
        public Book(string name) : base(name)
        {
        }

        public abstract event GradeAddedDelegate GradeAdded;

        // an abstract method is implicitly virtual - forces a derived class to provide implementation
        public abstract void AddGrade(double grade);

        public abstract Statistics GetStatistics();
       
    }
    // book is a BookBase
    public class InMemoryBook : Book
    {
        public InMemoryBook(string name) : base(name)
        {
            grades = new List<double>();
            Name = name;

        }

        public override void AddGrade(double grade)
        {
            if (grade <= 100 && grade >= 0)
            {
                grades.Add(grade);

                // another way: GradeAdded?.Invoke(this, new EventArgs());
                if (GradeAdded != null)
                {
                    GradeAdded(this, new EventArgs());
                }
            }
            else
            {
                throw new ArgumentException($"invalid {nameof(grade)}");
            }
        }

        public override event GradeAddedDelegate GradeAdded;

        public override Statistics GetStatistics()
        {
            var result = new Statistics();

            foreach (var grade in grades)
            {
                result.Add(grade);                
            }

            return result;
        }

        private List<double> grades;
        public const string CATEGORY = "Science";

    }

    public class DiskBook : Book
    {
        public DiskBook(string name) : base(name)
        {
        }

        public override event GradeAddedDelegate GradeAdded;

        public override void AddGrade(double grade)
        {
            using (StreamWriter writer = File.AppendText($"{Name}.txt"))
            {
                writer.WriteLine(grade);
                GradeAdded?.Invoke(this, new EventArgs()); // if someone provided an event handler/s for the grade added event - invoke the handler
            }
                
        }

        public override Statistics GetStatistics()
        {
            var result = new Statistics();

            using (StreamReader reader = File.OpenText($"{Name}.txt"))
            {
                while(!reader.EndOfStream)
                {
                    result.Add(double.Parse(reader.ReadLine()));
                }                
            }

            return result;
        }
    }
}



